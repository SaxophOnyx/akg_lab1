using GraphicsLabSFML.Models;
using GraphicsLabSFML.Parsing;
using SFML.Window;
using System.Diagnostics;
using System.Numerics;

namespace GraphicsLabSFML
{
    internal class Program
    {
        const int width = 1280;
        const int height = 720;

        private static Vector3 _modelRotation = new();
        private static Vector3 _cameraPos = new(0, 0, -20);
        private static Vector3 _cameraTarget = new(0, 0, 0);
        private static float _scale = 1f;

        private static Matrix4x4 _viewport = Matrix4x4.Transpose(Matrix4x4Factories.CreateViewport(width, height));

        static void Main()
        {
            string filePath = "C:\\Users\\German\\Downloads\\cat.obj";
            string[] source = File.ReadAllLines(filePath);

            IModelParser parser = new ModelParser();
            Model model = parser.Parse(source);

            Vector4[] transformed = new Vector4[model.Vertices.Length];
            int[] flatFaces = model.FlatFaces();

            CustomWindow window = new(width, height);
            window.KeyPressed += OnKeyPressed;

            Stopwatch stopwatch = new();

            while (window.IsOpen)
            {
                stopwatch.Restart();

                window.DispatchEvents();
                TransformVertices(model.Vertices, transformed);
                window.RenderFrame(transformed, flatFaces);

                stopwatch.Stop();
                Console.WriteLine($"Elapsed = {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static void TransformVertices(Vector4[] source, Vector4[] output)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(_scale);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_modelRotation.X));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_modelRotation.Y));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_modelRotation.Z));
            Matrix4x4 model = rotateZ * rotateY * rotateX * scale;

            Matrix4x4 projection = Matrix4x4Factories.CreateProjection(width, height, 0.1f, 100f);
            Matrix4x4 view = Matrix4x4Factories.CreateView(_cameraPos, _cameraTarget, Vector3.UnitY);

            Matrix4x4 beforeViewport = Matrix4x4.Transpose(projection * view * model);

            Parallel.For(0, source.Length, (i) =>
            {
                Vector4 tmp = Vector4.Transform(source[i], beforeViewport);
                tmp /= tmp.W;
                output[i] = Vector4.Transform(tmp, _viewport);
            });
        }

        private static void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            float rotateDelta = 1.5f;
            float movementDelta = 12.0f;

            switch (e.Code)
            {
                // Model rotation
                case Keyboard.Key.W:
                {
                    _modelRotation.Y += rotateDelta;
                    break;
                }
                case Keyboard.Key.S:
                {
                    _modelRotation.Y -= rotateDelta;
                    break;
                }
                case Keyboard.Key.A:
                {
                    _modelRotation.X -= rotateDelta;
                    break;
                }
                case Keyboard.Key.D:
                {
                    _modelRotation.X += rotateDelta;
                    break;
                }
                case Keyboard.Key.E:
                {
                    _modelRotation.Z += rotateDelta;
                    break;
                }
                case Keyboard.Key.Q:
                {
                    _modelRotation.Z -= rotateDelta;
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
                    _cameraPos.Y += movementDelta;
                    _cameraTarget.Y += movementDelta;
                    break;
                }
                case Keyboard.Key.Down:
                {
                    _cameraPos.Y -= movementDelta;
                    _cameraTarget.Y -= movementDelta;
                    break;
                }
                case Keyboard.Key.Left:
                {
                    _cameraPos.X -= movementDelta;
                    _cameraTarget.X -= movementDelta;
                    break;
                }
                case Keyboard.Key.Right:
                {
                    _cameraPos.X += movementDelta;
                    _cameraTarget.X += movementDelta;
                    break;
                }
                case Keyboard.Key.Space:
                {
                    _cameraPos.Z -= movementDelta;
                    _cameraTarget.Z -= movementDelta;
                    break;
                }
                case Keyboard.Key.Z:
                {
                    _cameraPos.Z += movementDelta;
                    _cameraTarget.Z += movementDelta;
                    break;
                }
            }
        }
    }
}
