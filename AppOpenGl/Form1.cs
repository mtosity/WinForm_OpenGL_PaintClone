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
        // started: luc bat dau mouse down va dang di chuyen tren man hinh, luc nay ended = false
        // ended: luc ket thuc mouse up, luc nay started = false
        private Stopwatch watch;
        private Stopwatch stopwatch = new Stopwatch();
        public const int DRAW_LINE = 0, DRAW_CIRCLE = 1, DRAW_RECTANGLE = 2, DRAW_TRIANGLE = 3, DRAW_ELIPSE = 4, DRAW_PENTAGON = 5, DRAW_HEXAGON = 6;
        public const double pi = 3.1415926f;

        //List<DrawChosenOption> listDrawing = new List<DrawChosenOption>();
        List<FillColorSpread> listColoring = new List<FillColorSpread>();
        List<FillColorScanline> listColoringScan = new List<FillColorScanline>();
        DrawRandomPolygon drawRandomPolygon; // de ve duong live cho ve da giac random
        //List<DrawRandomPolygon> listDrawingRandom = new List<DrawRandomPolygon>();

        List<DrawHelper> listDrawHelper = new List<DrawHelper>();
        List<Polygon> listPolygons = new List<Polygon>();
        Polygon selectedPolygon = new Polygon();

        private int getDrawingOption()
        {
            if (lineRadio.Checked)
            {
                return DrawChosenOption.DRAW_LINE;
            } else if (circleRadio.Checked)
            {
                return DrawChosenOption.DRAW_CIRCLE;
            } else if(rectangleRadio.Checked)
            {
                return DrawChosenOption.DRAW_RECTANGLE;
            }else if(triangleRadio.Checked)
            {
                return DrawChosenOption.DRAW_TRIANGLE;
            }else if(elipseRadio.Checked)
            {
                return DrawChosenOption.DRAW_ELIPSE;
            }else if(pentagonRadio.Checked)
            {
                return DrawChosenOption.DRAW_PENTAGON;
            }else if(hexagonRadio.Checked)
            {
                return DrawChosenOption.DRAW_HEXAGON;
            }
            return DrawChosenOption.DRAW_LINE;
        }

        private int getColoringOption()
        {
            if (lineRadio.Checked)
            {
                return DrawChosenOption.DRAW_LINE;
            }
            else if (circleRadio.Checked)
            {
                return DrawChosenOption.DRAW_CIRCLE;
            }
            else if (rectangleRadio.Checked)
            {
                return DrawChosenOption.DRAW_RECTANGLE;
            }
            else if (triangleRadio.Checked)
            {
                return DrawChosenOption.DRAW_TRIANGLE;
            }
            else if (elipseRadio.Checked)
            {
                return DrawChosenOption.DRAW_ELIPSE;
            }
            else if (pentagonRadio.Checked)
            {
                return DrawChosenOption.DRAW_PENTAGON;
            }
            else if (hexagonRadio.Checked)
            {
                return DrawChosenOption.DRAW_HEXAGON;
            }
            return DrawChosenOption.DRAW_LINE;
        }

        void resetPoints()
        {
            pStart.X = 0;
            pStart.Y = 0;
            pEnd.X = 0;
            pEnd.Y = 0;
        }

        public Form1()
        {
            InitializeComponent();
            OpenGL gl = openGLControl.OpenGL;

            drawRandomPolygon = new DrawRandomPolygon(gl);
            resetPoints();
            colorDialog1.Color = Color.Black;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<Point> cPoints = selectedPolygon.getPoints();
            FillColorScanline fcs = new FillColorScanline(openGLControl.OpenGL, cPoints);
            listColoringScan.Add(fcs);
        }

        private void openGLControl_MouseClick(object sender, MouseEventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            if (e.Button == MouseButtons.Left)
            {
                if (drawRandomRadio.Checked)
                {
                    started = true;
                    ended = false;
                    Point nextP = e.Location;
                    drawRandomPolygon.addPoint(nextP);
                }
                else if (coloringRadio.Checked) // Dang to mau
                {
                    Point startP = e.Location;
                    FillColorSpread fc = new FillColorSpread(gl, getColoringOption(), startP, colorDialog1.Color);
                    fc.FloodStackFill(Color.White);
                    listDrawHelper.Add(new DrawHelper(gl, OpenGL.GL_POINTS, fc.getFilledPoints()));
                } else if(selectRadio.Checked)
                {
                    Point p = e.Location;
                    p.Y = gl.RenderContextProvider.Height - p.Y;
                    bool selected = false;
                    foreach (Polygon polygon in listPolygons)
                    {
                        if (polygon.isPointInside(p))
                        {
                            Console.WriteLine("selected");
                            selectedPolygon.setPoints(polygon.getPoints());
                            selected = true;
                            break;
                        }
                    }

                    if (!selected)
                    {
                        //neu diem khong thuoc da giac nao, clear
                        Console.WriteLine("nothing selected");
                        selectedPolygon.clearPoints();
                    }
                }
            }

            if (e.Button == MouseButtons.Right)
            {
                if (drawRandomRadio.Checked)
                {
                    watch = System.Diagnostics.Stopwatch.StartNew();
                    drawRandomPolygon.setConfig(colorDialog1.Color, (float)numericUpDown1.Value);
                    //DrawRandomPolygon newDrp = new DrawRandomPolygon(drp);
                    List<Point> points = drawRandomPolygon.getControlPoints();
                    listDrawHelper.Add(new DrawHelper(gl, OpenGL.GL_LINE_LOOP, points, colorDialog1.Color, (float)numericUpDown1.Value));
                    listPolygons.Add(new Polygon(points));
                    //reset
                    started = false;
                    ended = true;
                    //resetPoints();
                    drawRandomPolygon.clearPoints();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button1.BackColor = colorDialog1.Color;

                var luma = 0.2126 * colorDialog1.Color.R + 0.7152 * colorDialog1.Color.G + 0.0722 * colorDialog1.Color.B; 
                // per ITU-R BT.709, de xet background dark hay light de set text color cho phu hop :D
                // https://stackoverflow.com/questions/12043187/how-to-check-if-hex-color-is-too-black

                if (luma < 40)
                {
                    button1.ForeColor = Color.White;
                }
                else
                {
                    button1.ForeColor = Color.Black;
                }
            }
        }

        private void openGLControl_OpenGLDraw(object sender, SharpGL.RenderEventArgs args)
        {
            OpenGL gl = openGLControl.OpenGL;
            // Clear the color and depth buffer.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            foreach(DrawHelper drawHelper in listDrawHelper)
            {
                drawHelper.draw();
            }

            List<Point> controlPoints = selectedPolygon.getPoints();
            if (controlPoints.Count > 0)
            {
                DrawHelper controlPointsDraw = new DrawHelper(gl, OpenGL.GL_POINTS, controlPoints, colorDialog1.Color, 1.0f, 5.0f);
                controlPointsDraw.draw();
            }


            if (started) // neu mouse dang down va di chuyen, ve duong live tuy vao draw cai gi
            {
                if (drawingRadio.Checked)
                {
                    int drawingMode = getDrawingOption();
                    DrawChosenOption liveDraw = new DrawChosenOption(drawingMode, pStart, pEnd, gl, colorDialog1.Color, (float)numericUpDown1.Value);

                    uint openglMode = (drawingMode == DRAW_LINE) ? OpenGL.GL_LINES : OpenGL.GL_LINE_LOOP;
                    DrawHelper liveDrawHelper = new DrawHelper(gl, openglMode, liveDraw.getDrawingPoints(), colorDialog1.Color, (float)numericUpDown1.Value);
                    liveDrawHelper.draw();
                } else if(drawRandomRadio.Checked)
                {
                    drawRandomPolygon.setConfig(colorDialog1.Color, (float)numericUpDown1.Value);
                    drawRandomPolygon.drawLive(pEnd);
                }
            }

            if (ended) // da mouse up, tinh thoi gian ve hinh
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
            if (drawingRadio.Checked)
            {
                // update toa do end
                pEnd = e.Location;
                // Them action ve hinh vao snapshot
                OpenGL gl = openGLControl.OpenGL;
                int drawingMode = getDrawingOption();
                DrawChosenOption newDrawChosen = new DrawChosenOption(drawingMode, pStart, pEnd, gl, colorDialog1.Color, (float)numericUpDown1.Value);
                uint openglMode = (drawingMode == DRAW_LINE) ? OpenGL.GL_LINES : OpenGL.GL_LINE_LOOP;
                listDrawHelper.Add(new DrawHelper(gl, openglMode, newDrawChosen.getDrawingPoints(), colorDialog1.Color, (float)numericUpDown1.Value));
                listPolygons.Add(new Polygon(newDrawChosen.getControlPoints()));
                // update status (da xong ve~ di chuyen tren man hinh)
                started = false;
                ended = true;
                // Bat dau stop watch dem thoi gian thuc thi ve hinh
                watch = System.Diagnostics.Stopwatch.StartNew();
                // Reset 2 diem
                resetPoints();
            }
        }

        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (drawingRadio.Checked)
            {
                pStart = e.Location;
                pEnd = pStart;
                started = true;
            }
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
