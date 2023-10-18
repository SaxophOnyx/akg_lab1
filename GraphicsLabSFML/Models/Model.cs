using System.Numerics;

namespace GraphicsLabSFML.Models
{
    public class Model
    {
        public Vector4[] Vertices { get; }

        public FaceInfo[] Faces { get; }




        public Model(IEnumerable<Vector4> vertices, IEnumerable<FaceInfo> faces)
        {
            Vertices = vertices.ToArray();
            Faces = faces.ToArray();
        }

        public int[] FlatFaces()
        {
            int step = Faces[0].VerticesIndices.Length;
            int[] flat = new int[Faces.Length * step];

            for (int i = 0; i < Faces.Length; ++i)
            {
                for (int j = 0; j < Faces[i].VerticesIndices.Length; ++j)
                {
                    flat[i * step + j] = Faces[i].VerticesIndices[j];
                }
            }    

            return flat;
        }
    }
}
