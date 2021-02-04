using EGullf.Services.DA.Reports;
using EGullf.Services.Models.Mail;
using EGullf.Services.Models.Operation;
using EGullf.Services.Reports.Models;
using EGullf.Services.Services.Operation;
using System;

namespace EGullf.Services.Services.Reports
{
    public class ReportServices
    {


        public byte[] AgreementReportFile(int OfferId, int UserType)
        {
            ReportDA ReportDAL = new ReportDA();
            return ReportDAL.AgreementReportFile(OfferId, UserType);
        }


        public byte[] GetAgreementReport(int OfferId, int UserId)
        {
            byte[] file = null;

            //get offer data
            OfferServices OfferServ = new OfferServices();
            OfferModel offerData = new OfferModel();
            offerData = OfferServ.GetById(OfferId);
            if (offerData == null)
                throw new Exception("NOT_FOUND");
            if(UserId != offerData.VesselAdmin.UserId && UserId != offerData.ProjectAdmin.UserId)
                throw new Exception("UNAUTHORIZED");
            
            if (UserId == offerData.VesselAdmin.UserId)
                file = AgreementReportFile(OfferId, (int)TypeUser.Vessel);
            else if (UserId == offerData.ProjectAdmin.UserId)
                file = AgreementReportFile(OfferId, (int)TypeUser.Project);

            return file;
        }


        public MailAttachments AgreementReportAttachment(int OfferId, int UserType)
        {
            ReportDA ReportDAL = new ReportDA();
            byte[] AgreementReport = ReportDAL.AgreementReportFile(OfferId, UserType);

            MailAttachments AgreementReportAttachment = new MailAttachments();
            AgreementReportAttachment.file = AgreementReport;
            AgreementReportAttachment.fileName = "AgreementReport.pdf";
            AgreementReportAttachment.contentType = "application/pdf";

            return AgreementReportAttachment;
        }



    }
}
