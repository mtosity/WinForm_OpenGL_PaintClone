using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOpenGl
{
    class DrawChosenOption
    {
        private int mode = 0; // ve hinh gi? vd: 0: duong thang, 1: duong tron,...
        public const double pi = 3.1415926f;
        public const int DRAW_LINE = 0, DRAW_CIRCLE = 1, DRAW_RECTANGLE = 2, DRAW_TRIANGLE = 3, DRAW_ELIPSE = 4, DRAW_PENTAGON = 5, DRAW_HEXAGON = 6;
        private Point pStart, pEnd;
        private OpenGL gl;
        private Color color = Color.Black;
        private float lineWidth;

        public DrawChosenOption(int newMode, Point nStart, Point nEnd, OpenGL ngl, Color newColor, float newLineWidth)
        {
            mode = newMode;
            pStart = nStart;
            pEnd = nEnd;
            gl = ngl;
            color = newColor;
            lineWidth = newLineWidth;
        }

        private void drawEqualPolygon(int egdeNum) // ve da giac deu cho so canh, pStart = tam - pEnd = 1 diem trong da giac
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
            gl.Flush();
        }

        private List<Point> getControlPointsFromEqualPolygon(int pNum)
        {
            List<Point> listP = new List<Point>();
            double x1 = pStart.X, x2 = pEnd.X, y1 = gl.RenderContextProvider.Height - pStart.Y, y2 = gl.RenderContextProvider.Height - pEnd.Y,
              AB = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
            double r = Math.Sqrt(Math.Pow(y2 - y1, 2) + Math.Pow(x2 - x1, 2)),
                alpha = Math.Atan2((x2 - x1), (y2 - y1));
            for (double ii = 0; ii < pNum; ii++)
            {
                double theta = (2.0f * pi * ii / pNum + pi / 2 - alpha);//get the current angle 
                double x = r * Math.Cos(theta);//calculate the x component 
                double y = r * Math.Sin(theta);//calculate the y component 
                //gl.Vertex(x1 + x, y1 + y);//output vertex 
                listP.Add(new Point((int)(x1 + x), (int)(y1 + y)));
            }
            return listP;
        }

        public void draw()
        {
            gl.Color(color.R / 255.0, color.G / 255.0, color.B / 255.0, 0.0f);
            gl.LineWidth(lineWidth);

            if (mode == DRAW_LINE)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.End();
                gl.Flush();
            }
            else if (mode == DRAW_CIRCLE)
            {
                drawEqualPolygon(450);
            }
            else if (mode == DRAW_RECTANGLE)
            {
                drawEqualPolygon(4);
            }
            else if (mode == DRAW_TRIANGLE)
            {
                drawEqualPolygon(3);
            }
            else if (mode == DRAW_PENTAGON)
            {
                drawEqualPolygon(5);
            }
            else if (mode == DRAW_HEXAGON)
            {
                drawEqualPolygon(6);
            }
            else if (mode == DRAW_ELIPSE)
            {
                /*
                 If the ellipse is ((x-cx)/a)^2 + ((y-cy)/b)^2 = 1 then change the glVertex2f call to glVertext2d(a*x + cx, b*y + cy);
                 https://stackoverflow.com/questions/5886628/effecient-way-to-draw-ellipse-with-opengl-or-d3d
                 */
                double x1 = pStart.X, x2 = pEnd.X, y1 = gl.RenderContextProvider.Height - pStart.Y, y2 = gl.RenderContextProvider.Height - pEnd.Y;
                // tinh r1, r2
                double rx = Math.Abs(x1 - x2), ry = Math.Abs(y1 - y2);

                double theta = 2 * 3.1415926 / 360.0;
                double c = Math.Cos(theta);//precalculate the sine and cosine
                double s = Math.Sin(theta);
                double t;

                double x = 1;//we start at angle = 0 
                double y = 0;

                gl.Begin(OpenGL.GL_LINE_LOOP);
                for (double ii = 0; ii < 360.0; ii++)
                {
                    //apply radius and offset
                    gl.Vertex(x * rx + x1, y * ry + y1);//output vertex 

                    //apply the rotation matrix
                    t = x;
                    x = c * x - s * y;
                    y = s * t + c * y;
                }
                gl.End();
                gl.Flush();
            }
        }

        public List<Point> getControlPoints()
        {
            List<Point> listP = new List<Point>();
            if(mode == DRAW_LINE)
            {
                listP.Add(pStart);
                listP.Add(pEnd);
            }
            else if(mode == DRAW_CIRCLE) // hinh tron thi tam va 4 diem tren duong tron
            {
                listP.Add(pStart);
                listP.AddRange(getControlPointsFromEqualPolygon(4));
            }
            else if (mode == DRAW_RECTANGLE) // hinh tron thi tam va 4 diem tren duong tron
            {
                listP.AddRange(getControlPointsFromEqualPolygon(4));
            }
            else if (mode == DRAW_TRIANGLE) // hinh tron thi tam va 4 diem tren duong tron
            {
                listP.AddRange(getControlPointsFromEqualPolygon(3));
            }
            else if(mode == DRAW_PENTAGON)
            {
                listP.AddRange(getControlPointsFromEqualPolygon(5));
            }
            else if (mode == DRAW_HEXAGON)
            {
                listP.AddRange(getControlPointsFromEqualPolygon(6));
            }
            else if (mode == DRAW_ELIPSE)
            {
                double x1 = pStart.X, x2 = pEnd.X, y1 = gl.RenderContextProvider.Height - pStart.Y, y2 = gl.RenderContextProvider.Height - pEnd.Y;
                // tinh r1, r2
                double rx = Math.Abs(x1 - x2), ry = Math.Abs(y1 - y2);

                double theta = 2 * 3.1415926 / 360.0;
                double c = Math.Cos(theta);//precalculate the sine and cosine
                double s = Math.Sin(theta);
                double t;

                double x = 1;//we start at angle = 0 
                double y = 0;
                
                for (double ii = 0; ii < 4; ii++)
                {
                    //apply radius and offset
                    listP.Add(new Point((int)(x * rx + x1), (int)(y * ry + y1)));//output vertex 

                    //apply the rotation matrix
                    t = x;
                    x = c * x - s * y;
                    y = s * t + c * y;
                }
            }

            // vi 1 la theo chieu kim dong ho, 2 la nguoc chieu => neu nguoc chieu thi doi nguoc lai
            if (listP[1].X < listP[0].X)
            {
                listP.Reverse();
            }
            return listP;
        }

        public List<Point> getDrawingPoints()
        {
            List<Point> re = new List<Point>();
            if (mode == DRAW_LINE)
            {
                Point start = new Point(pStart.X, gl.RenderContextProvider.Height - pStart.Y), end = new Point(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
                re.Add(start);
                re.Add(end);
            }
            else if (mode == DRAW_CIRCLE)
            {
                re = getControlPointsFromEqualPolygon(450);
            }
            else if (mode == DRAW_RECTANGLE)
            {
                re = getControlPointsFromEqualPolygon(4);
            }
            else if (mode == DRAW_TRIANGLE)
            {
                re = getControlPointsFromEqualPolygon(3);
            }
            else if (mode == DRAW_PENTAGON)
            {
                re = getControlPointsFromEqualPolygon(5);
            }
            else if (mode == DRAW_HEXAGON)
            {
                re = getControlPointsFromEqualPolygon(6);
            }
            else if (mode == DRAW_ELIPSE)
            {
                /*
                 If the ellipse is ((x-cx)/a)^2 + ((y-cy)/b)^2 = 1 then change the glVertex2f call to glVertext2d(a*x + cx, b*y + cy);
                 https://stackoverflow.com/questions/5886628/effecient-way-to-draw-ellipse-with-opengl-or-d3d
                 */
                double x1 = pStart.X, x2 = pEnd.X, y1 = gl.RenderContextProvider.Height - pStart.Y, y2 = gl.RenderContextProvider.Height - pEnd.Y;
                // tinh r1, r2
                double rx = Math.Abs(x1 - x2), ry = Math.Abs(y1 - y2);

                double theta = 2 * 3.1415926 / 360.0;
                double c = Math.Cos(theta);//precalculate the sine and cosine
                double s = Math.Sin(theta);
                double t;

                double x = 1;//we start at angle = 0 
                double y = 0;
                
                for (double ii = 0; ii < 360.0; ii++)
                {
                    //apply radius and offset
                    //gl.Vertex(x * rx + x1, y * ry + y1);//output vertex 
                    re.Add(new Point((int)(x * rx + x1), (int)(y * ry + y1)));

                    //apply the rotation matrix
                    t = x;
                    x = c * x - s * y;
                    y = s * t + c * y;
                }
            }
            return re;
        }
    }
}
