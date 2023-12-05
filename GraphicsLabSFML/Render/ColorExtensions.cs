using SFML.Graphics;
using System.Numerics;

namespace GraphicsLabSFML.Render
{
    public static class ColorExtensions
    {
        public static Color Multiply(this Color color, float value)
        {
            byte r = (byte)(color.R * value);
            byte g = (byte)(color.G * value);
            byte b = (byte)(color.B * value);

            return new Color(r, g, b, color.A);
        }

        public static Color Blend(this Color a, Color b, float ratio)
        {
            double red = (1 - ratio) * a.R + ratio * b.R;
            double green = (1 - ratio) * a.G + ratio * b.G;
            double blue = (1 - ratio) * a.B + ratio * b.B;

            return new((byte)red, (byte)green, (byte)blue);
        }
    }
}
