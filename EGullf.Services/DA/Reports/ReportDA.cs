using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;
using System.Data;
using EGullf.Services.Models.Utils;
using EGullf.Services.Reports.Models;

namespace EGullf.Services.DA.Reports
{
    public class ReportDA
    {

        string reportType = "PDF";
        string mimeType;
        string encoding;
        string fileNameExtension = "pdf";
        Warning[] warnings;
        string[] streams;


        public byte[] AgreementReportFile(int OfferId, int UserType)
        {
            byte[] renderedBytes;
            string reportPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Reports");
            string rdlPath = Path.Combine(reportPath, "AgreementReport.rdlc");

            ReportViewer cReportViewer = new ReportViewer();
            cReportViewer.ProcessingMode = ProcessingMode.Local;

            LocalReport lReport = cReportViewer.LocalReport;
            lReport.ReportPath = rdlPath;

            List<AgreementReportModel> dsAgreement = new List<AgreementReportModel>();
            var result = AgreementReportData(OfferId, UserType);
            if (result.Status == Status.Success)
                dsAgreement.Add(result.Data);

            // Create a report data source 
            ReportDataSource rdlDS = new ReportDataSource("DTAgreement", dsAgreement);
            lReport.DataSources.Add(rdlDS);

            // Create a report parameters
            ReportParameter paramOfferId = new ReportParameter("OfferId", Convert.ToString(OfferId));
            ReportParameter paramUserType = new ReportParameter("UserType", Convert.ToString(UserType));

            //Set the report parameters for the report
           lReport.SetParameters(new List<ReportParameter>() { paramOfferId, paramUserType });

           lReport.Refresh();
           renderedBytes = lReport.Render(reportType,
                                          null,
                                          out mimeType,
                                          out encoding,
                                          out fileNameExtension,
                                          out streams,
                                          out warnings);
           return renderedBytes;           
        }
        


        public RequestResult<AgreementReportModel> AgreementReportData(int OfferId, int? UserType)
        {
            RequestResult<AgreementReportModel> result = new RequestResult<AgreementReportModel>();
            using (var db = new EGULFEntities())
            {
                var queryResult = db.sp_RptAgreement(OfferId,
                                                     UserType).Select(x => new AgreementReportModel() {
                                                                       NumberID = x.NumberID,
                                                                       Day = x.Day,
                                                                       Month = x.Month,
                                                                       Year = x.Year,
                                                                       VesselOwnerCompanyName = x.VesselOwnerCompanyName,
                                                                       ProjectOwnerCompanyName = x.ProjectOwnerCompanyName,
                                                                       VesselName = x.VesselName,
                                                                       VesselIMO = x.VesselIMO,
                                                                       ProjectType = x.ProjectType,
                                                                       VesselOwnerCompanyRFC = x.VesselOwnerCompanyRFC,
                                                                       VesselOwnerCompanyAddress = x.VesselOwnerCompanyAddress,
                                                                       VesselOwnerName = x.VesselOwnerName,
                                                                       VesselOwnerEmail = x.VesselOwnerEmail,
                                                                       VesselOwnerPhone = x.VesselOwnerPhone,
                                                                       ProjectOwnerCompanyRFC = x.ProjectOwnerCompanyRFC,
                                                                       ProjectOwnerCompanyAddress = x.ProjectOwnerCompanyAddress,
                                                                       ProjectOwnerName = x.ProjectOwnerName,
                                                                       ProjectOwnerEmail = x.ProjectOwnerEmail,
                                                                       ProjectOwnerPhone = x.ProjectOwnerPhone,
                                                                       VesselYear = x.VesselYear,
                                                                       VesselCountry = x.VesselCountry,
                                                                       ProjectArea = x.ProjectArea,
                                                                       ProjectPeriodHire = x.ProjectPeriodHire,
                                                                       DailyRate = x.DailyRate,
                                                                       MealCost = x.MealCost,
                                                                       LaundryCost = x.LaundryCost,
                                                                       Accommodation = x.Accommodation,
                                                                       ProjectStartDate = x.ProjectStartDate                        
                                                                     }).ToList();

                result.Data = queryResult.FirstOrDefault();
                result.Status = Status.Success;
            }
            return result;
        }     
    








    }
}
