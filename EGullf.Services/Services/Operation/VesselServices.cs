using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.AzureStorage;
using EGullf.Services.Services.Images;
using EGullf.Services.Services.Management;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;

namespace EGullf.Services.Services.Operation
{
    public class VesselServices
    {
        public VesselModel GetFirst(VesselModel filter)
        {
            VesselDA vesselDA = new VesselDA();
            PagerModel pager = new PagerModel(0, 1, "", "");
            VesselModel resp = Get(pager, filter).FirstOrDefault();
            if(resp!= null)
                resp.Suitability = vesselDA.GetSuitability((int)resp.VesselId);
            return resp;
        }

        public VesselModel GetById(int VesselId)
        {
            VesselModel filter = new VesselModel();
            filter.VesselId = VesselId;
            VesselModel resp = GetFirst(filter);
            return resp;
        }

        public List<VesselModel> Get(PagerModel pager, VesselModel filter)
        {
            VesselDA vesselDA = new VesselDA();
            return vesselDA.Get(pager, filter);
        }

        public List<VesselModel> Get(VesselModel filter)
        {
            PagerModel pager = new PagerModel(0, Int32.MaxValue-1, "", "");
            VesselDA vesselDA = new VesselDA();
            return vesselDA.Get(pager, filter);
        }

        public RequestResult<VesselModel> InsUpd(VesselModel model)
        {
            bool isAdd = model.VesselId == null;
            ImagesServices ImageServ = new ImagesServices();
            FileServices FileServ = new FileServices();
            VesselDA vesselDA = new VesselDA();
            RequestResult<VesselModel> resp = new RequestResult<VesselModel>() { Status = Status.Success };
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    resp = vesselDA.InsUpd(model);

                    if (model.Image.FileContent != null && model.Image.FileContent.Length > 0)
                    {
                        Stream ProcessedImage = ImageServ.ResizeProfileImage(model.Image.FileContent);

                        ProcessedImage.Position = 0;

                        var FileNameExtension = ".jpg";
                        model.Image.FileName = "vesselimage-" + model.VesselId + FileNameExtension;
                        var path = "vessels/" + model.VesselId + "/images/";

                        model.Image.ContentType = "image/jpeg";
                        model.Image.Path = path;
                        model.Image.FileContent = ProcessedImage;

                        FileServ.SaveFile(model.Image);

                        if(isAdd)
                            resp = vesselDA.InsUpd(model);
                    }

                    ts.Complete();
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    resp = new RequestResult<VesselModel>() { Status = Status.Error, Message = ex.Message };
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    throw ex;
                }
            }
            return resp;
        }

        public RequestResult<string> Eval(VesselModel model)
        {
            VesselDA vesselDA = new VesselDA();
            return vesselDA.Val(model);
        }

        public int EvalAvailability(VesselAvailabilityModel model)
        {
            VesselDA vesselDA = new VesselDA();
            return vesselDA.ValAvailability(model);
        }

        public RequestResult<VesselSpecificInfoModel> InsUpdSpecificInfo(VesselSpecificInfoModel model)
        {
            VesselDA vesselDA = new VesselDA();
            RequestResult<VesselSpecificInfoModel> resp = new RequestResult<VesselSpecificInfoModel>() { Status = Status.Success };
            return vesselDA.InsUpdSpecificInfo(model);
        }

        public VesselSpecificInfoModel GetSpecificInfo(int VesselId)
        {
            VesselDA vesselDA = new VesselDA();
            return vesselDA.GetSpecificInfo(VesselId);
        }

        public RequestResult<VesselCostModel> InsUpdCost(VesselCostModel model)
        {
            VesselDA vesselDA = new VesselDA();
            RequestResult<VesselCostModel> resp = new RequestResult<VesselCostModel>() { Status = Status.Success };
            return vesselDA.InsUpdCost(model);
        }

        public VesselCostModel GetCost(int VesselId)
        {
            VesselDA vesselDA = new VesselDA();
            return vesselDA.GetCost(VesselId);
        }

        public SpecificInformationModel GetSpecificInfoExtra(int referenceId)
        {
            SpecificInformationServices specificInformationServices = new SpecificInformationServices();
            CabinSpecificationServices cabinSpecificationServices = new CabinSpecificationServices();

            SpecificInformationModel resp = specificInformationServices.GetByReferenceId(referenceId, SpecificInformationModel.VESSEL_TYPE);
            List<CabinSpecificationModel> lstCabins = cabinSpecificationServices.GetByReferenceId(referenceId, SpecificInformationModel.VESSEL_TYPE);

            CabinSpecificationModel c1 = lstCabins.Where(c => c.CabinType == CabinSpecificationModel.SINGLE_BERTH).FirstOrDefault() ?? new CabinSpecificationModel();
            CabinSpecificationModel c2 = lstCabins.Where(c => c.CabinType == CabinSpecificationModel.DOUBLE_BERTH).FirstOrDefault() ?? new CabinSpecificationModel();
            CabinSpecificationModel c3 = lstCabins.Where(c => c.CabinType == CabinSpecificationModel.FOUR_BERTH).FirstOrDefault() ?? new CabinSpecificationModel();
            resp.SingleBerth = c1.CabinQuantity;
            resp.DoubleBerth = c2.CabinQuantity;
            resp.FourBerth = c3.CabinQuantity;
            return resp;
        }

        public RequestResult<VesselModel> InsUpdComplete(VesselModel vessel, 
            VesselSpecificInfoModel vesselSpecificInfo, 
            SpecificInformationModel specificInfo,
            VesselCostModel vesselCost)
        {
            RequestResult<VesselModel> resp = new RequestResult<VesselModel>() { Status = Status.Success };
            SpecificInformationServices specificInfoServices = new SpecificInformationServices();
            CabinSpecificationServices cabinSpecificationServices = new CabinSpecificationServices();
            RegionServices regionServices = new RegionServices();
            PortServices portServices = new PortServices();
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    //Ciclo para ir a buscar diferentes puntos en el mapa, en caso de que el q se me asigne de forma aleatoria, se encuentre ya en BD
                    int valLocation = 2;
                    int test = 1;
                    int cont = 0;
                    while (valLocation > test && cont <= 100)
                    {
                        //Si se va a insertar, le agregamos un punto en el mapa
                        if (vessel.VesselId == null)
                        {
                            test = 0;
                            PortModel port = portServices.GetById((int)vessel.HomePort.PortId);
                            vessel.Location = regionServices.GetLocation(port.Region.RegionId);
                        }
                        else    //De lo contrario verificamos si ha cambiado su puerto, si cambió le asignamos una nueva locación
                        {
                            test = 1;
                            int? RegionIdAct = portServices.GetById((int)vessel.HomePort.PortId).Region.RegionId;
                            int? RegionIdAnt = GetById((int)vessel.VesselId).HomePort.Region.RegionId;
                            if (RegionIdAnt != RegionIdAct)
                                vessel.Location = regionServices.GetLocation(RegionIdAct);
                        }

                        //Verificamos si existe otro Barco en el mismo punto
                        if (vessel.Location.Lat == 0)
                            test = valLocation;
                        else
                        {
                            valLocation =
                                Get(new VesselModel()
                                {
                                    Location = new LatLng()
                                    {
                                        Lat = vessel.Location.Lat,
                                        Lng = vessel.Location.Lng
                                    }
                                }).Count();
                        }
                        cont++;
                    }
                    if (cont == 100)
                        throw new Exception("Se ha alcanzado el número de barcos permitidos para una región");

                    RequestResult<VesselModel> res1 = InsUpd(vessel);
                    if (res1.Status != Status.Success)
                        throw new Exception(res1.Message);

                    vesselSpecificInfo.VesselId = res1.Data.VesselId;
                    vesselSpecificInfo.UserModifiedId = vessel.UserModifiedId;
                    RequestResult<VesselSpecificInfoModel> res2 = InsUpdSpecificInfo(vesselSpecificInfo);
                    if (res2.Status != Status.Success)
                        throw new Exception(res2.Message);

                    specificInfo.MatchableId = res1.Data.VesselId;
                    specificInfo.Type = SpecificInformationModel.VESSEL_TYPE;
                    RequestResult<SpecificInformationModel> res3 = specificInfoServices.InsUpd(specificInfo);
                    if (res3.Status != Status.Success)
                        throw new Exception(res3.Message);

                    List<CabinSpecificationModel> lstCabins = specificInfo.GetCabinSpecificationList(CabinSpecificationModel.VESSEL_TYPE);
                    RequestResult<CabinSpecificationModel> respC;
                    foreach(CabinSpecificationModel cabin in lstCabins)
                    {
                        respC = cabinSpecificationServices.InsUpd(cabin);
                        if (respC.Status != Status.Success)
                            throw new Exception(respC.Message);
                    }

                    vesselCost.VesselId = res1.Data.VesselId;
                    RequestResult<VesselCostModel> res4 = InsUpdCost(vesselCost);
                    if (res4.Status != Status.Success)
                        throw new Exception(res4.Message);

                    resp = res1;
                    ts.Complete();
                }
                catch (Exception ex)
                {
                    resp.Status = Status.Error;
                    resp.Message = ex.Message;
                    ts.Dispose();
                    throw new Exception(ex.Message);
                }
            }

            return resp;
        }

        public List<SelectModel> GetEstatus()
        {
            List<SelectModel> lst = new List<SelectModel>();
            lst.Add(new SelectModel() { Text = "No disponible", Value="1" });
            lst.Add(new SelectModel() { Text = "Disponible", Value= "2" });
            lst.Add(new SelectModel() { Text = "Fuera de servicio", Value= "3" });

            return lst;
        }


        public List<VesselModel> VesselAvailableProject(int CompanyId, int ProjectId, int OfferId)
        {
            VesselDA da = new VesselDA();
            return da.VesselAvailableProject(CompanyId,ProjectId,OfferId);
        }
    }
}
