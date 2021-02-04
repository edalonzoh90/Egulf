using System;

namespace EGullf.Services.Models.Operation
{
    public class ProjectModel : SpecificInformationModel
    {
        public static int STATUS_NEW = 1;
        public static int STATUS_CANCELED = 2;
        public static int STATUS_ENDED = 3;
        public static int STATUS_FIXED = 4;

        public long? Number { get; set; }

        public int? Total { get; set; }

        public int? ProjectId { get; set; }
        public string Folio { get; set; }

        public int ProjectTypeId { get; set; }

        public string ProjectType { get; set; }

        public DateTime? StartDate { get; set; }

        //Es calculado en la consulta
        public DateTime? EndDate { get; set; }

        public int Duration { get; set; }

        public int? Extension { get; set; }

        public int RegionId { get; set; }

        public string Region { get; set; }

        public int Budget { get; set; }

        public decimal? MaxRateBudget { get; set; }

        public decimal? FreeDeckArea { get; set; }

        public decimal? MudCapacity { get; set; }

        public decimal? CementTankCapacity { get; set; }

        public decimal? OilRecoveryCapacity { get; set; }

        public int? DynamicPositionSystem { get; set; }

        public string DynamicPositionSystemName { get; set; }

        public int? Status { get; set; }

        public string StatusDescription { get; set; }

        public int? CompanyId { get; set; }

        public string CompanyName { get; set; }

        public decimal? Lat { get; set; }

        public decimal? Lng { get; set; }

        public string Username { get; set; }

        public string CancelationComment { get; set; }
        
    }
}
