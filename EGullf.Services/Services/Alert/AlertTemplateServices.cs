using EGullf.Services.DA.Alert;
using EGullf.Services.Models.Alert;

namespace EGullf.Services.Services.Alert
{
    public class AlertTemplateServices
    {
        public AlertTemplateModel GetById(int alertTemplateId)
        {
            AlertTemplateDA da = new AlertTemplateDA();
            return da.GetById(alertTemplateId);
        }
    }
}
