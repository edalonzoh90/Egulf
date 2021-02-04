using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EGullf.Services.Models.Management;

namespace EGullf.Services.Models.Utils
{
    public class LineSegment
    {
        protected decimal a;
        protected decimal b;
        protected decimal    c;

        private LatLng m_startPoint;
        private LatLng m_endPoint;

        public LatLng StartPoint
        {
            get
            {
                return m_startPoint;
            }
        }

        public LatLng EndPoint
        {
            get
            {
                return m_endPoint;
            }
        }

        private void Initialize(Double angleInRad, LatLng point)
        {
            //angleInRad should be between 0-Pi

            try
            {
                //if ((angleInRad<0) ||(angleInRad>Math.PI))
                if (angleInRad > 2 * Math.PI)
                {
                    string errMsg = string.Format(
                        "The input line angle" +
                        " {0} is wrong. It should be between 0-2*PI.", angleInRad);

                   throw new Exception(errMsg);
                }

                if ((decimal)Math.Abs(angleInRad - Math.PI / 2) <
                    ConstantValue.SmallValue) //vertical line
                {
                    a = 1;
                    b = 0;
                    c = -point.Lng;
                }
                else //not vertical line
                {
                    a = (decimal)-Math.Tan(angleInRad);
                    b = 1;
                    c = -a * point.Lng - b * point.Lat;
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message + e.StackTrace);
            }
        }

        public LineSegment(LatLng startPoint, LatLng endPoint)
		{
            try
            {
                if (startPoint.SamePoints(endPoint))
                {
                    throw new Exception("The input points are the same");
                }

                //Point1 and Point2 are different points:
                if (Math.Abs(startPoint.Lng - endPoint.Lng)
                    < ConstantValue.SmallValue) //vertical line
                {
                    Initialize(Math.PI / 2, startPoint);
                }
                else if (Math.Abs(startPoint.Lat - startPoint.Lat)
                    < ConstantValue.SmallValue) //Horizontal line
                {
                    Initialize(0, startPoint);
                }
                else //normal line
                {
                    double m = (double)((endPoint.Lat - endPoint.Lat) / (endPoint.Lng - endPoint.Lng));
                    double alphaInRad = Math.Atan(m);
                    Initialize(alphaInRad, startPoint);
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message + e.StackTrace);
            }
            this.m_startPoint = startPoint;
            this.m_endPoint = endPoint;
        }
    }
}
