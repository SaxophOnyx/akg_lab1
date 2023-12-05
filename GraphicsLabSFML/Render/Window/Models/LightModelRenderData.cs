using System.Numerics;

namespace GraphicsLabSFML.Render.Window.Models
{
    public class LightModelRenderData
    {
        public Vector3 CameraPos { get; set; } = new();

        public Vector3 CameraTarget { get; set; } = new();

        public Vector3 LightColor { get; set; } = Vector3.One;
        
        public RenderModel MainModel { get; set; } = new();

        public RenderModel LightSource { get; set; } = new();
    }
}
