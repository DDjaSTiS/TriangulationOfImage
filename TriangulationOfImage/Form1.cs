using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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

        private void button1_Click(object sender, EventArgs e)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var bitmap = new Bitmap(openFileDialog1.FileName);
            var points= ProcessingEngine.GetPoints(bitmap,(int)nud1.Value, (float)nud2.Value);
            Graphics gr = Graphics.FromImage(bitmap);
            Pen p = new Pen(Color.Black, 1);
            var triangles =ProcessingEngine.GetTriangles(points);
            for (int i = 0; i < triangles.Count; i++)
            {
                Color[] colors = new Color[3];
                colors[0] = triangles[i].Color1;
                colors[1] = triangles[i].Color2;
                colors[2] = triangles[i].Color3;
                Color avrg =ProcessingEngine.GetAvrgColor(colors);
                Brush brush = new SolidBrush(avrg);
                gr.FillPolygon(brush, triangles[i].Points);
                //gr.DrawPolygon(p, triangles[i].Points);
            }
            pictureBox1.Image = bitmap;
            gr.Dispose();
            sw.Stop();
            Text = points.Count().ToString() + "; " +sw.ElapsedMilliseconds.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image.Save("result.png", ImageFormat.Png);
            Text = "Saved!";
        }
    }
}
