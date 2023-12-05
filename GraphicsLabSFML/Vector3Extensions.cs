using SFML.Graphics;
using System.Numerics;

namespace GraphicsLabSFML
{
    public static class Vector3Extensions
    {
        public static Color ToClampedColor(this Vector3 vector)
        {
            Vector3 clamped = Vector3.Clamp(vector, Vector3.Zero, Vector3.One);

            byte r = (byte)(clamped.X * 255);
            byte g = (byte)(clamped.Y * 255);
            byte b = (byte)(clamped.Z * 255);
            
            return new Color(r, g, b);
        }
    }
}
