using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangulationOfImage
{
    public static class ProcessingEngine
    {
        public static List<Pixel> GetPoints(Bitmap image, int divider, float step)
        {
            List<Pixel> points = new List<Pixel>();
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, image.Height - 1);
            Point p3 = new Point(image.Width - 1, 0);
            Point p4 = new Point(image.Width - 1, image.Height - 1);
            points.Add(new Pixel(image.GetPixel(0, 0), p1));
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
        public static object GetFarthestElementFromAverage(Pixel[] array, float step)
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
    }
}
