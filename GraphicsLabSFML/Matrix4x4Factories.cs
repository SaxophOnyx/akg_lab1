using System.Numerics;

namespace GraphicsLabSFML
{
    public static class Matrix4x4Factories
    {
        public static Matrix4x4 CreateViewport(float width, float height)
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

            return matrix;
        }

        public static Matrix4x4 CreateView(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 dir = Vector3.Normalize(eye - target);
            Vector3 right = Vector3.Normalize(Vector3.Cross(up, dir));
            Vector3 newUp = Vector3.Cross(dir, right);

            Matrix4x4 a = new
            (
                right.X, right.Y, right.Z, 0f,
                newUp.X, newUp.Y, newUp.Z, 0f,
                dir.X, dir.Y, dir.Z, 0f,
                0f, 0f, 0f, 1f
            );

            Matrix4x4 b = new()
            {
                M11 = 1f,
                M22 = 1f,
                M33 = 1f,
                M44 = 1f,

                M14 = -eye.X,
                M24 = -eye.Y,
                M34 = -eye.Z
            };

            return a * b;
        }

        public static Matrix4x4 CreateProjection(uint width, uint height, float znear, float zfar)
        {
            float fov = Utils.DegreesToRadians(90f);
            float aspect = width / 1f / height;

            float tag = (float)Math.Tan(fov / 2);
            float rDist = 1f / (zfar - znear);

            Matrix4x4 m = new()
            {
                M11 = 1 / (aspect * tag),
                M22 = 1 / (tag),
                M33 = -1 * (znear + zfar) * rDist,
                M34 = -2f * zfar * znear * rDist,
                M43 = -1f
            };

            return m;
        }

        public static Matrix4x4 CreateTranslation(Vector3 vector)
        {
            Matrix4x4 result = Matrix4x4.Identity;

            result.M14 = vector.X;
            result.M24 = vector.Y;
            result.M34 = vector.Z;

            return result;
        }
    }
}
