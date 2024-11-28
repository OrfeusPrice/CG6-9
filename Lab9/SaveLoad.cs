using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab6_9
{
    internal class SaveLoad
    {
        public static void SaveObj(Object3D obj, string text)
        {
            if (text.Length != 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (Point3D v in obj.Vertices)
                {
                    sb.AppendLine($"v {v.X} {v.Y} {v.Z}");
                }

                foreach (Face f in obj.Faces)
                {
                    sb.Append("f");
                    foreach (FaceIndices fi in f.FaceIndices)
                        sb.Append($" {fi.VertexIndex}/{fi.TextureCoordinateIndex}/{fi.NormalIndex}");
                    sb.AppendLine("");
                }

                File.WriteAllText(text + ".obj", sb.ToString());
            }
        }

        public static Object3D LoadObj(string fname)
        {
            string[] file = File.ReadAllLines(fname);
            Object3D res = new Object3D();

            foreach (string s in file)
            {
                if (s == "" || s.Substring(0, 1) == "#" || s.Substring(0, 1) == "o" || (s.Length == 1 && s[0] != 'v' && s[0] != 'f')) continue;

                if (s.Substring(0, 2) == "vt") //Текстурные координаты
                {
                    float[] parsed = s.Substring(3, s.Length - 3).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToArray();
                    res.TextureCoordinates.Add(new Coordinates(parsed[0], parsed.Length < 2 ? 0 : parsed[1], parsed.Length < 3 ? 0 : parsed[2]));
                }
                else if (s.Substring(0, 2) == "vn") //Нормали
                {
                    float[] parsed = s.Substring(3, s.Length - 3).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToArray();
                    res.Normals.Add(new Point3D(parsed[0], parsed[1], parsed[2]));
                }
                else if (s.Substring(0, 1) == "v") //Список вершин
                {
                    float[] parsed = s.Substring(2, s.Length - 2).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToArray();
                    res.Vertices.Add(new Point3D(parsed[0], parsed[1], parsed[2], parsed.Length == 3 ? 1 : parsed[3]));
                }
                else if (s.Substring(0, 1) == "f") //Список поверхности сторон
                {
                    string[] parsed = s.Substring(2, s.Length - 2).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    Face f = new Face();
                    foreach (string v in parsed)
                    {
                        string[] vertex = v.Split('/');
                        FaceIndices faceIndices = new FaceIndices(int.Parse(vertex[0]));

                        if (vertex.Length > 1 && vertex[1] != "")
                        {
                            faceIndices.TextureCoordinateIndex = int.Parse(vertex[1]);
                            if (vertex.Length > 2)
                                faceIndices.NormalIndex = int.Parse(vertex[2]);
                        }

                        f.FaceIndices.Add(faceIndices);
                    }
                    res.Faces.Add(f);
                }

            }
            //Point3D center = new Point3D(0, 0, 0);
            //foreach (Point3D p in res.Vertices)
            //    center += p;
            //center /= res.Vertices.Count;

            //res.Vertices = res.Vertices.Select(p => p - center).ToList();

            return res;
        }
    }
}
