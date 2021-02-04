using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Operation
{
    public interface IMatchable
    {
        bool? IsMyVessel { get; set; }
        int? Offerted { get; set; } //for filter, to exclude this Company
        int? CompanyId { get; set; } //for filter, to exclude this Company
        int? MatchableId { get; set; } //for project id or vessel id
        int? SuitabilityId { get; set; } //for project type
        bool? PemexCheck { get; set; }
        DateTime? StartDate { get; set; }
        DateTime? EndDate { get; set; }
        int? RegionId { get; set; }
        decimal? DailyMaxRate { get; set; }

        int? DynamicPositionSystem { get; set; }
        string DynamicPositionSystemName { get; set; }
        int? SubtypeId { get; set; }
        string Subtype { get; set; }
        decimal? BollardPull { get; set; }
        decimal? BollardPullAhead { get; set; }
        decimal? BollardPullAstern { get; set; }
        int? NumberPassengers { get; set; }
        decimal? CargoCapacity { get; set; }
        decimal? BHP { get; set; }
        decimal? PumpRates { get; set; }

        object VesselMatch { get; set; }
        object ProjectMatch { get; set; }

        string VesselSuitabilityProject { get; set; }
    }
}
