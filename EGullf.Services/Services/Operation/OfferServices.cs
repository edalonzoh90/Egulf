using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Alert;
using EGullf.Services.Models.Mail;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Alert;
using EGullf.Services.Services.Mail;
using EGullf.Services.Services.Management;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Linq;
using EGullf.Services.Services.Reports;
using EGullf.Services.Reports.Models;
using EGullf.Services.Models.Configuration;
using EGullf.Services.DA;
using EGullf.Services.Services.Configuration;

namespace EGullf.Services.Services.Operation
{
    public class OfferServices
    {
        public OfferModel GetFirst(OfferModel offer)
        {
            OfferDA da = new OfferDA();
            return da.Get(offer).FirstOrDefault();
        }

        public List<OfferModel> Get(OfferModel offer)
        {
            OfferDA da = new OfferDA();
            return da.Get(offer);
        }

        public List<OfferModel> GetAll(PagerModel PagerParameters, OfferModel parameters)
        {
            OfferDA offerDA = new OfferDA();
            return offerDA.GetAll(PagerParameters,parameters);
        }

        public OfferModel GetById(int id)
        {
            OfferDA da = new OfferDA();
            OfferModel filter = new OfferModel();
            filter.OfferId = id;
            return da.Get(filter).FirstOrDefault();
        }


        public List<OfferModel> CancelOthers(int VesselId)
        {
            OfferDA da = new OfferDA();
            return da.CancelOthers(VesselId);
        }

        /// <summary>
        /// Offer from Project to Vessel
        /// Validations
        /// Insert offer with Status NEW
        /// Send alert to Vessel company owners
        /// Send mail to Vessel company owners
        /// </summary>
        /// <param name="offer"></param>
        /// <returns></returns>
        public RequestResult<List<AlertModel>> InsComplete(OfferModel offer)
        {
            RequestResult<List<AlertModel>> resp = new RequestResult<List<AlertModel>>() { Status = Status.Success };
            OfferDA offerDA = new OfferDA();
            VesselServices vesselServices = new VesselServices();
            PersonServices personServices = new PersonServices();
            AlertServices alertServices = new AlertServices();
            List<AlertModel> lstAlertToSend = new List<AlertModel>();
            MailServices MailServ = new MailServices();
            ITemplate factory = new TemplateMessagesFactory();

            TransactionOptions scopeOptions = new TransactionOptions();
            ////scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    if (offer.Vessel.VesselId == null)
                        throw new Exception("VesselId REQUIRED");
                    if (offer.Project.ProjectId == null)
                        throw new Exception("ProjectId REQUIRED");
                    if (offer.ProjectAdmin.PersonId == null)
                        throw new Exception("ProjectAdmin.PersonId REQUIRED");

                    OfferModel val = new OfferModel();
                    List<OfferModel> lstVal = new List<OfferModel>();
                    val.Project.ProjectId = offer.Project.ProjectId;
                    val.Vessel.VesselId = offer.Vessel.VesselId;
                    lstVal = Get(val);

                    if (lstVal.Count > 0)
                        throw new Exception("STATUS_NOT_VALID");

                    VesselModel vessel = new VesselModel();
                    vessel.VesselId = offer.Vessel.VesselId;
                    vessel = vesselServices.Get(vessel).FirstOrDefault();

                    // Insert offer with Status NEW
                    var respOffer = offerDA.InsUpd(offer);
                    if (respOffer.Status != Status.Success)
                        throw new Exception(respOffer.Message);

                    // Send alert to Vessel company owners
                    //Listado de los usuarios de una compañía
                    UserPersonModel person = new UserPersonModel();
                    person.CompanyId = vessel.Company.CompanyId;
                    List<UserPersonModel> lst = personServices.getUserPerson(person);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("IMO", vessel.Imo);
                    values.Add("VESSELNAME", vessel.Name);
                    AlertModel alert = alertServices.GetWithValues(6, values);

                    SystemVariableServices SVS = new SystemVariableServices();
                    Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                    string EgulfUrl = SVS.GetSystemVariableValue("EgulfWeb");            
                    param.Add("{Enfasis}", new string[] { vessel.Imo, vessel.Name });
                    param.Add("{Btn_url}", new string[] { EgulfUrl });
                    foreach (UserPersonModel personItem in lst)
                    {
                        AlertModel alertAux = alert.Clone();
                        alertAux.To = personItem.PersonId;
                        lstAlertToSend.Add(alertAux);
                        MailServ.SendMail(factory.GetTemplate(personItem.Email, "VesselOfferReceived", param));
                    }

                    var respAlert = alertServices.InsUpd(lstAlertToSend);
                    if (respAlert != null)
                        throw new Exception(respAlert.Message);

                    resp.Data = lstAlertToSend;

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    resp = new RequestResult<List<AlertModel>>() { Status = Status.Error, Message = ex.Message };
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }
            }
            return resp;
        }

        public RequestResult<OfferModel> InsUpd(OfferModel model)
        {
            OfferDA offerDA = new OfferDA();
            return offerDA.InsUpd(model);
        }

        public RequestResult<OfferCostModel> UpdCost(OfferCostModel model)
        {
            OfferDA offerDA = new OfferDA();
            return offerDA.InsUpdCost(model);
        }

        /// <summary>
        /// Validate Status
        /// Validate Availabilty
        /// Update Offer
        /// Update availability
        /// Cancel other offers if exists
        /// Notify to vessel owners cancelled if exists
        /// Notify to project owner by Signal and Mail
        /// Generate Agreement Report and send Mail
        /// </summary>
        /// <param name="model"></param>
        public RequestResult<List<AlertModel>> Accept(OfferModel model, int currentPersonId)
        {
            RequestResult<List<AlertModel>> resp = new RequestResult<List<AlertModel>>();
            MailServices MailServ = new MailServices();
            ITemplate factory = new TemplateMessagesFactory();
            VesselServices vesselServices = new VesselServices();
            VesselAvailabilityServices availabilityServices = new VesselAvailabilityServices();
            AlertServices alertServices = new AlertServices();
            AlertTemplateServices templateServices = new AlertTemplateServices();
            List<AlertModel> lstAlertToSend = new List<AlertModel>();
            ProjectServices projectServices = new ProjectServices();

            TransactionOptions scopeOptions = new TransactionOptions();
            scopeOptions.IsolationLevel = IsolationLevel.Serializable;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    //Validate
                    if (model.OfferId == null)
                        throw new Exception("REQUIRED OfferId");

                    if (model.VesselAdmin.PersonId == null)
                        throw new Exception("VesselAdmin.PersonId REQUIRED");

                    OfferModel val = GetById((int)model.OfferId);
                    if (val == null)
                        throw new Exception("NOT_FOUND");

                    if (val.Status != OfferModel.NEW)
                    {
                        if (!(val.Status == OfferModel.FIX && val.VesselAdmin.PersonId == currentPersonId))
                        {
                            throw new Exception("STATUS_NOT_VALID");
                        }
                    }

                    VesselAvailabilityModel availabilityModel = new VesselAvailabilityModel()
                    {
                        ReasonId = VesselAvailabilityModel.DEFAULT,
                        VesselId = val.Vessel.VesselId,
                        StartDate = val.Project.StartDate,
                        EndDate = val.Project.EndDate
                    };
                    int val2 = vesselServices.EvalAvailability(availabilityModel);
                    if (val2 > 0)
                        throw new Exception("NOT_AVAILABILITY");

                    //Update Offer
                    model.Status = OfferModel.ACCEPTED;
                    var respOffer = InsUpd(model);
                    if (respOffer.Status != Status.Success)
                        throw new Exception(respOffer.Message);

                    //Update vessel availabilty
                    var respAvailability = availabilityServices.InsUpd(availabilityModel);
                    if (respAvailability.Status != Status.Success)
                        throw new Exception(respAvailability.Message);

                    //Update Project Status
                    var respStatus = projectServices.UpdateStatus(val.Project.ProjectId, ProjectModel.STATUS_FIXED);
                    if (respStatus.Status != Status.Success)
                        throw new Exception(respStatus.Message);

                    //Get new values
                    val = GetById((int)model.OfferId);

                    // Cancel other offers if exists
                    List<OfferModel> lstNotifyCancel = CancelOthers((int)val.OfferId);
                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("FLAG", val.Vessel.Country.Name);
                    values.Add("HOMEPORT", val.Vessel.HomePort.Name);

                    AlertTemplateModel template = templateServices.GetById(4);
                    foreach (OfferModel offerCancelled in lstNotifyCancel)
                    {
                        if (!values.ContainsKey("FOLIO"))
                            values.Add("FOLIO", offerCancelled.Project.Folio);
                        else
                            values["FOLIO"] = offerCancelled.Project.Folio;

                        AlertModel alertCancelled = alertServices.GetWithValues(template, values);
                        alertCancelled.To = offerCancelled.ProjectAdmin.PersonId;
                        lstAlertToSend.Add(alertCancelled);
                    }


                    //Notify to project owner
                    values = new Dictionary<string, string>();
                    values.Add("IMO", val.Vessel.Imo);
                    values.Add("VESSELNAME", val.Vessel.Name);
                    values.Add("FOLIO", val.Project.Folio);
                    AlertModel alertAccepted = alertServices.GetWithValues(5, values);
                    alertAccepted.To = val.ProjectAdmin.PersonId;
                    lstAlertToSend.Add(alertAccepted);
                    var respAlert = alertServices.InsUpd(lstAlertToSend);
                    if (respAlert != null)
                        throw new Exception(respAlert.Message);

                    //Send mail
                    //Generate Agreement Report and send to mail
                    List<MailAttachments> agreementReportProject = new List<MailAttachments>();

                    SystemVariableServices SVS = new SystemVariableServices();
                    Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                    string EgulfUrl = SVS.GetSystemVariableValue("EgulfWeb");
                    param.Add("{Enfasis}", new string[] { val.Vessel.Imo, val.Vessel.Name, val.Project.Folio });
                    param.Add("{Btn_url}", new string[] { EgulfUrl });

                    ReportServices ReportServ = new ReportServices();             
                    agreementReportProject.Add(ReportServ.AgreementReportAttachment((int)model.OfferId,(int)TypeUser.Project));
                    MailServ.SendMailWithAttachment(factory.GetTemplate(val.ProjectAdmin.Email, "OfferAccepted", param),agreementReportProject);

                    param = new Dictionary<string, string[]>();
                    param.Add("{Enfasis}", new string[] { val.Project.Folio });
                    param.Add("{Btn_url}", new string[] { EgulfUrl });

                    List<MailAttachments> agreementReportVessel = new List<MailAttachments>();
                    agreementReportVessel.Add(ReportServ.AgreementReportAttachment((int)model.OfferId,(int)TypeUser.Vessel));
                    MailServ.SendMailWithAttachment(factory.GetTemplate(val.VesselAdmin.Email, "YouAcceptedOffer", param),agreementReportVessel);
                    
                    resp.Data = lstAlertToSend;

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    resp = new RequestResult<List<AlertModel>>() { Status = Status.Error, Message = ex.Message };
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }
            }

            return resp;
        }

        public RequestResult<List<AlertModel>> Reject(OfferModel model)
        {
            RequestResult<List<AlertModel>> resp = new RequestResult<List<AlertModel>>();
            MailServices MailServ = new MailServices();
            ITemplate factory = new TemplateMessagesFactory();
            VesselServices vesselServices = new VesselServices();
            AlertServices alertServices = new AlertServices();
            AlertTemplateServices templateServices = new AlertTemplateServices();
            List<AlertModel> lstAlertToSend = new List<AlertModel>();

            TransactionOptions scopeOptions = new TransactionOptions();
            ////scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    if (model.OfferId == null)
                        throw new Exception("REQUIRED OfferId");
                    if (model.VesselAdmin.PersonId == null)
                        throw new Exception("VesselAdmin.PersonId REQUIRED");

                    OfferModel val = GetById((int)model.OfferId);
                    if (val == null)
                        throw new Exception("NOT_FOUND");

                    if (val.Status != OfferModel.NEW && val.Status != OfferModel.FIX)
                        throw new Exception("STATUS_NOT_VALID");

                    OfferModel modelUpd = new OfferModel();
                    modelUpd.OfferId = model.OfferId;
                    model.Status = OfferModel.REJECTED;
                    InsUpd(model);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("FLAG", val.Vessel.Country.Name);
                    values.Add("HOMEPORT", val.Vessel.HomePort.Name);
                    values.Add("FOLIO", val.Project.Folio);

                    //Notify to project owner alert and mail
                    AlertModel alertRejected = alertServices.GetWithValues(4, values);
                    alertRejected.To = val.ProjectAdmin.PersonId;
                    lstAlertToSend.Add(alertRejected);
                    var respAlert = alertServices.InsUpd(alertRejected);
                    if (respAlert != null)
                        throw new Exception(respAlert.Message);

                    SystemVariableServices SVS = new SystemVariableServices();
                    Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                    string EgulfUrl = SVS.GetSystemVariableValue("EgulfWeb");
                    param.Add("{Enfasis}", new string[] { val.Vessel.Country.Name, val.Vessel.HomePort.Name, val.Project.Folio });
                    param.Add("{Btn_url}", new string[] { EgulfUrl });

                    MailServ.SendMail(factory.GetTemplate(val.ProjectAdmin.Email, "OfferRejected", param));

                    resp.Data = lstAlertToSend;

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    resp = new RequestResult<List<AlertModel>>() { Status = Status.Error, Message = ex.Message };
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }
            }
            return resp;
        }

        public RequestResult<List<AlertModel>> Fix(OfferModel model)
        {
            RequestResult<List<AlertModel>> resp = new RequestResult<List<AlertModel>>();
            MailServices MailServ = new MailServices();
            ITemplate factory = new TemplateMessagesFactory();
            VesselServices vesselServices = new VesselServices();
            AlertServices alertServices = new AlertServices();
            AlertTemplateServices templateServices = new AlertTemplateServices();
            List<AlertModel> lstAlertToSend = new List<AlertModel>();

            if (model.OfferId == null)
                throw new Exception("REQUIRED OfferId");

            TransactionOptions scopeOptions = new TransactionOptions();
            ////scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    OfferModel val = GetById((int)model.OfferId);
                    if (val == null)
                        throw new Exception("NOT_FOUND");

                    if (val.Status != OfferModel.NEW)
                        throw new Exception("STATUS_NOT_VALID");

                    OfferModel modelUpd = new OfferModel();
                    modelUpd.OfferId = model.OfferId;
                    model.Status = OfferModel.FIX;
                    InsUpd(model);

                    Dictionary<string, string> values = new Dictionary<string, string>();
                    values.Add("FLAG", val.Vessel.Country.Name);
                    values.Add("HOMEPORT", val.Vessel.HomePort.Name);
                    values.Add("IMO", val.Vessel.Imo);
                    values.Add("VESSELNAME", val.Vessel.Name);
                    values.Add("FOLIO", val.Project.Folio);

                    //Notify to project owner alert and mail
                    AlertModel alertFix = alertServices.GetWithValues(7, values);
                    alertFix.To = val.ProjectAdmin.PersonId;
                    lstAlertToSend.Add(alertFix);
                    var respAlert = alertServices.InsUpd(alertFix);
                    if (respAlert != null)
                        throw new Exception(respAlert.Message);

                    SystemVariableServices SVS = new SystemVariableServices();
                    Dictionary<string, string[]> param = new Dictionary<string, string[]>();
                    string EgulfUrl = SVS.GetSystemVariableValue("EgulfWeb");
                    param.Add("{Enfasis}", new string[] { val.Vessel.Country.Name, val.Vessel.HomePort.Name, val.Project.Folio });
                    param.Add("{Btn_url}", new string[] { EgulfUrl });
                                    
                    MailServ.SendMail(factory.GetTemplate(val.ProjectAdmin.Email, "OfferFix", param));
                    resp.Data = lstAlertToSend;

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    resp = new RequestResult<List<AlertModel>>() { Status = Status.Error, Message = ex.Message };
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }
            }
            return resp;
        }
    }
}
