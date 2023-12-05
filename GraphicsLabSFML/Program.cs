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
        const float cullingCoeff = -2;

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
            options.VSync = false;

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

            // vertices
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
                    if (isVisible)
                    {
                        viewportVertices[i].Status = Status.Visible;
                        viewportVertices[i].Value = Vector4.Transform(tmp, _viewport);
                    }
                    else
                    {
                        viewportVertices[i].Status = Status.OutOfScreen;
                    }
                }
            });

            // normals
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

            // normalmaps
            /*
            Map<Vector3> sourceNormalMap = renderModel.NormalMap;
            Map<Vector3> destNormalMap = renderModel.Transformed.NormalMap;

            for (int i = 0; i < renderModel.NormalMap.Width; ++i)
            {
                for (int j = 0; j < renderModel.NormalMap.Height; ++j)
                {
                    destNormalMap[i, j] = Vector3.Normalize(Vector3.Transform(sourceNormalMap[i, j], modelMatrix));
                }
            }
            */
            //

            // textures
            Vector3[] sourceTextures = renderModel.Mesh.Textures;
            Vector3[] worldTextures = renderModel.Transformed.Textures;

            OrderablePartitioner<Tuple<int, int>> texturesPartioner = _partioners[renderModel.Mesh.Textures];
            Parallel.ForEach(texturesPartioner, (range, _) =>
            {
                for (int i = range.Item1; i < range.Item2; ++i)
                {
                    // worldTextures[i] = Vector3.Transform(sourceTextures[i], it_model4x4);
                    worldTextures[i] = sourceTextures[i];
                }
            });

            // culling
            Matrix4x4 viewMatrix = view * modelMatrix;
            Matrix4x4 reversedViewMatrix;

            if (!Matrix4x4.Invert(Matrix4x4.Transpose(viewMatrix), out reversedViewMatrix))
            {
                throw new Exception();
            }

            OrderablePartitioner<Tuple<int, int>> testPartioner = _partioners[renderModel.Mesh.VertexIndices];
            Parallel.ForEach(testPartioner, (range, _) =>
            {
                for (int i = range.Item1; i < range.Item2; ++i)
                {
                    Vector3 normal = Vector3.Normalize(Vector3.Transform(sourceNormals[renderModel.Mesh.NormalIndices[i]], reversedViewMatrix));
                    float cos = Vector3.Dot(normal, -Vector3.UnitZ);

                    if (cos < cullingCoeff)
                    {
                        CVector4 vertex = viewportVertices[renderModel.Mesh.VertexIndices[i]];

                        if (vertex.IsVisible)
                        {
                            viewportVertices[renderModel.Mesh.VertexIndices[i]].Status = Status.Culled;
                        }
                    }
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

            inputHandler.OnModelRotated += (Vector3 v) => _data.MainModel.Rotation += v * 5;

            inputHandler.OnModelScaled += (float delta) => _data.MainModel.Scale *= delta;

            inputHandler.OnLightSourceWorldPosChanged += (Vector3 v) => _data.LightSource.WorldPosition += v;

            return inputHandler;
        }

        private static LightModelRenderData BuildRenderData()
        {
            string modelFilePath = "C:\\Users\\German\\Downloads\\HeadModel\\Model.obj";
            string modelDiffuseFilePath = "C:\\Users\\German\\Downloads\\HeadModel\\Diffuse.png";
            string modelNormalsFilePath = "C:\\Users\\German\\Downloads\\HeadModel\\Normal.png";
            string modelSpecularFilePath = "C:\\Users\\German\\Downloads\\HeadModel\\Specular.png";

            string lightSourceFilepath = "C:\\Users\\German\\Downloads\\lightSourceCube.obj";

            Mesh modelMesh = MeshParser.ParseFile(modelFilePath);
            Map<Vector3> modelDiffuse = MapReader.ReadDiffuse(modelDiffuseFilePath);
            Map<Vector3> modelNormal = MapReader.ReadNormal(modelNormalsFilePath);
            Map<Vector3> modelSpecular = MapReader.ReadDiffuse(modelSpecularFilePath);

            Mesh lightMesh = MeshParser.ParseFile(lightSourceFilepath);

            return new()
            {
                CameraTarget = new(0, 0, -19),
                CameraPos = new(0, 0, -20),
                LightColor = new(0.6f, 0.6f, 0.6f),
                MainModel = new()
                {
                    Mesh = modelMesh,
                    DiffuseMap = modelDiffuse,
                    NormalMap = modelNormal,
                    SpecularMap = modelSpecular,
                    Rotation = new(0, 1, 0),
                    Transformed = Transformed.InitializeFromMesh(modelMesh)
                },
                LightSource = new()
                {
                    Mesh = lightMesh,
                    Transformed = Transformed.InitializeFromMesh(lightMesh),
                    WorldPosition = new(0, 10, 0),
                    Scale = 0.05f,
                }
            };
        }

        private static void CreatePartioners(LightModelRenderData data)
        {
            RegisterPartioner(data.MainModel.Mesh.Vertices);
            RegisterPartioner(data.MainModel.Mesh.Normals);
            RegisterPartioner(data.MainModel.Mesh.Textures);
            RegisterPartioner(data.MainModel.Mesh.VertexIndices);
            RegisterPartioner(data.LightSource.Mesh.Vertices);
            RegisterPartioner(data.LightSource.Mesh.Normals);
            RegisterPartioner(data.LightSource.Mesh.Textures);
            RegisterPartioner(data.LightSource.Mesh.VertexIndices);
        }

        private static void RegisterPartioner(Array array)
        {
            _partioners.Add(array, Partitioner.Create(0, array.Length));
        }
    }
}
