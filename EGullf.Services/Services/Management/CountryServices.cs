using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class CountryServices
    {
        public CountryModel GetById(int id)
        {
            CountryDA da = new CountryDA();
            return da.GetById(id);
        }

        public List<CountryModel> Get(CountryModel filter)
        {
            CountryDA da = new CountryDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<CountryModel> lst = Get(new CountryModel());
            List<SelectModel> resp = lst
                .Select(x => new SelectModel { Value = x.CountryId.ToString(), Text = x.Acronym + " - " + x.Name })
                .OrderBy(x => x.Text)
                .ToList();

            resp.Insert(0, new SelectModel()
            {
                Value = null,
                Text = resource
            });

            return resp;
        }

    }
}
