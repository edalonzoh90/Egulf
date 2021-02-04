using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Management;

namespace EGullf.Services.Models.Utils
{
    public class RegionPolygon
    {
        private LatLng[] m_aVertices;

        public LatLng this[int index]
        {
            set
            {
                m_aVertices[index] = value;
            }
            get
            {
                return m_aVertices[index];
            }
        }

        public LatLng[] Vertices
        {
            get
            {
                return m_aVertices;
            }
            set
            {
                m_aVertices = value;
            }
        }
        public RegionPolygon()
        {

        }

        public RegionPolygon(LatLng[] points)
        {
            int nNumOfPoitns = points.Length;
            try
            {
                if (nNumOfPoitns < 3)
                {
                    throw new Exception("the polygon has been 3 sides");
                }
                else
                {
                    m_aVertices = new LatLng[nNumOfPoitns];
                    for (int i = 0; i < nNumOfPoitns; i++)
                    {
                        m_aVertices[i] = points[i];
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(
                    e.Message + e.StackTrace);
            }
        }

    }
}
