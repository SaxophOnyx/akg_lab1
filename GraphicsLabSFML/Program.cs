using GraphicsLabSFML.Models;
using GraphicsLabSFML.Parsing;
using SFML.Window;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace GraphicsLabSFML
{
    internal class Program
    {
        const uint width = 1280;
        const uint height = 720;

        private static float _angleX = 0;
        private static float _angleY = 0;
        private static float _angleZ = 0;
        private static Vector3 _cameraPos = new(0, 0, 0);
        private static Vector3 _modelPos = new(0, 0, -10);

        private static float _scale = 1;
        private static Vector3 _position = new(0);

        private static Matrix4x4 viewportM;
        private static Matrix4x4 projectionM;
        private static Matrix4x4 viewM;
        private static Matrix4x4 modelM;

        static void Main()
        {
            string filePath = "C:\\Users\\Bsuir\\Downloads\\cat.obj";
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

                window.RenderFrame(transformed);

                stopwatch.Stop();
                Console.WriteLine($"Elapsed = ${stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static Matrix4x4 createViewMatrix(Vector3 eye, Vector3 target, Vector3 up)
        {
            Vector3 zAxis = Vector3.Normalize(eye - target);
            Vector3 xAxis = Vector3.Normalize(Vector3.Cross(up, zAxis));
            Vector3 yAxis = up;

            return new Matrix4x4(
              xAxis.X, xAxis.Y, xAxis.Z, -Vector3.Dot(xAxis, eye),
              yAxis.X, yAxis.Y, yAxis.Z, -Vector3.Dot(yAxis, eye),
              zAxis.X, zAxis.Y, zAxis.Z, -Vector3.Dot(zAxis, eye),
              0, 0, 0, 1
            );
        }

        private static Matrix4x4 createProjectionMatrix(float znear, float zfar) {
            var a = 2 * 10 / 1000;
            var b = 2 * 10f / 1000;
            return new Matrix4x4(
                2f / width, 0, 0, 0,
                0, 2f / height, 0, 0,
                0, 0, 1 / (znear - zfar), znear / (znear - zfar),
                0, 0, 0, 1
                );
        }

        private static Matrix4x4 CreateResultMatrix(uint width, uint height)
        {
         
            // для вращения используется порядок вращения Эйлера

            Matrix4x4 scale = Matrix4x4.CreateScale(_scale);
            // Matrix4x4 offset = Matrix4x4.CreateTranslation(width / 2, height / 2, 0);
            Matrix4x4 offset = Matrix4x4.CreateTranslation(new Vector3(width / 2,  height / 2, 10));
            Matrix4x4 offset2 = Matrix4x4.CreateTranslation(_position);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_angleX));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_angleY));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_angleZ));

            //same one
            Matrix4x4 camera = Matrix4x4.CreateLookAt(_cameraPos, _modelPos, new Vector3(0, 1, 0));

            Matrix4x4 projection = 
                createProjectionMatrix(0.01f, 1);

                //not works;
                //Matrix4x4.CreatePerspective(width, height, 0.01f, 1f);
            Matrix4x4 screen = Matrix4x4Factories.CreateScreen(1280, 720);

            Vector3 eye = _cameraPos;
            Vector3 target = _modelPos;       
            Vector3 up = new(0, 1, 0);

            viewportM = Matrix4x4.Transpose(screen);
            projectionM = Matrix4x4.Transpose(projection);
            viewM = Matrix4x4.Transpose(createViewMatrix(eye, target, up));
            modelM = scale * rotateZ * rotateY * rotateX * offset * offset2;

            Matrix4x4 res =
                viewportM *
                projectionM *
                viewM *
                modelM;

            return res;
        }
    
        private static IEnumerable<Vector4> TransformVertices(IEnumerable<Vector4> vertices, Matrix4x4 matrix)
        {
            var a = vertices.Select(v => Vector4.Transform(v, matrix));//.Select(v => v / v.W);
           // var a = vertices.Select(v => v / v.W);
            return a;
        }

        private static void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            float rotateDelta = 1.5f;
            float scaleDelta = 0.5f;
            float movementDelta = 20.0f;

            switch (e.Code)
            {
                // Model rotation
                case Keyboard.Key.W:
                {
                        // _angleX += rotateDelta;
                        _cameraPos.X += rotateDelta;
                        break;
                }
                case Keyboard.Key.S:
                {
                 //   _angleX -= rotateDelta;
                        _cameraPos.Y -= rotateDelta;
                        break;
                }
                case Keyboard.Key.A:
                {
                        // _angleY -= rotateDelta;
                        _cameraPos.X -= rotateDelta;
                    break;
                }
                case Keyboard.Key.D:
                {
                  //  _angleY += rotateDelta;
                        _cameraPos.X += rotateDelta;
                        break;
                }
                case Keyboard.Key.E:
                {
                  //  _angleZ += rotateDelta;
                        _cameraPos.Z += rotateDelta;
                        break;
                }
                case Keyboard.Key.Q:
                {
                    //_angleZ -= rotateDelta;
                        _cameraPos.Z -= rotateDelta;
                        break;
                }
                case Keyboard.Key.Equal:
                {
                    //_scale += scaleDelta;
                    _scale *= 1.5f;
                    break;
                }
                case Keyboard.Key.Hyphen:
                {
                        _scale *= 0.7f;

                    break;
                }

                // Model movement
                case Keyboard.Key.Up:
                {
                    _position.Y += movementDelta;
                    break;
                }
                case Keyboard.Key.Down:
                {
                    _position.Y -= movementDelta;
                    break;
                }
                case Keyboard.Key.Left:
                {
                    _position.X -= movementDelta;
                    break;
                }
                case Keyboard.Key.Right:
                {
                        _position.X += movementDelta;
                  //  _modelPos.X += movementDelta;
                    break;
                }
                case Keyboard.Key.Space:
                {
                    _modelPos.Y -= movementDelta;
                    break;
                }
                case Keyboard.Key.Z:
                {
                    _modelPos.Y += movementDelta;
                    break;
                }
            }
        }
    }
}
