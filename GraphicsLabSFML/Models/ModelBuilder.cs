using System.Numerics;

namespace GraphicsLabSFML.Models
{
    public class ModelBuilder
    {
        private List<Vector4> _vertices = new();
        private List<Vector3> _normals = new();
        private List<FaceInfo> _faces = new();


        public ModelBuilder AddVertex(Vector4 vertex)
        {
            _vertices.Add(vertex);
            return this;
        }

        public ModelBuilder AddVertices(IEnumerable<Vector4> vertices)
        {
            _vertices.AddRange(vertices);
            return this;
        }

        public ModelBuilder AddNormal(Vector3 normal)
        {
            _normals.Add(normal);
            return this;
        }

        public ModelBuilder AddNormals(IEnumerable<Vector3> normals)
        {
            _normals.AddRange(normals);
            return this;
        }

        public ModelBuilder AddFace(FaceInfo face)
        {
            if (!CanFaceBeAdded(face))
                throw new ArgumentException();

            _faces.Add(face);
            return this;
        }

        public ModelBuilder AddFaces(IEnumerable<FaceInfo> faces)
        {
            foreach (var face in faces)
                AddFace(face);

            return this;
        }

        public TriangulatedModel BuildTriangulated()
        {
            int verticesPerFace = _faces[0].VerticesIndices.Length;
            int triangulatedLength = _faces.Count * 3 * (verticesPerFace - 2);
            int i = 0;
            int n = 0;

            int[] triangVertices = new int[triangulatedLength];
            int[] triangNormals = new int[triangulatedLength];

            for (int j = 0; j < _faces.Count; ++j)
            {
                int[] indices = _faces[j].VerticesIndices;

                for (int k = 1; k < indices.Length - 1; ++k)
                {
                    triangVertices[i++] = indices[0];
                    triangVertices[i++] = indices[k];
                    triangVertices[i++] = indices[k + 1];
                }

                int[] normals = _faces[j].NormalsIndices;

                for (int k = 1; k < normals.Length - 1; ++k)
                {
                    triangNormals[n++] = normals[0];
                    triangNormals[n++] = normals[k];
                    triangNormals[n++] = normals[k + 1];
                }
            }

            return new TriangulatedModel(_vertices, _normals, triangVertices, triangNormals);
        }

        private bool CanFaceBeAdded(FaceInfo face)
        {
            for (int i = 0; i < face.VerticesIndices.Length; ++i)
            {
                if (face.VerticesIndices[i] >= _vertices.Count)
                    return false;
            }

            return true;
        }
    }
}
