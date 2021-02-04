using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EGullf.Services.Models.Operation
{
    public class MessageModel
    {
        public int? MessageId { set; get; }

        public int? From { set; get; }

        public int? To { set; get; }

        public int? Status { set; get; }

        public string Message { set; get; }

        public DateTime? CreatedAt { set; get; }

        public int? ReferenceId { set; get; }
        public string Alias { set; get; }

    }
}
