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

            VideoMode videoMode = new(width, height);
            _window = new(videoMode, "АКГ - ЛР1", Styles.Close);
            _window.Closed += (object? _, EventArgs __) => _window.Close();
            _window.SetVerticalSyncEnabled(vsync);
        }


        public void DispatchEvents() => _window.DispatchEvents();

        public void RenderFrame(IEnumerable<Vector4> vertices)
        {
            _window.Clear();
            _pixelBuffer.Clear();

            foreach (var v in vertices)
            {
                if (v.X > 0 && v.Y > 0)
                {
                    uint x = (uint)v.X;
                    uint y = (uint)v.Y;

                    _pixelBuffer.SetPixel(x, y, _renderColor);
                }
                else
                {
                    // Console.WriteLine("Outside of screen");
                }
            }

            _texture.Update(_pixelBuffer.Bytes);
            _window.Draw(_sprite);
            _window.Display();
        }
    }
}
