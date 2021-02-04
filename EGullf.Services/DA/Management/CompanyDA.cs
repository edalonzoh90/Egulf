using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Utils;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System.Data.Entity.Core.Objects;


namespace EGullf.Services.DA.Management
{
    public class CompanyDA
    {
        public List<CompanyModel> Get(PagerModel pager, CompanyModel filter)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelPagCompany(filter.CompanyId, filter.CompanyName, filter.RFC,  filter.Address, filter.PhoneNumber,filter.Email,
                pager.Start, pager.Offset, pager.SortBy, pager.SortDir).ToList();

                if (resp.Count() > 0)
                {
                    var first = resp.FirstOrDefault();
                    pager.TotalRecords = first.TotalRecords.HasValue ? first.TotalRecords.Value : 0;
                }

                return (from x in resp
                        select new CompanyModel()
                        {
                            CompanyId = x.CompanyId,
                            CompanyName = x.CompanyName.Trim(),
                            RFC = x.RFC.Trim(),
                            Address = x.Address.Trim(),
                            PhoneNumber = x.PhoneNumber.Trim(),
                            Email = x.Email.Trim(),

                        }).ToList();
            }
        }

        public List<CompanyModel> Get(CompanyModel filter)
        {
            using (var db = new EGULFEntities())
            {
                var resp = db.sp_SelCompany(filter.CompanyId, filter.CompanyName, filter.RFC, filter.Address, filter.PhoneNumber, filter.Email).ToList();
                return (from x in resp
                        select new CompanyModel()
                        {
                            CompanyId = x.CompanyId,
                            CompanyName = x.CompanyName,
                            RFC = x.RFC.Trim(),
                            Address = x.Address,
                            PhoneNumber = x.PhoneNumber,
                            Email = x.Email,
                        }).ToList();
            }
        }

        public CompanyModel GeyById(int CompanyId)
        {
            return Get(new CompanyModel() { CompanyId = CompanyId }).FirstOrDefault();
        }

        public RequestResult<object> InsUpd(CompanyModel model)
        {
            using (var db = new EGULFEntities())
            {
                ObjectParameter Id = new ObjectParameter("CompanyId", typeof(int?));
                Id.Value = model.CompanyId;

                var ER = db.sp_InsUpdCompany(Id, model.CompanyName, model.RFC, model.Address, model.PhoneNumber, model.Email).
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
                    model.CompanyId = Convert.ToInt32(Id.Value.ToString());
                    return new RequestResult<object>() { Status = Status.Success, Data = model };
                }
            }
        }

        public RequestResult<object> Del(CompanyModel model, PersonModel person)
        {
            using (var db = new EGULFEntities())
            {
                

                var ER = db.sp_delCompanyPerson(model.CompanyId, person.PersonId).
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
