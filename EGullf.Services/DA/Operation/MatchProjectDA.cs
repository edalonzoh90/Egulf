using EGullf.Services.DA;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Services.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.DA.Operation
{
    public class MatchProjectDA: IMatchDA
    {



        public List<IMatchable> Get(IMatchable Matchable)
        {
            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_SelProjectSuitability(
                                                              Matchable.CompanyId,
                                                              Matchable.MatchableId,
                                                              Matchable.SuitabilityId,
                                                              Matchable.PemexCheck,
                                                              Matchable.StartDate,
                                                              Matchable.EndDate,
                                                              Matchable.RegionId,
                                                              Matchable.DynamicPositionSystem,
                                                              Matchable.DailyMaxRate,
                                                              Matchable.SubtypeId,
                                                              Matchable.BollardPull,
                                                              Matchable.BollardPullAhead,
                                                              Matchable.BollardPullAstern,
                                                              Matchable.NumberPassengers,
                                                              Matchable.CargoCapacity
                                                              ).ToList();

                List<IMatchable> result = new List<IMatchable>();
                foreach (var c in queryResult.ToList())
                {
                    IMatchable nItem = new MatchModel();
                    nItem.Offerted = c.Offerted;
                    nItem.MatchableId = c.VesselId;
                    nItem.SuitabilityId = c.ProjectTypeId;
                    nItem.PemexCheck = c.PemexCheck;
                    nItem.StartDate = c.StartDate;
                    nItem.EndDate = c.EndDate;
                    nItem.RegionId = c.RegionId;
                    nItem.DynamicPositionSystem = c.DynamicPositionSystem;
                    nItem.DynamicPositionSystemName = c.DynamicPositionSystemName;
                    nItem.DailyMaxRate = c.DailyMaxRate;
                    nItem.SubtypeId = c.SubtypeId;
                    nItem.Subtype = c.Subtype;
                    nItem.BollardPull = c.BollardPull;
                    nItem.BollardPullAhead = c.BollardPullAhead;
                    nItem.BollardPullAstern = c.BollardPullAstern;
                    nItem.NumberPassengers = c.NumberPassengers;
                    nItem.CargoCapacity = c.CargoCapacity;
                    nItem.BHP = c.BHP;
                    nItem.PumpRates = c.PumpRates;

                    nItem.VesselMatch = new VesselModel() { Location = new LatLng() { Lat = c.Lat, Lng = c.Lng },
                        VesselType = new VesselTypeModel() { Name = c.VesselType },
                        YearBuild = c.YearBuild,
                        Country = new CountryModel() { Name = c.Country },
                        HomePort = new PortModel() { Name = c.HomePort },
                        Imo = c.IMO,
                        Name = c.VesselName,
                        Company = new CompanyModel() { CompanyId = c.VesselCompanyId }
                    };
                    nItem.VesselSuitabilityProject = c.VesselProjectTypes;
                    nItem.IsMyVessel = (c.IsMyVessel == 1);
                    result.Add(nItem);
                }
                return result;
            }
        }





    }
}
