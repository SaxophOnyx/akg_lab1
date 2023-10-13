using GraphicsLabSFML.Models;
using System.Globalization;
using System.Numerics;

namespace GraphicsLabSFML.Parsing
{
    public class ModelParser : IModelParser
    {
        public Model Parse(IEnumerable<string> source)
        {
            var faces = new List<Face>();

            var vertices = new List<Vector4>();
            var textureVectices = new List<Vector3>();
            var normalVertices = new List<Vector3>();

            foreach (var rawLine in source)
            {
                string line = rawLine.Trim();

                if (line != string.Empty)
                {
                    string[] items = line.Split(' ').Where(item => item != string.Empty).ToArray();
                    switch (items[0])
                    {
                        case "v":
                        {
                            float x = float.Parse(items[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(items[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(items[3], CultureInfo.InvariantCulture);
                            float w = items.Length > 4
                                ? float.Parse(items[4], CultureInfo.InvariantCulture)
                                : 1;

                            vertices.Add(new Vector4(x, y, z, w));
                            break;
                        }

                        case "vt":
                        {
                            float u = float.Parse(items[1], CultureInfo.InvariantCulture);
                            float v = items.Length > 2
                                ? float.Parse(items[2], CultureInfo.InvariantCulture)
                                : 0;
                            float w = items.Length > 3
                                ? float.Parse(items[3], CultureInfo.InvariantCulture)
                                : 0;

                            textureVectices.Add(new Vector3(u, v, w));
                            break;
                        }

                        case "vn":
                        {
                            float x = float.Parse(items[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(items[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(items[3], CultureInfo.InvariantCulture);

                            normalVertices.Add(new Vector3(x, y, z));
                            break;
                        }

                        case "f":
                        {
                            var face = new Face();

                            foreach (var item in items.Skip(1))
                            {
                                var verticesSet = item.Split('/');
                                var vertexIndex = int.Parse(verticesSet[0]) - 1;
                                face.Vertices.Add(vertices[vertexIndex]);
                            }

                            faces.Add(face);
                            break;
                        }

                        default:
                        continue;
                    }
                }
            }

            return new Model(faces);
        }
    }
}
