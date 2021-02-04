using System;

using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Management;

namespace EGullf.Services.Models.Utils
{
    public class RegionShape
    {
        private LatLng[] m_aInputVertices;
        private LatLng[] m_aUpdatedPolygonVertices;

        private ArrayList m_alEars = new ArrayList();
        private LatLng[][] m_aPolygons;

        public int NumberOfPolygons
        {
            get
            {
                return m_aPolygons.Length;
            }
        }

        public LatLng[] Polygons(int index)
        {
            if (index < m_aPolygons.Length)
                return m_aPolygons[index];
            else
                return null;
        }

        public RegionShape(LatLng[] vertices)
        {
            int nVertices = vertices.Length;
            if (nVertices < 3)
            {
                System.Diagnostics.Trace.WriteLine("To make a polygon, "
                    + " at least 3 points are required!");
                return;
            }

            //initalize the 2D points
            m_aInputVertices = new LatLng[nVertices];

            for (int i = 0; i < nVertices; i++)
                m_aInputVertices[i] = vertices[i];

            //make a working copy,  m_aUpdatedPolygonVertices are
            //in count clock direction from user view
            SetUpdatedPolygonVertices();
        }

        private void SetUpdatedPolygonVertices()
        {
            int nVertices = m_aInputVertices.Length;
            m_aUpdatedPolygonVertices = new LatLng[nVertices];

            for (int i = 0; i < nVertices; i++)
                m_aUpdatedPolygonVertices[i] = m_aInputVertices[i];

            //m_aUpdatedPolygonVertices should be in count clock wise
            if (m_aUpdatedPolygonVertices.PointsDirection()
                == PolygonDirection.Clockwise)
               m_aUpdatedPolygonVertices.ReversePointsDirection();
        }

        /****************************************************************
		To check whether the Vertex is an ear or not based updated Polygon vertices

		ref. www-cgrl.cs.mcgill.ca/~godfried/teaching/cg-projects/97/Ian
		/algorithm1.html

		If it is an ear, return true,
		If it is not an ear, return false;
		*****************************************************************/
        private bool IsEarOfUpdatedPolygon(LatLng vertex)
        {
            RegionPolygon polygon = new RegionPolygon(m_aUpdatedPolygonVertices);

            if (polygon.Vertices.PolygonVertex(vertex))
            {
                bool bEar = true;
                if (polygon.Vertices.PolygonVertexType(vertex) == PolygonType.Convex)
                {
                    LatLng pi = vertex;
                    LatLng pj = polygon.Vertices.PreviousPoint(vertex); //previous vertex
                    LatLng pk = polygon.Vertices.NextPoint(vertex);//next vertex

                    for (int i = m_aUpdatedPolygonVertices.GetLowerBound(0);
                        i < m_aUpdatedPolygonVertices.GetUpperBound(0); i++)
                    {
                        LatLng pt = m_aUpdatedPolygonVertices[i];
                        if (!(pt.EqualsPoint(pi) || pt.EqualsPoint(pj) || pt.EqualsPoint(pk)))
                        {
                            if (TriangleContainsPoint(new LatLng[] { pj, pi, pk }, pt))
                                bEar = false;
                        }
                    }
                } //ThePolygon.getVertexType(Vertex)=ConvexPt
                else  //concave point
                    bEar = false; //not an ear/
                return bEar;
            }
            else //not a polygon vertex;
            {
                System.Diagnostics.Trace.WriteLine("IsEarOfUpdatedPolygon: " +
                    "Not a polygon vertex");
                return false;
            }
        }

        /**********************************************************
		To check the Pt is in the Triangle or not.
		If the Pt is in the line or is a vertex, then return true.
		If the Pt is out of the Triangle, then return false.

		This method is used for triangle only.
		***********************************************************/
        private bool TriangleContainsPoint(LatLng[] trianglePts, LatLng pt)
        {
            if (trianglePts.Length != 3)
                return false;

            for (int i = trianglePts.GetLowerBound(0);
                i < trianglePts.GetUpperBound(0); i++)
            {
                if (pt.EqualsPoint(trianglePts[i]))
                    return true;
            }

            bool bIn = false;

            LineSegment line0 = new LineSegment(trianglePts[0], trianglePts[1]);
            LineSegment line1 = new LineSegment(trianglePts[1], trianglePts[2]);
            LineSegment line2 = new LineSegment(trianglePts[2], trianglePts[0]);

            if (pt.InLine(line0) || pt.InLine(line1)
                || pt.InLine(line2))
                bIn = true;
            else //point is not in the lines
            {
                double dblArea0 = new LatLng[]{trianglePts[0],trianglePts[1], pt}.PolygonArea();
                double dblArea1 = new LatLng[]{trianglePts[1],trianglePts[2], pt}.PolygonArea();
                double dblArea2 = new LatLng[]{trianglePts[2],trianglePts[0], pt}.PolygonArea();

                if (dblArea0 > 0)
                {
                    if ((dblArea1 > 0) && (dblArea2 > 0))
                        bIn = true;
                }
                else if (dblArea0 < 0)
                {
                    if ((dblArea1 < 0) && (dblArea2 < 0))
                        bIn = true;
                }
            }
            return bIn;
        }

        /********************************************************
		To update m_aUpdatedPolygonVertices:
		Take out Vertex from m_aUpdatedPolygonVertices array, add 3 points
		to the m_aEars
		**********************************************************/
        private void UpdatePolygonVertices(LatLng vertex)
        {
            ArrayList alTempPts = new ArrayList();

            for (int i = 0; i < m_aUpdatedPolygonVertices.Length; i++)
            {
                if (vertex.EqualsPoint(
                    m_aUpdatedPolygonVertices[i])) //add 3 pts to FEars
                {
                    RegionPolygon polygon = new RegionPolygon(m_aUpdatedPolygonVertices);
                    LatLng pti = vertex;
                    LatLng ptj = polygon.Vertices.PreviousPoint(vertex); //previous point
                    LatLng ptk = polygon.Vertices.NextPoint(vertex); //next point

                    LatLng[] aEar = new LatLng[3]; //3 vertices of each ear
                    aEar[0] = ptj;
                    aEar[1] = pti;
                    aEar[2] = ptk;

                    m_alEars.Add(aEar);
                }
                else
                {
                    alTempPts.Add(m_aUpdatedPolygonVertices[i]);
                } //not equal points
            }

            if (m_aUpdatedPolygonVertices.Length
                - alTempPts.Count == 1)
            {
                int nLength = m_aUpdatedPolygonVertices.Length;
                m_aUpdatedPolygonVertices = new LatLng[nLength - 1];

                for (int i = 0; i < alTempPts.Count; i++)
                    m_aUpdatedPolygonVertices[i] = (LatLng)alTempPts[i];
            }
        }

        /****************************************************
		Set up m_aPolygons:
		add ears and been cut Polygon togather
		****************************************************/
        private void SetPolygons()
        {
            int nPolygon = m_alEars.Count + 1; //ears plus updated polygon
            m_aPolygons = new LatLng[nPolygon][];

            for (int i = 0; i < nPolygon - 1; i++) //add ears
            {
                LatLng[] points = (LatLng[])m_alEars[i];

                m_aPolygons[i] = new LatLng[3]; //3 vertices each ear
                m_aPolygons[i][0] = points[0];
                m_aPolygons[i][1] = points[1];
                m_aPolygons[i][2] = points[2];
            }

            //add UpdatedPolygon:
            m_aPolygons[nPolygon - 1] = new
                LatLng[m_aUpdatedPolygonVertices.Length];

            for (int i = 0; i < m_aUpdatedPolygonVertices.Length; i++)
            {
                m_aPolygons[nPolygon - 1][i] = m_aUpdatedPolygonVertices[i];
            }
        }

        public void CutEar()
        {
            RegionPolygon polygon = new RegionPolygon(m_aUpdatedPolygonVertices);
            bool bFinish = false;

            //if (polygon.GetPolygonType()==PolygonType.Convex) //don't have to cut ear
            //	bFinish=true;

            if (m_aUpdatedPolygonVertices.Length == 3) //triangle, don't have to cut ear
                bFinish = true;

            LatLng pt = new LatLng();
            while (bFinish == false) //UpdatedPolygon
            {
                int i = 0;
                bool bNotFound = true;
                while (bNotFound
                    && (i < m_aUpdatedPolygonVertices.Length)) //loop till find an ear
                {
                    pt = m_aUpdatedPolygonVertices[i];
                    if (IsEarOfUpdatedPolygon(pt))
                        bNotFound = false; //got one, pt is an ear
                    else
                        i++;
                } //bNotFount
                  //An ear found:}
                if (pt != null)
                    UpdatePolygonVertices(pt);

                polygon = new RegionPolygon(m_aUpdatedPolygonVertices);
                //if ((polygon.GetPolygonType()==PolygonType.Convex)
                //	&& (m_aUpdatedPolygonVertices.Length==3))
                if (m_aUpdatedPolygonVertices.Length == 3)
                    bFinish = true;
            } //bFinish=false
            SetPolygons();
        }
    }

    public struct ConstantValue
    {
        internal const decimal SmallValue = (decimal)0.00001;
        internal const decimal BigValue = 99999;
    }

    public enum PolygonType
    {
        Unknown,
        Convex,
        Concave
    }

    public enum PolygonDirection
    {
        Unknown,
        Clockwise,
        Count_Clockwise
    }
}
