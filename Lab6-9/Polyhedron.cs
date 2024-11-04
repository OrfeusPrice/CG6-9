using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    foreach (int i in f.VertexIndexes)
                    {
                        sum += temp.Vertexes[i];
                    }
                    obj.Vertexes.Add(new Point3D(a * sum.X / 3, a * sum.Y / 3, a * sum.Z / 3));
                }

                float k = a / (float)Math.Sqrt(Math.Pow(obj.Vertexes[0].X - obj.Vertexes[1].X, 2) + Math.Pow(obj.Vertexes[0].Y - obj.Vertexes[1].Y, 2) + Math.Pow(obj.Vertexes[0].Z - obj.Vertexes[1].Z, 2));
                for (int i = 0; i < obj.Vertexes.Count; i++)
                    obj.Vertexes[i] = obj.Vertexes[i] * k;

                obj.Faces.Add(new Face(0, 1, 2, 3, 4));
                obj.Faces.Add(new Face(5, 6, 7, 8, 9));

                obj.Faces.Add(new Face(0, 1, 12, 11, 10));
                obj.Faces.Add(new Face(1, 2, 14, 13, 12));
                obj.Faces.Add(new Face(2, 3, 16, 15, 14));
                obj.Faces.Add(new Face(3, 4, 18, 17, 16));
                obj.Faces.Add(new Face(4, 0, 10, 19, 18));

                obj.Faces.Add(new Face(5, 6, 13, 12, 11));
                obj.Faces.Add(new Face(6, 7, 15, 14, 13));
                obj.Faces.Add(new Face(7, 8, 17, 16, 15));
                obj.Faces.Add(new Face(8, 9, 19, 18, 17));
                obj.Faces.Add(new Face(9, 5, 11, 10, 19));

            }

            public static void Icosahedron(ref Object3D obj, float a)
            {
                float R = a / (2 * (float)Math.Sin(Math.PI / 6));
                float r = R * (float)Math.Cos(Math.PI / 6);

                float step = 360 / 5;

                obj = new Object3D();

                obj.Vertexes.Add(new Point3D(0, 0, R));
                for (float angle = 0; angle < 360; angle += step)
                {
                    obj.Vertexes.Add(new Point3D(R * (float)Math.Cos(angle / 180D * Math.PI), R * (float)Math.Sin(angle / 180D * Math.PI), a / 2));
                }

                obj.Vertexes.Add(new Point3D(0, 0, -R));
                for (float angle = step / 2; angle < 360; angle += step)
                {
                    obj.Vertexes.Add(new Point3D(R * (float)Math.Cos(angle / 180D * Math.PI), R * (float)Math.Sin(angle / 180D * Math.PI), -a / 2));
                }

                obj.Faces.Add(new Face(0, 1, 2));
                obj.Faces.Add(new Face(0, 2, 3));
                obj.Faces.Add(new Face(0, 3, 4));
                obj.Faces.Add(new Face(0, 4, 5));
                obj.Faces.Add(new Face(0, 5, 1));

                obj.Faces.Add(new Face(6, 7, 8));
                obj.Faces.Add(new Face(6, 8, 9));
                obj.Faces.Add(new Face(6, 9, 10));
                obj.Faces.Add(new Face(6, 10, 11));
                obj.Faces.Add(new Face(6, 11, 7));

                obj.Faces.Add(new Face(1, 2, 7));
                obj.Faces.Add(new Face(7, 8, 2));
                obj.Faces.Add(new Face(2, 3, 8));
                obj.Faces.Add(new Face(8, 9, 3));
                obj.Faces.Add(new Face(3, 4, 9));
                obj.Faces.Add(new Face(9, 10, 4));
                obj.Faces.Add(new Face(4, 5, 10));
                obj.Faces.Add(new Face(10, 11, 5));
                obj.Faces.Add(new Face(5, 1, 11));
                obj.Faces.Add(new Face(11, 7, 1));

            }

            public static void Hexahedron(ref Object3D obj, float a)
            {
                float R = a / (2 * (float)Math.Sin(Math.PI / 4));
                float r = R * (float)Math.Cos(Math.PI / 4);

                obj = new Object3D();

                obj.Vertexes.Add(new Point3D(r, r, -r));
                obj.Vertexes.Add(new Point3D(-r, r, -r));
                obj.Vertexes.Add(new Point3D(-r, -r, -r));
                obj.Vertexes.Add(new Point3D(r, -r, -r));
                obj.Vertexes.Add(new Point3D(r, r, r));
                obj.Vertexes.Add(new Point3D(-r, r, r));
                obj.Vertexes.Add(new Point3D(-r, -r, r));
                obj.Vertexes.Add(new Point3D(r, -r, r));

                obj.Faces.Add(new Face(0, 1, 2, 3));
                obj.Faces.Add(new Face(4, 5, 6, 7));
                obj.Faces.Add(new Face(0, 1, 5, 4));
                obj.Faces.Add(new Face(1, 2, 6, 5));
                obj.Faces.Add(new Face(2, 3, 7, 6));
                obj.Faces.Add(new Face(0, 3, 7, 4));
            }

            public static void Tetrahedron(ref Object3D obj, float a)
            {
                float R = a / (2 * (float)Math.Sin(Math.PI / 3));
                float r = R * (float)Math.Cos(Math.PI / 3);

                obj = new Object3D();

                obj.Vertexes.Add(new Point3D(0, 0, R));
                obj.Vertexes.Add(new Point3D(R, 0, -r));
                obj.Vertexes.Add(new Point3D(-r, a / 2, -r));
                obj.Vertexes.Add(new Point3D(-r, -a / 2, -r));

                obj.Faces.Add(new Face(0, 1, 2));
                obj.Faces.Add(new Face(0, 2, 3));
                obj.Faces.Add(new Face(0, 1, 3));
                obj.Faces.Add(new Face(1, 2, 3));
            }
        }
    }
