using System.Numerics;

namespace GraphicsLabSFML.Render.Window.Models
{
    public class Mesh
    {
        public Vector4[] Vertices { get; set; } = Array.Empty<Vector4>();

        public int[] VertexIndices { get; set; } = Array.Empty<int>();

        public Vector3[] Normals { get; set; } = Array.Empty<Vector3>();

        public int[] NormalIndices { get; set; } = Array.Empty<int>();
    }
}
