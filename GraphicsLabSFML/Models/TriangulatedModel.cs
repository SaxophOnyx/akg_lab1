using System.Numerics;

namespace GraphicsLabSFML.Models
{
    public class TriangulatedModel
    {
        public Vector4[] Vertices { get; }

        public int[] FlatFaces { get; }


        public TriangulatedModel(IEnumerable<Vector4> vertices, IEnumerable<int> indicesPerFace)
        {
            Vertices = vertices.ToArray();
            FlatFaces = indicesPerFace.ToArray();
        }
    }
}
