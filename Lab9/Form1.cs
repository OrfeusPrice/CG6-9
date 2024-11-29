using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Lab6_9.Polyhedron;
using static Lab6_9.GeometryAndMatrix;
using static Lab6_9.SaveLoad;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace Lab6_9
{
    public partial class Form1 : Form
    {
        public enum Axis
        {
            X,
            Y,
            Z
        }

        public enum Projection
        {
            Perspective,
            Axonometric,
            Parallel
        }

        Bitmap _bm;
        Graphics _g;

        Camera _camera;
        int angle;
        int move;

        float[,] _ZBuffer;

        List<Point3D> _points;
        Object3D _obj;
        List<Object3D> _objects = new List<Object3D>();
        int _countOfObjs;
        public static float[][] MChanges;

        public static bool _isNewObj = false;
        public static int _tCount;

        public Form1()
        {
            InitializeComponent();

            _bm = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = _bm;
            _g = Graphics.FromImage(pictureBox.Image);
            _g.Clear(Color.White);
            _g.TranslateTransform(pictureBox.ClientSize.Width / 2, pictureBox.ClientSize.Height / 2);
            _g.ScaleTransform(1, 1);

            _isNewObj = false;
            _tCount = 0;
            InitMChanges();

            angle = 5;
            move = 5;
            _camera = new Camera();
            _camera.Location = new Point3D(0, 0, 0);
            _camera.Rotation = new Point3D(0, 0, 0);

            _ZBuffer = new float[pictureBox.Width, pictureBox.Height];

            _countOfObjs = 0;

            //Hexahedron(ref _obj, 100);
            _obj = LoadObj("cube.obj");
            _points = new List<Point3D>();
            _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, 75, 0, 0)).ToList();
            GeometryAndMatrix.Scale(ref _obj, 30, 30, 30);
            _obj.color1 = Color.Red;
            _obj.color2 = Color.Blue;
            _obj.name = "obj" + _countOfObjs++.ToString();
            _objects.Add(_obj);
            Triangulate(ref _obj);
            OBJS_CB.Items.Add(_obj.name);

            _obj = LoadObj("cube.obj");
            _points = new List<Point3D>();
            _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, 0, 0, 75)).ToList();
            GeometryAndMatrix.Scale(ref _obj, 30, 30, 30);
            _obj.color1 = Color.Pink;
            _obj.color2 = Color.Violet;
            _obj.name = "obj" + _countOfObjs++.ToString();
            _objects.Add(_obj);
            Triangulate(ref _obj);
            OBJS_CB.Items.Add(_obj.name);

            _obj = LoadObj("sphere.obj");
            _points = new List<Point3D>();
            _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, -75, 0, 0)).ToList();
            GeometryAndMatrix.Scale(ref _obj, 30, 30, 30);
            _obj.color1 = Color.Yellow;
            _obj.color2 = Color.Green;
            _obj.name = "obj" + _countOfObjs++.ToString();
            _objects.Add(_obj);
            Triangulate(ref _obj);
            OBJS_CB.Items.Add(_obj.name);

            _obj = LoadObj("teapot.obj");
            _points = new List<Point3D>();
            _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, 0, 0, 0)).ToList();
            GeometryAndMatrix.Scale(ref _obj, 60, 60, 60);
            _obj.Vertices = _obj.Vertices.Select(p => XRotatePoint(p, 180)).ToList();
            _obj.color1 = Color.Coral;
            _obj.color2 = Color.LightGoldenrodYellow;
            _obj.name = "obj" + _countOfObjs++.ToString();
            _objects.Add(_obj);
            Triangulate(ref _obj);
            OBJS_CB.Items.Add(_obj.name);


            DrawObjects();
        }

        public void InitMChanges()
        {
            MChanges = new float[4][]
            {
                new float[4] { 1, 0, 0, 0 },
                new float[4] { 0, 1, 0, 0 },
                new float[4] { 0, 0, 1, 0 },
                new float[4] { 0, 0, 0, 1 },
            };
        }

        public Point3D View(Point3D p, Camera cam, Point3D center)
        {
            p = XRotatePoint(p, cam.Rotation.X);
            p = YRotatePoint(p, cam.Rotation.Y);
            p = ZRotatePoint(p, -cam.Rotation.Z);

            Point3D temp = cam.Location;
            temp = XRotatePoint(temp, -cam.Rotation.X);
            temp = YRotatePoint(temp, -cam.Rotation.Y);
            temp = ZRotatePoint(temp, -cam.Rotation.Z);

            p = TranslatePoint(p, -temp.X, -temp.Y, -temp.Z);
            return p;
        }

        public void DrawObjects()
        {
            for (int i = 0; i < pictureBox.Width; i++)
                for (int j = 0; j < pictureBox.Height; j++)
                    _ZBuffer[i, j] = float.MaxValue;

            foreach (Object3D obj in _objects)
            {
                DrawObject(obj);
            }
        }

        public void DrawObject(Object3D obj)
        {
            List<Point3D> vertexes = obj.Vertices;

            Point3D center = new Point3D(0, 0, 0);
            foreach (Point3D p in obj.Vertices)
                center += p;
            center /= obj.Vertices.Count;

            if (_isNewObj)
            {
                //vertexes = vertexes.Select(p => MultiplyMatrix(MChanges, p)).ToList();
                _isNewObj = false;
            }

            vertexes = vertexes.Select(p => View(p, _camera, center)).ToList();

            int len = 100;
            List<Point3D> Ox = new List<Point3D>() { new Point3D(0, 0, 0), new Point3D(-len, 0, 0) };
            List<Point3D> Oy = new List<Point3D>() { new Point3D(0, 0, 0), new Point3D(0, -len, 0) };
            List<Point3D> Oz = new List<Point3D>() { new Point3D(0, 0, 0), new Point3D(0, 0, len) };
            List<Color> colors = new List<Color>() { Color.Red, Color.Green, Color.Blue };
            var axeses = new List<List<Point3D>>() { Ox, Oy, Oz };
            for (int i = 0; i < axeses.Count; i++)
            {
                var axes = axeses[i];
                for (int j = 0; j < axes.Count; j++)
                {
                    axes[j] = View(axes[j], _camera, center);
                    switch (_camera.Projection)
                    {
                        case Projection.Perspective: axes[j] = Perspective(axes[j]); break;
                        case Projection.Axonometric: axes[j] = Axonometric(axes[j]); break;
                        case Projection.Parallel: axes[j] = Parallel(axes[j]); break;
                        default: break;
                    }
                }

                _g.DrawLine(
                                new Pen(colors[i], 1.5f),
                                -axes[0].X, axes[0].Y,
                                -axes[1].X, axes[1].Y
                                );
            }

            switch (_camera.Projection)
            {
                case Projection.Perspective: vertexes = vertexes.Select(p => Perspective(p)).ToList(); break;
                case Projection.Axonometric: vertexes = vertexes.Select(p => Axonometric(p)).ToList(); break;
                case Projection.Parallel: vertexes = vertexes.Select(p => Parallel(p)).ToList(); break;
                default: break;
            }

            foreach (Point3D p in vertexes)
            {
                //_g.DrawRectangle(new Pen(Color.Red), p.X - 1, p.Y - 1, 2, 2);
            }

            float maxZ = float.MinValue;
            float minZ = float.MaxValue;
            foreach (Point3D v in vertexes)
            {
                if (v.Z < minZ) minZ = v.Z;
                if (v.Z > maxZ) maxZ = v.Z;
            }


            foreach (Face face in obj.Faces)
            {
                Point3D v1 = vertexes[face.FaceIndices[1].VertexIndex - 1] - vertexes[face.FaceIndices[0].VertexIndex - 1];
                Point3D v2 = vertexes[face.FaceIndices[2].VertexIndex - 1] - vertexes[face.FaceIndices[0].VertexIndex - 1];

                Point3D normal = new Point3D(v1.Y * v2.Z - v1.Z * v2.Y, v1.Z * v2.X - v1.X * v2.Z, v1.X * v2.Y - v1.Y * v2.X);
                float l = (float)Math.Sqrt(Math.Pow(normal.X, 2) + Math.Pow(normal.Y, 2) + Math.Pow(normal.Z, 2));
                normal /= l;

                if (normal * _camera.ViewVector < 0) continue;

                List<Point3D> points = face.FaceIndices.Select(i => vertexes[i.VertexIndex - 1]).ToList();
                Rasterization(points, obj.color1, obj.color2, minZ, maxZ); //Закомментировать, чтобы увидеть отсечение нелицевых граней

                //Закомментировать, чтобы спрятать рёбра, тут начало
                for (int i = 0; i < face.FaceIndices.Count; i++)
                {
                    Point3D p1 = vertexes[face.FaceIndices[i].VertexIndex - 1];
                    Point3D p2 = vertexes[face.FaceIndices[(i + 1) % face.FaceIndices.Count].VertexIndex - 1];
                    _g.DrawLine(new Pen(Color.Black),
                        p1.X,
                        p1.Y,
                        p2.X,
                        p2.Y);
                }
                //Вот тут конец

            }
        }

        public void Triangulate(ref Object3D obj)
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

        int Interpolation(float x0, float y0, float x1, float y1, float x)
        {
            return (int)Math.Round(y0 + (float)(y1 - y0) * (x - x0) / (x1 - x0));
        }

        void Rasterization(List<Point3D> points, Color color1, Color color2, float minZ, float maxZ)
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
                    if (_ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        _ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;
                        //_g.DrawRectangle(new Pen(Color.FromArgb(R, G, B)), j, i, 1, 1);
                        _bm.SetPixel(pictureBox.Width / 2 + j, pictureBox.Height - pictureBox.Height / 2 + i, Color.FromArgb(R, G, B));
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
                    if (_ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] > z)
                    {
                        _ZBuffer[pictureBox.Width / 2 + j, pictureBox.Height / 2 + i] = z;
                        //_g.DrawRectangle(new Pen(Color.FromArgb(R, G, B)), j, i, 1, 1);
                        _bm.SetPixel(pictureBox.Width / 2 + j, pictureBox.Height - pictureBox.Height / 2 + i, Color.FromArgb(R, G, B));
                    }
                }
                x1 += _inc13;
                x2 += inc23;
            }

        }

        public Point3D Perspective(Point3D p)
        {
            float c = -pictureBox.Width;

            float[][] PerspectiveMatrix = new float[4][]
            {
                    new float[4] { 1, 0, 0, 0},
                    new float[4] { 0, 1, 0, 0 },
                    new float[4] { 0, 0, 0, -1/c },
                    new float[4] { 0, 0, 0, 1 }
            };

            Point3D temp = MultiplyMatrix(PerspectiveMatrix, p);
            return new Point3D(p.X / temp.W, p.Y / temp.W, p.Z, p.W);
        }

        public Point3D Axonometric(Point3D p)
        {
            float phi = (float)((35.26 / 180D) * Math.PI); ;
            float psi = (float)((45 / 180D) * Math.PI); ;

            float[][] AxonometricMatrix = new float[4][]
            {
                    new float[4] { (float)Math.Cos(psi), (float)(Math.Sin(phi) * Math.Sin(psi)),  0, 0},
                    new float[4] { 0,                    (float)Math.Cos(phi),                    0, 0 },
                    new float[4] { (float)Math.Sin(psi), -(float)(Math.Sin(phi) * Math.Cos(psi)), 1, 0 },
                    new float[4] { 0,                    0,                                       0, 1 }
            };

            return MultiplyMatrix(AxonometricMatrix, p);
        }

        public Point3D Parallel(Point3D p)
        {
            float c = -pictureBox.Width;

            float[][] PerspectiveMatrix = new float[4][]
            {
                    new float[4] { 1, 0, 0, 0},
                    new float[4] { 0, 1, 0, 0 },
                    new float[4] { 0, 0, 0, 0 },
                    new float[4] { 0, 0, 0, 1 }
            };

            Point3D temp = MultiplyMatrix(PerspectiveMatrix, p);
            return new Point3D(p.X / temp.W, p.Y / temp.W, p.Z, p.W);
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) _camera.Projection = Projection.Perspective;
            if (radioButton2.Checked) _camera.Projection = Projection.Axonometric;
            if (Parallel_RB.Checked) _camera.Projection = Projection.Parallel;

            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float dx, dy, dz;
            if (float.TryParse(textBox1.Text, out dx) && float.TryParse(textBox2.Text, out dy) && float.TryParse(textBox3.Text, out dz))
            {
                _tCount = 0;
                _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, dx, dy, dz)).ToList();

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float angle;
            if (float.TryParse(textBox4.Text, out angle))
            {
                _tCount = 0;
                if (radioButton3.Checked) _obj.Vertices = _obj.Vertices.Select(p => XRotatePoint(p, angle)).ToList();
                if (radioButton4.Checked) _obj.Vertices = _obj.Vertices.Select(p => YRotatePoint(p, angle)).ToList();
                if (radioButton5.Checked) _obj.Vertices = _obj.Vertices.Select(p => ZRotatePoint(p, angle)).ToList();

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            float mx, my, mz;
            if (float.TryParse(textBox5.Text, out mx) && float.TryParse(textBox6.Text, out my) && float.TryParse(textBox7.Text, out mz))
            {
                _tCount = 0;
                _obj.Vertices = _obj.Vertices.Select(p => ScalePoint(p, mx, my, mz)).ToList();

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            _tCount = 0;
            if (radioButton6.Checked) _obj.Vertices = _obj.Vertices.Select(p => XYMirrorPoint(p)).ToList();
            if (radioButton7.Checked) _obj.Vertices = _obj.Vertices.Select(p => XZMirrorPoint(p)).ToList();
            if (radioButton8.Checked) _obj.Vertices = _obj.Vertices.Select(p => YZMirrorPoint(p)).ToList();

            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            float mx, my, mz;
            if (float.TryParse(textBox8.Text, out mx) && float.TryParse(textBox9.Text, out my) && float.TryParse(textBox10.Text, out mz))
            {
                _tCount = 0;
                GeometryAndMatrix.Scale(ref _obj, mx, my, mz);

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            float angle;
            if (float.TryParse(textBox11.Text, out angle))
            {
                _tCount = 0;
                if (radioButton9.Checked) XRotate(ref _obj, angle);
                if (radioButton10.Checked) YRotate(ref _obj, angle);
                if (radioButton11.Checked) ZRotate(ref _obj, angle);

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            float x1, y1, z1, x2, y2, z2, angle;
            if (float.TryParse(textBox12.Text, out x1) && float.TryParse(textBox13.Text, out y1) && float.TryParse(textBox14.Text, out z1) &&
                float.TryParse(textBox15.Text, out x2) && float.TryParse(textBox16.Text, out y2) && float.TryParse(textBox17.Text, out z2) &&
                float.TryParse(textBox18.Text, out angle))
            {
                _tCount = 0;
                Rotate(ref _obj, new Point3D(x1, y1, z1), new Point3D(x2, y2, z2), angle);

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }
        }

        private void B_Create_Click(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Tetrahedron(ref _obj, 100);
                    break;
                case 1:
                    Hexahedron(ref _obj, 100);
                    break;
                case 2:
                    Octahedron(ref _obj, 100);
                    break;
                case 3:
                    Icosahedron(ref _obj, 100);
                    break;
                case 4:
                    Dodecahedron(ref _obj, 100);
                    break;
                default:
                    break;
            }
            InitMChanges();

            _isNewObj = true;
            _countOfObjs++;
            _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, 0, 0, 0)).ToList();
            _obj.name = "obj" + _countOfObjs.ToString();
            _objects.Add(_obj);
            OBJS_CB.Items.Add(_obj.name);
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void BSave_Click(object sender, EventArgs e)
        {
            SaveObj(_obj, SaveTB.Text);
        }

        private void BLoad_Click(object sender, EventArgs e)
        {
            _obj = new Object3D();
            _obj = LoadObj(LoadTB.Text + ".obj");
            _obj.name = LoadTB.Text;
            _obj.color2 = Color.Red;
            _obj.color1 = Color.Blue;
            Triangulate(ref _obj);
            _objects.Add(_obj);
            OBJS_CB.Items.Add(_obj.name);

            InitMChanges();
            _isNewObj = true;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void Create_FigRotB_Click(object sender, EventArgs e)
        {
            Axis selectedAxis = Axis.Y;
            if (X_FigRotRB.Checked) selectedAxis = Axis.X;
            if (Y_FigRotRB.Checked) selectedAxis = Axis.Y;
            if (Z_FigRotRB.Checked) selectedAxis = Axis.Z;
            RotationFigure(ref _obj, _points, selectedAxis, int.Parse(Fragmentation_FigRotTB.Text));

            InitMChanges();
            _isNewObj = true;
            _countOfObjs++;
            _obj.Vertices = _obj.Vertices.Select(p => TranslatePoint(p, 0, 0, 0)).ToList();
            _obj.name = "obj" + _countOfObjs.ToString();
            _objects.Add(_obj);
            OBJS_CB.Items.Add(_obj.name);
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseButtons.Right == e.Button)
            {
                _points.Clear();
                _g.Clear(Color.White);
                _obj = new Object3D();
                if (_camera.Projection == Projection.Perspective)
                {
                    _g.DrawLine(new Pen(Color.Blue, 2), 1000, 0, 0, 0); //X
                    _g.DrawLine(new Pen(Color.Red, 2), 0, 1000, 0, 0); //Y
                }
                else
                {
                    _g.DrawLine(new Pen(Color.Green, 2), 0, 0, 500, -300); // Z
                    _g.DrawLine(new Pen(Color.Red, 2), 0, 1000, 0, 0); // Y
                    _g.DrawLine(new Pen(Color.Blue, 2), 0, 0, 500, 300); //X
                }
                pictureBox.Refresh();
            }
            else
            {
                _points.Add(new Point3D(e.X - pictureBox.Width / 2, -e.Y + pictureBox.Height / 2, 0));
                _g.Clear(Color.White);
                foreach (Point3D p in _points)
                {
                    _g.FillEllipse(Brushes.Red, p.X - 2, p.Y - 2, 4, 4);
                }
                if (_camera.Projection == Projection.Perspective)
                {
                    _g.DrawLine(new Pen(Color.Blue, 2), 1000, 0, 0, 0); //X
                    _g.DrawLine(new Pen(Color.Red, 2), 0, 1000, 0, 0); //Y
                }
                else
                {
                    _g.DrawLine(new Pen(Color.Green, 2), 0, 0, 500, -300); // Z
                    _g.DrawLine(new Pen(Color.Red, 2), 0, 1000, 0, 0); // Y
                    _g.DrawLine(new Pen(Color.Blue, 2), 0, 0, 500, 300); //X
                }
                pictureBox.Refresh();
            }
        }

        private void FuncCreate_Click(object sender, EventArgs e)
        {
            Func<float, float, float> f = (x, y) => { return (x * x + y * y); };
            int minx = int.Parse(Funcx0.Text);
            int miny = int.Parse(Funcy0.Text);
            int maxx = int.Parse(Funcx1.Text);
            int maxy = int.Parse(Funcy1.Text);
            int splits = int.Parse(FuncStep.Text);

            switch (FuncComboBox.SelectedIndex)
            {
                case 0:
                    f = (x, y) => { return (x * x + y * y); };
                    break;
                case 1:
                    //f = (x, y) => { float r = x * x + y * y; return (float)(100 - (3 / Math.Sqrt(r)) + Math.Sin(Math.Sqrt(r)) + Math.Sqrt​(200 - r + (10 * Math.Sin​(x)) + 10 * Math.Sin​(y)) / 1000); };
                    f = (x, y) => { float r = x * x * x + y * y * y; return r; };
                    break;
                case 2:
                    f = f = (x, y) => { return (float)Math.Sin(x) * (float)Math.Cos(y); };
                    break;
            }

            label28.Text = FuncComboBox.SelectedIndex.ToString();
            Graph(ref _obj, f, minx, maxx, miny, maxy, splits);

            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void OBJS_CB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_objects.Count > 0)
                foreach (var obj in _objects)
                {
                    if (OBJS_CB.SelectedItem.ToString() == obj.ToString())
                    {
                        _obj = obj;
                        break;
                    }
                }
        }

        private void DELOBJ_B_Click(object sender, EventArgs e)
        {
            if (_objects.Count > 0)
            {
                Object3D temp = _objects[0];
                foreach (var obj in _objects)
                {
                    if (OBJS_CB.SelectedItem.ToString() == obj.ToString())
                    {
                        temp = obj;
                        break;
                    }
                }
                OBJS_CB.Items.Remove(temp.ToString());
                _objects.Remove(temp);

                _g.Clear(Color.White);
                DrawObjects();
                pictureBox.Refresh();
            }
        }

        private void CLEAROBJS_B_Click(object sender, EventArgs e)
        {
            foreach (var obj in _objects)
            {
                OBJS_CB.Items.Remove(obj.ToString());
            }

            _objects.Clear();

            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }
        private void CameraRotateY_B_Click(object sender, EventArgs e)
        {
            _camera.Rotation.Y += angle;
            //_camera.Location.Z = (float)Math.Cos((_camera.Rotation.Y / 180D) * Math.PI) * 100;
            //_camera.Location.X = (float)Math.Sin((_camera.Rotation.Y / 180D) * Math.PI) * 100;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }
        private void CamRotateX_B_Click(object sender, EventArgs e)
        {
            _camera.Rotation.X += angle;
            //_camera.Location.Y = (float)Math.Sin((_camera.Rotation.X / 180D) * Math.PI) * 100;
            //_camera.Location.Z = (float)Math.Cos((_camera.Rotation.X / 180D) * Math.PI) * 100;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void CamRotateZ_B_Click(object sender, EventArgs e)
        {
            _camera.Rotation.Z += angle;
            //_camera.Location.Y = (float)Math.Cos((_camera.Rotation.Z / 180D) * Math.PI) * 100;
            //_camera.Location.X = (float)Math.Sin((_camera.Rotation.Z / 180D) * Math.PI) * 100;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void CamRotateNY_B_Click(object sender, EventArgs e)
        {
            _camera.Rotation.Y -= angle;
            //_camera.Location.Z = (float)Math.Cos((_camera.Rotation.Y / 180D) * Math.PI) * 100;
            //_camera.Location.X = (float)Math.Sin((_camera.Rotation.Y / 180D) * Math.PI) * 100;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void CamRotateNX_B_Click(object sender, EventArgs e)
        {
            _camera.Rotation.X -= angle;
            //_camera.Location.Y = (float)Math.Sin((_camera.Rotation.X / 180D) * Math.PI) * 100;
            //_camera.Location.Z = (float)Math.Cos((_camera.Rotation.X / 180D) * Math.PI) * 100;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void CamRotateNZ_B_Click(object sender, EventArgs e)
        {
            _camera.Rotation.Z -= angle;
            //_camera.Location.Y = (float)Math.Cos((_camera.Rotation.Z / 180D) * Math.PI) * 100;
            //_camera.Location.X = (float)Math.Sin((_camera.Rotation.Z / 180D) * Math.PI) * 100;
            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }

        private void SetCamStart_B_Click(object sender, EventArgs e)
        {
            _camera = new Camera();
            _camera.Location = new Point3D(0, 0, 0);
            _camera.Rotation = new Point3D(0, 0, 0);

            _g.Clear(Color.White);
            DrawObjects();
            pictureBox.Refresh();
        }
    }
}

