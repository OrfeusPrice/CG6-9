using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lab6_9.Form1;
using static Lab6_9.GeometryAndMatrix;
using System.Drawing;

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

    public class LightSource
    {
        public Point3D Position { get; set; }
        public Color Color { get; set; }

        public Point3D Direction { get; set; }
        public float Intensity { get; set; }

        public LightSource(Point3D position, Color color, float intensity)
        {
            Position = position;
            Color = color;
            Direction = new Point3D(0, 0, -1);
            Intensity = intensity;
        }

        public Color CalculateLighting(Point3D normal, Point3D vertex)
        {
            // Направление на источник света
            Point3D lightDir = Position - vertex;
            lightDir = lightDir / (float)Math.Sqrt(lightDir.X * lightDir.X + lightDir.Y * lightDir.Y + lightDir.Z * lightDir.Z);

            // Интенсивность диффузного освещения
            float diff = Math.Max(0, normal * lightDir);

            // Цвет объекта с учетом освещенности
            float r = diff * Color.R / 255.0f;
            float g = diff * Color.G / 255.0f;
            float b = diff * Color.B / 255.0f;

            return Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255));
        }
    }




    public class Face
    {
        public List<FaceIndices> FaceIndices;

        public Face()
        {
            FaceIndices = new List<FaceIndices>();
        }

        public Face(params int[] indexes)
        {
            FaceIndices = new List<FaceIndices>();
            foreach (int i in indexes)
                FaceIndices.Add(new FaceIndices(i));
        }
    }

    public class FaceIndices
    {
        public int VertexIndex { get; set; }
        public int TextureCoordinateIndex { get; set; }
        public int NormalIndex { get; set; }

        public FaceIndices(int v)
        {
            VertexIndex = v;
            TextureCoordinateIndex = v;
            NormalIndex = v;
        }

        public FaceIndices(int v, int vt, int vn)
        {
            VertexIndex = v;
            TextureCoordinateIndex = vt;
            NormalIndex = vn;
        }
    }

    public class Coordinates
    {
        public float U { get; set; }
        public float V { get; set; }
        public float W { get; set; }

        public Coordinates(float u, float v = 0, float w = 0)
        {
            U = u;
            V = v;
            W = w;
        }
    }

    public class Object3D
    {
        public List<Point3D> Vertices;
        public List<Face> Faces;
        public List<Point3D> Normals;
        public List<Coordinates> TextureCoordinates;
        public List<Coordinates> ParameterSpaceVertices;
        public Color color1;
        public Color color2;
        public string name;

        public Object3D()
        {
            Vertices = new List<Point3D>();
            Faces = new List<Face>();
            Normals = new List<Point3D>();
            TextureCoordinates = new List<Coordinates>();
            ParameterSpaceVertices = new List<Coordinates>();
            color1 = Color.FromArgb(35, 35, 35);
            color2 = Color.FromArgb(220, 220, 220);
        }

        public override string ToString()
        {
            return name;
        }
    }

    public class Camera
    {
        public Point3D Location { get; set; }
        public Point3D Rotation
        {
            get { return _rotation; }
            set
            {
                Point3D temp = new Point3D(0, 0, -1);
                temp = XRotatePoint(temp, value.X);
                temp = YRotatePoint(temp, value.Y);
                temp = ZRotatePoint(temp, value.Z);

                ViewVector = temp;
                _rotation = value;
            }
        }

        private Point3D _rotation;
        public Point3D ViewVector { get; set; }
        public Projection Projection { get; set; }

        public Camera()
        {
            Location = new Point3D(0, 0, 0);
            Rotation = new Point3D(0, 0, 0);
            ViewVector = new Point3D(0, 0, -1);
            Projection = Projection.Perspective;
        }

        public Point3D View(Point3D p, Point3D center)
        {
            p = XRotatePoint(p, Rotation.X);
            p = YRotatePoint(p, Rotation.Y);
            p = ZRotatePoint(p, -Rotation.Z);

            Point3D temp = Location;
            temp = XRotatePoint(temp, -Rotation.X);
            temp = YRotatePoint(temp, -Rotation.Y);
            temp = ZRotatePoint(temp, -Rotation.Z);

            p = TranslatePoint(p, -temp.X, -temp.Y, -temp.Z);
            return p;
        }
    }
}

