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

        public static float Interpolation1(float x0, float y0, float x1, float y1, float x)
        {
            return y0 + (float)(y1 - y0) * (x - x0) / (x1 - x0);
        }

        public static float Clamp(float x, float min, float max)
        {
            return Math.Min(Math.Max(x, min), max);
        }

        public static void Rasterization(List<Point3D> points, float[,] ZBuffer, PictureBox pictureBox, Bitmap bm, List<Color> colors, Graphics g)
        {
            points = points.Select(p => new Point3D((float)Math.Round(p.X), (float)Math.Round(p.Y), p.Z, p.W)).ToList();

            List<(Point3D, Color)> temp = (new List<int> { 0, 1, 2 }).Select(i => (points[i], colors[i])).ToList();

            temp.Sort((a, b) => a.Item1.Y == b.Item1.Y ? 0 : (a.Item1.Y < b.Item1.Y ? -1 : 1));

            points = temp.Select(x => x.Item1).ToList();
            colors = temp.Select(x => x.Item2).ToList();

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
                        if (pictureBox.Width / 2 + j < bm.Width && pictureBox.Height - pictureBox.Height / 2 + i < bm.Height &&
                            pictureBox.Width / 2 + j > 0 && pictureBox.Height - pictureBox.Height / 2 + i > 0)
                            g.DrawRectangle(new Pen(Color.FromArgb(R, G, B)), j, i, 1, 1);
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
                        if (pictureBox.Width / 2 + j < bm.Width && pictureBox.Height - pictureBox.Height / 2 + i < bm.Height &&
                            pictureBox.Width / 2 + j > 0 && pictureBox.Height - pictureBox.Height / 2 + i > 0)
                            g.DrawRectangle(new Pen(Color.FromArgb(R, G, B)), j, i, 1, 1);
                    }
                }
                x1 += _inc13;
                x2 += inc23;
            }

        }

        public static void RasterizationFong(List<Point3D> points, List<Point3D> normals, float[,] ZBuffer, PictureBox pictureBox, Bitmap bm, Graphics g, Color color, Light light, int colorSubdivision)
        {
            points = points.Select(p => new Point3D((float)Math.Round(p.X), (float)Math.Round(p.Y), p.Z, p.W)).ToList();
            List<(Point3D, Point3D)> temp = (new List<int> { 0, 1, 2 }).Select(i => (points[i], normals[i])).ToList();

            temp.Sort((a, b) => a.Item1.Y == b.Item1.Y ? 0 : (a.Item1.Y < b.Item1.Y ? -1 : 1));

            points = temp.Select(x => x.Item1).ToList();
            normals = temp.Select(x => x.Item2).ToList();

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
                float nLeftX = Interpolation1(points[0].Y, normals[0].X, points[left].Y, normals[left].X, i);
                float nLeftY = Interpolation1(points[0].Y, normals[0].Y, points[left].Y, normals[left].Y, i);
                float nLeftZ = Interpolation1(points[0].Y, normals[0].Z, points[left].Y, normals[left].Z, i);

                float nRightX = Interpolation1(points[0].Y, normals[0].X, points[right].Y, normals[right].X, i);
                float nRightY = Interpolation1(points[0].Y, normals[0].Y, points[right].Y, normals[right].Y, i);
                float nRightZ = Interpolation1(points[0].Y, normals[0].Z, points[right].Y, normals[right].Z, i);

                int zLeft = Interpolation(points[0].Y, points[0].Z, points[left].Y, points[left].Z, i);
                int zRight = Interpolation(points[0].Y, points[0].Z, points[right].Y, points[right].Z, i);

                for (int j = (int)x1; j < (int)x2; j++)
                {
                    float X = Interpolation1((int)x1, nLeftX, (int)x2, nRightX, j);
                    float Y = Interpolation1((int)x1, nLeftY, (int)x2, nRightY, j);
                    float Z = Interpolation1((int)x1, nLeftZ, (int)x2, nRightZ, j);

                    int z = Interpolation((int)x1, zLeft, (int)x2, zRight, j);
                    if (pictureBox.Width / 2 + j > pictureBox.Width - 1 || pictureBox.Width / 2 + j < 0 || pictureBox.Height / 2 + i > pictureBox.Height - 1 || pictureBox.Height / 2 + i < 0) continue;
                    if (ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;

                        Point3D v = new Point3D(-j, -i, -z);
                        v.Normalize();

                        Point3D l = light.ViewLocation - new Point3D(j, i, z); // Light to point
                        l.Normalize();

                        Point3D n = new Point3D(X, Y, Z);
                        n.Normalize();

                        float nl = n * l;
                        Point3D h = n; h *= 2 * nl; h -= l; // h = 2*nl*n - l  
                        h.Normalize();
                        float nh = Math.Max(0.0f, h * v);

                        float D = Clamp(Math.Max(0.0f, light.DiffuseIntensity * nl), 0.0f, 1.0f);
                        float S = Clamp(light.SpecularIntensity * (float)Math.Pow(nh, 2), 0.0f, 1.0f);
                        if (D == 0) S = 0;

                        int R = (int)Clamp((color.R * (light.AmbientIntensity + D) + 255f * S), 0, 255);
                        int G = (int)Clamp((color.G * (light.AmbientIntensity + D) + 255f * S), 0, 255);
                        int B = (int)Clamp((color.B * (light.AmbientIntensity + D) + 255f * S), 0, 255);

                        int step = (int)((255 - (color.R * light.AmbientIntensity)) / colorSubdivision);
                        R = Interpolation(color.R * light.AmbientIntensity, 0, 255, colorSubdivision, R) * step + (int)(color.R * light.AmbientIntensity);
                        step = (int)((255 - (color.G * light.AmbientIntensity)) / colorSubdivision);
                        G = Interpolation(color.G * light.AmbientIntensity, 0, 255, colorSubdivision, G) * step + (int)(color.G * light.AmbientIntensity);
                        step = (int)((255 - (color.B * light.AmbientIntensity)) / colorSubdivision);
                        B = Interpolation(color.B * light.AmbientIntensity, 0, 255, colorSubdivision, B) * step + (int)(color.B * light.AmbientIntensity);

                        Color newColor = Color.FromArgb(R, G, B);

                        if (pictureBox.Width / 2 + j < bm.Width && pictureBox.Height - pictureBox.Height / 2 + i < bm.Height &&
                            pictureBox.Width / 2 + j > 0 && pictureBox.Height - pictureBox.Height / 2 + i > 0)
                            g.DrawRectangle(new Pen(newColor), j, i, 1, 1);
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
                float nLeftX = Interpolation1(points[2].Y, normals[2].X, points[left].Y, normals[left].X, i);
                float nLeftY = Interpolation1(points[2].Y, normals[2].Y, points[left].Y, normals[left].Y, i);
                float nLeftZ = Interpolation1(points[2].Y, normals[2].Z, points[left].Y, normals[left].Z, i);

                float nRightX = Interpolation1(points[2].Y, normals[2].X, points[right].Y, normals[right].X, i);
                float nRightY = Interpolation1(points[2].Y, normals[2].Y, points[right].Y, normals[right].Y, i);
                float nRightZ = Interpolation1(points[2].Y, normals[2].Z, points[right].Y, normals[right].Z, i);

                int zLeft = Interpolation(points[2].Y, points[2].Z, points[left].Y, points[left].Z, i);
                int zRight = Interpolation(points[2].Y, points[2].Z, points[right].Y, points[right].Z, i);

                for (int j = (int)x1; j < (int)x2; j++)
                {
                    float X = Interpolation1((int)x1, nLeftX, (int)x2, nRightX, j);
                    float Y = Interpolation1((int)x1, nLeftY, (int)x2, nRightY, j);
                    float Z = Interpolation1((int)x1, nLeftZ, (int)x2, nRightZ, j);

                    int z = Interpolation((int)x1, zLeft, (int)x2, zRight, j);
                    if (pictureBox.Width / 2 + j > pictureBox.Width - 1 || pictureBox.Width / 2 + j < 0 || pictureBox.Height / 2 + i > pictureBox.Height - 1 || pictureBox.Height / 2 + i < 0) continue;
                    if (ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;

                        Point3D v = new Point3D(-j, -i, -z);
                        v.Normalize();

                        Point3D l = light.ViewLocation - new Point3D(j, i, z); // Light to point
                        l.Normalize();

                        Point3D n = new Point3D(X, Y, Z);
                        n.Normalize();

                        float nl = l * n;
                        Point3D h = n; h *= 2 * nl; h -= l;
                        h.Normalize();
                        float nh = Math.Max(0.0f, h * v);

                        float D = Clamp(Math.Max(0.0f, light.DiffuseIntensity * nl), 0.0f, 1.0f);
                        float S = Clamp(light.SpecularIntensity * (float)Math.Pow(nh, 2), 0.0f, 1.0f);
                        if (D == 0) S = 0;

                        int R = (int)Clamp((color.R * (light.AmbientIntensity + D) + 255f * S), 0, 255);
                        int G = (int)Clamp((color.G * (light.AmbientIntensity + D) + 255f * S), 0, 255);
                        int B = (int)Clamp((color.B * (light.AmbientIntensity + D) + 255f * S), 0, 255);

                        int step = (int)((255 - (color.R * light.AmbientIntensity)) / colorSubdivision);
                        R = Interpolation(color.R * light.AmbientIntensity, 0, 255, colorSubdivision, R) * step + (int)(color.R * light.AmbientIntensity);
                        step = (int)((255 - (color.G * light.AmbientIntensity)) / colorSubdivision);
                        G = Interpolation(color.G * light.AmbientIntensity, 0, 255, colorSubdivision, G) * step + (int)(color.G * light.AmbientIntensity);
                        step = (int)((255 - (color.B * light.AmbientIntensity)) / colorSubdivision);
                        B = Interpolation(color.B * light.AmbientIntensity, 0, 255, colorSubdivision, B) * step + (int)(color.B * light.AmbientIntensity);

                        Color newColor = Color.FromArgb(R, G, B);

                        if (pictureBox.Width / 2 + j < bm.Width && pictureBox.Height - pictureBox.Height / 2 + i < bm.Height &&
                            pictureBox.Width / 2 + j > 0 && pictureBox.Height - pictureBox.Height / 2 + i > 0)
                            g.DrawRectangle(new Pen(newColor), j, i, 1, 1);
                    }
                }
                x1 += _inc13;
                x2 += inc23;
            }

        }
    }
}
