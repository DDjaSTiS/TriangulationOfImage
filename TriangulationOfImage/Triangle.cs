using System;
using System.Drawing;

namespace TriangulationOfImage
{
    public class Triangle : Line
    {
        public Color Color3 { get; set; }
        public PointF Center { get; set; }
        public double Radius { get; set; }
        public Point[] Points { get; set; }
        public Triangle (Pixel p1, Pixel p2, Pixel p3) : base (p1,p2,p3)
        {
            Color1 = p1.Color1;
            Color2 = p2.Color1;
            Color3 = p3.Color1;
            Points = new Point[3];
            Points[0] = p1.Point1;
            Points[1] = p2.Point1;
            Points[2] = p3.Point1;
            GetCenter();
            GetRadius();
        }
        private void GetRadius()
        {
            Radius = Math.Sqrt((Point1.X - Center.X) * (Point1.X - Center.X) + (Point1.Y - Center.Y) * (Point1.Y - Center.Y));
        }
        public void GetCenter()
        {
            PointF PF1 = Points[0];
            PointF PF2 = Points[1];
            PointF PF3 = Points[2];
            PointF centerPoint1 = new PointF((PF2.X + PF1.X)/2, (PF2.Y + PF1.Y)/2);
            PointF centerPoint2 = new PointF((PF3.X + PF2.X)/2, (PF3.Y + PF2.Y)/2);
            float k1;
            float k2;
            float x;
            float y;
            if (PF2.X == PF1.X)
            {
                k2 = (PF3.Y - PF2.Y) / (PF3.X - PF2.X);
                y = centerPoint1.Y;
                x = centerPoint2.X - k2 * (centerPoint1.Y - centerPoint2.Y);
            }
            else if(PF2.X == PF3.X)
            {
                k1 = (PF2.Y - PF1.Y) / (PF2.X - PF1.X);
                y = centerPoint2.Y;
                x = centerPoint1.X - k1 * (centerPoint2.Y - centerPoint1.Y);
            }
            else
            {
                k1 = (PF2.Y - PF1.Y) / (PF2.X - PF1.X);
                k2 = (PF3.Y - PF2.Y) / (PF3.X - PF2.X);
                if(k1==0)
                {
                    k2 = (PF3.Y - PF2.Y) / (PF3.X - PF2.X);
                    x = centerPoint1.X;
                    y = centerPoint2.X/k2 - centerPoint1.X/k2+centerPoint2.Y;
                }
                else if(k2==0)
                {
                    k1 = (PF2.Y - PF1.Y) / (PF2.X - PF1.X);
                    x = centerPoint2.X;
                    y = centerPoint1.X / k1 - centerPoint2.X / k1 + centerPoint1.Y;
                }
                else
                {
                    x = (centerPoint2.X / k2 + centerPoint2.Y - centerPoint1.X / k1 - centerPoint1.Y)/(1/k2-1/k1);
                    y = -x/k1+centerPoint1.Y+centerPoint1.X/k1;
                }
            }
            Center = new PointF(x, y);
        }
    }
}
