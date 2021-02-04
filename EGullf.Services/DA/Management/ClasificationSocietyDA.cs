using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class ClasificationSocietyDA
    {
        public ClasificationSocietyModel GetById(int id)
        {
            return 
                Get(new ClasificationSocietyModel()
                {
                    ClasificationSocietyId = id
                }).FirstOrDefault();
        }

        public List<ClasificationSocietyModel> Get(ClasificationSocietyModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.ClasificationSociety
                        where ct.ClasificationSocietyId == filter.ClasificationSocietyId 
                        || filter.ClasificationSocietyId == null
                        select new ClasificationSocietyModel
                        {
                            ClasificationSocietyId = ct.ClasificationSocietyId,
                            Name = ct.Name,
                            Acronym = ct.Acronym
                        }).OrderBy(x=>x.Acronym).ToList();
            }
        }
    }
}
