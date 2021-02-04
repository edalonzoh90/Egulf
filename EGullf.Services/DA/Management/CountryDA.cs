using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.DA.Management
{
    public class CountryDA
    {
        public CountryModel GetById(int id)
        {
            return
                Get(new CountryModel()
                {
                    CountryId = id
                }).FirstOrDefault();
        }

        public List<CountryModel> Get(CountryModel filter)
        {
            using (var db = new EGULFEntities())
            {
                return (from ct in db.Country
                        where ct.CountryId == filter.CountryId
                        || filter.CountryId == null
                        select new CountryModel
                        {
                            CountryId = ct.CountryId,
                            Name = ct.Name,
                            Acronym = ct.Acronym
                        }).OrderBy(x=>x.Acronym).ToList();
            }
        }
    }
}
