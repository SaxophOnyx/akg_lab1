using System.Numerics;

namespace GraphicsLabSFML.Render.Window
{
    public class RenderData
    {
        public Vector3 CameraPos { get; set; } = new();

        public Vector3 CameraTarget { get; set; } = new();

        public Vector3 ModelRotation { get; set; } = new();

        public float Scale { get; set; } = 1;

        public Vector4[] Vertices { get; set; } = Array.Empty<Vector4>();

        public int[] FlatFaces { get; set; } = Array.Empty<int>();

        public int VerticesPerFace { get; set; } = 0;
    }
}
