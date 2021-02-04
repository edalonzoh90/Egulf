using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.DA.Management;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Utils;
using System.Transactions;
using Security.Sevices;
using EGullf.Services.Services.Configuration;

namespace EGullf.Services.Services.Management
{
    public class InvitationCompanyServices
    {
        public List<InvitationCompanyModel> Get(PagerModel pager, InvitationCompanyModel filter)
        {
            InvitationCompanyDA da = new InvitationCompanyDA();
            return da.Get(pager, filter);
        }

        public RequestResult<object> InsUpd(InvitationCompanyModel model)
        {
            InvitationCompanyDA da = new InvitationCompanyDA();
            return da.InsUpd(model);
        }

        public RequestResult<object> ConfirmInvitation(InvitationCompanyModel invitation, PersonModel person) {
            TransactionOptions scp = new TransactionOptions();
            scp.IsolationLevel = IsolationLevel.ReadCommitted;


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scp))
            {
                try
                {
                    var response = InsUpd(invitation);
                    if (response.Status != Status.Success)
                        throw new Exception("InvitationCompanyServices.ConfirmInvitation: Error to update invitation - " + response.Message);
                    PersonServices psrv = new PersonServices();
                    var p = psrv.insUpdPerson(person);
                    if (response.Status != Status.Success)
                        throw new Exception("InvitationCompanyServices.ConfirmInvitation: Error to update person - " + response.Message);
                    CompanyServices CompanyServ = new CompanyServices();
                    CompanyServ.UpdateRolePerson(person.UserId, "UserCompanyRole");

                    ts.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new Exception("InvitationCompanyServices.ConfirmInvitation: Error to confirm invitation - " + ex.Message);
                }
            }


            return new RequestResult<object>() { Status=Status.Success};
        }

        public RequestResult<object> RemoveCompanyInvitation(InvitationCompanyModel model, PersonModel person)
        {
            TransactionOptions scp = new TransactionOptions();
            scp.IsolationLevel = IsolationLevel.ReadCommitted;


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scp))
            {
                try
                {

                    var response = DelWithCompanyPerson(model, person);
                    if (response.Status != Status.Success)
                        throw new Exception("InvitationCompanyServices.RemoveCompanyInvitation: Error to delete the invitation - " + response.Message);
                    ts.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new Exception("CompanyServices.RemoveCompany: Error to delete company - " + ex.Message);
                }
            }


            return new RequestResult<object>();
        }

        public RequestResult<object> DelWithCompanyPerson(InvitationCompanyModel model, PersonModel person)
        {
            InvitationCompanyDA da = new InvitationCompanyDA();
            return da.Del(model, person);
        }

        public RequestResult<object> Del(InvitationCompanyModel model)
        {
            InvitationCompanyDA da = new InvitationCompanyDA();
            return da.Del(model);
        }
    }
}
