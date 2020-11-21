using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpGL;

namespace AppOpenGl
{
    public partial class Form1 : Form
    {
        private Point pStart, pEnd;
        private bool started = false, ended = false;
        private Stopwatch watch;
        private Stopwatch stopwatch = new Stopwatch();
        public const int DRAW_LINE = 0, DRAW_CIRCLE = 1, DRAW_RECTANGLE = 2, DRAW_TRIANGLE = 3, DRAW_ELIPSE = 4, DRAW_PENTAGON = 5, DRAW_HEXAGON = 6;
        public const double pi = 3.1415926f;

        private int getDrawOption()
        {
            /*
             0: Line
             1: Circle
             2: Square
             3: Elipse
             4: Equilateral Triangle
             5: Pentagon
             6: Hexagon
            */
            if (lineRadio.Checked)
            {
                return DRAW_LINE;
            } else if (circleRadio.Checked)
            {
                return DRAW_CIRCLE;
            } else if(rectangleRadio.Checked)
            {
                return DRAW_RECTANGLE;
            }else if(triangleRadio.Checked)
            {
                return DRAW_TRIANGLE;
            }else if(elipseRadio.Checked)
            {
                return DRAW_ELIPSE;
            }else if(pentagonRadio.Checked)
            {
                return DRAW_PENTAGON;
            }else if(hexagonRadio.Checked)
            {
                return DRAW_HEXAGON;
            }
            return DRAW_LINE;
        }

        public Form1()
        {
            InitializeComponent();

            pStart.X = 0;
            pStart.Y = 0;
            pEnd.X = 0;
            pEnd.Y = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
        }

        private void drawEqualPolygon(int egdeNum) // ve da giac deu cho so canh
        {
            OpenGL gl = openGLControl.OpenGL;
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

        private void openGLControl_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            // Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            // Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Color(colorDialog1.Color.R / 255.0, colorDialog1.Color.G / 255.0, colorDialog1.Color.B / 255.0, 0.0f);
            gl.LineWidth((float)numericUpDown1.Value);
            
            int drawOp = getDrawOption();
            if(drawOp == DRAW_LINE)
            {
                gl.Begin(OpenGL.GL_LINES);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.End();

                gl.Flush();
            }
            else if(drawOp == DRAW_CIRCLE)
            {
                drawEqualPolygon(450);
            }
            else if(drawOp == DRAW_RECTANGLE)
            {
                gl.Begin(OpenGL.GL_LINE_LOOP);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pStart.Y);
                gl.Vertex(pEnd.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.Vertex(pStart.X, gl.RenderContextProvider.Height - pEnd.Y);
                gl.End();
            }
            else if(drawOp == DRAW_TRIANGLE)
            {
                drawEqualPolygon(3);
            }
            else if(drawOp == DRAW_PENTAGON)
            {
                drawEqualPolygon(5);
            }
            else if (drawOp == DRAW_HEXAGON)
            {
                drawEqualPolygon(6);
            }
            else if(drawOp == DRAW_ELIPSE)
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

            if(ended)
            {
                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                textBox1.Text = elapsedMs.ToString() + " ms";
                ended = false;
            }
        }

        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            // Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            // Set the clear color.
            gl.ClearColor(255, 255, 255, 255);
            // Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            // Load the identity.
            gl.LoadIdentity();
        }

        private void openGLControl_Resized(object sender, EventArgs e)
        { 
            // Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            // Set the projection matrix.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            // Load the identity.
            gl.LoadIdentity();
            // Create a perspective transformation.
            gl.Viewport(0, 0, openGLControl.Width, openGLControl.Height);
            gl.Ortho2D(0, openGLControl.Width, 0, openGLControl.Height);
        }

        private void openGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            pEnd = e.Location;
            started = false;
            watch = System.Diagnostics.Stopwatch.StartNew();
            ended = true;
        }

        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            pStart = e.Location;
            pEnd = pStart;
            started = true;
        }

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (started)
            {
                pEnd = e.Location;
            }
        }
    }
}
