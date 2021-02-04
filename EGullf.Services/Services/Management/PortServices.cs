using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class PortServices
    {
        public PortModel GetById(int id)
        {
            PortDA da = new PortDA();
            return da.GetById(id);
        }

        public List<PortModel> Get(PortModel filter)
        {
            PortDA da = new PortDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<PortModel> lst = Get(new PortModel());
            List<SelectModel> resp = lst
                .Select(x => new SelectModel { Value = x.PortId.ToString(), Text = x.Name })
                .OrderBy(x=>x.Text)
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
