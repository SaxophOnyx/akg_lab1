using GraphicsLabSFML.Models;
using GraphicsLabSFML.Parsing;
using GraphicsLabSFML.Render.Window;
using GraphicsLabSFML.Render.Window.Input;
using GraphicsLabSFML.Render.Window.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Numerics;

namespace GraphicsLabSFML
{
    internal class Program
    {
        const int width = 1280;
        const int height = 720;

        private static LightModelRenderData _data;
        private static Matrix4x4 _viewport = Matrix4x4.Transpose(Matrix4x4Factories.CreateViewport(width, height));
        private static Dictionary<object, OrderablePartitioner<Tuple<int, int>>> _partioners = new();

        static void Main()
        {
            _data = BuildRenderData();
            CreatePartioners(_data);

            CustomWindowOptions options = CustomWindowOptions.Default;
            options.Width = width;
            options.Height = height;
            options.VSync = true;

            InputHandler inputHandler = CreateInputHandler();

            CustomWindow window = new(_data, options, inputHandler);

            Stopwatch stopwatch = new();

            while (window.IsOpen)
            {
                stopwatch.Restart();

                window.DispatchEvents();

                TransformRenderModel(_data.MainModel);
                TransformRenderModel(_data.LightSource);

                window.Render();

                stopwatch.Stop();
                Console.WriteLine($"Elapsed = {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static void TransformRenderModel(RenderModel renderModel)
        {
            Matrix4x4 modelMatrix = renderModel.CreateModelMatrix();
            Matrix4x4 it_model4x4;
            Matrix4x4.Invert(modelMatrix, out it_model4x4);

            Matrix4x4 projection = Matrix4x4Factories.CreateProjection(width, height, 0.1f, 100f);
            Matrix4x4 view = Matrix4x4Factories.CreateView(_data.CameraPos, _data.CameraTarget, Vector3.UnitY);
            Matrix4x4 beforeViewport = Matrix4x4.Transpose(projection * view * modelMatrix);

            Vector4[] sourceVertices = renderModel.Mesh.Vertices;
            Vector4[] worldVertices = renderModel.Transformed.WorldVertices;
            CVector4[] viewportVertices = renderModel.Transformed.ViewportVertices;

            OrderablePartitioner<Tuple<int, int>> vertexPartioner = _partioners[renderModel.Mesh.Vertices];
            Parallel.ForEach(vertexPartioner, (range, _) =>
            {
                for (int i = range.Item1; i < range.Item2; ++i)
                {
                    worldVertices[i] = Vector4.Transform(sourceVertices[i], modelMatrix);

                    Vector4 tmp = Vector4.Transform(sourceVertices[i], beforeViewport);
                    tmp /= tmp.W;

                    bool isVisible = (tmp.Z > -1 && tmp.Z < 1) && (tmp.X > -1 && tmp.X < 1) && (tmp.Y > -1 && tmp.Y < 1);
                    viewportVertices[i].IsVisible = isVisible;

                    if (isVisible)
                    {
                        viewportVertices[i].Value = Vector4.Transform(tmp, _viewport);
                    }
                }
            });

            Vector3[] sourceNormals = renderModel.Mesh.Normals;
            Vector3[] worldNormals = renderModel.Transformed.Normals;

            OrderablePartitioner<Tuple<int, int>> normalsPartioner = _partioners[renderModel.Mesh.Normals];
            Parallel.ForEach(normalsPartioner, (range, _) =>
            {
                for (int i = range.Item1; i < range.Item2; ++i)
                {
                    worldNormals[i] = Vector3.Transform(sourceNormals[i], it_model4x4);
                }
            });
        }

        private static InputHandler CreateInputHandler()
        {
            InputHandler inputHandler = new();

            inputHandler.OnCameraMoved += (Vector3 v) =>
            {
                _data.CameraTarget += v;
                _data.CameraPos += v;
            };

            inputHandler.OnModelRotated += (Vector3 v) => _data.MainModel.Rotation += v;

            inputHandler.OnModelScaled += (float delta) => _data.MainModel.Scale *= delta;

            inputHandler.OnLightSourceWorldPosChanged += (Vector3 v) => _data.LightSource.WorldPosition += v;

            return inputHandler;
        }

        private static LightModelRenderData BuildRenderData()
        {
            string modelFilePath = "C:\\Users\\German\\Downloads\\cat.obj";
            string lightSourceFilepath = "C:\\Users\\German\\Downloads\\lightSourceCube.obj";
            //string modelFilePath = "/mnt/sata0/Workshop/cat.obj";
            //string modelFilePath = "/mnt/sata0/Workshop/cat.obj";
            //string lightSourceFilepath = "/mnt/sata0/Workshop/lightSourceCube.obj";

            string[] modelLines = File.ReadAllLines(modelFilePath);
            string[] lightSourceLines = File.ReadAllLines(lightSourceFilepath);

            IModelParser parser = new ModelParser();

            TriangulatedModel model = parser.ParseTriangulated(modelLines);
            TriangulatedModel light = parser.ParseTriangulated(lightSourceLines);

            Mesh modelMesh = new()
            {
                Vertices = model.Vertices.Select(v => v).ToArray(),
                VertexIndices = model.FaceVertexIndices,
                Normals = model.Normals.Select(n => n).ToArray(),
                NormalIndices = model.FaceNormalIndices,
            };

            Mesh lightMesh = new()
            {
                Vertices = light.Vertices.Select(v => v).ToArray(),
                VertexIndices = light.FaceVertexIndices,
                Normals = light.Normals.Select(n => n).ToArray(),
                NormalIndices = light.FaceNormalIndices,
            };

            return new()
            {
                CameraTarget = new(0, 0, 1),
                MainModel = new()
                {
                    Mesh = modelMesh,
                    Transformed = new()
                    {
                        WorldVertices = model.Vertices.Select(v => v).ToArray(),
                        ViewportVertices = model.Vertices.Select(v => new CVector4(v)).ToArray(),
                        Normals = model.Normals.Select(n => n).ToArray(),
                    }
                },
                LightSource = new()
                {
                    Mesh = lightMesh,
                    Transformed = new()
                    {
                        WorldVertices = light.Vertices.Select(v => v).ToArray(),
                        ViewportVertices = light.Vertices.Select(v => new CVector4(v)).ToArray(),
                        Normals = light.Normals.Select(n => n).ToArray(),
                    },
                    WorldPosition = new(0, 10, 0),
                    Scale = 0.5f,
                }
            };
        }

        private static void CreatePartioners(LightModelRenderData data)
        {
            RegisterPartioner(data.MainModel.Mesh.Vertices);
            RegisterPartioner(data.MainModel.Mesh.Normals);
            RegisterPartioner(data.LightSource.Mesh.Vertices);
            RegisterPartioner(data.LightSource.Mesh.Normals);
        }

        private static void RegisterPartioner(Array array)
        {
            _partioners.Add(array, Partitioner.Create(0, array.Length));
        }
    }
}
