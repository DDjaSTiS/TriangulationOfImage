using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;
using System.Diagnostics;


namespace TriangulationOfImage
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                var bitmap = new Bitmap(openFileDialog1.FileName);
                pictureBox1.Image = bitmap;
            }
        }

        private List<Pixel> GetPoints(Bitmap image, int divider, float step)
        {
            List<Pixel> points = new List<Pixel>();
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, image.Height - 1);
            Point p3 = new Point(image.Width - 1, 0);
            Point p4 = new Point(image.Width - 1, image.Height - 1);
            points.Add(new Pixel(image.GetPixel(0, 0),p1));
            points.Add(new Pixel(image.GetPixel(0, image.Height - 1), p2));
            points.Add(new Pixel(image.GetPixel(image.Width - 1, 0), p3));
            points.Add(new Pixel(image.GetPixel(image.Width - 1, image.Height - 1), p4));

            for (int I = 0; I <= image.Height - divider; I += divider)
            {
                for (int J = 0; J <= image.Width - divider; J += divider)
                {
                    Pixel[] pixels = new Pixel[divider * divider];
                    int k = 0;
                    for (int i = I; i < I + divider; i++)
                    {
                        for (int j = J; j < J + divider; j++)
                        {
                            Point z = new Point(j, i);
                            pixels[k] = new Pixel(image.GetPixel(j, i), z);
                            k++;
                        }
                    }
                    var pointOnImage = GetFarthestElementFromAverage(pixels, step);
                    if (pointOnImage != null)
                        points.Add((Pixel)pointOnImage);
                }
            }
            return points;
        }
        private object GetFarthestElementFromAverage(Pixel[] array, float step)
        {
            float sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i].Color1.GetBrightness();
            float avrg = sum / array.Length;
            float maxDifference = 0;
            int indexOfFarthest = -1;
            for (int i = 0; i < array.Length; i++)
            {
                var t = Math.Abs(array[i].Color1.GetBrightness() - avrg);
                if (t > maxDifference)
                {
                    maxDifference = t;
                    indexOfFarthest = i;
                }
            }
            if (maxDifference > step)
                return array[indexOfFarthest];
            else
                return null;
        }

        private Color GetAvrgColor(Color[] colors)
        {
            int red = 0;
            int blue = 0;
            int green = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                red += colors[i].R;
                blue += colors[i].B;
                green += colors[i].G;
            }
            Color avrg = Color.FromArgb(red / colors.Length, green / colors.Length, blue / colors.Length);
            return avrg;
        }

        private static List<Line> GetPolygonFromTriangle (List<Triangle> triangles)
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i < triangles.Count; i++)
            {
                var pix1 = new Pixel(triangles[i].Color1, triangles[i].Point1);
                var pix2 = new Pixel(triangles[i].Color2, triangles[i].Point2);
                var pix3 = new Pixel(triangles[i].Color3, triangles[i].Point3);
                lines.Add(new Line(pix1, pix2));
                lines.Add(new Line(pix1,pix3));
                lines.Add(new Line(pix2,pix3));
            }
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i+1; j < lines.Count; j++)
                {
                    if(lines[i].Equals(lines[j]))
                    {
                        lines.RemoveAt(i);
                        lines.RemoveAt(j-1);
                        i--;
                        break;
                    }
                }
            }
            return lines;
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var bitmap = new Bitmap(openFileDialog1.FileName);
            var points= GetPoints(bitmap,(int)nud1.Value, (float)nud2.Value);
            Text = "Points Done";
            Graphics gr = Graphics.FromImage(bitmap);
            Pen p = new Pen(Color.Black, 1);
            var triangles = GetTriangles(points);
            for (int i = 0; i < triangles.Count; i++)
            {
                Color[] colors = new Color[3];
                colors[0] = triangles[i].Color1;
                colors[1] = triangles[i].Color2;
                colors[2] = triangles[i].Color3;
                Color avrg = GetAvrgColor(colors);
                Brush brush = new SolidBrush(avrg);
                gr.FillPolygon(brush, triangles[i].Points);
                //gr.DrawPolygon(p, triangles[i].Points);
            }
            pictureBox1.Image = bitmap;
            gr.Dispose();
            sw.Stop();
            Text = points.Count().ToString() + "; " +sw.ElapsedMilliseconds.ToString();
        }

        private List<Triangle> GetTriangles (List<Pixel> points)
        {
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(points[0], points[1], points[2]));
            triangles.Add(new Triangle(points[1], points[2], points[3]));
            for (int i = 4 ; i < points.Count; i++)
            {
                List<Triangle> badTriangles = new List<Triangle>();
                for (int k = 0; k < triangles.Count; k++)
                {
                    PointF center = triangles[k].Center;
                    PointF newPoint = points[i].Point1;
                    var distanceToPoint = GetLength(center, newPoint);
                    if (distanceToPoint < triangles[k].Radius)
                    {
                        badTriangles.Add(triangles[k]);
                        triangles.RemoveAt(k);
                        k--;
                    }
                }
                List<Line> linesForNewTriangles = GetPolygonFromTriangle(badTriangles);
                for (int t = 0; t < linesForNewTriangles.Count; t++)
                {
                    if(IsPointOnLine(linesForNewTriangles[t],points[i]))
                    { }
                    else
                    {
                        var pix1 = new Pixel(linesForNewTriangles[t].Color1, linesForNewTriangles[t].Point1);
                        var pix2 = new Pixel(linesForNewTriangles[t].Color2, linesForNewTriangles[t].Point2);
                        triangles.Add(new Triangle(pix1, pix2, points[i]));
                    }
                }
            }
            return triangles;
        }

        public double GetLength(PointF pointf1,PointF pointf2)
        {
            return Math.Sqrt((pointf2.X - pointf1.X) * (pointf2.X - pointf1.X) + (pointf2.Y - pointf1.Y) * (pointf2.Y - pointf1.Y));
        }

        private bool IsPointOnLine(Line line, Pixel point)
        {
            PointF pf1 = line.Point1;
            PointF pf2 = line.Point2;
            double fromStartToPoint = GetLength(pf1, point.Point1);
            double fromEndToPoint = GetLength(pf2, point.Point1);
            double lengthOfLine = GetLength(pf1, pf2);
            double sum = fromStartToPoint + fromEndToPoint;
            if (Math.Abs(sum - lengthOfLine) < 0.000001)
                return true;
            else
                return false;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("result.png", ImageFormat.Png);
            Text = "Saved!";
        }
    }
}
