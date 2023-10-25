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


        public uint Width => _window.Size.X;

        public uint Height => _window.Size.Y;

        public bool IsOpen => _window.IsOpen;


        public CustomWindow(RenderData viewModel, CustomWindowOptions options, IInputHandler inputHandler)
        {
            _data = viewModel;
            _options = options;
            _inputHandler = inputHandler;

            _canvas = new(_options.Width, _options.Height);

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
        }

        private void RenderModelCanvas()
        {
            Vector4[] v = _data.Vertices;
            int[] flatFaces = _data.FlatFaces;
            int vPerFace = _data.VerticesPerFace;

            _canvas.ClearPixels();

            Parallel.For(0, (flatFaces.Length - 1) / vPerFace, (i) =>
            {
                int off = i * vPerFace;

                for (int j = 0; j < vPerFace - 1; ++j)
                {
                    DrawLineDDA(v[flatFaces[off + j]], v[flatFaces[off + j + 1]]);
                }
                
                DrawLineDDA(v[flatFaces[off + vPerFace - 1]], v[flatFaces[off + 0]]);
            });

            _window.Draw(_canvas);
        }

        private void DrawLineDDA(Vector4 a, Vector4 b)
        {
            const int l = -1;
            const int h = 1;

            if (a.Z < l || a.Z > h || b.Z < l || b.Z > h)
            {
                return;
            }

            if (a.X <= 5 || a.X >= _canvas.Width - 5 || a.Y <= 5 || a.Y >= _canvas.Height - 5)
            {
                return;
            }

            if (b.X <= 5 || b.X >= _canvas.Width - 5 || b.Y <= 5 || b.Y >= _canvas.Height - 5)
            {
                return;
            }

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
