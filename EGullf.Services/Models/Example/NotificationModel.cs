using System;

namespace EGullf.Services.Models.Example
{
    public class NotificationModel
    {
        public int? NotificationId { set; get; }

        public int? Type { set; get; }

        public DateTime? Date { set; get; }

        public string Description { set; get; }

        public int? Status { set; get; }

        public int? SourceId { set; get; }

    }
}
