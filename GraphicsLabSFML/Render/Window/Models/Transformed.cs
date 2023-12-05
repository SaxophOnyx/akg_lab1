using GraphicsLabSFML.Models;
using System.Numerics;

namespace GraphicsLabSFML.Render.Window.Models
{
    public class Transformed
    {
        public Vector4[] WorldVertices { get; set; } = Array.Empty<Vector4>();

        public CVector4[] ViewportVertices { get; set; } = Array.Empty<CVector4>();

        public Vector3[] Normals { get; set; } = Array.Empty<Vector3>();

        public Vector3[] Textures { get; set; } = Array.Empty<Vector3>();


        public static Transformed InitializeFromMesh(Mesh mesh)
        {
            return new Transformed()
            {
                WorldVertices = mesh.Vertices.Select(v => v).ToArray(),
                ViewportVertices = mesh.Vertices.Select(v => new CVector4(v)).ToArray(),
                Normals = mesh.Normals.Select(n => n).ToArray(),
                Textures = mesh.Textures.Select(t => t).ToArray(),
            };
        }
    }
}
