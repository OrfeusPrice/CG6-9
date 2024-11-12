using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lab6_9.Form1;
using static Lab6_9.GeometryAndMatrix;

namespace Lab6_9
{
    internal class Polyhedron
    {
        public static void Dodecahedron(ref Object3D obj, float a)
        {
            Object3D temp = new Object3D();
            Icosahedron(ref temp, 1);

            obj = new Object3D();

            foreach (Face f in temp.Faces)
            {
                Point3D sum = new Point3D(0, 0, 0);
                foreach (FaceIndices i in f.FaceIndices)
                {
                    sum += temp.Vertices[i.VertexIndex-1];
                }
                obj.Vertices.Add(new Point3D(a * sum.X / 3, a * sum.Y / 3, a * sum.Z / 3));
            }

            float k = a / (float)Math.Sqrt(Math.Pow(obj.Vertices[0].X - obj.Vertices[1].X, 2) + Math.Pow(obj.Vertices[0].Y - obj.Vertices[1].Y, 2) + Math.Pow(obj.Vertices[0].Z - obj.Vertices[1].Z, 2));
            for (int i = 0; i < obj.Vertices.Count; i++)
                obj.Vertices[i] = obj.Vertices[i] * k;

            obj.Faces.Add(new Face(1, 2, 3, 4, 5));
            obj.Faces.Add(new Face(6, 7, 8, 9, 10));

            obj.Faces.Add(new Face(1, 2, 13, 12, 11));
            obj.Faces.Add(new Face(2, 3, 15, 14, 13));
            obj.Faces.Add(new Face(3, 4, 17, 16, 15));
            obj.Faces.Add(new Face(4, 5, 19, 18, 17));
            obj.Faces.Add(new Face(5, 1, 11, 20, 19));

            obj.Faces.Add(new Face(6, 7, 14, 13, 12));
            obj.Faces.Add(new Face(7, 8, 16, 15, 14));
            obj.Faces.Add(new Face(8, 9, 18, 17, 16));
            obj.Faces.Add(new Face(9, 10, 20, 19, 18));
            obj.Faces.Add(new Face(10, 6, 12, 11, 20));

        }

        public static void Icosahedron(ref Object3D obj, float a)
        {
            float R = a / (2 * (float)Math.Sin(Math.PI / 6));
            float r = R * (float)Math.Cos(Math.PI / 6);

            float step = 360 / 5;

            obj = new Object3D();

            obj.Vertices.Add(new Point3D(0, 0, R));
            for (float angle = 0; angle < 360; angle += step)
            {
                obj.Vertices.Add(new Point3D(R * (float)Math.Cos(angle / 180D * Math.PI), R * (float)Math.Sin(angle / 180D * Math.PI), a / 2));
            }

            obj.Vertices.Add(new Point3D(0, 0, -R));
            for (float angle = step / 2; angle < 360; angle += step)
            {
                obj.Vertices.Add(new Point3D(R * (float)Math.Cos(angle / 180D * Math.PI), R * (float)Math.Sin(angle / 180D * Math.PI), -a / 2));
            }

            obj.Faces.Add(new Face(1, 2, 3));
            obj.Faces.Add(new Face(1, 3, 4));
            obj.Faces.Add(new Face(1, 4, 5));
            obj.Faces.Add(new Face(1, 5, 6));
            obj.Faces.Add(new Face(1, 6, 2));

            obj.Faces.Add(new Face(7, 8, 9));
            obj.Faces.Add(new Face(7, 9, 10));
            obj.Faces.Add(new Face(7, 10, 11));
            obj.Faces.Add(new Face(7, 11, 12));
            obj.Faces.Add(new Face(7, 12, 8));

            obj.Faces.Add(new Face(2, 3, 8));
            obj.Faces.Add(new Face(8, 9, 3));
            obj.Faces.Add(new Face(3, 4, 9));
            obj.Faces.Add(new Face(9, 10, 4));
            obj.Faces.Add(new Face(4, 5, 10));
            obj.Faces.Add(new Face(10, 11, 5));
            obj.Faces.Add(new Face(5, 6, 11));
            obj.Faces.Add(new Face(11, 12, 6));
            obj.Faces.Add(new Face(6, 2, 12));
            obj.Faces.Add(new Face(12, 8, 2));

        }

        public static void Hexahedron(ref Object3D obj, float a)
        {
            float R = a / (2 * (float)Math.Sin(Math.PI / 4));
            float r = R * (float)Math.Cos(Math.PI / 4);

            obj = new Object3D();

            obj.Vertices.Add(new Point3D(r, r, -r));
            obj.Vertices.Add(new Point3D(-r, r, -r));
            obj.Vertices.Add(new Point3D(-r, -r, -r));
            obj.Vertices.Add(new Point3D(r, -r, -r));
            obj.Vertices.Add(new Point3D(r, r, r));
            obj.Vertices.Add(new Point3D(-r, r, r));
            obj.Vertices.Add(new Point3D(-r, -r, r));
            obj.Vertices.Add(new Point3D(r, -r, r));

            obj.Faces.Add(new Face(1, 2, 3, 4));
            obj.Faces.Add(new Face(5, 6, 7, 8));
            obj.Faces.Add(new Face(1, 2, 6, 5));
            obj.Faces.Add(new Face(2, 3, 7, 6));
            obj.Faces.Add(new Face(3, 4, 8, 7));
            obj.Faces.Add(new Face(1, 4, 8, 5));
        }

        public static void Tetrahedron(ref Object3D obj, float a)
        {
            float R = a / (2 * (float)Math.Sin(Math.PI / 3));
            float r = R * (float)Math.Cos(Math.PI / 3);

            obj = new Object3D();

            obj.Vertices.Add(new Point3D(0, 0, R));
            obj.Vertices.Add(new Point3D(R, 0, -r));
            obj.Vertices.Add(new Point3D(-r, a / 2, -r));
            obj.Vertices.Add(new Point3D(-r, -a / 2, -r));

            obj.Faces.Add(new Face(1, 2, 3));
            obj.Faces.Add(new Face(1, 3, 4));
            obj.Faces.Add(new Face(1, 2, 4));
            obj.Faces.Add(new Face(2, 3, 4));
        }
        public static void Octahedron(ref Object3D obj, float a)
        {
            obj = new Object3D();

            obj.Vertices.Add(new Point3D(a, 0, 0));
            obj.Vertices.Add(new Point3D(-a, 0, 0));
            obj.Vertices.Add(new Point3D(0, a, 0));
            obj.Vertices.Add(new Point3D(0, -a, 0));
            obj.Vertices.Add(new Point3D(0, 0, a));
            obj.Vertices.Add(new Point3D(0, 0, -a));

            obj.Faces.Add(new Face(1, 3, 5));
            obj.Faces.Add(new Face(1, 6, 6));
            obj.Faces.Add(new Face(1, 4, 6));
            obj.Faces.Add(new Face(1, 6, 3));

            obj.Faces.Add(new Face(2, 5, 3));
            obj.Faces.Add(new Face(2, 4, 5));
            obj.Faces.Add(new Face(2, 6, 4));
            obj.Faces.Add(new Face(2, 3, 6));
        }

        public static void Graph(ref Object3D obj, Func<float, float, float> f, float minx, float maxx, float miny, float maxy, int splits)
        {
            obj = new Object3D();

            float stepx = (maxx - minx) / splits;
            float stepy = (maxy - miny) / splits;

            for (int y = 0; y < splits; y++)
            {
                for (int x = 0; x < splits; x++)
                {
                    float x1 = minx + x * stepx;
                    float y1 = miny + y * stepy;

                    obj.Vertices.Add(new Point3D(x1, y1, f(x1, y1)));

                    if (x != 0 && y != 0)
                    {
                        int cur = y * splits + x + 1;
                        obj.Faces.Add(new Face(cur, cur - splits, cur - splits - 1, cur - 1));
                    }
                }
            }
        }

        public static void RotationFigure(ref Object3D obj, List<Point3D> points, Axis axis, int splits)
        {
            obj = new Object3D();

            float step = 360 / splits;
            for (int i = 0; i <= splits; i++)
            {
                foreach (Point3D p in points)
                {
                    Point3D newP;
                    switch (axis)
                    {
                        case Axis.X: newP = XRotatePoint(p, i * step); break;
                        case Axis.Y: newP = YRotatePoint(p, i * step); break;
                        case Axis.Z: newP = ZRotatePoint(p, i * step); break;
                        default: newP = new Point3D(0, 0, 0); break;
                    }
                    obj.Vertices.Add(newP);
                }
                if (i != 0)
                {
                    for (int j = 0; j < points.Count; j++)
                    {
                        obj.Faces.Add(new Face(
                            i * points.Count + j + 1,
                            i * points.Count + (j + 1) % points.Count + 1,
                            (i - 1) * points.Count + (j + 1) % points.Count + 1,
                            (i - 1) * points.Count + j % points.Count + 1));
                    }
                }
            }
        }
    }
}
