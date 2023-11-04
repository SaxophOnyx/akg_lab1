using GraphicsLabSFML.Render.Components;
using GraphicsLabSFML.Render.Window.Input;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System.Numerics;

namespace GraphicsLabSFML.Render.Window
{
    public class CustomWindow
    {
        private readonly RenderData _data;
        private readonly CustomWindowOptions _options;
        private readonly IInputHandler _inputHandler;

        private Vector3Display _cameraPosDisplay;
        private Vector3Display _modelRotationDisplay;
        private Vector3Display _cameraTargetDisplay;
        private FloatDisplay _modelScaleDisplay;

        private readonly RenderWindow _window;
        private readonly Canvas _canvas;
        private readonly ZBuffer _zBuffer;


        public uint Width => _window.Size.X;

        public uint Height => _window.Size.Y;

        public bool IsOpen => _window.IsOpen;


        public CustomWindow(RenderData viewModel, CustomWindowOptions options, IInputHandler inputHandler)
        {
            _data = viewModel;
            _options = options;
            _inputHandler = inputHandler;

            _canvas = new(_options.Width, _options.Height);
            _zBuffer = new(_options.Width, _options.Height);

            InitGUI();

            VideoMode videoMode = new((uint)_options.Width, (uint)_options.Height);
            _window = new(videoMode, _options.Name, Styles.Close);
            _window.Closed += (_, __) => _window.Close();
            _window.KeyPressed += (object? sender, KeyEventArgs e) => _inputHandler.DispatchEvent(e.Code);
            _window.SetVerticalSyncEnabled(_options.VSync);
        }


        public void DispatchEvents() => _window.DispatchEvents();

        public void Render()
        {
            ClearPrevious();
            RenderModelCanvas();
            RenderGUI();

            _window.Display();
        }

        private void ClearPrevious()
        {
            _window.Clear();
            _canvas.ClearPixels();
            _zBuffer.Clear();
        }

        private void RenderModelCanvas()
        {
            CVector4[] v = _data.Vertices;
            int[] ff = _data.FlatFaces;

            _canvas.ClearPixels();

            for (int i = 0; i < (ff.Length - 1) / 3; ++i)
            {
                int j = i * 3;
                CVector4 a = v[ff[j]];
                CVector4 b = v[ff[j + 1]];
                CVector4 c = v[ff[j + 2]];

                if (a.IsVisible && b.IsVisible && c.IsVisible)
                {
                    RenderFaceBarocentric(a.Value, b.Value, c.Value);
                }
            }

            _window.Draw(_canvas);
        }

        private void RenderFace(Vector4 a, Vector4 b, Vector4 c)
        {
            DrawLineDDA(a, b);
            DrawLineDDA(b, c);
            DrawLineDDA(c, a);
        }

        private void RenderFaceBarocentric(Vector4 a, Vector4 b, Vector4 c)
        {
            Vector2i tl; // top left
            Vector2i br; // bottom right

            tl.X = (int)Math.Ceiling(Utils.Min(a.X, b.X, c.X));
            tl.Y = (int)Math.Ceiling(Utils.Min(a.Y, b.Y, c.Y));

            br.X = (int)Math.Ceiling(Utils.Max(a.X, b.X, c.X));
            br.Y = (int)Math.Ceiling(Utils.Max(a.Y, b.Y, c.Y));

            for (int i = tl.X; i < br.X; ++i)
            {
                for (int j = tl.Y; j < br.Y; ++j)
                {
                    if (IsInsideTriangle(a, b, c, i, j))
                    {
                        float z = GetBarocentricZ(a, b, c, i, j);
                        float currZ = _zBuffer[i, j];

                        if (z < currZ)
                        {
                            _zBuffer[i, j] = z;
                            _canvas.SetPixel(i, j, _options.RenderColor);
                        }
                    }
                }
            }
        }

        private static bool IsInsideTriangle(Vector4 a, Vector4 b, Vector4 c, int x, int y)
        {
            Vector4 p = new(x, y, 0, 0);

            float d1 = Sign(p, a, b);
            float d2 = Sign(p, b, c);
            float d3 = Sign(p, c, a);

            bool hasNeg = (d1 < 0) || (d2 < 0) || (d3 < 0);
            bool hasPos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(hasNeg && hasPos);
        }

        private static float GetBarocentricZ(Vector4 a, Vector4 b, Vector4 c, int x, int y)
        {
            float denominator = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);
            float coordinate1 = ((b.Y - c.Y) * (x - c.X) + (c.X - b.X) * (y - c.Y)) / denominator;
            float coordinate2 = ((c.Y - a.Y) * (x - c.X) + (a.X - c.X) * (y - c.Y)) / denominator;

            return 1 - coordinate1 - coordinate2;
        }

        private static float Sign(Vector4 p1, Vector4 p2, Vector4 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
        }

        private void DrawLineDDA(Vector4 a, Vector4 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;

            int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));

            float floatX = a.X;
            float floatY = a.Y;

            float xIncrement = dx / steps;
            float yIncrement = dy / steps;

            for (int i = 0; i < steps; ++i)
            {
                int x = (int)Math.Truncate(floatX);
                int y = (int)Math.Truncate(floatY);

                _canvas.SetPixel(x, y, _options.RenderColor);

                floatX += xIncrement;
                floatY += yIncrement;
            }
        }

        private void RenderGUI()
        {
            _cameraPosDisplay.Value = _data.CameraPos;
            _window.Draw(_cameraPosDisplay);

            _cameraTargetDisplay.Value = _data.CameraTarget;
            _window.Draw(_cameraTargetDisplay);

            _modelRotationDisplay.Value = _data.ModelRotation;
            _window.Draw(_modelRotationDisplay);

            _modelScaleDisplay.Value = _data.Scale;
            _window.Draw(_modelScaleDisplay);
        }

        private void InitGUI()
        {
            Font font = new("C:\\Windows\\Fonts\\Roboto-Light.ttf");
            uint charSize = 24;
            float indent = 12;

            Vector2f position = new(10, 10);
            _cameraPosDisplay = new Vector3Display()
            {
                Position = position,
                Font = font,
                CharacterSize = charSize,
                TextColor = _options.RenderColor,
                Label = "Camera Pos",
                Value = _data.CameraPos
            };

            position.Y += _cameraPosDisplay.Height + indent;
            _cameraTargetDisplay = new Vector3Display()
            {
                Position = position,
                Font = font,
                CharacterSize = charSize,
                TextColor = _options.RenderColor,
                Label = "Camera Target",
                Value = _data.CameraTarget
            };

            position.Y += _cameraTargetDisplay.Height + indent;
            _modelRotationDisplay = new Vector3Display()
            {
                Position = position,
                Font = font,
                CharacterSize = charSize,
                TextColor = _options.RenderColor,
                Label = "Model Rotation",
                Value = _data.ModelRotation
            };

            position.Y += _modelRotationDisplay.Height + indent;
            _modelScaleDisplay = new FloatDisplay()
            {
                Position = position,
                Font = font,
                CharacterSize = charSize,
                TextColor = _options.RenderColor,
                Label = "Model Scale",
                Value = _data.Scale
            };
        }
    }
}
