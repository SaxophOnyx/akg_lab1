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


        public CustomWindow(uint width, uint height, bool vsync = true)
        {
            _pixelBuffer = new(width, height);
            _texture = new(width, height);
            _texture.Update(_pixelBuffer.Bytes);
            _sprite = new(_texture);

            _renderColor = Color.Green;

            _text = new();
            _text.Font = new("C:\\Windows\\Fonts\\Roboto-Light.ttf");
            _text.CharacterSize = 64;
            _text.FillColor = _renderColor;
            _text.DisplayedString = "Test";

            VideoMode videoMode = new(width, height);
            _window = new(videoMode, "АКГ - ЛР1", Styles.Close);
            _window.Closed += (object? _, EventArgs __) => _window.Close();
            _window.SetVerticalSyncEnabled(vsync);
        }


        public void DispatchEvents() => _window.DispatchEvents();

        public void RenderFrame(IEnumerable<Vector4> vertices)
        {
            ClearWindow();
            RenderModelCanvas(vertices);
            RenderGUI();

            _window.Display();
        }

        private void ClearWindow()
        {
            _window.Clear();
            _pixelBuffer.Clear();
        }

        private void RenderModelCanvas(IEnumerable<Vector4> vertices)
        {
            _pixelBuffer.Clear();
            int offsetPointsCount = 0;

            foreach (var v in vertices)
            {
                if (v.X > 0 && v.Y > 0)
                if (v.X > 0 && v.Y > 0 && v.X <_window.Size.X && v.Y < _window.Size.Y)
                {
                    uint x = (uint)v.X;
                    uint y = (uint)v.Y;

                    _pixelBuffer.SetPixel(x, y, _renderColor);
                }
                else
                {
                    ++offsetPointsCount;
                }
            }

            _texture.Update(_pixelBuffer.Bytes);
            _window.Draw(_sprite);

            Console.WriteLine($"Points outside of the screen: {offsetPointsCount}");
        }

        private void RenderGUI()
        {
            // _window.Draw(_text);
        }
    }
}
