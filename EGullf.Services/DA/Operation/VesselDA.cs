using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Operation
{
    public class VesselDA
    {
        public List<ProjectTypeModel> GetSuitability(int VesselId)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelSuitability(VesselId).ToList();

                return (from x in resp
                        select new ProjectTypeModel()
                        {
                            ProjectTypeId = x.ProjectTypeId,
                            Acronym = x.Acronym,
                            Name = x.Name
                        }).ToList();
            }
        }

        public List<VesselModel> Get(PagerModel pager, VesselModel filter)
        {
            if (filter.Location == null)
                filter.Location = new LatLng();

            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelPagVessel(
                 filter.VesselId, filter.Status, filter.Name, filter.Imo,
                 filter.Company.CompanyId, filter.Location.Lat, filter.Location.Lng,
                pager.Start, pager.Offset, pager.SortBy, pager.SortDir).ToList();

                if (resp.Count() > 0)
                {
                    var first = resp.FirstOrDefault();
                    pager.TotalRecords = first.TotalRecords.HasValue ? first.TotalRecords.Value : 0;
                }

                return (from x in resp
                        select new VesselModel()
                        {
                            VesselId = x.VesselId,
                            Status = x.Status,
                            Name = x.Name,
                            Imo = x.IMO,
                            YearBuild = x.YearBuild,
                            ClassNotation = x.ClassNotation,
                            ClassValidity = x.ClassValidity,
                            VesselType = new VesselTypeModel() { VesselTypeId = x.VesselTypeId, Name = x.VesselType },
                            HomePort = new PortModel() { PortId = x.PortId, Region = new RegionModel() { RegionId = x.RegionId } },
                            Image = new FileModel() { FileReferenceId = x.FileReferenceId },
                            Company = new CompanyModel() { CompanyId = x.CompanyId },
                            Country = new CountryModel() { CountryId = x.CountryId },
                            ClasificationSociety = new ClasificationSocietyModel() { ClasificationSocietyId = x.ClasificationSocietyId },
                            Location = new LatLng() { Lat = (decimal)x.Lat, Lng = (decimal)x.Lng },
                        }).ToList();
            }
        }

        public RequestResult<VesselModel> InsUpd(VesselModel model)
        {
            RequestResult<VesselModel> ER = new RequestResult<VesselModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("VesselId", typeof(int?));
                Id.Value = model.VesselId;

                ER = db.sp_InsUpdVessel(Id, model.Name, model.Imo,
                    model.Country.CountryId, model.YearBuild,
                    model.ClasificationSociety.ClasificationSocietyId,
                    model.ClassNotation, model.ClassValidity, model.VesselType.VesselTypeId, model.HomePort.PortId,
                    model.Image.FileReferenceId, model.Status, model.Location.Lat, model.Location.Lng, model.SuitabilityIds,
                    model.UserModifiedId)
                    .Select(x => new RequestResult<VesselModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message,
                        Data = model
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                    ER.Data.VesselId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }

        public RequestResult<VesselSpecificInfoModel> InsUpdSpecificInfo(VesselSpecificInfoModel model)
        {
            RequestResult<VesselSpecificInfoModel> ER = new RequestResult<VesselSpecificInfoModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {

                ER = db.sp_InsUpdVesselSpecificInfo(model.VesselId, model.GrossTonnage, model.NetTonnage, model.BeamOverall, model.LengthOverall,
                    model.MaximumLoadedDraft, model.FreeDeckArea, model.DeckStrenght, model.FreshWaterCapacity, model.FuelOilCapacity,
                    model.BallastWaterCapacity, model.MudCapacity, model.CementTanksCapacity, model.OilRecoveryCapacity,
                    model.WaterMarkerPlant, model.HotWaterCalorifier, model.SewageTreatmentPlant, model.CruisingSpeed,
                    model.MaximumSpeed, model.DistanceCruisingSpeed, model.DistanceMaxSpeed, model.FuelConsumptionCruisingSpeed,
                    model.FuelConsumptionMaxSpeed, model.DynamicPositionSystem, model.UserModifiedId)
                    .Select(x => new RequestResult<VesselSpecificInfoModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                return ER;
            }
        }

        public VesselSpecificInfoModel GetSpecificInfo(int VesselId)
        {
            using (var db = new EGULFEntities())
            {
                var data = db.sp_SelVesselSpecificInfo(VesselId).FirstOrDefault();

                return new VesselSpecificInfoModel()
                {
                    VesselId = data.VesselId,
                    GrossTonnage = data.GrossTonnage,
                    NetTonnage = data.NetTonnage,
                    BeamOverall = data.BeamOverall,
                    LengthOverall = data.LengthOverall,
                    MaximumLoadedDraft = data.MaximumLoadedDraft,
                    FreeDeckArea = data.FreeDeckArea,
                    DeckStrenght = data.DeckStrenght,
                    FreshWaterCapacity = data.FreshWaterCapacity,
                    FuelOilCapacity = data.FuelOilCapacity,
                    BallastWaterCapacity = data.BallastWaterCapacity,
                    MudCapacity = data.MudCapacity,
                    CementTanksCapacity = data.CementTanksCapacity,
                    OilRecoveryCapacity = data.OilRecoveryCapacity,
                    WaterMarkerPlant = data.WaterMarkerPlant,
                    HotWaterCalorifier = data.HotWaterCalorifier,
                    SewageTreatmentPlant = data.SewageTreatmentPlant,
                    CruisingSpeed = data.CruisingSpeed,
                    MaximumSpeed = data.MaximumSpeed,
                    DistanceCruisingSpeed = data.DistanceCruisingSpeed,
                    DistanceMaxSpeed = data.DistanceMaxSpeed,
                    FuelConsumptionCruisingSpeed = data.FuelConsumptionCruisingSpeed,
                    FuelConsumptionMaxSpeed = data.FuelConsumptionMaxSpeed,
                    DynamicPositionSystem = data.DynamicPositionSystem
                };
            }
        }

        public RequestResult<VesselCostModel> InsUpdCost(VesselCostModel model)
        {
            RequestResult<VesselCostModel> ER = new RequestResult<VesselCostModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {

                ER = db.sp_InsUpdVesselCost(model.VesselId, model.LodgingCost, model.MealCost, model.LaundryCost,
                    model.DailyRateTowing, model.DailyRatePersonnelTransportation, model.DailyRateMaterialTransportation,
                    model.DailyRateFlotel, model.UserModifiedId)
                    .Select(x => new RequestResult<VesselCostModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                return ER;
            }
        }

        public VesselCostModel GetCost(int VesselId)
        {
            using (var db = new EGULFEntities())
            {
                var data = db.sp_SelVesselCost(VesselId).FirstOrDefault();

                return new VesselCostModel()
                {
                    VesselId = data.VesselId,
                    LodgingCost = data.LodgingCost,
                    MealCost = data.MealCost,
                    LaundryCost = data.LaundryCost,
                    DailyRateTowing = data.DailyRateTowing,
                    DailyRatePersonnelTransportation = data.DailyRatePersonnelTransportation,
                    DailyRateMaterialTransportation = data.DailyRateMaterialTransportation,
                    DailyRateFlotel = data.DailyRateFlotel,
                };
            }
        }

        public RequestResult<string> Val(VesselModel model)
        {
            RequestResult<string> resp = new RequestResult<string>() { Status = Status.Success };
            using (var db = new EGULFEntities())
            {
                var ER = db.sp_ValVessel(model.VesselId, model.Name, model.Imo)
                    .Select(x => new RequestResult<string>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Warning,
                        Message = x.Message,
                        Data = x.Message
                    }).FirstOrDefault();

                return ER ?? resp;
            }
        }

        public int ValAvailability(VesselAvailabilityModel model)
        {
            RequestResult<string> resp = new RequestResult<string>() { Status = Status.Success };
            using (var db = new EGULFEntities())
            {
                return (int)db.sp_ValAvailabilityVessel(model.VesselId, model.StartDate, model.EndDate).FirstOrDefault();
            }
        }

        public List<VesselModel> VesselAvailableProject(int CompanyId, int ProjectId, int OfferId)
        {
            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_SelVesselMatchedAvailable(CompanyId,
                                                                  ProjectId,
                                                                  OfferId).Select(x => new VesselModel()
                                                                  {
                                                                      VesselId = x.VesselId,
                                                                      VesselType = new VesselTypeModel() {
                                                                          Name = x.VesselType
                                                                      },
                                                                      YearBuild = x.YearBuild,
                                                                      Country = new CountryModel() {
                                                                          Name = x.Country
                                                                      },
                                                                      HomePort = new PortModel() {
                                                                          Name = x.HomePort,
                                                                          Region = new RegionModel()
                                                                          {
                                                                              Name = x.Region
                                                                          }
                                                                      },
                                                                      Imo = x.IMO,
                                                                      Name = x.VesselName,
                                                                      SuitabilityDescription = x.VesselProjectTypes,
                                                                      Image = new FileModel() {
                                                                          FileName = x.FileName,
                                                                          Path = x.Path,
                                                                          ContentType = x.ContentType
                                                                      },
                                                                      VesselCost = new VesselCostModel() {
                                                                          LodgingCost = x.LodgingCost,
                                                                          MealCost = x.MealCost,
                                                                          LaundryCost = x.MealCost,
                                                                          DailyMaxRate = x.DailyMaxRate
                                                                      }
                                                                  }).ToList();

                return queryResult;
            }
        }
    }
}
