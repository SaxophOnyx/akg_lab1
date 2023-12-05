using System.Numerics;

namespace GraphicsLabSFML.Render.Window.Models
{
    public enum Status { Visible, OutOfScreen, Culled }

    public struct CVector4
    {
        public bool IsVisible => Status == Status.Visible;

        public bool IsCulled => Status == Status.Culled;

        public Vector4 Value;

        public Status Status;


        public CVector4(Vector4 value)
        {
            Value = value;
            Status = Status.Visible;
        }
    }
}
