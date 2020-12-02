using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOpenGl
{
    class DrawRandomPolygon
    {
        private OpenGL gl;
        private Color color;
        private float lineWidth;
        private List<Point> listPoints = new List<Point>();

        public DrawRandomPolygon(OpenGL ngl)
        {
            gl = ngl;
        }

        public DrawRandomPolygon(DrawRandomPolygon newDrp)
        {
            gl = newDrp.gl;
            color = newDrp.color;
            lineWidth = newDrp.lineWidth;
            listPoints = new List<Point>(newDrp.listPoints);
        }

        public void setConfig(Color nColor, float nLineWidth)
        {
            lineWidth = nLineWidth;
            color = nColor;
        }

        public void addPoint(Point nP)
        {
            listPoints.Add(nP);
        }

        public void clearPoints() 
        {
            listPoints.Clear();
        }

        public void draw()
        {
            if (listPoints.Count >= 2)
            {
                gl.Begin(OpenGL.GL_LINE_LOOP);
                gl.Color(color.R / 255.0, color.G / 255.0, color.B / 255.0, 0.0f);
                gl.LineWidth(lineWidth);

                foreach (Point p in listPoints)
                {
                    gl.Vertex(p.X, gl.RenderContextProvider.Height - p.Y);
                }
                gl.End();
                gl.Flush();
            }
        }

        public void drawLive(Point nEnd)
        {
            if (listPoints.Count >= 1)
            {
                gl.Begin(OpenGL.GL_LINE_LOOP);
                gl.Color(color.R / 255.0, color.G / 255.0, color.B / 255.0, 0.0f);
                gl.LineWidth(lineWidth);

                foreach (Point p in listPoints)
                {
                    gl.Vertex(p.X, gl.RenderContextProvider.Height - p.Y);
                }
                gl.Vertex(nEnd.X, gl.RenderContextProvider.Height - nEnd.Y);
                gl.End();
                gl.Flush();
            }
        }

        public List<Point> getControlPoints() // get theo chieu kim dong ho
        {
            List<Point> cp = new List<Point>();
            foreach(Point p in listPoints)
            {
                cp.Add(new Point(p.X, gl.RenderContextProvider.Height - p.Y));
            }
            // vi 1 la theo chieu kim dong ho, 2 la nguoc chieu => neu nguoc chieu thi doi nguoc lai
            if(cp[1].X < cp[0].X)
            {
                cp.Reverse();
            }
            return cp;
        }
    }
}
