using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6_9
{

    public class Point3D
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public float W { get; set; }

        public Point3D(float x, float y, float z, float w = 1)
        {
            X = x; Y = y; Z = z;
            W = w;
        }

        public static Point3D operator +(Point3D a, Point3D b) => new Point3D(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Point3D operator -(Point3D a, Point3D b) => new Point3D(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Point3D operator /(Point3D a, float b) => new Point3D(a.X / b, a.Y / b, a.Z / b);
        public static Point3D operator *(Point3D a, float b) => new Point3D(a.X * b, a.Y * b, a.Z * b);
        public static float operator *(Point3D a, Point3D b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    public class Face
    {
        public List<int> VertexIndexes;

        public Face(params int[] indexes)
        {
            VertexIndexes = new List<int>();
            foreach (int i in indexes)
                VertexIndexes.Add(i);
        }
    }

    public class Object3D
    {
        public List<Point3D> Vertexes;
        public List<Face> Faces;

        public Object3D()
        {
            Vertexes = new List<Point3D>();
            Faces = new List<Face>();
        }
    }

    public class Camera
    {
        public Point3D U { get; set; }
        public Point3D V { get; set; }
        public Point3D N { get; set; }
        public Point3D Location { get; set; }

        public Camera()
        {
            U = new Point3D(1, 0, 0);
            V = new Point3D(0, 1, 0);
            N = new Point3D(0, 0, 1);
            Location = new Point3D(0, 0, 0);
        }
    }
}
