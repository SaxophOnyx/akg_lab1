using GraphicsLabSFML.Models;
using GraphicsLabSFML.Parsing;
using SFML.Window;
using System.Diagnostics;
using System.Numerics;

namespace GraphicsLabSFML
{
    internal class Program
    {
        const uint width = 1280;
        const uint height = 720;

        private static float _angleX = 0;
        private static float _angleY = 0;
        private static float _angleZ = 0;
        private static Vector3 _cameraPos = new(0, 0, -20);
        private static Vector3 _cameraTarget = new(0, 0, 0);
        private static Vector3 _modelPos = new(0, 0, 20);

        private static float _scale = 1;
        private static Vector3 _position = new(0);

        static void Main()
        {
            string filePath = "C:\\Users\\German\\Downloads\\cat.obj";
            string[] source = File.ReadAllLines(filePath);

            IModelParser parser = new ModelParser();
            Model model = parser.Parse(source);
            IEnumerable<Vector4> vertices = model.Faces.SelectMany(f => f.Vertices);

            Stopwatch stopwatch = new();

            CustomWindow window = new(width, height);
            window.KeyPressed += OnKeyPressed;

            while (window.IsOpen)
            {
                stopwatch.Restart();

                window.DispatchEvents();

                Matrix4x4 matrix = CreateResultMatrix(width, height);
                IEnumerable<Vector4> transformed = TransformVertices(vertices, matrix);

                // IEnumerable<Vector4> transformed = Process(vertices);
                window.RenderFrame(transformed);

                stopwatch.Stop();
                // Console.WriteLine($"Elapsed = ${stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static Matrix4x4 CreateResultMatrix(uint width, uint height)
        {
            Vector3 centerScreenVector = new(width / 2, height / 2, 0);

            // для вращения используется порядок вращения Эйлера

            Matrix4x4 scale = Matrix4x4.CreateScale(_scale);
            Matrix4x4 offset = Matrix4x4.CreateTranslation(_position + centerScreenVector);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_angleX));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_angleY));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_angleZ));

            Matrix4x4 projection = Matrix4x4Factories.CreateProjection(width, height, 0.1f, 100f);
            Matrix4x4 screen = Matrix4x4Factories.CreateViewport(width, height);

            Vector3 up = new(0, 1, 0);

            viewportM = screen;
            projectionM = projection;
            viewM = Matrix4x4Factories.CreateView(_cameraPos, _cameraTarget, up);
            // modelM = scale * rotateZ * rotateY * rotateX * offset;
            modelM = scale;

            /*
            modelM =
                scale *
                rotateZ *
                rotateY *
                rotateX *
                offset *
                offset2;
             */

            // Matrix4x4 res = viewportM * projectionM * viewM * modelM;
            Matrix4x4 res = projectionM * viewM * modelM;
            return Matrix4x4.Transpose(res);
        }

        private static IEnumerable<Vector4> Process(IEnumerable<Vector4> vertices)
        {
            Vector3 centerScreenVector = new(width / 2, height / 2, 0);

            // для вращения используется порядок вращения Эйлера

            Matrix4x4 scale = Matrix4x4.CreateScale(_scale);
            Matrix4x4 offset = Matrix4x4.CreateTranslation(_position + centerScreenVector);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_angleX));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_angleY));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_angleZ));

            Matrix4x4 projection = Matrix4x4Factories.CreateProjection(width, height, 0.1f, 1f);
            var ppp = Matrix4x4.CreatePerspective(width, height, 0.1f, 1f);
            Matrix4x4 screen = Matrix4x4Factories.CreateViewport(width, height);

            Vector3 up = new(0, 10, 0);

            viewportM = Matrix4x4.Transpose(screen);
            projectionM = Matrix4x4.Transpose(projection);
            viewM = Matrix4x4.Transpose(Matrix4x4Factories.CreateView(_cameraPos, _cameraTarget, up));
            var lll = Matrix4x4.CreateLookAt(_cameraPos, _cameraTarget, up);
            modelM = scale * rotateZ * rotateY * rotateX * offset;

            Matrix4x4 res = viewportM * projectionM * viewM * modelM;

            IEnumerable<Vector4> a = vertices.Select(v => Vector4.Transform(v, viewportM));
            IEnumerable<Vector4> b = a.Select(v => Vector4.Transform(v, projectionM));
            IEnumerable<Vector4> c  = b.Select(v => v /= v.W);
            IEnumerable<Vector4> d = c.Select(v => Vector4.Transform(v, viewM)); 
            IEnumerable<Vector4> e = d.Select(v => Vector4.Transform(v, modelM));

            return e;
        }

        private static IEnumerable<Vector4> TransformVertices(IEnumerable<Vector4> vertices, Matrix4x4 matrix)
        {
            var a = vertices.Select(v =>
            {
                Vector4 tmp = Vector4.Transform(v, matrix);
                tmp /= tmp.W;

                return tmp;
            });

            var viewport = Matrix4x4.Transpose(Matrix4x4Factories.CreateViewport(1280, 720));
            var b = a.Select(v => Vector4.Transform(v, viewport));

            return b;
        }

        private static void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            float rotateDelta = 1.5f;
            float movementDelta = 120.0f;

            switch (e.Code)
            {
                // Model rotation
                case Keyboard.Key.W:
                {
                    _angleX += rotateDelta;
                    break;
                }
                case Keyboard.Key.S:
                {
                    _angleX -= rotateDelta;
                    break;
                }
                case Keyboard.Key.A:
                {
                    _angleY -= rotateDelta;
                    break;
                }
                case Keyboard.Key.D:
                {
                    _angleY += rotateDelta;
                    break;
                }
                case Keyboard.Key.E:
                {
                    _angleZ += rotateDelta;
                    break;
                }
                case Keyboard.Key.Q:
                {
                    _angleZ -= rotateDelta;
                    break;
                }

                // Model scale
                case Keyboard.Key.Equal:
                {
                    _scale *= 1.5f;
                    break;
                }
                case Keyboard.Key.Hyphen:
                {
                    _scale *= 0.7f;

                    break;
                }

                // Camera movement
                case Keyboard.Key.Up:
                {
                    _cameraPos.Y += movementDelta / 10;
                    _cameraTarget.Y += movementDelta / 10;
                    break;
                }
                case Keyboard.Key.Down:
                {
                    _cameraPos.Y -= movementDelta / 10;
                    _cameraTarget.Y -= movementDelta / 10;
                    break;
                }
                case Keyboard.Key.Left:
                {
                    _cameraPos.X -= movementDelta / 10;
                    _cameraTarget.X -= movementDelta / 10;
                    break;
                }
                case Keyboard.Key.Right:
                {
                    _cameraPos.X += movementDelta / 10;
                    _cameraTarget.X += movementDelta / 10;
                    break;
                }
                case Keyboard.Key.Space:
                {
                    _cameraPos.Z -= movementDelta / 10;
                    _cameraTarget.Z -= movementDelta / 10;
                    break;
                }
                case Keyboard.Key.Z:
                {
                    _cameraPos.Z += movementDelta / 10;
                    _cameraTarget.Z += movementDelta / 10;
                    break;
                }
            }
        }
    }
}
