using EGullf.Services.Models.Alert;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;

namespace EGullf.Services.DA.Alert
{
    public class AlertDA
    {
        public RequestResult<object> InsUpd(AlertModel model)
        {
            RequestResult<object> ER = new RequestResult<object>();

            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("AlertId", typeof(int?));
                Id.Value = model.AlertId;

                ER = db.sp_InsUpdAlert(Id, model.AlertTemplateId, model.From,
                    model.To, model.Subject, model.Body, model.Url, model.Extra,
                    model.Status, model.CreatedAt, model.UpdatedAt)
                    .Select(x => new RequestResult<object>()
                    {
                        Status = (bool)x.IsError ? Status.Error : Status.Success,
                        Message = x.Message
                    }).FirstOrDefault();

                if (ER == null)
                    model.AlertId = Convert.ToInt32(Id.Value.ToString());

                return ER;
            }
        }

        public List<AlertModel> Get(PagerModel pager, AlertModel filter)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelPagAlert(
                 filter.AlertId, filter.AlertTemplateId, filter.From, filter.To, filter.Subject, filter.Body, filter.Status,
                pager.Start, pager.Offset, pager.SortBy, pager.SortDir).ToList();

                if (resp.Count() > 0)
                {
                    var first = resp.FirstOrDefault();
                    pager.TotalRecords = first.TotalRecords.HasValue ? first.TotalRecords.Value : 0;
                }

                return (from x in resp
                        select new AlertModel()
                        {
                            AlertId = x.AlertId,
                            AlertTemplateId = x.AlertTemplateId,
                            From = x.From,
                            To = x.To,
                            Subject = x.Subject,
                            Body = x.Body,
                            Url = x.Url,
                            Extra = x.Extra,
                            Status = x.Status,
                            CreatedAt = x.CreatedAt,
                            UpdatedAt = x.UpdatedAt,
                            TimeAgo = x.TimeAgo,
                            Icon = x.Icon

                        }).ToList();
            }
        }
    }
}
