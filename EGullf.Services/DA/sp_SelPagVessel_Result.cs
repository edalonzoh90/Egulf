//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EGullf.Services.DA
{
    using System;
    
    public partial class sp_SelPagVessel_Result
    {
        public Nullable<int> TotalRecords { get; set; }
        public int VesselId { get; set; }
        public Nullable<int> Status { get; set; }
        public string Name { get; set; }
        public string IMO { get; set; }
        public int CountryId { get; set; }
        public int YearBuild { get; set; }
        public int ClasificationSocietyId { get; set; }
        public string ClassNotation { get; set; }
        public System.DateTime ClassValidity { get; set; }
        public Nullable<int> VesselTypeId { get; set; }
        public int PortId { get; set; }
        public Nullable<int> FileReferenceId { get; set; }
        public int CompanyId { get; set; }
        public decimal Lat { get; set; }
        public decimal Lng { get; set; }
        public int RegionId { get; set; }
        public string VesselType { get; set; }
    }
}