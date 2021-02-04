using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class ClasificationSocietyServices
    {
        public ClasificationSocietyModel GetById(int id)
        {
            ClasificationSocietyDA da = new ClasificationSocietyDA();
            return da.GetById(id);
        }

        public List<ClasificationSocietyModel> Get(ClasificationSocietyModel filter)
        {
            ClasificationSocietyDA da = new ClasificationSocietyDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<ClasificationSocietyModel> lst = Get(new ClasificationSocietyModel());
            List<SelectModel> resp = lst
                .Select(x => new SelectModel { Value = x.ClasificationSocietyId.ToString(), Text = x.Acronym + " - " + x.Name })
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
