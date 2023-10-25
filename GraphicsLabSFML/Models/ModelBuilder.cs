using System.Numerics;

namespace GraphicsLabSFML.Models
{
    public class ModelBuilder
    {
        private List<Vector4> _vertices = new();
        private List<FaceInfo> _faces = new();


        public Model Build()
        {
            return new(_vertices, _faces);
        }

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
