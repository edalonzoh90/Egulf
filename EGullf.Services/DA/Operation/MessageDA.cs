using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Operation
{
    public class MessageDA
    {
        public RequestResult<MessageModel> InsUpd(MessageModel model)
        {
            RequestResult<MessageModel> ER = new RequestResult<MessageModel>();

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("MessageId", typeof(int?));
                ObjectParameter CreatedAt = new ObjectParameter("CreatedAt", typeof(DateTime?));
                ObjectParameter _Status = new ObjectParameter("Status", typeof(int?));
                Id.Value = model.MessageId;
                CreatedAt.Value = model.CreatedAt;
                _Status.Value = model.Status;

                ER = db.sp_InsUpdMessage(Id, model.ReferenceId, model.From, model.To, _Status, model.Message, CreatedAt)
                    .Select(x => new RequestResult<MessageModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message,
                        Data = model
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                {
                    ER.Data.MessageId = Convert.ToInt32(Id.Value.ToString());
                    ER.Data.CreatedAt = Convert.ToDateTime(CreatedAt.Value);
                    ER.Data.Status = Convert.ToInt32(_Status.Value.ToString());
                }

                return ER;
            }
        }

        public RequestResult<MessageModel> MarkAsReaded(MessageModel model)
        {
            RequestResult<MessageModel> ER = new RequestResult<MessageModel>();

            using (var db = new EGULFEntities())
            {
                ER = db.sp_UpdMessageAsReaded(model.ReferenceId, model.From)
                    .Select(x => new RequestResult<MessageModel>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message,
                        Data = model
                    }).FirstOrDefault();

                return ER;
            }
        }

        public List<MessageModel> Get(PagerModel pager, MessageModel filter)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelPagMessage(
                 filter.MessageId, filter.ReferenceId, filter.From, filter.To, filter.Status,
                pager.Start, pager.Offset).ToList();

                if (resp.Count() > 0)
                {
                    var first = resp.FirstOrDefault();
                    pager.TotalRecords = first.TotalRecords.HasValue ? first.TotalRecords.Value : 0;
                }

                return (from x in resp
                        select new MessageModel()
                        {
                            ReferenceId = x.ReferenceId,
                            MessageId = x.MessageId,
                            From = x.From,
                            To = x.To,
                            Status = x.Status,
                            Message = x.Message,
                            CreatedAt = x.CreatedAt,
                            Alias = x.Alias
                        }).ToList();
            }
        }
    }
}
