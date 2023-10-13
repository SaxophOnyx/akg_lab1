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
        private static Vector3 _cameraPos = new(-250, -250, -1250);
        //private static Vector3 _modelPos = new(width / 2, height / 2, 100);
        private static Vector3 _modelPos = new(100, 100, 100);

        private static float _scale = 1;

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

            Vector3 zAxis1 = eye - target;
            Vector3 xAxis1 = up * zAxis1;
            Vector3 yAxis1 = zAxis1 * xAxis1;

        //    Vector3 zAxis = Vector3.Normalize(zAxis1);
      //      Vector3 xAxis = Vector3.Normalize(yAxis1);
      //      Vector3 yAxis = Vector3.Normalize(xAxis1);

                   Vector3 zAxis = Vector3.Normalize(eye - target);
                   Vector3 xAxis = Vector3.Normalize(up * zAxis);
                   Vector3 yAxis = up;

            return new Matrix4x4(
              xAxis.X, xAxis.Y, xAxis.Z, -Vector3.Dot(xAxis, eye),
              yAxis.X, yAxis.Y, yAxis.Z, -Vector3.Dot(yAxis, eye),
              zAxis.X, zAxis.Y, zAxis.Z, -Vector3.Dot(zAxis, eye),
              0, 0, 0, 1
            );
        }

        private static Matrix4x4 CreateResultMatrix(uint width, uint height)
        {
         
            // для вращения используется порядок вращения Эйлера

            Matrix4x4 scale = Matrix4x4.CreateScale(_scale);
            // Matrix4x4 offset = Matrix4x4.CreateTranslation(width / 2, height / 2, 0);
            Matrix4x4 offset = Matrix4x4.CreateTranslation(new Vector3(width / 2,  height / 2, 10));
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_angleX));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_angleY));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_angleZ));

            Matrix4x4 camera = Matrix4x4.CreateLookAt(_cameraPos, new Vector3(), Vector3.UnitY);
            Matrix4x4 projection = Matrix4x4.CreatePerspective(width, height, 1f, 10000f);
            Matrix4x4 screen = Matrix4x4Factories.CreateScreen(1280, 720);

            Vector3 eye = new(0, 0, 0);
            Vector3 target = _modelPos;             //new Vector3(0, 0, 0);
            Vector3 up = new(0, 1000, 0);

            Matrix4x4 viewport1 = screen;
            Matrix4x4 projection1 = Matrix4x4.Transpose(projection);
            Matrix4x4 view1 = createViewMatrix(eye, target, up);
            //Matrix4x4 view1 = Matrix4x4.Transpose(Matrix4x4.CreateBillboard(target, eye, up, new Vector3(0,0,0)));
            Matrix4x4 model1 = scale * rotateZ * rotateY * rotateX * offset;

            // Matrix4x4 res = scale * rotateZ * rotateY * rotateX * offset * projection;
            Matrix4x4 res =
                viewport1 *
                projection1 *
                view1 * 
                model1;

            return res;
        }
    
        private static IEnumerable<Vector4> TransformVertices(IEnumerable<Vector4> vertices, Matrix4x4 matrix)
        {
               var a = vertices.Select(v => Vector4.Transform(v, matrix)).Select(v => v / v.W);
            //var a = vertices.Select(v => v / v.W);
            return a;

            var v1 = vertices.Select(v => Vector4.Transform(v, matrix));
            var v2 = v1.Select(v => v / v.W);
            var v3 = v2.Where(v => (v.X > 0 && v.X < 1) && (v.Y > 0 && v.Y < 1));
            
            return v3;
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
                case Keyboard.Key.Equal:
                {
                    _scale += scaleDelta;
                    break;
                }
                case Keyboard.Key.Hyphen:
                {
                    if (_scale > 1)
                    {
                        _scale -= scaleDelta;
                    }

                    break;
                }

                // Model movement
                case Keyboard.Key.Up:
                {
                    _modelPos.Z += movementDelta;
                    break;
                }
                case Keyboard.Key.Down:
                {
                    _modelPos.Z -= movementDelta;
                    break;
                }
                case Keyboard.Key.Left:
                {
                    _modelPos.X -= movementDelta;
                    break;
                }
                case Keyboard.Key.Right:
                {
                    _modelPos.X += movementDelta;
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
