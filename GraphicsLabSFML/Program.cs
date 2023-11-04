using GraphicsLabSFML.Models;
using GraphicsLabSFML.Parsing;
using GraphicsLabSFML.Render;
using GraphicsLabSFML.Render.Window;
using GraphicsLabSFML.Render.Window.Input;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace GraphicsLabSFML
{
    internal class Program
    {
        const int width = 1280;
        const int height = 720;

        private static RenderData _data;
        private static Matrix4x4 _viewport = Matrix4x4.Transpose(Matrix4x4Factories.CreateViewport(width, height));
        private static OrderablePartitioner<Tuple<int, int>> _partitioner;

        static void Main()
        {
            string filePath = "C:\\Users\\German\\Downloads\\cat.obj";
            string[] source = File.ReadAllLines(filePath);

            IModelParser parser = new ModelParser();
            TriangulatedModel model = parser.ParseTriangulated(source);

            _data = new()
            {
                CameraTarget = new(0, 0, 1),
                Vertices = model.Vertices.Select(v => new CVector4(v)).ToArray(),
                FlatFaces = model.FlatFaces,
                VerticesPerFace = 4,
            };

            _partitioner = Partitioner.Create(0, _data.Vertices.Length);
            
            CustomWindowOptions options = CustomWindowOptions.Default;
            options.Width = width;
            options.Height = height;
            options.VSync = false;

            InputHandler inputHandler = CreateInputHandler();

            CustomWindow window = new(_data, options, inputHandler);

            Stopwatch stopwatch = new();

            while (window.IsOpen)
            {
                stopwatch.Restart();

                window.DispatchEvents();
                TransformVertices(model.Vertices, _data.Vertices);
                window.Render();

                stopwatch.Stop();
                Console.WriteLine($"Elapsed = {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static void TransformVertices(Vector4[] source, CVector4[] output)
        {
            Matrix4x4 scale = Matrix4x4.CreateScale(_data.Scale);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_data.ModelRotation.X));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_data.ModelRotation.Y));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_data.ModelRotation.Z));
            Matrix4x4 model = rotateZ * rotateY * rotateX * scale;

            Matrix4x4 projection = Matrix4x4Factories.CreateProjection(width, height, 0.1f, 100f);
            Matrix4x4 view = Matrix4x4Factories.CreateView(_data.CameraPos, _data.CameraTarget, Vector3.UnitY);

            Matrix4x4 beforeViewport = Matrix4x4.Transpose(projection * view * model);

            Parallel.ForEach(_partitioner, (range, state) =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    Vector4 tmp = Vector4.Transform(source[i], beforeViewport);
                    tmp /= tmp.W;

                    bool isVisible = (tmp.Z > -1 && tmp.Z < 1) && (tmp.X > -1 && tmp.X < 1) && (tmp.Y > -1 && tmp.Y < 1);
                    output[i].IsVisible = isVisible;

                    if (isVisible)
                    {
                        output[i].Value = Vector4.Transform(tmp, _viewport);
                    }
                }
            });
            
        }

        private static InputHandler CreateInputHandler()
        {
            InputHandler inputHandler = new();

            inputHandler.OnCameraMoved += (Vector3 v) => {
                _data.CameraTarget += v;
                _data.CameraPos += v;
            };

            inputHandler.OnModelRotated += (Vector3 v) => _data.ModelRotation += v;

            inputHandler.OnModelScaled += (float delta) => _data.Scale *= delta;

            return inputHandler;
        }
    }
}
