using System;
using System.Collections.Generic;
using System.Drawing;


namespace TriangulationOfImage
{
    public static class ProcessingEngine
    {
        public static List<Pixel> GetPoints(Bitmap image, int sizeOfProcessingArea, float brightnessStep)
        {
            List<Pixel> points = new List<Pixel>();
            Point upperLeft = new Point(0, 0);
            Point lowerLeft = new Point(0, image.Height - 1);
            Point upperRight = new Point(image.Width - 1, 0);
            Point lowerRight = new Point(image.Width - 1, image.Height - 1);
            points.Add(new Pixel(image.GetPixel(0, 0), upperLeft));
            points.Add(new Pixel(image.GetPixel(0, image.Height - 1), lowerLeft));
            points.Add(new Pixel(image.GetPixel(image.Width - 1, 0), upperRight));
            points.Add(new Pixel(image.GetPixel(image.Width - 1, image.Height - 1), lowerRight));

            for (int I = 0; I <= image.Height - sizeOfProcessingArea; I += sizeOfProcessingArea)
            {
                for (int J = 0; J <= image.Width - sizeOfProcessingArea; J += sizeOfProcessingArea)
                {
                    Pixel[] pixels = new Pixel[sizeOfProcessingArea * sizeOfProcessingArea];
                    int k = 0;
                    for (int i = I; i < I + sizeOfProcessingArea; i++)
                    {
                        for (int j = J; j < J + sizeOfProcessingArea; j++)
                        {
                            pixels[k] = new Pixel(image.GetPixel(j, i), new Point(j, i));
                            k++;
                        }
                    }
                    var pointOnImage = FarthestPixelFromAverageByBrightness(pixels, brightnessStep);
                    if (pointOnImage != null)
                        points.Add((Pixel)pointOnImage);
                }
            }
            return points;
        }
        public static double GetLength(PointF pointf1, PointF pointf2)
        {
            return Math.Sqrt((pointf2.X - pointf1.X) * (pointf2.X - pointf1.X) + (pointf2.Y - pointf1.Y) * (pointf2.Y - pointf1.Y));
        }

        public static List<Line> GetPolygonFromTriangles(List<Triangle> triangles)
        {
            List<Line> lines = new List<Line>();
            for (int i = 0; i < triangles.Count; i++)
            {
                var pix1 = new Pixel(triangles[i].Color1, triangles[i].Point1);
                var pix2 = new Pixel(triangles[i].Color2, triangles[i].Point2);
                var pix3 = new Pixel(triangles[i].Color3, triangles[i].Point3);
                lines.Add(new Line(pix1, pix2));
                lines.Add(new Line(pix1, pix3));
                lines.Add(new Line(pix2, pix3));
            }
            for (int i = 0; i < lines.Count; i++)
            {
                for (int j = i + 1; j < lines.Count; j++)
                {
                    if (lines[i].Equals(lines[j]))
                    {
                        lines.RemoveAt(i);
                        lines.RemoveAt(j - 1);
                        i--;
                        break;
                    }
                }
            }
            return lines;
        }

        public static bool IsPointOnLine(Line line, Pixel point)
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
        public static List<Triangle> GetTriangles(List<Pixel> points)
        {
            List<Triangle> triangles = new List<Triangle>();
            triangles.Add(new Triangle(points[0], points[1], points[2]));
            triangles.Add(new Triangle(points[1], points[2], points[3]));
            for (int i = 4; i < points.Count; i++)
            {
                List<Triangle> badTriangles = new List<Triangle>();
                for (int j = 0; j < triangles.Count; j++)
                {
                    PointF center = triangles[j].Center;
                    PointF newPoint = points[i].Point1;
                    var distanceToPoint = GetLength(center, newPoint);
                    if (distanceToPoint < triangles[j].Radius)
                    {
                        badTriangles.Add(triangles[j]);
                        triangles.RemoveAt(j);
                        j--;
                    }
                }
                List<Line> linesForNewTriangles = GetPolygonFromTriangles(badTriangles);
                for (int t = 0; t < linesForNewTriangles.Count; t++)
                {
                    if (IsPointOnLine(linesForNewTriangles[t], points[i]))
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
        public static object FarthestPixelFromAverageByBrightness(Pixel[] pixels, float brightnessStep)
        {
            float sumOfPixelsBrightness = 0;
            for (int i = 0; i < pixels.Length; i++)
                sumOfPixelsBrightness += pixels[i].Color1.GetBrightness();
            float avrg = sumOfPixelsBrightness / pixels.Length;
            float maxDifference = 0;
            short indexOfFarthest = -1;
            for (int i = 0; i < pixels.Length; i++)
            {
                var t = Math.Abs(pixels[i].Color1.GetBrightness() - avrg);
                if (t > maxDifference)
                {
                    maxDifference = t;
                    indexOfFarthest =(short)i;
                }
            }
            if (maxDifference > brightnessStep)
                return pixels[indexOfFarthest];
            else
                return null;
        }
        
        public static Color GetAvrgColor(Color[] colors)
        {
            ushort red = 0;
            ushort blue = 0;
            ushort green = 0;
            for (int i = 0; i < colors.Length; i++)
            {
                red += colors[i].R;
                blue += colors[i].B;
                green += colors[i].G;
            }
            Color avrg = Color.FromArgb(red / colors.Length, green / colors.Length, blue / colors.Length);
            return avrg;
        }
    }
}
