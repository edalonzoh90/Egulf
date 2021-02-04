using EGullf.Services.DA.Alert;
using EGullf.Services.Models.Alert;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace EGullf.Services.Services.Alert
{
    public class AlertServices
    {
        public AlertModel MarkAsReaded(AlertModel model)
        {
            //Se marca como leída
            model.Status = (int)AlertStatus.Readed;
            AlertDA alertDA = new AlertDA();
            var resp = alertDA.InsUpd(model);
            if (resp != null)
                throw new Exception(resp.Message);

            //Retornamos la última alerta, nos servirá para sustituir la que eliminamos en la UI
            PagerModel pager = new PagerModel(7, 1, "CreatedAt", "desc");
            AlertModel filter = new AlertModel();
            filter.Status = (int)AlertStatus.New;
            filter.To = model.To;
            return Get(pager, filter).FirstOrDefault();
        }

        public RequestResult<object> InsUpd(AlertModel model)
        {
            AlertDA alertDA = new AlertDA();
            return alertDA.InsUpd(model);
        }

        public RequestResult<object> InsUpd(List<AlertModel> lst)
        {
            RequestResult<object> resp = new RequestResult<object>();
            AlertDA alertDA = new AlertDA();

            TransactionOptions scopeOptions = new TransactionOptions();
            scopeOptions.IsolationLevel = IsolationLevel.Serializable;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    foreach (AlertModel model in lst)
                    {
                        resp = alertDA.InsUpd(model);
                        if (resp != null && resp.Status == Status.Error)
                            throw new Exception(resp.Message);
                    }
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    resp = new RequestResult<object>() { Status = Status.Error, Message = ex.Message };
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }
            }

            return resp;
        }

        public List<AlertModel> Get(PagerModel pager, AlertModel filter)
        {
            AlertDA alertDA = new AlertDA();
            return alertDA.Get(pager, filter);
        }

        public AlertModel GetWithValues(int alertTemplateId, Dictionary<string, string> values)
        {
            AlertTemplateServices templateServices = new AlertTemplateServices();
            AlertTemplateModel template = templateServices.GetById(alertTemplateId);
            AlertModel alert = new AlertModel();

            alert.Body = template.Body;
            alert.Extra = template.Extra;
            alert.Subject = template.Subject;
            alert.Url = template.Url;

            if (values != null)
            {
                foreach (KeyValuePair<string, string> entry in values)
                {
                    alert.Body = alert.Body != null ? alert.Body.Replace("#"+entry.Key + "#", entry.Value) : "";
                    alert.Extra = alert.Extra != null ? alert.Extra.Replace("#" + entry.Key + "#", entry.Value) : "";
                    alert.Subject = alert.Subject != null ? alert.Subject.Replace("#" + entry.Key + "#", entry.Value) : "";
                    alert.Url = alert.Url != null ? alert.Url.Replace("#" + entry.Key + "#", entry.Value) : "";
                }
            }

            alert.AlertTemplateId = template.AlertTemplateId;
            alert.Icon = template.Icon;
            alert.Status = (int)AlertStatus.New;
            alert.TimeAgo = "Recien";

            return alert;
        }

        public AlertModel GetWithValues(AlertTemplateModel template, Dictionary<string, string> values)
        {
            AlertModel alert = new AlertModel();

            alert.Body = template.Body;
            alert.Extra = template.Extra;
            alert.Subject = template.Subject;
            alert.Url = template.Url;

            if (values != null)
            {
                foreach (KeyValuePair<string, string> entry in values)
                {
                    alert.Body = alert.Body.Replace("#" + entry.Key + "#", entry.Value);
                    alert.Extra = alert.Extra.Replace("#" + entry.Key + "#", entry.Value);
                    alert.Subject = alert.Subject.Replace("#" + entry.Key + "#", entry.Value);
                    alert.Url = alert.Url.Replace("#" + entry.Key + "#", entry.Value);
                }
            }

            alert.AlertTemplateId = template.AlertTemplateId;
            alert.Icon = template.Icon;
            alert.Status = (int)AlertStatus.New;
            alert.TimeAgo = "Recien";

            return alert;
        }
    }
}
