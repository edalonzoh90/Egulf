using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Reports.Models
{
    public class AgreementReportModel
    {
        public string NumberID { get; set; }
        public int? Day { get; set; }
        public string Month { get; set; }
        public int? Year { get; set; }
        public string VesselOwnerCompanyName { get; set; }
        public string ProjectOwnerCompanyName { get; set; }
        public string VesselName { get; set; }
        public string VesselIMO { get; set; }
        public string ProjectType { get; set; }
        public string VesselOwnerCompanyRFC { get; set; }
        public string VesselOwnerCompanyAddress { get; set; }
        public string VesselOwnerName { get; set; }
        public string VesselOwnerEmail { get; set; }
        public string VesselOwnerPhone { get; set;  }
        public string ProjectOwnerCompanyRFC { get; set; }
        public string ProjectOwnerCompanyAddress { get; set; }
        public string ProjectOwnerName { get; set; }
        public string ProjectOwnerEmail { get; set; }
        public string ProjectOwnerPhone { get; set; }
        public string VesselYear { get; set; }
        public string VesselCountry { get; set; }
        public string ProjectArea { get; set; }
        public DateTime? ProjectPeriodHire { get; set; }
        public decimal? DailyRate { get; set; }
        public decimal? MealCost { get; set; }
        public decimal? LaundryCost { get; set; }
        public decimal? Accommodation { get; set; }
        public DateTime? ProjectStartDate { get; set; }
    }



    public enum TypeUser
    {
        Vessel,
        Project
    }


}
