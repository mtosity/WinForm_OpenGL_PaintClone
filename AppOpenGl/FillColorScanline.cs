﻿using SharpGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppOpenGl
{ 
    class FillColorScanline
    {
        private OpenGL gl;
        private Color fillColor = Color.Black;
        private List<Point> listPoints = new List<Point>();
        private List<Point> filledPoints = new List<Point>();

        public FillColorScanline(OpenGL ngl, List<Point> nPoints)
        {
            gl = ngl;
            listPoints = nPoints;
        }

        private Dictionary<string, double> calAllLinesDelta() // {name: delta} ex: {"01": 2.3}
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            int n = listPoints.Count;
            if (n > 2)
            {
                for(int i=0;i<n;i++)
                {
                    result.Add(i.ToString() + ((i + 1) % n).ToString(), 1.0 * (listPoints[i].Y - listPoints[(i + 1) % n].Y) / (listPoints[i].X - listPoints[(i + 1) % n].X));
                }
            }
            return result;
        }

        private List<LineData> getLinesByPoints()
        {
            List<LineData> re = new List<LineData>();
            int n = listPoints.Count;
            if(n > 2)
            {
                Point preP = listPoints[listPoints.Count - 1];
                for(int i=0;i<n;i++)
                {
                    if(i == 0)
                    {
                        LineData nLine = new LineData(preP, listPoints[i], (listPoints.Count - 1).ToString() + i.ToString());
                        re.Add(nLine);
                    }
                    else
                    {
                        LineData nLine = new LineData(preP, listPoints[i], (i - 1).ToString() + i.ToString());
                        re.Add(nLine);
                    }
                    preP = listPoints[i];
                }
            }
            return re;
        }
        
        private List<int> getYMinMax() // [min, max]
        {
            List<int> re = new List<int>();
            re.Add(listPoints[0].Y);
            re.Add(listPoints[0].Y);
            foreach (Point p in listPoints)
            {
                if(p.Y < re[0])
                {
                    re[0] = p.Y;
                }
                if (p.Y > re[1])
                {
                    re[1] = p.Y;
                }
            }
            return re;
        }

        public void fill()
        {
            int n = listPoints.Count;
            if (n > 2)
            {
                List<LineData> linesData = getLinesByPoints();
                List<int> yMinMax = getYMinMax();
                for(int yi = yMinMax[0]; yi <= yMinMax[1]; yi++)
                {
                    List<LineData> listLinesDidThrough = new List<LineData>();
                    List<Point> listPointsFill= new List<Point>();
                    foreach(LineData line in linesData)
                    {
                        line.setModelineGoThrough(yi);
                        int mode = line.getModeLineGoThrough();
                        if (mode != LineData.THROUGH_NONE)
                        {
                            line.updatePointGoThrough(yi);
                            listLinesDidThrough.Add(line);
                        }
                    }

                    listLinesDidThrough.Sort((a, b) =>
                    {
                        int x1 = a.getPointGoThrough().X, x2 = b.getPointGoThrough().X;
                        if (x1 > x2)
                        {
                            return 1;
                        } else if (x1 < x2)
                        {
                            return -1;
                        }
                        return 0;
                    });

                    int nDid = listLinesDidThrough.Count;
                    for(int i=0;i<nDid;i++)
                    {
                        Point p = listLinesDidThrough[i].getPointGoThrough();
                        int mode = listLinesDidThrough[i].getModeLineGoThrough();
                        //Console.WriteLine("mode " + mode);
                        if (mode == LineData.THROUGH_MIDDLE)
                        {
                            listPointsFill.Add(p);
                        }
                        if(mode == LineData.THROUGH_END || mode == LineData.THROUGH_START)
                        {
                            if (i < nDid - 1)
                            {
                                Point p1 = listLinesDidThrough[i].getPointGoThrough(), p2 = listLinesDidThrough[i + 1].getPointGoThrough();
                                if (p1.X == p2.X &&
                                    listLinesDidThrough[i].ymax >= p.Y && listLinesDidThrough[i+1].ymax > p.Y
                                    )
                                {
                                    listPointsFill.Add(p);
                                }

                                if (p1.X == p2.X &&
                                    listLinesDidThrough[i].ymin <= p.Y && listLinesDidThrough[i + 1].ymin < p.Y
                                    )
                                {
                                    listPointsFill.Add(p);
                                }
                            } 
                        }
                    }

                    if (listPointsFill.Count >= 2)
                    {
                        /*
                        gl.Begin(OpenGL.GL_LINES);
                        gl.Color(fillColor.R / 255.0, fillColor.G / 255.0, fillColor.B / 255.0, 0.0f);
                        //gl.LineWidth(width);
                        //Console.WriteLine(listPointsFill.Count);
                        foreach (Point p in listPointsFill)
                        {
                            gl.Vertex(p.X, p.Y);
                        }
                        gl.End();
                        gl.Flush();
                        */

                        foreach (Point p in listPointsFill)
                        {
                            filledPoints.Add(new Point(p.X, p.Y));
                        }
                    } 
                }
            }
        }

        public List<Point> getFilledPoints()
        {
            return filledPoints;
        }
    }
}
