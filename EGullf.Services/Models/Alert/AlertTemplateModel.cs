namespace EGullf.Services.Models.Alert
{
    public class AlertTemplateModel
    {
        public int? AlertTemplateId { set; get; }
        public string Description { set; get; }
        public string Subject { set; get; }
        public string Body { set; get; }
        public string Url { set; get; }
        public string Params { set; get; }
        public string Extra { set; get; }
        public string Icon { set; get; }

        public AlertModel ToAlert()
        {
            AlertModel alert = new AlertModel();

            alert.Body = Body;
            alert.Extra = Extra;
            alert.Subject = Subject;
            alert.Url = Url;

            alert.AlertTemplateId = AlertTemplateId;
            alert.Icon = Icon;
            alert.Status = (int)AlertStatus.New;
            alert.TimeAgo = "Recien";

            return alert;
        }
    }
}
