using System.Drawing;

namespace TriangulationOfImage
{
    public class Pixel
    {
        public Color Color1 { get; set; }
        public Point Point1 { get; set; }
       
        public Pixel(Color color, Point point)
        {
            Color1 = color;
            Point1 = point;
        }
        public Pixel(Pixel p1, Pixel p2) : this(p1.Color1,p1.Point1)
        {
            Color1 = p1.Color1;
            Point1 = p1.Point1;
        }
    }
}
