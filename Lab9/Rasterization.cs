using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Lab6_9.Form1;

namespace Lab6_9
{
    internal class Raster
    {
        public static void Triangulate(ref Object3D obj)
        {
            List<Face> faces = new List<Face>();
            foreach (Face f in obj.Faces)
            {
                if (f.FaceIndices.Count == 3)
                {
                    faces.Add(f);
                    continue;
                }

                for (int i = 2; i < f.FaceIndices.Count; i++)
                {
                    Face newf = new Face();
                    newf.FaceIndices.Add(f.FaceIndices[0]);
                    newf.FaceIndices.Add(f.FaceIndices[i - 1]);
                    newf.FaceIndices.Add(f.FaceIndices[i]);
                    faces.Add(newf);
                }

            }

            obj.Faces = faces;
        }

        public static int Interpolation(float x0, float y0, float x1, float y1, float x)
        {
            return (int)Math.Round(y0 + (float)(y1 - y0) * (x - x0) / (x1 - x0));
        }

        public static void Rasterization(List<Point3D> points, float[,] ZBuffer, PictureBox pictureBox, Bitmap bm, Color color1, Color color2, float minZ, float maxZ)
        {
            points = points.Select(p => new Point3D((float)Math.Round(p.X), (float)Math.Round(p.Y), p.Z, p.W)).ToList();
            points.Sort((a, b) => a.Y == b.Y ? 0 : (a.Y < b.Y ? -1 : 1));

            List<Color> colors = points.Select(p => Color.FromArgb(Interpolation(minZ, color1.R, maxZ, color2.R, p.Z),
                                                                    Interpolation(minZ, color1.G, maxZ, color2.G, p.Z),
                                                                    Interpolation(minZ, color1.B, maxZ, color2.B, p.Z))).ToList();

            float inc12, inc13, inc23;

            if (points[0].Y == points[1].Y)
                inc12 = 0;
            else
                inc12 = (float)(points[1].X - points[0].X) / (points[1].Y - points[0].Y);

            if (points[0].Y == points[2].Y)
                inc13 = 0;
            else
                inc13 = (float)(points[2].X - points[0].X) / (points[2].Y - points[0].Y);

            if (points[1].Y == points[2].Y)
                inc23 = 0;
            else
                inc23 = (float)(points[2].X - points[1].X) / (points[2].Y - points[1].Y);

            float x1 = points[0].X;
            float x2 = x1;

            float _inc13 = inc13;

            if (inc13 > inc12)
                (inc13, inc12) = (inc12, inc13);

            int left, right;
            (left, right) = points[1].X < Interpolation(points[0].Y, points[0].X, points[2].Y, points[2].X, points[1].Y) ? (1, 2) : (2, 1);

            for (int i = (int)(points[0].Y); i < (int)(points[1].Y); i++)
            {
                int cLeftR = Interpolation(points[0].Y, colors[0].R, points[left].Y, colors[left].R, i);
                int cLeftG = Interpolation(points[0].Y, colors[0].G, points[left].Y, colors[left].G, i);
                int cLeftB = Interpolation(points[0].Y, colors[0].B, points[left].Y, colors[left].B, i);

                int cRightR = Interpolation(points[0].Y, colors[0].R, points[right].Y, colors[right].R, i);
                int cRightG = Interpolation(points[0].Y, colors[0].G, points[right].Y, colors[right].G, i);
                int cRightB = Interpolation(points[0].Y, colors[0].B, points[right].Y, colors[right].B, i);

                int zLeft = Interpolation(points[0].Y, points[0].Z, points[left].Y, points[left].Z, i);
                int zRight = Interpolation(points[0].Y, points[0].Z, points[right].Y, points[right].Z, i);

                for (int j = (int)x1; j < (int)x2; j++)
                {
                    int R = Interpolation((int)x1, cLeftR, (int)x2, cRightR, j);
                    int G = Interpolation((int)x1, cLeftG, (int)x2, cRightG, j);
                    int B = Interpolation((int)x1, cLeftB, (int)x2, cRightB, j);

                    int z = Interpolation((int)x1, zLeft, (int)x2, zRight, j);
                    if (pictureBox.Width / 2 + j > pictureBox.Width - 1 || pictureBox.Width / 2 + j < 0 || pictureBox.Height / 2 + i > pictureBox.Height - 1 || pictureBox.Height / 2 + i < 0) continue;
                    if (ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;
                        //_g.DrawRectangle(new Pen(Color.FromArgb(R, G, B)), j, i, 1, 1);
                        bm.SetPixel(pictureBox.Width / 2 + j, pictureBox.Height - pictureBox.Height / 2 + i, Color.FromArgb(R, G, B));
                    }
                }
                x1 += inc13;
                x2 += inc12;
            }
            if (points[0].Y == points[1].Y)
            {
                x1 = Math.Min(points[0].X, points[1].X);
                x2 = Math.Max(points[0].X, points[1].X);
            }
            if (_inc13 < inc23)
                (_inc13, inc23) = (inc23, _inc13);

            (left, right) = Interpolation(points[0].Y, points[0].X, points[2].Y, points[2].X, points[1].Y) < points[1].X ? (0, 1) : (1, 0);

            for (int i = (int)(points[1].Y); i < (int)(points[2].Y); i++)
            {
                int cLeftR = Interpolation(points[2].Y, colors[2].R, points[left].Y, colors[left].R, i);
                int cLeftG = Interpolation(points[2].Y, colors[2].G, points[left].Y, colors[left].G, i);
                int cLeftB = Interpolation(points[2].Y, colors[2].B, points[left].Y, colors[left].B, i);

                int cRightR = Interpolation(points[2].Y, colors[2].R, points[right].Y, colors[right].R, i);
                int cRightG = Interpolation(points[2].Y, colors[2].G, points[right].Y, colors[right].G, i);
                int cRightB = Interpolation(points[2].Y, colors[2].B, points[right].Y, colors[right].B, i);

                int zLeft = Interpolation(points[2].Y, points[2].Z, points[left].Y, points[left].Z, i);
                int zRight = Interpolation(points[2].Y, points[2].Z, points[right].Y, points[right].Z, i);

                for (int j = (int)x1; j < (int)x2; j++)
                {
                    int R = Interpolation((int)x1, cLeftR, (int)x2, cRightR, j);
                    int G = Interpolation((int)x1, cLeftG, (int)x2, cRightG, j);
                    int B = Interpolation((int)x1, cLeftB, (int)x2, cRightB, j);

                    int z = Interpolation((int)x1, zLeft, (int)x2, zRight, j);
                    if (pictureBox.Width / 2 + j > pictureBox.Width - 1 || pictureBox.Width / 2 + j < 0 || pictureBox.Height / 2 + i > pictureBox.Height - 1 || pictureBox.Height / 2 + i < 0) continue;
                    if (ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;
                        //_g.DrawRectangle(new Pen(Color.FromArgb(R, G, B)), j, i, 1, 1);
                        bm.SetPixel(pictureBox.Width / 2 + j, pictureBox.Height - pictureBox.Height / 2 + i, Color.FromArgb(R, G, B));
                    }
                }
                x1 += _inc13;
                x2 += inc23;
            }

        }
    }
}
