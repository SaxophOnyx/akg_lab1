using GraphicsLabSFML.Models;
using System.Globalization;
using System.Numerics;

namespace GraphicsLabSFML.Parsing
{
    public class MeshParser
    {
        public static Mesh ParseFile(string path)
        {
            string[] source = File.ReadAllLines(path);
            return Parse(source);
        }

        public static Mesh Parse(string[] source)
        {
            List<Vector4> vertices = new();
            List<Vector3> normals = new();
            List<Vector3> textures = new();

            List<int> vIndices = new();
            List<int> nIndices = new();
            List<int> tIndices = new();

            int numberOfFaces = 0;

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

                            textures.Add(new Vector3(u, v, w));
                            break;
                        }

                        case "vn":
                        {
                            float x = float.Parse(items[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(items[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(items[3], CultureInfo.InvariantCulture);

                            normals.Add(new Vector3(x, y, z));
                            break;
                        }

                        case "f":
                        {
                            numberOfFaces++;

                            List<int> _vertices = new();
                            List<int> _textures = new();
                            List<int> _normals = new();

                            foreach (var item in items.Skip(1))
                            {
                                string[] verticesSet = item.Split('/');

                                int vertexIndex = int.Parse(verticesSet[0]) - 1;
                                vIndices.Add(vertexIndex);

                                int textureIndex = int.Parse(verticesSet[1]) - 1;
                                tIndices.Add(textureIndex);

                                int normalIndex = int.Parse(verticesSet[2]) - 1;
                                nIndices.Add(normalIndex);
                            }

                            break;
                        }

                        default:
                        {
                            continue;
                        }
                    }
                }
            }

            vIndices = TriangulateIndices(vIndices, numberOfFaces);
            nIndices = TriangulateIndices(nIndices, numberOfFaces);
            tIndices = TriangulateIndices(tIndices, numberOfFaces);

            return new Mesh()
            {
                Vertices = vertices.ToArray(),
                VertexIndices = vIndices.ToArray(),
                Normals = normals.ToArray(),
                NormalIndices = nIndices.ToArray(),
                Textures = textures.ToArray(),
                TextureIndices = tIndices.ToArray()
            };
        }

        public static List<int> TriangulateIndices(List<int> indices, int numberOfFaces)
        {
            int indicesPerFace = indices.Count / numberOfFaces;
            int triangulatedLength = numberOfFaces * 3 * (indicesPerFace - 2);

            List<int> result = new(triangulatedLength);

            for (int j = 0; j < numberOfFaces; ++j)
            {
                int offset = j * indicesPerFace;

                for (int k = 1; k < indicesPerFace - 1; ++k)
                {
                    result.Add(indices[offset]);
                    result.Add(indices[offset + k]);
                    result.Add(indices[offset + k + 1]);
                }
            }

            return result;
        }
    }
}
