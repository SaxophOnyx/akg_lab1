using System.Numerics;
using GraphicsLabSFML.Models;

namespace GraphicsLabSFML.Render.Window.Models
{
    public class RenderModel
    {
        public Mesh Mesh { get; set; } = new();

        public Transformed Transformed { get; set; } = new();

        public Map<Vector3> DiffuseMap { get; set; } = Map<Vector3>.Empty();
        
        public Map<Vector3> NormalMap { get; set; } = Map<Vector3>.Empty();

        public Map<Vector3> SpecularMap { get; set; } = Map<Vector3>.Empty();

        public float Scale { get; set; } = 1;

        public Vector3 Rotation { get; set; } = new();

        public Vector3 WorldPosition { get; set; } = new();


        public Matrix4x4 CreateModelMatrix()
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(Scale);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(Rotation.X));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(Rotation.Y));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(Rotation.Z));
            Matrix4x4 translation = Matrix4x4Factories.CreateTranslation(WorldPosition);

            return translation * rotateZ * rotateY * rotateX * scale;
        }

        public Matrix4x4 CreateRotation()
        {
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(Rotation.X));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(Rotation.Y));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(Rotation.Z));

            return rotateZ * rotateY * rotateX;
        }
    }
}
