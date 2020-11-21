using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOpenGl
{
    class DrawSnapshot
    {
        private int mode = 0;
        public const double pi = 3.1415926f;
        public const int DRAW_LINE = 0, DRAW_CIRCLE = 1, DRAW_RECTANGLE = 2, DRAW_TRIANGLE = 3, DRAW_ELIPSE = 4, DRAW_PENTAGON = 5, DRAW_HEXAGON = 6;
        private Point pStart, pEnd;
        private OpenGL gl;

        public DrawSnapshot(int newMode, Point nStart, Point nEnd, OpenGL ngl)
        {
            mode = newMode;
            pStart.X = nStart.X;
            pStart.Y = nStart.Y;
            pEnd.X = nEnd.X;
            pEnd.Y = nEnd.Y;
            gl = ngl;
        }

        private void drawEqualPolygon(int egdeNum) // ve da giac deu cho so canh
        {
            gl.Begin(OpenGL.GL_LINE_LOOP);
            double x1 = pStart.X, x2 = pEnd.X, y1 = gl.RenderContextProvider.Height - pStart.Y, y2 = gl.RenderContextProvider.Height - pEnd.Y,
              AB = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            double r = Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2)),
                alpha = Math.Atan2((x2 - x1), (y2 - y1));
            for (double ii = 0; ii < egdeNum; ii++)
            {
                double theta = (2.0f * pi * ii / egdeNum + pi / 2 - alpha);//get the current angle 
                double x = r * Math.Cos(theta);//calculate the x component 
                double y = r * Math.Sin(theta);//calculate the y component 
                gl.Vertex(x1 + x, y1 + y);//output vertex 
            }
            gl.End();
            gl.Flush();// Thực hiện lệnh vẽ ngay lập tức thay vì đợi sau 1 khoảng thời gian
        }

        public void draw(Color color, float lineWidth)
        {
            // Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Color(color.R / 255.0, color.G / 255.0, color.B / 255.0, 0.0f);
            gl.LineWidth(lineWidth);

            int drawOp = mode;
            if (drawOp == DRAW_LINE)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.End();

                gl.Flush();
            }
            else if (drawOp == DRAW_CIRCLE)
            {
                drawEqualPolygon(450);
            }
            else if (drawOp == DRAW_RECTANGLE)
            {
                gl.Begin(OpenGL.GL_LINE_LOOP);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.End();
            }
            else if (drawOp == DRAW_TRIANGLE)
            {
                drawEqualPolygon(3);
            }
            else if (drawOp == DRAW_PENTAGON)
            {
                drawEqualPolygon(5);
            }
            else if (drawOp == DRAW_HEXAGON)
            {
                drawEqualPolygon(6);
            }
            else if (drawOp == DRAW_ELIPSE)
            {
                double x1 = pStart.X, x2 = pEnd.X, y1 = gl.RenderContextProvider.Height - pStart.Y, y2 = gl.RenderContextProvider.Height - pEnd.Y;
                // tinh r1, r2
                double rx = Math.Abs(x1 - x2), ry = Math.Abs(y1 - y2);

                double i, inc;
                gl.Begin(OpenGL.GL_LINE_LOOP);
                inc = pi / Math.Max(rx, ry) / 2;
                i = 0.0;
                while (i <= 360.0)
                {
                    gl.Vertex(((rx * Math.Cos(i) + y1) + .5), ((ry * Math.Sin(i) + x1) + .5));
                    i = i + inc;
                }
                gl.End();
            }

        }
    }
}
