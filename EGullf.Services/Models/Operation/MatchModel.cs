using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Operation
{
    public class MatchModel: IMatchable 
    {
        public bool? IsMyVessel { get; set; }
        public int? CompanyId { get; set; } //for filter, to exclude this Company
        public int? Offerted { get; set; }
        public int? MatchableId { get; set; } //for project id or vessel id
        public int? SuitabilityId { get; set; } //for project type
        public bool? PemexCheck { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? RegionId { get; set; }
        public decimal? DailyMaxRate { get; set; }

        public int? DynamicPositionSystem { get; set; }
        public string DynamicPositionSystemName { get; set; }
        public int? SubtypeId { get; set; }
        public string Subtype { get; set; }
        public decimal? BollardPull { get; set; }
        public decimal? BollardPullAhead { get; set; }
        public decimal? BollardPullAstern { get; set; }
        public int? NumberPassengers { get; set; }
        public decimal? CargoCapacity { get; set; }
        public decimal? PumpRates { get; set; }
        public decimal? BHP { get; set; }

        public object VesselMatch { get; set; }
        public object ProjectMatch { get; set; }  

        //added for suitability
        public string VesselSuitabilityProject { get; set; }

        public IMatchable item { get; set; }
    }

    public enum TypeMatch
    {
        Vessel,
        Project
    }


}
