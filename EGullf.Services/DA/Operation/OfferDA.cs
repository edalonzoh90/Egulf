using EGullf.Services.Models.AzureStorage;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Management;
using EGullf.Services.Services.Operation;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Operation
{
    public class OfferDA
    {

        public List<OfferModel> CancelOthers(int VesselId)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_UpdOfferCancelOthers(VesselId)
                    .Select(x => new OfferModel()
                    {
                        ProjectAdmin = new PersonModel()
                        {
                            PersonId = x.PersonId
                        },
                        Project = new ProjectModel()
                        {
                            ProjectId = x.ProjectId,
                            Folio = x.Folio
                        }
                    }).ToList();

                return resp;
            }
        }

        public List<OfferModel> Get(OfferModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return db.sp_SelOffer(filter.OfferId, filter.Vessel.VesselId, filter.Project.ProjectId)
                    .Select(x => new OfferModel()
                    {
                        OfferId = x.OfferId,
                        Status = x.Status,
                        Vessel = new VesselModel()
                        {
                            VesselId = x.VesselId,
                            Name = x.VesselName,
                            Imo = x.IMO,
                            Country = new CountryModel()
                            {
                                Name = x.CountryName
                            },
                            HomePort = new PortModel()
                            {
                                Name = x.PortName
                            },
                            Company = new CompanyModel()
                            {
                                CompanyId = x.VesselCompanyId
                            }
                        },
                        VesselAdmin = new PersonModel()
                        {
                            PersonId = x.VesselOwnerId,
                            Email = x.VesselOwnerEmail
                        },
                        Project = new ProjectModel()
                        {
                            Folio = x.Folio,
                            ProjectId = x.ProjectId,
                            StartDate = x.StartDate,
                            EndDate = x.EndDate,
                            CompanyId = x.ProjectCompanyId
                        },
                        ProjectAdmin = new PersonModel()
                        {
                            PersonId = x.ProjectOwnerId,
                            Email = x.ProjectOwnerEmail
                        }

                    }).ToList();
            }
        }

        public RequestResult<OfferModel> InsUpd(OfferModel model)
        {
            RequestResult<OfferModel> ER = new RequestResult<OfferModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("OfferId", typeof(int?));
                Id.Value = model.OfferId;

                ER = db.sp_InsUpdOffer(Id, model.Vessel.VesselId, model.Project.ProjectId,
                    model.Status, model.VesselAdmin.PersonId, model.ProjectAdmin.PersonId, 
                    model.OfferReferenceId, model.Comment, model.UserModifiedId)
                    .Select(x => new RequestResult<OfferModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message,
                        Data = model
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                {
                    ER.Data.OfferId = Convert.ToInt32(Id.Value.ToString());
                    model.OfferId = Convert.ToInt32(Id.Value.ToString());
                }

                return ER;
            }
        }

        public RequestResult<OfferCostModel> InsUpdCost(OfferCostModel model)
        {
            RequestResult<OfferCostModel> ER = new RequestResult<OfferCostModel>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("OfferCostId", typeof(int?));
                Id.Value = model.OfferCostId;

                ER = db.sp_InsUpdOfferCost(Id, model.OfferId, model.DailyRate)
                    .Select(x => new RequestResult<OfferCostModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message,
                        Data = model
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                    ER.Data.OfferCostId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }


        public List<OfferModel> GetAll(PagerModel pagerParameters, OfferModel parameters)
        {
            List<OfferModel> result = new List<OfferModel>();

            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_SelPagOffer(parameters.UserId, 
                                                    null, 
                                                    (parameters.OfferReceived != null) ? Convert.ToInt32(parameters.OfferReceived) : (int?)null, 
                                                    parameters.Status,
                                                    pagerParameters.Start,
                                                    pagerParameters.Offset == 0 ? 10 : pagerParameters.Offset,
                                                    pagerParameters.SortBy,
                                                    pagerParameters.SortDir).ToList();

                //VesselServices VesselServ = new VesselServices();
                //ProjectServices ProjectServ = new ProjectServices();
                //PersonServices PersonServ = new PersonServices();
                //PortServices PortServ = new PortServices();
                //RegionServices RegionServ = new RegionServices();
                //CountryServices CountryServ = new CountryServices();
                //CompanyServices CompanyServ = new CompanyServices();

                foreach (var citem in queryResult.ToList())
                {
                    OfferModel nItem = new OfferModel();

                    nItem.OfferId = citem.OfferId;
                    nItem.Status = citem.Status;
                    nItem.OfferedDate = citem.OfferDate;
                    nItem.OfferReferenceId = citem.OfferReferenceId;
                    nItem.Comment = citem.Comment;
                    nItem.OfferReceived = (citem.OfferReceived == 1) ? true : false;
                    nItem.Type = citem.Type;
                    nItem.MessageNotReaded = citem.MsgNotReaded;
                    nItem.VesselAdmin = new PersonModel()
                    {
                        PersonId = citem.VesselOwnerId,
                        FirstName = citem.VPFirstName,
                        LastName = citem.VPLastName,
                        Email = citem.VPEmail,
                        PhoneNumber = citem.VPPhoneNumber,
                        CompanyId = citem.VPCompanyId,
                        UserId = citem.VPUserId
                    };

                    nItem.ProjectAdmin = new PersonModel()
                    {
                        PersonId = citem.ProjectOwnerId,
                        FirstName = citem.PPFirstName,
                        LastName = citem.PPLastName,
                        Email = citem.PPEmail,
                        PhoneNumber = citem.PPPhoneNumber,
                        CompanyId = citem.PPCompanyId,
                        UserId = citem.PPUserId
                    };

                    nItem.Project = new ProjectModel()
                    {
                        Folio = citem.Folio,
                        ProjectId = citem.ProjectId,
                        ProjectTypeId = citem.PProjectTypeId,
                        ProjectType = citem.PProjectType,
                        StartDate = citem.PStartDate,
                        EndDate = citem.PEndDate,
                        Duration = citem.PDuration,
                        Extension = citem.PExtension,
                        RegionId = citem.PRegionId,
                        Region = citem.PRegion,
                        Budget = citem.PBudget,
                        FreeDeckArea = citem.PFreeDeckArea,
                        MudCapacity = citem.PMudCapacity,
                        CementTankCapacity = citem.PCementTankCapacity,
                        OilRecoveryCapacity = citem.POilRecoveryCapacity,
                        DynamicPositionSystem = citem.PDynamicPositionSystem,
                        DynamicPositionSystemName = citem.PDynamicPositionSystemName,
                        PemexCheck = citem.PPemexCheck,
                        SubtypeId = citem.PSubtypeId,
                        Subtype = citem.PSubtype,
                        BollardPull = citem.PBollardPull,
                        BollardPullAhead = citem.PBollardPullAhead,
                        BollardPullAstern = citem.PBollardPullAstern,
                        NumberPassenger = citem.PNumberPassenger,
                        //CabinQuantity = citem.PCabinSpecification,
                        //SingleBerth = citem.PSingleBerth,
                        //DoubleBerth = citem.PDoubleBerth,
                        //FourBerth = citem.PFourBerth,
                        AirCondition = citem.PAirCondition,
                        MessRoom = citem.PMessRoom,
                        ControlRoom = citem.PControlRoom,
                        ConferenceRoom = citem.PConferenceRoom,
                        Gymnasium = citem.PGym,
                        SwimingPool = citem.PPool,
                        Office = citem.POffice,
                        Hospital = citem.PHospital,
                        CargoCapacity = citem.PCargoCapacity,
                        DeckStrenght = citem.PDeckStrenght,
                        TankCapacity = citem.PTankCapacity,
                        DischargeRate = citem.PDischargeRate,
                        CompanyId = citem.PCompanyId,
                        CompanyName = citem.PCompanyName
                    };

                    nItem.Vessel = new VesselModel()
                    {
                        VesselId = citem.VesselId,
                        SuitabilityDescription = citem.VSuitability,
                        Name = citem.VVesselName,
                        Imo = citem.VIMO,
                        YearBuild = citem.VYearBuild,

                        ClassNotation = citem.VClassNotation,
                        ClassValidity = citem.VClassValidity,

                        VesselType = new VesselTypeModel()
                        {
                            Name = citem.VVesselType
                        },
                        ClasificationSociety = new ClasificationSocietyModel()
                        {
                            Name = citem.VClass,
                            Acronym = citem.VClassNotation
                        },
                        HomePort = new PortModel() {
                            Name = citem.VHomePort,
                            Region = new RegionModel()
                            {
                                Name = citem.VRegion
                            }
                        },
                        Company = new CompanyModel()
                        {
                            CompanyId = citem.VCompanyId,
                            CompanyName = citem.VCompanyName
                        },
                        Country = new CountryModel()
                        {
                            Name = citem.VCountry
                        },
                        Image = new FileModel() {
                            FileName = citem.VFileName,
                            Path = citem.VFilePath,
                            ContentType = citem.VFileContentType
                        },
                        VesselCost = new VesselCostModel()
                        {
                            VesselId = citem.VesselId,
                            LodgingCost = citem.VLodgingCost,
                            MealCost = citem.VMealCost,
                            LaundryCost = citem.VLaundryCost,
                            DailyRateTowing = citem.VDailyMaxRate,
                            DailyRatePersonnelTransportation = citem.VDailyMaxRate,
                            DailyRateMaterialTransportation = citem.VDailyMaxRate,
                            DailyRateFlotel = citem.VDailyMaxRate
                        }
                    };

                    nItem.VesselSpecificInfoModel = new VesselSpecificInfoModel()
                    {
                        VesselId = citem.VesselId,
                        GrossTonnage = citem.VGrossTonnage,
                        NetTonnage = citem.VNetTonnage,
                        BeamOverall = citem.VBeamOverall,
                        LengthOverall = citem.VLengthOverall,
                        MaximumLoadedDraft = citem.VMaximumLoadedDraft,
                        FreeDeckArea = citem.VFreeDeckArea,
                        DeckStrenght = citem.VDeckStrenght,
                        FreshWaterCapacity = citem.VFreshWaterCapacity,
                        FuelOilCapacity = citem.VFuelOilCapacity,
                        BallastWaterCapacity = citem.VBallastWaterCapacity,
                        MudCapacity = citem.VMudCapacity,
                        CementTanksCapacity = citem.VCementTankCapacity,
                        OilRecoveryCapacity = citem.VOilRecoveryCapacity,
                        WaterMarkerPlant = citem.VWaterMarkerPlant,
                        HotWaterCalorifier = citem.VHotWaterCalorifier,
                        SewageTreatmentPlant = citem.VSewageTreatmentPlant,
                        CruisingSpeed = citem.VCruisingSpeed,
                        MaximumSpeed = citem.VMaximunSpeed,
                        DistanceCruisingSpeed = citem.VDistanceCruisingSpeed,
                        DistanceMaxSpeed = citem.VDistanceMaxSpeed,
                        FuelConsumptionCruisingSpeed = citem.VFuelConsumptionCruisingSpeed,
                        FuelConsumptionMaxSpeed = citem.VFuelConsumptionMaxSpeed,
                        DynamicPositionSystem = citem.VDynamicPositionSystem,
                        DynamicPositionSystemName = citem.VDynamicPositionSystemName
                    };

                    nItem.VesselSpecificInfoModelExtra = new SpecificInformationModel()
                    {
                        BHP = citem.VBHP,
                        SubtypeId = citem.VSubtypeId,
                        Subtype = citem.VSubtype,
                        BollardPull = citem.VBollardPull,
                        BollardPullAhead = citem.VBollardPullAhead,
                        BollardPullAstern = citem.VBollardPullAstern,
                        NumberPassenger = citem.VNumberPassenger,
                        CabinQuantity = citem.VCabinSpecification,
                        SingleBerth = citem.VSingleBerth,
                        DoubleBerth = citem.VDoubleBerth,
                        FourBerth = citem.VFourBerth,
                        AirCondition = citem.VAirCondition,
                        MessRoom = citem.VMessRoom,
                        ControlRoom = citem.VControlRoom,
                        ConferenceRoom = citem.VConferenceRoom,
                        Gymnasium = citem.VGym,
                        SwimingPool = citem.VPool,
                        Office = citem.VOffice,
                        Hospital = citem.VHospital,
                        CargoCapacity = citem.VCargoCapacity,
                        PumpRates = citem.VPumpRates,
                        TankCapacity = citem.VTankCapacity,
                        DischargeRate = citem.PDischargeRate,
                        PemexCheck = citem.VPemexCheck,
                        DeckStrenght = citem.VDeckStrenght
                    };
                                          
                    result.Add(nItem);
                }
            }
            return result;
        }


    }
}
