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
                    Color color = i % 2 == 0 ? _options.RenderColor : Color.Blue;
                    RenderFaceBarocentric(a.Value, b.Value, c.Value, color);
                }
            }

            _window.Draw(_canvas);
        }

        private void RenderFaceBarocentric(Vector4 a, Vector4 b, Vector4 c, Color color)
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
                    Vector3 bar = GetBarocentric(a, b, c, i, j);
                    if (IsInsideTriangleBarocentric(bar))
                    {
                        float z = (a.Z * bar.X) + (b.Z * bar.Y) + (c.Z * bar.Z);
                        if (z < _zBuffer[i, j])
                        {
                            _zBuffer[i, j] = z;
                            _canvas.SetPixel(i, j, color);
                        }
                    }
                }
            }
        }

        private static bool IsInsideTriangleBarocentric(Vector3 a)
        {
            bool x = (a.X >= 0) && (a.X <= 1);
            bool y = (a.Y >= 0) && (a.Y <= 1);
            bool z = (a.Z >= 0) && (a.Z <= 1);

            return x && y && z;
        }

        public Vector3 GetBarocentric(Vector4 a, Vector4 b, Vector4 c, int x, int y)
        {
            float denom = (b.Y - c.Y) * (a.X - c.X) + (c.X - b.X) * (a.Y - c.Y);

            float bx = ((b.Y - c.Y) * (x - c.X) + (c.X - b.X) * (y - c.Y)) / denom;
            float by = ((c.Y - a.Y) * (x - c.X) + (a.X - c.X) * (y - c.Y)) / denom;
            float bz = 1 - bx - by;

            return new(bx, by, bz);
        }

        private static float Sign(Vector4 p1, Vector4 p2, Vector4 p3)
        {
            return (p1.X - p3.X) * (p2.Y - p3.Y) - (p2.X - p3.X) * (p1.Y - p3.Y);
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
