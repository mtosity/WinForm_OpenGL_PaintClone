using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOpenGl
{
    class DrawPoints
    {
        private List<Point> listPoints = new List<Point>();
        private OpenGL gl;

        public DrawPoints(OpenGL ngl)
        {
            gl = ngl;
        }

        public void setPoints(List<Point> listp)
        {
            listPoints = new List<Point>(listp);
        }

        public void drawControlPoints()
        {
            if (listPoints.Count >= 3)
            {
                gl.PointSize(5.0f);
                gl.Begin(OpenGL.GL_POINTS);
                gl.Color(125, 122, 0);
                foreach (Point p in listPoints)
                {
                    gl.Vertex(p.X, p.Y);
                }
                gl.End();
                gl.Flush();
            }
        }

        public void clearPoints()
        {
            listPoints.Clear();
        }

        public List<Point> getPoints()
        {
            return listPoints;
        }
    }
}
