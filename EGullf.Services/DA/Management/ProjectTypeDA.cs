using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class ProjectTypeDA
    {
        public ProjectTypeModel GetById(int id)
        {
            return
                Get(new ProjectTypeModel()
                {
                    ProjectTypeId = id
                }).FirstOrDefault();
        }

        public List<ProjectTypeModel> Get(ProjectTypeModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.ProjectType
                        where ct.ProjectTypeId == filter.ProjectTypeId
                        || filter.ProjectTypeId == null
                        select new ProjectTypeModel
                        {
                            ProjectTypeId = ct.ProjectTypeId,
                            Name = ct.Name,
                            Acronym = ct.Acronym,
                            Category = ct.Category
                        }).OrderBy(x=>x.Acronym).ToList();
            }
        }
    }
}
