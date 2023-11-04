namespace GraphicsLabSFML
{
    static class Utils
    {
        public static float DegreesToRadians(double degrees)
        {
            return (float)(Math.PI / 180 * degrees);
        }

        public static float Min(float a, float b, float c)
        {
            return Math.Min(a, Math.Min(b, c));
        }

        public static float Max(float a, float b, float c)
        {
            return Math.Max(a, Math.Max(b, c));
        }
    }
}
