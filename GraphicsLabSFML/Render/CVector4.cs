using System.Numerics;

namespace GraphicsLabSFML.Render
{
    public struct CVector4
    {
        public Vector4 Value;

        public bool IsVisible;


        public CVector4(Vector4 value)
        {
            Value = value;
            IsVisible = false;
        }
    }
}
