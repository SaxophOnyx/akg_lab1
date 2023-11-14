using System.Numerics;

namespace GraphicsLabSFML.Render.Window.Models
{
    public class Transformed
    {
        public Vector4[] WorldVertices { get; set; } = Array.Empty<Vector4>();

        public CVector4[] ViewportVertices { get; set; } = Array.Empty<CVector4>();

        public Vector3[] Normals { get; set; } = Array.Empty<Vector3>();
    }
}
