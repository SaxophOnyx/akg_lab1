using GraphicsLabSFML.Models;
using System.Globalization;
using System.Numerics;

namespace GraphicsLabSFML.Parsing
{
    public class ModelParser : IModelParser
    {
        public Model Parse(IEnumerable<string> source)
        {
            ModelBuilder builder = new();
            ParseToBuilder(source, builder);
            return builder.Build();
        }

        public TriangulatedModel ParseTriangulated(IEnumerable<string> source)
        {
            ModelBuilder builder = new();
            ParseToBuilder(source, builder);
            return builder.BuildTriangulated();
        }

        private static void ParseToBuilder(IEnumerable<string> source, ModelBuilder builder)
        {
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

                            builder.AddVertex(new Vector4(x, y, z, w));
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

                            // TODO(SaxophOnyx): Add texture vertex to builder
                            break;
                        }

                        case "vn":
                        {
                            float x = float.Parse(items[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(items[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(items[3], CultureInfo.InvariantCulture);

                            // TODO(SaxophOnyx): Add mormal vertex to builder
                            break;
                        }

                        case "f":
                        {
                            List<int> indices = new();

                            foreach (var item in items.Skip(1))
                            {
                                var verticesSet = item.Split('/');
                                var vertexIndex = int.Parse(verticesSet[0]) - 1;
                                indices.Add(vertexIndex);
                            }

                            builder.AddFace(new(indices));

                            break;
                        }

                        default:
                            continue;
                    }
                }
            }
        }
    }
}
