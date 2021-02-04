using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EGullf.Services.Models.Management;
using EGullf.Services.Services.Management;
using EGullf.Services.Services.Operation;
using EGullf.Services.Models.Operation;
using EGullf.Services.Models.Utils;
using EGULF.Helpers;

namespace EGULF.Controllers
{
    public class MapController : Controller
    {
        public int? UserCompanyId { get { return SessionWeb.User.CompanyId; } set { SessionWeb.User.CompanyId = value; } }
        // GET: Map
        public ActionResult Index()
        {
            return View();
        }

        //POST: Map/regions
        [HttpPost]
        public ActionResult Regions()
        {
            RegionServices regSrv = new RegionServices();
            var regions = regSrv.Get(new RegionModel());
            var regions2 = new List<RegionModel>();
            foreach (var r in regions)
            {
                RegionShape shape = new RegionShape(r.Coordenates.ToArray());
                shape.CutEar();
                for(int i =0; i < shape.NumberOfPolygons; i++)
                {
                    var reg = new RegionModel();
                    reg.Coordenates = new List<LatLng>();
                    int nPoints = shape.Polygons(i).Length;
                    for(int j = 0; j < nPoints; j++)
                    {
                        reg.Coordenates.Add(new LatLng() { Lat = shape.Polygons(i)[j].Lat, Lng = shape.Polygons(i)[j].Lng });
                    }
                    regions2.Add(reg);

                }

            }
            RegionServices vesSrv = new RegionServices();
            var location = vesSrv.GetLocation(1);
               
            return Json(regions2);
        }

        //POST: Map/regions
        [HttpPost]
        public ActionResult Vessels(int? companId)
        {
            var company = companId == null ? new CompanyModel() : new CompanyModel() { CompanyId = UserCompanyId };
            VesselServices regSrv = new VesselServices();
            return Json(regSrv.Get(new VesselModel() { Company =  company}));
        }

    }
}
