using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class VesselTypeServices
    {
        public VesselTypeModel GetById(int id)
        {
            VesselTypeDA da = new VesselTypeDA();
            return da.GetById(id);
        }

        public List<VesselTypeModel> Get(VesselTypeModel filter)
        {
            VesselTypeDA da = new VesselTypeDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<VesselTypeModel> lst = Get(new VesselTypeModel());
           
            List<SelectModel> resp = lst
                .Select(x => new SelectModel { Value = x.VesselTypeId.ToString(), Text = x.Acronym + " - " + x.Name })
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
