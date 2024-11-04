using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{
    internal class GeometryAndMatrix
    {
        public static void Scale(ref Object3D obj, float mx, float my, float mz)
        {
            Point3D center = new Point3D(0, 0, 0);
            foreach (Point3D p in obj.Vertexes)
                center += p;
            center /= obj.Vertexes.Count;

            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, -center.X, -center.Y, -center.Z)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => ScalePoint(p, mx, my, mz)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, center.X, center.Y, center.Z)).ToList();
        }

        public static void Rotate(ref Object3D obj, Point3D a, Point3D b, float angle)
        {
            b -= a;
            b /= (float)Math.Sqrt(Math.Pow(b.X, 2) + Math.Pow(b.Y, 2) + Math.Pow(b.Z, 2));
            float l = b.X;
            float m = b.Y;
            float n = b.Z;

            angle = (float)((angle / 180D) * Math.PI);

            float sin = (float)Math.Sin(angle);
            float cos = (float)Math.Cos(angle);

            float[][] RotateMatrix = new float[4][]
            {
                    new float[4] { l*l + cos*(1 - l*l), l*(1 - cos)*m + n * sin, l*(1 - cos)*n - m * sin, 0},
                    new float[4] { l*(1 - cos)*m - n * sin, m*m + cos*(1 - m*m), m*(1 - cos)*n + l*sin, 0 },
                    new float[4] { l*(1 - cos)*n + m * sin, m*(1 - cos)*n - l*sin, n*n + cos*(1 - n*n), 0 },
                    new float[4] { 0, 0, 0, 1 }
            };

            Point3D center = new Point3D(0, 0, 0);
            foreach (Point3D p in obj.Vertexes)
                center += p;
            center /= obj.Vertexes.Count;

            obj.Vertexes = obj.Vertexes.Select(p => MultiplyMatrix(RotateMatrix, p)).ToList();
        }

        public static void XRotate(ref Object3D obj, float angle)
        {
            Point3D center = new Point3D(0, 0, 0);
            foreach (Point3D p in obj.Vertexes)
                center += p;
            center /= obj.Vertexes.Count;

            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, -center.X, -center.Y, -center.Z)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => XRotatePoint(p, angle)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, center.X, center.Y, center.Z)).ToList();
        }

        public static void YRotate(ref Object3D obj, float angle)
        {
            Point3D center = new Point3D(0, 0, 0);
            foreach (Point3D p in obj.Vertexes)
                center += p;
            center /= obj.Vertexes.Count;

            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, -center.X, -center.Y, -center.Z)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => YRotatePoint(p, angle)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, center.X, center.Y, center.Z)).ToList();
        }

        public static void ZRotate(ref Object3D obj, float angle)
        {
            Point3D center = new Point3D(0, 0, 0);
            foreach (Point3D p in obj.Vertexes)
                center += p;
            center /= obj.Vertexes.Count;

            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, -center.X, -center.Y, -center.Z)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => ZRotatePoint(p, angle)).ToList();
            obj.Vertexes = obj.Vertexes.Select(p => TranslatePoint(p, center.X, center.Y, center.Z)).ToList();
        }

        public static Point3D XYMirrorPoint(Point3D p)
        {
            float[][] XYMirrorMatrix = new float[4][]
            {
                    new float[4] { 1, 0, 0,  0},
                    new float[4] { 0, 1, 0,  0 },
                    new float[4] { 0, 0, -1, 0 },
                    new float[4] { 0, 0, 0,  1 }
            };
            return MultiplyMatrix(XYMirrorMatrix, p);
        }

        public static Point3D XZMirrorPoint(Point3D p)
        {
            float[][] XZMirrorMatrix = new float[4][]
            {
                    new float[4] { 1, 0,  0, 0 },
                    new float[4] { 0, -1, 0, 0 },
                    new float[4] { 0, 0,  1, 0 },
                    new float[4] { 0, 0,  0, 1 }
            };
            return MultiplyMatrix(XZMirrorMatrix, p);
        }

        public static Point3D YZMirrorPoint(Point3D p)
        {
            float[][] YZMirrorMatrix = new float[4][]
            {
                    new float[4] { -1, 0, 0, 0 },
                    new float[4] { 0,  1, 0, 0 },
                    new float[4] { 0,  0, 1, 0 },
                    new float[4] { 0,  0, 0, 1 }
            };
            return MultiplyMatrix(YZMirrorMatrix, p);
        }

        public static Point3D XRotatePoint(Point3D p, float angle)
        {
            angle = (float)((angle / 180D) * Math.PI);
            float[][] XRotationMatrix = new float[4][]
            {
                    new float[4] { 1, 0,                        0,                      0 },
                    new float[4] { 0, (float)Math.Cos(angle),   (float)Math.Sin(angle), 0 },
                    new float[4] { 0, -(float)Math.Sin(angle),  (float)Math.Cos(angle), 0 },
                    new float[4] { 0, 0,                        0,                      1 }
            };

            return MultiplyMatrix(XRotationMatrix, p);
        }

        public static Point3D YRotatePoint(Point3D p, float angle)
        {
            angle = (float)((angle / 180D) * Math.PI);
            float[][] XRotationMatrix = new float[4][]
            {
                    new float[4] { (float)Math.Cos(angle),  0, -(float)Math.Sin(angle), 0 },
                    new float[4] { 0,                       1, 0,                       0 },
                    new float[4] { (float)Math.Sin(angle),  0,  (float)Math.Cos(angle), 0 },
                    new float[4] { 0,                       0, 0,                       1 }
            };

            return MultiplyMatrix(XRotationMatrix, p);
        }

        public static Point3D ZRotatePoint(Point3D p, float angle)
        {
            angle = (float)((angle / 180D) * Math.PI);
            float[][] XRotationMatrix = new float[4][]
            {
                    new float[4] { (float)Math.Cos(angle),  (float)Math.Sin(angle), 0, 0},
                    new float[4] { -(float)Math.Sin(angle), (float)Math.Cos(angle), 0, 0},
                    new float[4] { 0,                       0,                      1, 0},
                    new float[4] { 0,                       0,                      0, 1}
            };

            return MultiplyMatrix(XRotationMatrix, p);
        }

        public static Point3D ScalePoint(Point3D p, float mx, float my, float mz)
        {
            float[][] TranslationMatrix = new float[4][]
            {
                    new float[4] { mx, 0,  0,  0 },
                    new float[4] { 0,  my, 0,  0 },
                    new float[4] { 0,  0,  mz, 0 },
                    new float[4] { 0,  0,  0,  1 }
            };

            return MultiplyMatrix(TranslationMatrix, p);
        }

        public static Point3D TranslatePoint(Point3D p, float dx, float dy, float dz)
        {
            float[][] TranslationMatrix = new float[4][]
            {
                    new float[4] { 1,  0,  0,  0 },
                    new float[4] { 0,  1,  0,  0 },
                    new float[4] { 0,  0,  1,  0 },
                    new float[4] { dx, dy, dz, 1 }
            };

            return MultiplyMatrix(TranslationMatrix, p);
        }

        public static Point3D MultiplyMatrix(float[][] matrix, Point3D p)
        {
            float[] tempVector = new float[4] { p.X, p.Y, p.Z, p.W };
            float[] resultVector = new float[4];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                    resultVector[i] += matrix[j][i] * tempVector[j];
            }
            return new Point3D(resultVector[0], resultVector[1], resultVector[2], resultVector[3]);
        }
    }
}
