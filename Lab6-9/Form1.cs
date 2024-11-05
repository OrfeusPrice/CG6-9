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

namespace Lab6_9
{
    public partial class Form1 : Form
    {
        Bitmap _bm;
        Graphics _g;
        Camera _cam;

        Object3D _obj;

        bool _isPerspective = true;

        public Form1()
        {
            InitializeComponent();

            _bm = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Image = _bm;
            _g = Graphics.FromImage(pictureBox.Image);
            _g.Clear(Color.White);
            _g.TranslateTransform(pictureBox.ClientSize.Width / 2, pictureBox.ClientSize.Height / 2);
            _g.ScaleTransform(1, -1);

            _cam = new Camera();

            Tetrahedron(ref _obj, 100);

            _obj.Vertexes = _obj.Vertexes.Select(p => TranslatePoint(p, 0, 0, 0)).ToList();
            DrawObject(_obj);
        }

        public void DrawObject(Object3D _obj)
        {
            List<Point3D> vertexes = _obj.Vertexes;

            vertexes = vertexes.Select(p => View(p, _cam)).ToList();

            if (_isPerspective) vertexes = vertexes.Select(p => Perspective(p)).ToList();
            else vertexes = vertexes.Select(p => Axonometric(p)).ToList();

            foreach (Point3D p in vertexes)
            {
                _g.DrawRectangle(new Pen(Color.Red), p.X - 1, p.Y - 1, 2, 2);
            }

            foreach (Face face in _obj.Faces)
            {
                for (int i = 0; i < face.VertexIndexes.Count; i++)
                {
                    Point3D p1 = vertexes[face.VertexIndexes[i]];
                    Point3D p2 = vertexes[face.VertexIndexes[(i + 1) % face.VertexIndexes.Count]];
                    _g.DrawLine(new Pen(Color.Black),
                        p1.X,
                        p1.Y,
                        p2.X,
                        p2.Y);

                }
            }
        }

        public Point3D View(Point3D p, Camera cam)
        {
            float[][] ViewMatrix = new float[4][]
            {
                    new float[4] { cam.U.X,                                cam.V.X,                               cam.N.X,                              0 },
                    new float[4] { cam.U.Y,                                cam.V.Y,                               cam.N.Y,                              0 },
                    new float[4] { cam.U.Z,                                cam.V.Z,                               cam.N.Z,                              0 },
                    new float[4] { -(cam.U * cam.Location),   -(cam.V * cam.Location),  -(cam.N * cam.Location), 1 }
            };
            return MultiplyMatrix(ViewMatrix, p);
        }

        public Point3D Perspective(Point3D p)
        {
            float c = -pictureBox.Width * 0.8f;

            float[][] PerspectiveMatrix = new float[4][]
            {
                    new float[4] { 1, 0, 0, 0},
                    new float[4] { 0, 1, 0, 0 },
                    new float[4] { 0, 0, 0, -1/c },
                    new float[4] { 0, 0, 0, 1 }
            };
            _g.DrawLine(new Pen(Color.Blue, 2), 1000, 0, 0, 0); //X
            _g.DrawLine(new Pen(Color.Green, 2), 0, 1000, 0, 0); //Y
            Point3D temp = MultiplyMatrix(PerspectiveMatrix, p);
            return new Point3D(p.X / temp.W, p.Y / temp.W, 0, p.W);
        }

        public Point3D Axonometric(Point3D p)
        {
            float phi = (float)((35 / 180D) * Math.PI); ;
            float ksi = (float)((45 / 180D) * Math.PI); ;

            float[][] AxonometricMatrix = new float[4][]
            {
                    new float[4] { (float)Math.Cos(ksi), (float)(Math.Sin(phi) * Math.Sin(ksi)), 0, 0},
                    new float[4] { 0, (float)Math.Cos(phi), 0, 0 },
                    new float[4] { (float)Math.Sin(ksi), -(float)(Math.Sin(phi) * Math.Cos(ksi)), 0, 0 },
                    new float[4] { 0, 0, 0, 1 }
            };
            _g.DrawLine(new Pen(Color.Green, 2), 0, 0, 500, -300); //Y
            _g.DrawLine(new Pen(Color.Red, 2), 0, 1000, 0, 0); //Z
            _g.DrawLine(new Pen(Color.Blue, 2), 0, 0, -500, -300); //X
            return MultiplyMatrix(AxonometricMatrix, p);
        }



        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked) _isPerspective = true;
            if (radioButton2.Checked) _isPerspective = false;

            _g.Clear(Color.White);
            DrawObject(_obj);
            pictureBox.Refresh();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float dx, dy, dz;
            if (float.TryParse(textBox1.Text, out dx) && float.TryParse(textBox2.Text, out dy) && float.TryParse(textBox3.Text, out dz))
            {
                _obj.Vertexes = _obj.Vertexes.Select(p => TranslatePoint(p, dx, dy, dz)).ToList();

                _g.Clear(Color.White);
                DrawObject(_obj);
                pictureBox.Refresh();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            float angle;
            if (float.TryParse(textBox4.Text, out angle))
            {
                if (radioButton3.Checked) _obj.Vertexes = _obj.Vertexes.Select(p => XRotatePoint(p, angle)).ToList();
                if (radioButton4.Checked) _obj.Vertexes = _obj.Vertexes.Select(p => YRotatePoint(p, angle)).ToList();
                if (radioButton5.Checked) _obj.Vertexes = _obj.Vertexes.Select(p => ZRotatePoint(p, angle)).ToList();

                _g.Clear(Color.White);
                DrawObject(_obj);
                pictureBox.Refresh();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            float mx, my, mz;
            if (float.TryParse(textBox5.Text, out mx) && float.TryParse(textBox6.Text, out my) && float.TryParse(textBox7.Text, out mz))
            {
                _obj.Vertexes = _obj.Vertexes.Select(p => ScalePoint(p, mx, my, mz)).ToList();

                _g.Clear(Color.White);
                DrawObject(_obj);
                pictureBox.Refresh();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (radioButton6.Checked) _obj.Vertexes = _obj.Vertexes.Select(p => XYMirrorPoint(p)).ToList();
            if (radioButton7.Checked) _obj.Vertexes = _obj.Vertexes.Select(p => XZMirrorPoint(p)).ToList();
            if (radioButton8.Checked) _obj.Vertexes = _obj.Vertexes.Select(p => YZMirrorPoint(p)).ToList();

            _g.Clear(Color.White);
            DrawObject(_obj);
            pictureBox.Refresh();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            float mx, my, mz;
            if (float.TryParse(textBox8.Text, out mx) && float.TryParse(textBox9.Text, out my) && float.TryParse(textBox10.Text, out mz))
            {
                GeometryAndMatrix.Scale(ref _obj, mx, my, mz);

                _g.Clear(Color.White);
                DrawObject(_obj);
                pictureBox.Refresh();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            float angle;
            if (float.TryParse(textBox11.Text, out angle))
            {
                if (radioButton9.Checked) XRotate(ref _obj, angle);
                if (radioButton10.Checked) YRotate(ref _obj, angle);
                if (radioButton11.Checked) ZRotate(ref _obj, angle);

                _g.Clear(Color.White);
                DrawObject(_obj);
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
                Rotate(ref _obj, new Point3D(x1, y1, z1), new Point3D(x2, y2, z2), angle);

                _g.Clear(Color.White);
                DrawObject(_obj);
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

            _obj.Vertexes = _obj.Vertexes.Select(p => TranslatePoint(p, 0, 0, 0)).ToList();
            _g.Clear(Color.White);
            DrawObject(_obj);
            pictureBox.Refresh();
        }
    }
}

