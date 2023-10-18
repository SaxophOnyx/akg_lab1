using GraphicsLabSFML.Models;
using SFML.Graphics;
using SFML.Window;
using System.Numerics;

namespace GraphicsLabSFML
{
    public class CustomWindow
    {
        private readonly RenderWindow _window;
        private readonly PixelBuffer _pixelBuffer;
        private readonly Texture _texture;
        private readonly Sprite _sprite;
        private readonly Color _renderColor;

        private Text _text;


        public uint Width => _window.Size.X;

        public uint Height => _window.Size.Y;

        public bool IsOpen => _window.IsOpen;

        public event EventHandler<KeyEventArgs> KeyPressed
        {
            add => _window.KeyPressed += value;
            remove => _window.KeyPressed -= value;
        }


        public CustomWindow(int width, int height, bool vsync = true)
        {
            uint uWidth = (uint)width;
            uint uHeight = (uint)height;

            _pixelBuffer = new(width, height);
            _texture = new(uWidth, uHeight);
            _texture.Update(_pixelBuffer.Bytes);
            _sprite = new(_texture);

            _renderColor = Color.Green;

            _text = new();
            _text.Font = new("C:\\Windows\\Fonts\\Roboto-Light.ttf");
            _text.CharacterSize = 64;
            _text.FillColor = _renderColor;
            _text.DisplayedString = "Test";

            VideoMode videoMode = new(uWidth, uHeight);
            _window = new(videoMode, "АКГ - ЛР1", Styles.Close);
            _window.Closed += (object? _, EventArgs __) => _window.Close();
            _window.SetVerticalSyncEnabled(vsync);
        }


        public void DispatchEvents() => _window.DispatchEvents();

        public void RenderFrame(Vector4[] vertices, int[] flatFaces)
        {
            ClearWindow();
            RenderModelCanvas(vertices, flatFaces);
            RenderGUI();

            _window.Display();
        }

        private void ClearWindow()
        {
            _window.Clear();
            _pixelBuffer.Clear();
        }

        private void RenderModelCanvas(Vector4[] v, int[] flatFaces)
        {
            _pixelBuffer.Clear();

            /*
            Parallel.For(0, v.Length, (i) =>
            {
                // DrawFaceOld(v, faces[i]);
                DrawPoint(v[i]);
            });
            */

            const int step = 4;

            for (int i = 0; i < (flatFaces.Length - 1) / step; ++i)
            {
                int off = i * step;
                DrawLineDDA(v[flatFaces[off + 0]], v[flatFaces[off + 1]]);
                DrawLineDDA(v[flatFaces[off + 1]], v[flatFaces[off + 2]]);
                DrawLineDDA(v[flatFaces[off + 2]], v[flatFaces[off + 3]]);
                DrawLineDDA(v[flatFaces[off + 3]], v[flatFaces[off + 0]]);
            }

            _texture.Update(_pixelBuffer.Bytes);
            _window.Draw(_sprite);
        }



        private void DrawFaceOld(Vector4[] vertices, FaceInfo face)
        {
            for (int i = 1; i < face.VerticesIndices.Length; ++i)
            {
                Vector4 a = vertices[face.VerticesIndices[i - 1]];
                Vector4 b = vertices[face.VerticesIndices[i]];

                DrawLineDDA(a.X, a.Y, b.X, b.Y);
            }

            Vector4 c = vertices[face.VerticesIndices[0]];
            Vector4 d = vertices[face.VerticesIndices[1]];

            DrawLineDDA(c.X, c.Y, d.X, d.Y);
        }

        private void DrawPoint(Vector4 point)
        {
            int x = (int)point.X;
            int y = (int)point.Y;

            if ((x >= 0) && (x < _pixelBuffer.Width) && (y >= 0) && (y < _pixelBuffer.Height) && (point.Z > -1) && (point.Z < 1))
            {
                _pixelBuffer.SetPixel(x, y, _renderColor);
            }
        }

        private void DrawLineDDA(Vector4 a, Vector4 b)
        {
            float dx = b.X - a.X;
            float dy = b.Y - a.Y;

            int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xIncrement = dx / steps;
            float yIncrement = dy / steps;

            float floatX = a.X;
            float floatY = b.Y;

            for (int i = 0; i < steps; ++i)
            {
                int x = (int)floatX;
                int y = (int)floatY;

                if ((x >= 0) && (x < _pixelBuffer.Width) && (y >= 0) && (y < _pixelBuffer.Height))
                {
                    _pixelBuffer.SetPixel(x, y, _renderColor);
                }

                floatX += xIncrement;
                floatY += yIncrement;
            }
        }

        private void DrawLineDDA(float x1, float y1, float x2, float y2)
        {
            float dx = x2 - x1;
            float dy = y2 - y1;

            int steps = (int)Math.Max(Math.Abs(dx), Math.Abs(dy));

            float xIncrement = dx / steps;
            float yIncrement = dy / steps;

            float floatX = x1;
            float floatY = y2;

            for (int i = 0; i < steps; ++i)
            {
                int x = (int)floatX;
                int y = (int)floatY;

                if ((x >= 0) && (x < _pixelBuffer.Width) && (y >= 0) && (y < _pixelBuffer.Height))
                {
                    _pixelBuffer.SetPixel(x, y, _renderColor);
                }

                floatX += xIncrement;
                floatY += yIncrement;
            }
        }

        private void RenderGUI()
        {
            // _window.Draw(_text);
        }
    }
}
