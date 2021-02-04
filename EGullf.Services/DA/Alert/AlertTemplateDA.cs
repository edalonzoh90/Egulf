using EGullf.Services.Models.Alert;
using System.Linq;

namespace EGullf.Services.DA.Alert
{
    public class AlertTemplateDA
    {
        public AlertTemplateModel GetById(int AlertTemplateId)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelAlertTemplateById( AlertTemplateId).FirstOrDefault();

                return new AlertTemplateModel()
                        {
                            AlertTemplateId = resp.AlertTemplateId,
                            Description = resp.Description,
                            Subject = resp.Subject,
                            Body = resp.Body,
                            Url = resp.Url,
                            Params = resp.Params,
                            Extra = resp.Extra,
                            Icon = resp.Icon,
                        };
            }
        }
    }
}
