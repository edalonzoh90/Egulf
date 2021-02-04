using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Example;
using System.Collections.Generic;
using System.Linq;
using EGullf.Services.Models.Utils;
using System;
using System.Data.Entity.Core.Objects;

namespace EGullf.Services.DA.Example
{
    public class NotificationDA
    {
        public List<NotificationModel> Get(PagerModel pager, NotificationModel filter)
        {
            using (var db = new EGULFEntities())
                {
                var resp = db.sp_SelPagNotification(
                 filter.NotificationId, filter.Type, filter.Date, pager.Search, filter.Status, filter.SourceId,
                pager.Start, pager.Offset, pager.SortBy, pager.SortDir).ToList();

                if (resp.Count() > 0)
                {
                    var first = resp.FirstOrDefault();
                    pager.TotalRecords = first.TotalRecords.HasValue ? first.TotalRecords.Value : 0;
                }

                return (from x in resp
                        select new NotificationModel()
                        {
                            NotificationId = x.NotificationId,
                            Type = x.Type,
                            Date = x.Date,
                            Description = x.Description,
                            Status = x.Status,
                            SourceId = x.SourceId,

                        }).ToList();
            }
        }

        public RequestResult<object> InsUpd(NotificationModel model)
        {
            RequestResult<object> ER = new RequestResult<object>();

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("NotificationId", typeof(int?));
                Id.Value = model.NotificationId;

                ER = db.sp_InsUpdNotification(Id, model.Type, model.Date, model.Description, model.Status, model.SourceId)
                    .Select(x => new RequestResult<object>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                if (ER.Status == Status.Success)
                    model.NotificationId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }
    }
}
