using System;

namespace EGullf.Services.Models.Alert
{
    public class AlertModel
    {
        public int? AlertId { set; get; }
        public int? AlertTemplateId { set; get; }
        public int? From { set; get; }
        public int? To { set; get; }
        public string Subject { set; get; }
        public string Body { set; get; }
        public string Url { set; get; }
        public string Extra { set; get; }
        public string Icon { set; get; }
        public int? Status { set; get; }
        public DateTime? CreatedAt { set; get; }
        public DateTime? UpdatedAt { set; get; }
        public string TimeAgo { set; get; }

        public AlertModel Clone()
        {
            return new AlertModel
            {
                AlertId = this.AlertId,
                AlertTemplateId = this.AlertTemplateId,
                From = this.From,
                To = this.To,
                Subject = this.Subject,
                Body = this.Body,
                Url = this.Url,
                Extra = this.Extra,
                Icon = this.Icon,
                Status = this.Status,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt,
                TimeAgo = this.TimeAgo
            };
        }
    }

    public enum AlertStatus
    {

        None, New, Readed
    }
}
