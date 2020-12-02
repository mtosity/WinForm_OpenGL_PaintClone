using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOpenGl
{
    class Polygon
    {
        List<Point> listPoints = new List<Point>();

        public Polygon()
        {

        }

        public Polygon(List<Point> points)
        {
            listPoints = new List<Point>(points);
        }

        public List<Point> getPoints()
        {
            return new List<Point>(listPoints);
        }

        public void setPoints(List<Point> points)
        {
            listPoints = new List<Point>(points);
        }

        public void clearPoints()
        {
            listPoints.Clear();
        }

        public bool isPointInside(Point pQ) // !!!!cac diem trong da giac phai theo chieu kim dong ho //  su dung vector phap tuyen
        {
            int n = listPoints.Count;
            if (n > 2)
            {
                Point preP = listPoints[listPoints.Count - 1];
                for(int i=0;i<n;i++)
                { 
                    // p(t) = (1-t) * p + t * q 
                    // tinh vector phap tuyen * Q so voi d
                    int x1 = preP.X, y1 = preP.Y, x2 = listPoints[i].X, y2 = listPoints[i].Y;
                    // n * Q > d => return 0
                    int d = (y1 - y2) * x1 + (x2 - x1) * y1;
                    if (((y1 - y2) * pQ.X + (x2 - x1) * pQ.Y) > d)
                    {
                        return false;
                    }
                    preP = listPoints[i];
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
