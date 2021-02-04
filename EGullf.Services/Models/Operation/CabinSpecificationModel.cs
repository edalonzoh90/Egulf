namespace EGullf.Services.Models.Operation
{
    public class CabinSpecificationModel
    {
        public static int VESSEL_TYPE = 1;
        public static int PROYECT_TYPE = 2;
        public static int SINGLE_BERTH = 1;
        public static int DOUBLE_BERTH = 2;
        public static int FOUR_BERTH = 3;

        public int? CabinSpecificationId { set; get; }

        public int? ReferenceId { set; get; }

        public int? CabinType { set; get; }

        public int? CabinQuantity { set; get; }

        public int? Type { get; set; }
    }
}
