using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TriangulationOfImage
{
    public class Line: Pixel
    {        
        public Point Point2 { get; set; }    
        public Color Color2 { get; set; }

        public Line (Pixel pix1, Pixel pix2) : base(pix1,pix2)
        {
            Point1 = pix1.Point1;
            Point2 = pix2.Point1;
            Color1 = pix1.Color1;
            Color2 = pix2.Color1;           
        }
        public Line(Pixel p1, Pixel p2, Pixel p3) : this(p1,p2)
        {
            Point1 = p1.Point1;
            Point2 = p2.Point1;
            Color1 = p1.Color1;
            Color2 = p2.Color1;
        }
        public bool Equals(Line l1)
        {
            if (Point1 == l1.Point1 && Point2 == l1.Point2)
                return true;
            else
                return false;
        }
    }
}
