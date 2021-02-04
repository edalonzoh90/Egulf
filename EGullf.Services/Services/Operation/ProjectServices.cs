using EGullf.Services.DA.Operation;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGullf.Services.Services.Configuration;
using EGullf.Services.Services.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace EGullf.Services.Services.Operation
{
    public class ProjectServices
    {

        public RequestResult<ProjectModel> GetProject(ProjectModel parameters)
        {
            ProjectDA DAProject = new ProjectDA();
            return DAProject.GetProject(parameters);
        }

        public RequestResult<List<ProjectModel>> GetProjectCollection(ProjectModel parameters, PagerModel pagerParameters)
        {
            ProjectDA DAProject = new ProjectDA();
            return DAProject.GetProjectCollection(parameters,pagerParameters);
        }

        public List<ProjectModel> GetProjectCollection(ProjectModel parameters)
        {
            PagerModel pagerParameters = new PagerModel(0, int.MaxValue-1, "", "");
            ProjectDA DAProject = new ProjectDA();
            return DAProject.GetProjectCollection(parameters, pagerParameters).Data;
        }


        public RequestResult<object> SaveProject(ProjectModel parameters)
        {
            TransactionOptions scopeOptions = new TransactionOptions();
            //scopeOptions.IsolationLevel = IsolationLevel.ReadCommitted;
            using (TransactionScope ts = new TransactionScope(TransactionScopeOption.Required, scopeOptions))
            {
                try
                {
                    //here we save basic project info
                    var respSaveProjectBasicInfo = InsUpdProjectInfo(parameters);
                    if (respSaveProjectBasicInfo.Status == Status.Success)
                    {
                        SystemVariableServices SystemVariableServ = new SystemVariableServices();
                        Int16 TypeProject = Convert.ToInt16(SystemVariableServ.GetSystemVariableValue("TypeProject"));

                        var dataProjectBasicInfo = (ProjectModel)respSaveProjectBasicInfo.Data;
                        parameters.ProjectId = dataProjectBasicInfo.ProjectId;

                        //we save specific info
                        SpecificInformationServices SpecificInfoServ = new SpecificInformationServices();
                        SpecificInformationModel SpecificInfoModel = parameters;
                        SpecificInfoModel.MatchableId = parameters.ProjectId;
                        SpecificInfoModel.Type = TypeProject;
                        var respSaveProjectSpecificInfo = SpecificInfoServ.InsUpd(SpecificInfoModel);
                        if (respSaveProjectSpecificInfo.Status == Status.Success)
                        {
                            //we validate project category and only save cabin specifications if project category is personnel transportation   
                            ProjectTypeServices ProjectTypeServ = new ProjectTypeServices();
                            ProjectTypeModel ProjectType = ProjectTypeServ.GetById(parameters.ProjectTypeId);
                            string CategoryPersonnelTransportation = SystemVariableServ.GetSystemVariableValue("CategoryPersonnelTransportation");
                            if (ProjectType.Category == CategoryPersonnelTransportation)
                            {
                                ////at the end we save cabin specifications 
                                //CabinSpecificationServices CabinSpecificationServ = new CabinSpecificationServices();
                                //foreach (var item in parameters.CabinSpecification.ToList())
                                //{
                                //    item.ReferenceId = parameters.ProjectId;
                                //    item.Type = TypeProject;
                                //    var respSaveCabinSpecification = CabinSpecificationServ.InsUpd(item);
                                //    if (respSaveCabinSpecification.Status == Status.Error)
                                //        throw new Exception(respSaveCabinSpecification.Message);
                                //}
                            }                        

                            ts.Complete();
                            return new RequestResult<object>() { Status = Status.Success, Data = parameters };
                        }
                        else
                        {
                            throw new Exception(respSaveProjectSpecificInfo.Message);
                        }         
                    }
                    else
                    {
                        //return respSaveProjectBasicInfo;
                        throw new Exception(respSaveProjectBasicInfo.Message);
                    }
                }
                catch (Exception ex)
                {
                    ts.Dispose();
                    return new RequestResult<object>() { Status = Status.Error, Message = ex.Message };
                }
            }
        }


        public RequestResult<object> InsUpdProjectInfo(ProjectModel parameters)
        {
            ProjectDA DAProject = new ProjectDA();
            return DAProject.InsUpdProjectInfo(parameters);
        }


        public RequestResult<object> ValidateStartDate(int? ProjectId, DateTime StartDate, int CompanyId)
        {
            if (!(ProjectId > 0) && !(StartDate.Date >= DateTime.Now.Date))
            {
                return new RequestResult<object>() { Status = Status.Warning };
            }
            else if ((ProjectId == 0 || ProjectId == null) && StartDate.Date >= DateTime.Now.Date)
            {
                return new RequestResult<object>() { Status = Status.Success };
            }
            else if (ProjectId > 0 && ProjectId != null)
            {
                var result = GetProject(new ProjectModel() { ProjectId = (int)ProjectId, CompanyId = CompanyId });
                if (!(StartDate.Date >= result.Data.StartDate.Value.Date))
                {
                    return new RequestResult<object>() { Status = Status.Warning };
                }
                else
                {
                    return new RequestResult<object>() { Status = Status.Success };
                }
            }
            else
            {
                return new RequestResult<object>() { Status = Status.Warning };
            }     
        }


        public RequestResult<object> DelProject(ProjectModel parameters)
        {
            ProjectDA DAProject = new ProjectDA();
            return DAProject.DelProject(parameters);
        }

        public RequestResult<object> FinishProject(ProjectModel parameters)
        {
            ProjectDA DAProject = new ProjectDA();
            return DAProject.FinishProject(parameters);
        }

        #region Match
        public List<ProjectModel> Get(ProjectModel filter)
        {
            PagerModel pager = new PagerModel(0, Int32.MaxValue - 1, "", "");
            return GetProjectCollection(filter, pager).Data;
        }

        public List<SelectModel> GetSelect(string resource, ProjectModel model)
        {
            List<ProjectModel> lst = Get(model);
            return lst
                .Select(x => new SelectModel { Value = x.ProjectId.ToString(), Text = x.Folio + " - " + x.ProjectType })
                .OrderBy(x => x.Text)
                .ToList();
        }

        public RequestResult<object> UpdateStatus(int? projectId, int statusId)
        {
            ProjectDA DAProject = new ProjectDA();
            return DAProject.UpdStatus(projectId, statusId);
        }
        #endregion


    }
}
