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

            /*
            Vector3 zAxis1 = eye - target;
            Vector3 xAxis1 = Vector3.Cross(up, zAxis1);
            Vector3 yAxis1 = Vector3.Cross(zAxis1, xAxis1);

            Vector3 zAxis = Vector3.Normalize(zAxis1);
            Vector3 xAxis = Vector3.Normalize(xAxis1);
            Vector3 yAxis = Vector3.Normalize(yAxis1);

            return new Matrix4x4(
              xAxis.X, xAxis.Y, xAxis.Z, -Vector3.Dot(xAxis, eye),
              yAxis.X, yAxis.Y, yAxis.Z, -Vector3.Dot(yAxis, eye),
              zAxis.X, zAxis.Y, zAxis.Z, -Vector3.Dot(zAxis, eye),
              0, 0, 0, 1
            );
            */
        }

        public static Matrix4x4 CreateProjection(uint width, uint height, float znear, float zfar)
        {
            float fov = Utils.DegreesToRadians(90f);
            float aspect = width / 1f / height;

            float tag = (float)Math.Tan(fov / 2);
            float rDist = 1f / (zfar - znear);

            Matrix4x4 m = new Matrix4x4()
            {
                M11 = 1 / (aspect * tag),
                M22 = 1 / (tag),
                M33 = (znear - zfar) * rDist,
                M34 = -2f * zfar * znear * rDist,
                M43 = -1f
            };

            return m;


            /*
            var m1 = new Matrix4x4
            (
                2f / width, 0, 0, 0,
                0, 2f / height, 0, 0,
                0, 0, 1 / (znear - zfar), znear / (znear - zfar),
                0, 0, 0, 1
            );

            var m2 = new Matrix4x4
            (
                2f * znear / width, 0, 0, 0,
                0, 2f * znear / height, 0, 0,
                0, 0, zfar / (znear - zfar), znear * zfar / (znear - zfar),
                0, 0, -1f, 0
            );

            return m2;
            */
        }
    }
}
