using EGullf.Services.DA.Management;
using EGullf.Services.Models.Configuration;
using EGullf.Services.Models.Management;
using EGullf.Services.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EGullf.Services.Services.Management
{
    public class RegionServices
    {
        public RegionModel GetById(int id)
        {
            RegionDA da = new RegionDA();
            return da.GetById(id);
        }

        public List<RegionModel> Get(RegionModel filter)
        {
            RegionDA da = new RegionDA();
            return da.Get(filter);
        }

        public List<SelectModel> GetSelect(string resource)
        {
            List<RegionModel> lst = Get(new RegionModel());
            List<SelectModel> resp = lst
                .Select(x => new SelectModel { Value = x.RegionId.ToString(), Text = x.Name })
                .OrderBy(x => x.Text)
                .ToList();

            if (!String.IsNullOrEmpty(resource))
            {
                resp.Insert(0, new SelectModel()
                {
                    Value = null,
                    Text = resource
                });
            }          

            return resp;
        }

        public LatLng GetCoordinate(RegionModel region)
        {
            
            return new LatLng();
        }

        public LatLng GetLocation(int? regionId)
        {
            RegionServices regSrv = new RegionServices();
            var regions = regSrv.Get(new RegionModel() { RegionId = regionId });
            var regions2 = new List<RegionModel>();
            foreach (var r in regions)
            {
                RegionShape shape = new RegionShape(r.Coordenates.ToArray());
                shape.CutEar();
                for (int i = 0; i < shape.NumberOfPolygons; i++)
                {
                    var reg = new RegionModel();
                    reg.Coordenates = new List<LatLng>();
                    int nPoints = shape.Polygons(i).Length;
                    for (int j = 0; j < nPoints; j++) //Triangulo
                    {
                        reg.Coordenates.Add(new LatLng() { Lat = shape.Polygons(i)[j].Lat, Lng = shape.Polygons(i)[j].Lng });
                    }
                    regions2.Add(reg);

                }

            }
            var tRandom = regions2[new Random().Next((regions2.Count - 1))];

            return getRandomPoint(getAreaTriangule(tRandom.Coordenates[0], tRandom.Coordenates[1], tRandom.Coordenates[2]),
                tRandom.Coordenates[0], tRandom.Coordenates[1], tRandom.Coordenates[2]);//tRandom.Coordenates.ToArray().GetCentroid();
        }

        private decimal getAreaTriangule(LatLng A, LatLng B, LatLng C) {
            //(Xa(Yc-Yb) + Xc(Yb-Ya) + Xb(Ya-Yc)) / 2
            decimal a = (A.Lng * (C.Lat - B.Lat) + C.Lng * (B.Lat - A.Lat) + B.Lng * (A.Lat - C.Lat)) / 2;
            return a;
        }

        private LatLng getRandomPoint(decimal a, LatLng v1, LatLng v2, LatLng v3){
            LatLng res = new LatLng();
            var area = Math.Sqrt((double)a);
            var inc = 0.1 / area;
            List<LatLng> points = new List<LatLng>();
            for (double r1 = 0; r1 <= 1; r1 += inc)
            {
                for (double r2 = 0; r2 <= 1; r2 += inc)
                {
                    // of course this is javascript we have to write this out instead of
                    // using only one line
                    var sqrtR = Math.Sqrt(r1);
                    var A = (1 - sqrtR);
                    var B = (sqrtR * (1 - r2));
                    var C = (sqrtR * r2);
                    var x = A * (double)v1.Lng + B * (double)v2.Lng + C * (double)v3.Lng;
                    var y = A * (double)v1.Lat + B * (double)v2.Lat + C * (double)v3.Lat;
                    points.Add(new LatLng() { Lat = (decimal)y, Lng = (decimal)x });
                }
            }
            res = points[new Random().Next((points.Count - 1))];
            return res;
        }
    }
}
