﻿using GraphicsLabSFML.Models;
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
        private static Vector3 _cameraPos = new(-250, -250, -1250);
        private static Vector3 _modelPos = new(width / 2, height / 2, 0);

        private static float _scale = 1;

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

                window.RenderFrame(transformed);

                stopwatch.Stop();
                Console.WriteLine($"Elapsed = ${stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static Matrix4x4 CreateResultMatrix(uint width, uint height)
        {
            // для вращения используется порядок вращения Эйлера

            Matrix4x4 scale = Matrix4x4.CreateScale(_scale);
            // Matrix4x4 offset = Matrix4x4.CreateTranslation(width / 2, height / 2, 0);
            Matrix4x4 offset = Matrix4x4.CreateTranslation(_modelPos);
            Matrix4x4 rotateX = Matrix4x4.CreateRotationX(Utils.DegreesToRadians(_angleX));
            Matrix4x4 rotateY = Matrix4x4.CreateRotationY(Utils.DegreesToRadians(_angleY));
            Matrix4x4 rotateZ = Matrix4x4.CreateRotationZ(Utils.DegreesToRadians(_angleZ));

            Matrix4x4 camera = Matrix4x4.CreateLookAt(_cameraPos, new Vector3(), Vector3.UnitY);
            Matrix4x4 projection = Matrix4x4.CreatePerspective(width, height, 0.1f, 1000.0f);
            Matrix4x4 screen = Matrix4x4Factories.CreateScreen(1280, 720);

            // Matrix4x4 res = scale * rotateZ * rotateY * rotateX * offset * projection;
            // Matrix4x4 res = screen * projection * scale * rotateZ * rotateY * rotateX * offset;
            Matrix4x4 res = scale;

            return res;
        }
    
        private static IEnumerable<Vector4> TransformVertices(IEnumerable<Vector4> vertices, Matrix4x4 matrix)
        {
            var a = vertices.Select(v => Vector4.Transform(v, matrix)).Select(v => v / v.W);
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
