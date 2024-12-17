using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab6_9
{
    internal class Texturing
    {
        public Texturing()
        {

        }

        public void Textur(ref Object3D obj)
        {
            string filename = "cat.jpg";
            Bitmap texture = new Bitmap(filename);

            List<Point3D> vertexes = obj.Vertices;

            List<Coordinates> textureCoords = new List<Coordinates>();

            foreach (Face f in obj.Faces)
            {
                List<Point3D> points = new List<Point3D>();

                foreach (FaceIndices fi in f.FaceIndices)
                {
                    points.Add(vertexes[fi.VertexIndex - 1]);
                    textureCoords.Add(obj.TextureCoordinates[fi.TextureCoordinateIndex - 1]);
                }

                //Rasterization_Linear_Texture(points, textureCoords, texture);
            }

            
        }

        public static int Interpolation(float x0, float y0, float x1, float y1, float x)
        {
            return (int)Math.Round(y0 + (float)(y1 - y0) * (x - x0) / (x1 - x0));
        }

        public static float Interpolation1(float x0, float y0, float x1, float y1, float x)
        {
            return y0 + (float)(y1 - y0) * (x - x0) / (x1 - x0);
        }

        public static void Rasterization_Linear_Texture(List<Point3D> points, List<Coordinates> textureCoords, Bitmap texture, Bitmap bm, PictureBox pictureBox, float[,] ZBuffer)
        {
            points = points.Select(p => new Point3D((float)Math.Round(p.X), (float)Math.Round(p.Y), p.Z, p.W)).ToList();

            List<(Point3D, Coordinates)> temp = (new List<int> { 0, 1, 2 }).Select(i => (points[i], textureCoords[i])).ToList();

            temp.Sort((a, b) => a.Item1.Y == b.Item1.Y ? 0 : (a.Item1.Y < b.Item1.Y ? -1 : 1));

            points = temp.Select(x => x.Item1).ToList();
            textureCoords = temp.Select(x => x.Item2).ToList();

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
                float tLeftU = Interpolation1(points[0].Y, textureCoords[0].U, points[left].Y, textureCoords[left].U, i);
                float tLeftV = Interpolation1(points[0].Y, textureCoords[0].V, points[left].Y, textureCoords[left].V, i);

                float tRightU = Interpolation1(points[0].Y, textureCoords[0].U, points[right].Y, textureCoords[right].U, i);
                float tRightV = Interpolation1(points[0].Y, textureCoords[0].V, points[right].Y, textureCoords[right].V, i);

                int zLeft = Interpolation(points[0].Y, points[0].Z, points[left].Y, points[left].Z, i);
                int zRight = Interpolation(points[0].Y, points[0].Z, points[right].Y, points[right].Z, i);

                for (int j = (int)x1; j < (int)x2; j++)
                {
                    float U = Interpolation1((int)x1, tLeftU, (int)x2, tRightU, j);
                    float V = Interpolation1((int)x1, tLeftV, (int)x2, tRightV, j);

                    int z = Interpolation((int)x1, zLeft, (int)x2, zRight, j);
                    if (pictureBox.Width / 2 + j > pictureBox.Width - 1 || pictureBox.Width / 2 + j < 0 || pictureBox.Height / 2 + i > pictureBox.Height - 1 || pictureBox.Height / 2 + i < 0) continue;
                    if (ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;
                        if (pictureBox.Width / 2 + j < bm.Width && pictureBox.Height - pictureBox.Height / 2 + i < bm.Height &&
                            pictureBox.Width / 2 + j > 0 && pictureBox.Height - pictureBox.Height / 2 + i > 0)
                            bm.SetPixel(pictureBox.Width / 2 + j, pictureBox.Height - pictureBox.Height / 2 + i, (texture.GetPixel((int)((texture.Width) * U % texture.Width), texture.Height - (int)((texture.Height) * V))));
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
                float tLeftU = Interpolation1(points[2].Y, textureCoords[2].U, points[left].Y, textureCoords[left].U, i);
                float tLeftV = Interpolation1(points[2].Y, textureCoords[2].V, points[left].Y, textureCoords[left].V, i);

                float tRightU = Interpolation1(points[2].Y, textureCoords[2].U, points[right].Y, textureCoords[right].U, i);
                float tRightV = Interpolation1(points[2].Y, textureCoords[2].V, points[right].Y, textureCoords[right].V, i);

                int zLeft = Interpolation(points[2].Y, points[2].Z, points[left].Y, points[left].Z, i);
                int zRight = Interpolation(points[2].Y, points[2].Z, points[right].Y, points[right].Z, i);

                for (int j = (int)x1; j < (int)x2; j++)
                {
                    float U = Interpolation1((int)x1, tLeftU, (int)x2, tRightU, j);
                    float V = Interpolation1((int)x1, tLeftV, (int)x2, tRightV, j);

                    int z = Interpolation((int)x1, zLeft, (int)x2, zRight, j);
                    if (pictureBox.Width / 2 + j > pictureBox.Width - 1 || pictureBox.Width / 2 + j < 0 || pictureBox.Height / 2 + i > pictureBox.Height - 1 || pictureBox.Height / 2 + i < 0) continue;
                    if (ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;
                        if (pictureBox.Width / 2 + j < bm.Width && pictureBox.Height - pictureBox.Height / 2 + i < bm.Height &&
                            pictureBox.Width / 2 + j > 0 && pictureBox.Height - pictureBox.Height / 2 + i > 0)
                            bm.SetPixel(pictureBox.Width / 2 + j, pictureBox.Height - pictureBox.Height / 2 + i, (texture.GetPixel((int)((texture.Width) * U % texture.Width), texture.Height - (int)((texture.Height) * V))));
                    }
                }
                x1 += _inc13;
                x2 += inc23;
            }
        }
    }
}
