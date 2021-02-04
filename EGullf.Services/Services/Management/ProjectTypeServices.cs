using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class ProjectTypeServices
    {
        public ProjectTypeModel GetById(int id)
        {
            ProjectTypeDA da = new ProjectTypeDA();
            return da.GetById(id);
        }

        public List<ProjectTypeModel> Get(ProjectTypeModel filter)
        {
            ProjectTypeDA da = new ProjectTypeDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<ProjectTypeModel> lst = Get(new ProjectTypeModel());
            if (resource != null)
            {
                lst.Insert(0, new ProjectTypeModel()
                {
                    ProjectTypeId = null,
                    Name = resource
                });
            }

            return lst
                .Select(x => new SelectModel { Value = x.ProjectTypeId.ToString(), Text = x.Acronym + " - " + x.Name })
                .OrderBy(x => x.Text)
                .ToList();
        }

    }
}
