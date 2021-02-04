using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using EGullf.Services.Models.Operation;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Operation;
using System.Transactions;
using Security.Sevices;

namespace EGullf.Services.Services.Management
{
    public class CompanyServices
    {
        public CompanyModel GeyById(int CompanyId)
        {
            CompanyDA da = new CompanyDA();
            return da.GeyById(CompanyId);
        }

        public List<CompanyModel> Get(PagerModel pager, CompanyModel filter)
        {
            CompanyDA da = new CompanyDA();
            return da.Get(pager, filter);
        }

        public List<CompanyModel> Get(CompanyModel filter)
        {
            CompanyDA da = new CompanyDA();
            return da.Get(filter);
        }

        public RequestResult<object> AddCompany(CompanyModel model, PersonModel person = null)
        {

            if (person == null)
                return InsUpd(model);

            TransactionOptions scp = new TransactionOptions();
            scp.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scp))
            {
                try
                {
                    var response = InsUpd(model);
                    if (response.Status != Status.Success)
                        throw new Exception("CompanyServices.AddCompany: Error to register company");
                    if (response != null && response.Data != null)
                        person.CompanyId = ((CompanyModel)response.Data).CompanyId;
                    PersonServices psrv = new PersonServices();
                    var p = psrv.insUpdPerson(person);
                    UpdateRolePerson(person.UserId, "RolAdminCompany");
                    ts.Complete();
                    return response;
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    throw new Exception("CompanyServices.AddCompany: Error to register company");
                }
            }

        }

        public RequestResult<object> RemoveCompany(CompanyModel model, PersonModel person)
        {
            TransactionOptions scp = new TransactionOptions();
            scp.IsolationLevel = IsolationLevel.ReadCommitted;


            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scp))
            {
                try
                {

                    var response = Del(model, person);
                    if (response.Status != Status.Success)
                        throw new Exception("CompanyServices.RemoveCompany: Error to delete company - " + response.Message);

                    UpdateRolePerson(person.UserId, "DefaultNewUserRole");
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

        public RequestResult<object> InsUpd(CompanyModel model)
        {
            CompanyDA da = new CompanyDA();
            return da.InsUpd(model);
        }

        public RequestResult<object> Del(CompanyModel model, PersonModel person)
        {
            CompanyDA da = new CompanyDA();
            return da.Del(model, person);
        }

        public bool HasProjectsVesselsCompany(CompanyModel model)
        {
            ProjectServices pSrv = new ProjectServices();
            var project = pSrv.GetProjectCollection(new ProjectModel() { CompanyId = (int)model.CompanyId }, new PagerModel(0, 1, "", "")).Data.FirstOrDefault();
            if (project != null)
                return true;

            VesselServices vSrv = new VesselServices();
            var vessel = vSrv.GetFirst(new VesselModel() { Company = new CompanyModel() { CompanyId = model.CompanyId } });
            if (vessel != null)
                return true;
            
            return false;
        }

        public void UpdateRolePerson(int? userId, string rol)
        {
            UserServices userServ = new UserServices();
            SystemVariableServices SystemVariableServ = new SystemVariableServices();
            userServ.UpdateRoleUser(userId, int.Parse(SystemVariableServ.GetSystemVariableValue(rol)));
        }

    }
}
