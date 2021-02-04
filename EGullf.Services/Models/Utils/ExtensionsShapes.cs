using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Management;

namespace EGullf.Services.Models.Utils
{
    internal static class ExtensionsShapes
    {
        public static PolygonDirection PointsDirection(this LatLng[] points)
        {
            //X = Lng, Y=Lat
            int nCount = 0, j = 0, k = 0;
            int nPoints = points.Length;

            if (nPoints < 3)
                return PolygonDirection.Unknown;

            for (int i = 0; i < nPoints; i++)
            {
                j = (i + 1) % nPoints; //j:=i+1;
                k = (i + 2) % nPoints; //k:=i+2;

                decimal? crossProduct = (points[j].Lng - points[i].Lng)
                    * (points[k].Lat - points[j].Lat);
                crossProduct = crossProduct - (
                    (points[j].Lat - points[i].Lat)
                    * (points[k].Lng - points[j].Lng)
                    );

                if (crossProduct > 0)
                    nCount++;
                else
                    nCount--;
            }

            if (nCount < 0)
                return PolygonDirection.Count_Clockwise;
            else if (nCount > 0)
                return PolygonDirection.Clockwise;
            else
                return PolygonDirection.Unknown;
        }

        public static void ReversePointsDirection(this LatLng[] points)
        {
            int nVertices = points.Length;
            LatLng[] aTempPts = new LatLng[nVertices];

            for (int i = 0; i < nVertices; i++)
                aTempPts[i] = points[i];

            for (int i = 0; i < nVertices; i++)
                points[i] = aTempPts[nVertices - 1 - i];
        }

        public static bool SamePoints(this LatLng Point1, LatLng Point2)
        {

            decimal dDeff_X =
                Math.Abs(Point1.Lng - Point2.Lng);
            decimal dDeff_Y =
                Math.Abs(Point1.Lat - Point2.Lat);

            if ((dDeff_X < ConstantValue.SmallValue)
                && (dDeff_Y < ConstantValue.SmallValue))
                return true;
            else
                return false;
        }

        public static bool EqualsPoint(this LatLng point, LatLng newPoint)
        {

            decimal dDeff_X =
                Math.Abs(point.Lng - newPoint.Lng);
            decimal dDeff_Y =
                Math.Abs(point.Lat - newPoint.Lat);

            if ((dDeff_X < ConstantValue.SmallValue)
                && (dDeff_Y < ConstantValue.SmallValue))
                return true;
            else
                return false;

        }

        /***********************************************
			To check a vertex concave point or a convex point
			-----------------------------------------------------------
			The out polygon is in count clock-wise direction
		************************************************/
        public static PolygonType PolygonVertexType(this LatLng[] p, LatLng vertex)
        {
            PolygonType vertexType = PolygonType.Unknown;

            if (p.PolygonVertex(vertex))
            {
                LatLng pti = vertex;
                LatLng ptj = p.PreviousPoint(vertex);
                LatLng ptk = p.NextPoint(vertex);

                double dArea = new LatLng[] { ptj, pti, ptk }.PolygonArea();

                if (dArea < 0)
                    vertexType = PolygonType.Convex;
                else if (dArea > 0)
                    vertexType = PolygonType.Concave;
            }
            return vertexType;
        }

        /******************************************
		To calculate the area of polygon made by given points 

		Good for polygon with holes, but the vertices make the 
		hole  should be in different direction with bounding 
		polygon.
		
		Restriction: the polygon is not self intersecting
		ref: www.swin.edu.au/astronomy/pbourke/
			geometry/polyarea/

		As polygon in different direction, the result coulb be
		in different sign:
		If dblArea>0 : polygon in clock wise to the user 
		If dblArea<0: polygon in count clock wise to the user 		
		*******************************************/
        public static double PolygonArea(this LatLng[] points)
        {
            //X = Lng, Y=Lat
            double dblArea = 0;
            int nNumOfPts = points.Length;

            int j;
            for (int i = 0; i < nNumOfPts; i++)
            {
                j = (i + 1) % nNumOfPts;
                dblArea += (double)(points[i].Lng * points[j].Lat);
                dblArea -= (double)(points[i].Lat * points[j].Lng);
            }

            dblArea = dblArea / 2;
            return dblArea;
        }

        public static bool PolygonVertex(this LatLng[] m_aVertices, LatLng point)
        {
            bool bVertex = false;
            int nIndex = m_aVertices.VertexIndex(point);

            if ((nIndex >= 0) && (nIndex <= m_aVertices.Length - 1))
                bVertex = true;

            return bVertex;
        }

        public static int VertexIndex(this LatLng[] m_aVertices, LatLng vertex)
        {
            int nIndex = -1;

            int nNumPts = m_aVertices.Length;
            for (int i = 0; i < nNumPts; i++) //each vertex
            {
                if (m_aVertices[i].SamePoints(vertex))
                    nIndex = i;
            }
            return nIndex;
        }

        /***********************************
		 From a given vertex, get its previous vertex point.
		 If the given point is the first one, 
		 it will return  the last vertex;
		 If the given point is not a polygon vertex, 
		 it will return null; 
		 ***********************************/
        public static LatLng PreviousPoint(this LatLng[] m_aVertices, LatLng vertex)
        {
            int nIndex;

            nIndex = m_aVertices.VertexIndex(vertex);
            if (nIndex == -1)
                return null;
            else //a valid vertex
            {
                if (nIndex == 0) //the first vertex
                {
                    int nPoints = m_aVertices.Length;
                    return m_aVertices[nPoints - 1];
                }
                else //not the first vertex
                    return m_aVertices[nIndex - 1];
            }
        }

        /***************************************
			 From a given vertex, get its next vertex point.
			 If the given point is the last one, 
			 it will return  the first vertex;
			 If the given point is not a polygon vertex, 
			 it will return null; 
		***************************************/
        public static LatLng NextPoint(this LatLng[] m_aVertices, LatLng vertex)
        {
            LatLng nextPt = new LatLng();

            int nIndex;
            nIndex = m_aVertices.VertexIndex(vertex);
            if (nIndex == -1)
                return null;
            else //a valid vertex
            {
                int nNumOfPt = m_aVertices.Length;
                if (nIndex == nNumOfPt - 1) //the last vertex
                {
                    return m_aVertices[0];
                }
                else //not the last vertex
                    return m_aVertices[nIndex + 1];
            }
        }

        public static bool InLine(this LatLng point, LineSegment lineSegment)
        {
            bool bInline = false;

            double Ax, Ay, Bx, By, Cx, Cy;
            Bx = (double)lineSegment.EndPoint.Lng;
            By = (double)lineSegment.EndPoint.Lat;
            Ax = (double)lineSegment.StartPoint.Lng;
            Ay = (double)lineSegment.StartPoint.Lat;
            Cx = (double)point.Lng;
            Cy = (double)point.Lat;

            double L = lineSegment.GetLineSegmentLength();
            double s = Math.Abs(((Ay - Cy) * (Bx - Ax) - (Ax - Cx) * (By - Ay)) / (L * L));

            if (Math.Abs(s - 0) < (double)ConstantValue.SmallValue)
            {
                if ((point.SamePoints(lineSegment.StartPoint)) ||
                    (point.SamePoints(lineSegment.EndPoint)))
                    bInline = true;
                else if ((Cx < lineSegment.GetXmax())
                    && (Cx > lineSegment.GetXmin())
                    && (Cy < lineSegment.GetYmax())
                    && (Cy > lineSegment.GetYmin()))
                    bInline = true;
            }
            return bInline;
        }

        /***Get the minimum x value of the points in the line***/
        public static double GetXmin(this LineSegment line)
        {
            return (double)Math.Min(line.StartPoint.Lng, line.EndPoint.Lng);
        }

        /***Get the maximum  x value of the points in the line***/
        public static double GetXmax(this LineSegment line)
        {
            return (double)Math.Max(line.StartPoint.Lng, line.EndPoint.Lng);
        }

        /***Get the minimum y value of the points in the line***/
        public static double GetYmin(this LineSegment line)
        {
            return (double)Math.Min(line.StartPoint.Lat, line.EndPoint.Lat);
        }

        /***Get the maximum y value of the points in the line***/
        public static double GetYmax(this LineSegment line)
        {
            return (double)Math.Max(line.StartPoint.Lat, line.EndPoint.Lat);
        }

        public static double GetLineSegmentLength(this LineSegment line)
        {
            double d = (double)((line.EndPoint.Lng - line.StartPoint.Lng) * (line.EndPoint.Lng - line.StartPoint.Lng));
            d += (double)((line.EndPoint.Lat - line.StartPoint.Lat) * (line.EndPoint.Lat - line.StartPoint.Lat));
            d = Math.Sqrt(d);

            return d;
        }

        public static LatLng GetCentroid(this LatLng[] pointArray)
        {
            decimal centroidX = 0;
            decimal centroidY = 0;

            for (int i = 0; i < pointArray.Length; i++)
            {
                centroidX += pointArray[i].Lng;
                centroidY += pointArray[i].Lat;
            }
            centroidX /= pointArray.Length;
            centroidY /= pointArray.Length;

            return (new LatLng() { Lat = centroidY, Lng = centroidX });
        }
    }
}
