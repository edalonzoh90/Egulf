using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Utils;
using System.Data.Entity.Core.Objects;


namespace EGullf.Services.DA.Management
{
    public class InvitationCompanyDA
    {
        public List<InvitationCompanyModel> Get(PagerModel pager, InvitationCompanyModel filter)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelInvitationsByCompanyId(filter.CompanyId, pager.Search, pager.Start, pager.Offset, pager.SortBy, pager.SortDir).ToList();

                if (resp.Count() > 0)
                {
                    var first = resp.FirstOrDefault();
                    pager.TotalRecords = first.TotalRecords.HasValue ? first.TotalRecords.Value : 0;
                }
                //return new List<InvitationCompanyModel>();
                return (from x in resp
                        select new InvitationCompanyModel()
                        {
                            CompanyInvitationId = x.CompanyInvitationId,
                            CompanyId = x.CompanyId,
                            From = x.From,
                            To = x.To,
                            CreatedAt = x.created_at,
                            Status = x.status,
                            Email = x.Email,
                            FirstName = x.FirstName,
                            LastName = x.LastName,
                            CompanyName = x.CompanyName
                        }).ToList();
            }
        }

        public RequestResult<object> InsUpd(InvitationCompanyModel model)
        {
            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("CompanyInvitationId", typeof(int?));
                Id.Value = model.CompanyInvitationId;

                var ER = db.sp_InsUpdCompanyInvitation(Id, model.CompanyId, model.From, model.To, model.CreatedAt, model.Status, model.Email).
                    Select(x => new ErrorResult()
                    {
                        IsError = x.IsError,
                        Message = x.Message,
                        Line = x.Line,
                        Subject = x.Subject
                    }).FirstOrDefault();
                //throw new Exception("Probando alert");
                if (ER != null && !string.IsNullOrEmpty(ER.Message))
                {
                    if (ER.IsError == true)
                        return new RequestResult<object>() { Status = Status.Error, Message = ER.Message };
                    else
                        return new RequestResult<object>() { Status = Status.Warning, Message = ER.Message };
                }
                else
                {
                    model.CompanyInvitationId = Convert.ToInt32(Id.Value.ToString());
                    return new RequestResult<object>() { Status = Status.Success, Data = model };
                }
            }
        }

        public RequestResult<object> Del(InvitationCompanyModel model, PersonModel person)
        {
            using (var db = new EGULFEntities())
            {


                var ER = db.sp_delCompanyInvitationPerson(model.CompanyInvitationId, person.PersonId).
                    Select(x => new ErrorResult()
                    {
                        IsError = x.IsError,
                        Message = x.Message,
                        Line = x.Line,
                        Subject = x.Subject
                    }).FirstOrDefault();
                if (ER != null && !string.IsNullOrEmpty(ER.Message))
                {
                    if (ER.IsError == true)
                        return new RequestResult<object>() { Status = Status.Error, Message = ER.Message };
                    else
                        return new RequestResult<object>() { Status = Status.Warning, Message = ER.Message };
                }
                return new RequestResult<object>() { Status = Status.Success, Data = model };
            }
        }

        public RequestResult<object> Del(InvitationCompanyModel model)
        {
            using (var db = new EGULFEntities())
            {


                var ER = db.sp_delCompanyInvitation(model.CompanyInvitationId).
                    Select(x => new ErrorResult()
                    {
                        IsError = x.IsError,
                        Message = x.Message,
                        Line = x.Line,
                        Subject = x.Subject
                    }).FirstOrDefault();
                if (ER != null && !string.IsNullOrEmpty(ER.Message))
                {
                    if (ER.IsError == true)
                        return new RequestResult<object>() { Status = Status.Error, Message = ER.Message };
                    else
                        return new RequestResult<object>() { Status = Status.Warning, Message = ER.Message };
                }
                return new RequestResult<object>() { Status = Status.Success, Data = model };
            }
        }
    }
}
