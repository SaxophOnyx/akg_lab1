using System.Numerics;

namespace GraphicsLabSFML.Models
{
    public class TriangulatedModel
    {
        public Vector4[] Vertices { get; }

        public Vector3[] Normals { get; }

        public int[] FaceVertexIndices { get; }

        public int[] FaceNormalIndices { get; }


        public TriangulatedModel(IEnumerable<Vector4> vertices, IEnumerable<Vector3> normals, IEnumerable<int> faceVertexIndices, IEnumerable<int> faceNormalIndices)
        {
            Vertices = vertices.ToArray();
            Normals = normals.ToArray();
            FaceVertexIndices = faceVertexIndices.ToArray();
            FaceNormalIndices = faceNormalIndices.ToArray();
        }
    }
}
