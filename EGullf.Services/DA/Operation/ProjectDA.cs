using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Operation;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.DA.Operation
{
    public class ProjectDA
    {


        public RequestResult<ProjectModel> GetProject(ProjectModel parameters)
        {
            SystemVariableServices SystemVariableServ = new SystemVariableServices();
            CabinSpecificationServices CabinSpecificationServ = new CabinSpecificationServices();
            int TypeProject = Convert.ToInt16(SystemVariableServ.GetSystemVariableValue("TypeProject"));

            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_SelProject(parameters.CompanyId,
                                                   parameters.ProjectId).Select(x => new ProjectModel()
                                                     {
                                                       Folio = x.Folio,
                                                       ProjectId = x.ProjectId,
                                                       ProjectTypeId = x.ProjectTypeId,
                                                       ProjectType = x.ProjectType,
                                                       StartDate = x.StartDate,
                                                       Duration = x.Duration,
                                                       Extension = x.Extension,
                                                       RegionId = x.RegionId,
                                                       Region = x.Region,
                                                       Budget = x.Budget,
                                                       MaxRateBudget = x.MaxRateBudget,
                                                       FreeDeckArea = x.FreeDeckArea,
                                                       MudCapacity = x.MudCapacity,
                                                       CementTankCapacity = x.CementTankCapacity,
                                                       OilRecoveryCapacity = x.OilRecoveryCapacity,
                                                       DynamicPositionSystem = x.DynamicPositionSystem,
                                                       Status = x.Status,
                                                       //Lat = x.Lat,
                                                       //Lng = x.Lng,
                                                       CompanyId = (int)x.CompanyId,
                                                       CompanyName = x.CompanyName,
                                                       //BHP = x.BHP,
                                                       SubtypeId = x.SubtypeId,
                                                       BollardPull = x.BollardPull,
                                                       BollardPullAhead = x.BollardPullAhead,
                                                       BollardPullAstern = x.BollardPullAstern,
                                                       NumberPassenger = x.NumberPassenger,
                                                       CabinSpecification = CabinSpecificationServ.GetByReferenceId(x.ProjectId,TypeProject),
                                                       AirCondition = x.AirCondition,
                                                       MessRoom = x.MessRoom,
                                                       ControlRoom = x.ControlRoom,
                                                       ConferenceRoom = x.ConferenceRoom,
                                                       Gymnasium = x.Gym,
                                                       SwimingPool = x.Pool,
                                                       Office = x.Office,
                                                       Hospital = x.Hospital,
                                                       CargoCapacity = x.CargoCapacity,
                                                       PumpRates = x.PumpRates,
                                                       TankCapacity = x.TankCapacity,
                                                       DischargeRate = x.DischargeRate,
                                                       PemexCheck = x.PemexCheck,
                                                       DeckStrenght = x.DeckStrenght,
                                                       Type = x.Type,
                                                       CancelationComment = x.CancelationComments
                                                   }).FirstOrDefault();

                return new RequestResult<ProjectModel>() { Status = Status.Success, Data = queryResult };
            }
        }


        public RequestResult<List<ProjectModel>> GetProjectCollection(ProjectModel parameters, PagerModel pagerParameters)
        {
            SystemVariableServices SystemVariableServ = new SystemVariableServices();
            CabinSpecificationServices CabinSpecificationServ = new CabinSpecificationServices();
            int TypeProject = Convert.ToInt16(SystemVariableServ.GetSystemVariableValue("TypeProject"));

            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_SelPagProject(parameters.CompanyId,
                                                        parameters.ProjectId,
                                                        parameters.ProjectTypeId,
                                                        parameters.StartDate,
                                                        parameters.RegionId,
                                                        parameters.Status,
                                                        pagerParameters.Start,
                                                        pagerParameters.Offset,
                                                        pagerParameters.SortBy,
                                                        pagerParameters.SortDir
                                                     ).Select(x => new ProjectModel() {
                                                         Number = x.Number,
                                                         Total = x.Total,
                                                         Folio = x.Folio,
                                                         ProjectId = x.ProjectId,
                                                         ProjectTypeId = x.ProjectTypeId,
                                                         ProjectType = x.ProjectType,
                                                         StartDate = x.StartDate,
                                                         Duration = x.Duration,
                                                         Extension = x.Extension,
                                                         RegionId = x.RegionId,
                                                         Region = x.Region,
                                                         Budget = x.Budget,
                                                         MaxRateBudget = x.MaxRateBudget,
                                                         FreeDeckArea = x.FreeDeckArea,
                                                         MudCapacity = x.MudCapacity,
                                                         CementTankCapacity = x.CementTankCapacity,
                                                         OilRecoveryCapacity = x.OilRecoveryCapacity,
                                                         DynamicPositionSystem = x.DynamicPositionSystem,
                                                         Status = x.Status,
                                                         StatusDescription = x.StatusDescription,
                                                         //Lat = x.Lat,
                                                         //Lng = x.Lng,
                                                         CompanyId = (int)x.CompanyId,
                                                         CompanyName = x.CompanyName,
                                                         //BHP = x.BHP,
                                                         SubtypeId = x.SubtypeId,
                                                         BollardPull = x.BollardPull,
                                                         BollardPullAhead = x.BollardPullAhead,
                                                         BollardPullAstern = x.BollardPullAstern,
                                                         NumberPassenger = x.NumberPassenger,
                                                         CabinSpecification = CabinSpecificationServ.GetByReferenceId(x.ProjectId,TypeProject),
                                                         AirCondition = x.AirCondition,
                                                         MessRoom = x.MessRoom,
                                                         ControlRoom = x.ControlRoom,
                                                         ConferenceRoom = x.ConferenceRoom,
                                                         Gymnasium = x.Gym,
                                                         SwimingPool = x.Pool,
                                                         Office = x.Office,
                                                         Hospital = x.Hospital, 
                                                         CargoCapacity = x.CargoCapacity,
                                                         PumpRates = x.PumpRates,
                                                         TankCapacity = x.TankCapacity,
                                                         DischargeRate = x.DischargeRate,
                                                         PemexCheck = x.PemexCheck,
                                                         DeckStrenght = x.DeckStrenght,
                                                         Type = x.Type
                                                     }).ToList();

                if (queryResult.Count > 0)
                {
                    var data = queryResult.FirstOrDefault();
                    pagerParameters.TotalRecords = data.Total.HasValue ? data.Total.Value : 0;
                }

                return new RequestResult<List<ProjectModel>>() { Status = Status.Success, Data = queryResult };
            }
        }

        public RequestResult<object> UpdStatus(int? projectId, int statusId)
        {
            RequestResult<object> ER = new RequestResult<object>() { Status = Status.Success };

            using (var db = new EGULFEntities())
            {

                var queryResult = db.sp_UpdStatus(projectId,statusId)
                        .Select(x => new RequestResult<VesselModel>()
                        {
                            Status = (bool)x.IsError ? Status.Error : Status.Success,
                            Message = x.Message,
                        }).FirstOrDefault();

                    return ER;
            }
        }

        public RequestResult<object> InsUpdProjectInfo(ProjectModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                ObjectParameter ProjectId = new ObjectParameter("ProjectId", typeof(int?));
                ProjectId.Value = parameters.ProjectId;

                var queryResult = db.sp_InsUpdProject(ProjectId,
                                                    parameters.ProjectTypeId,
                                                    parameters.StartDate,
                                                    parameters.Duration,
                                                    parameters.Extension,
                                                    parameters.RegionId,
                                                    parameters.Budget,
                                                    parameters.MaxRateBudget,
                                                    parameters.FreeDeckArea,
                                                    parameters.MudCapacity,
                                                    parameters.CementTankCapacity,
                                                    parameters.OilRecoveryCapacity,
                                                    parameters.DynamicPositionSystem,
                                                    parameters.Lat,
                                                    parameters.Lng,
                                                    parameters.CompanyId,
                                                    parameters.Status,
                                                    parameters.Username
                                                    ).FirstOrDefault();

                if (queryResult != null && !string.IsNullOrEmpty(queryResult.Message))
                {
                    return new RequestResult<object>() { Status = Status.Error, Message = queryResult.Message };
                }
                else
                {
                    parameters.ProjectId = Convert.ToInt32(ProjectId.Value.ToString());
                    return new RequestResult<object>() { Status = Status.Success, Data = parameters };
                }
            }
        }


        public RequestResult<object> DelProject(ProjectModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_DelProject(parameters.ProjectId,
                                                   parameters.CancelationComment
                                                   ).FirstOrDefault();

                if (queryResult != null && !string.IsNullOrEmpty(queryResult.Message))
                {
                    return new RequestResult<object>() { Status = Status.Error, Message = queryResult.Message };
                }
                else
                {
                    return new RequestResult<object>() { Status = Status.Success };
                }
            }
        }


        public RequestResult<object> FinishProject(ProjectModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_InsUpdFinishProject(parameters.ProjectId).FirstOrDefault();

                if (queryResult != null && !string.IsNullOrEmpty(queryResult.Message))
                    return new RequestResult<object>() { Status = Status.Error, Message = queryResult.Message };
                else
                    return new RequestResult<object>() { Status = Status.Success };
            }
        }

        public RequestResult<object> UpdStatus(ProjectModel parameters)
        {
            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_InsUpdFinishProject(parameters.ProjectId).FirstOrDefault();

                if (queryResult != null && !string.IsNullOrEmpty(queryResult.Message))
                    return new RequestResult<object>() { Status = Status.Error, Message = queryResult.Message };
                else
                    return new RequestResult<object>() { Status = Status.Success };
            }
        }




    }
}
