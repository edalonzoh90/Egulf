using System.Collections.Generic;

namespace EGullf.Services.Models.Operation
{
    public class SpecificInformationModel
    {
        public static int VESSEL_TYPE = 1;

        public static int PROJECT_TYPE = 2;

        public int? MatchableId { get; set; } //not null

        public decimal? BHP { get; set; }

        public int? SubtypeId { get; set; } //not null

        public string Subtype { get; set; }

        public decimal? BollardPull { get; set; }

        public decimal? BollardPullAhead { get; set; }

        public decimal? BollardPullAstern { get; set; }

        public int? NumberPassenger { get; set; }

        public int? SingleBerth { get; set; }

        public int? DoubleBerth { get; set; }

        public int? FourBerth { get; set; }

        public int? CabinQuantity { get; set; }

        public bool? AirCondition { get; set; }

        public bool? MessRoom { get; set; }

        public bool? ControlRoom { get; set; }

        public bool? ConferenceRoom { get; set; }

        public bool? Gymnasium { get; set; }

        public bool? SwimingPool { get; set; }

        public bool? Office { get; set; }

        public bool? Hospital { get; set; }

        public decimal? CargoCapacity { get; set; }

        public decimal? PumpRates { get; set; }

        public decimal? TankCapacity { get; set; }

        public decimal? DischargeRate { get; set; }

        public bool? PemexCheck { get; set; }

        public decimal? DeckStrenght { get; set; }

        public int? Type { get; set; }

        public List<CabinSpecificationModel> CabinSpecification { get; set; }

        public List<CabinSpecificationModel> GetCabinSpecificationList(int Type)
        {
            List<CabinSpecificationModel> resp = new List<CabinSpecificationModel>();

            CabinSpecificationModel c1 = new CabinSpecificationModel();
            c1.Type = Type;
            c1.CabinQuantity = SingleBerth;
            c1.ReferenceId = MatchableId;
            c1.CabinType = CabinSpecificationModel.SINGLE_BERTH;
            resp.Add(c1);

            CabinSpecificationModel c2 = new CabinSpecificationModel();
            c2.Type = Type;
            c2.CabinQuantity = DoubleBerth;
            c2.ReferenceId = MatchableId;
            c2.CabinType = CabinSpecificationModel.DOUBLE_BERTH;
            resp.Add(c2);

            CabinSpecificationModel c3 = new CabinSpecificationModel();
            c3.Type = Type;
            c3.CabinQuantity = FourBerth;
            c3.ReferenceId = MatchableId;
            c3.CabinType = CabinSpecificationModel.FOUR_BERTH;
            resp.Add(c3);

            return resp;
        }
    }
}
