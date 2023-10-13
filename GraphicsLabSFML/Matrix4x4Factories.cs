using System.Numerics;

namespace GraphicsLabSFML
{
    public static class Matrix4x4Factories
    {
        public static Matrix4x4 CreateScreen(float width, float height)
        {
            float hWidth = width / 2;
            float hHeight = height / 2;
            float xMin = 0;
            float yMin = 0;

            Matrix4x4 matrix = new
            (
                hWidth, 0, 0, xMin + hWidth,
                0, -hHeight, 0, yMin + hHeight,
                0, 0, 1, 0,
                0, 0, 0, 1
            );

        //    return Matrix4x4.Transpose(matrix);

           return matrix;
        }
    }
}
