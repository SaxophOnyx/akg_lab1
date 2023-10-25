using SFML.Graphics;

namespace GraphicsLabSFML.Render.Components
{
    public class Canvas : Drawable
    {
        private const int BYTES_PER_PIXEL = 4;

        private readonly int _width;
        private readonly int _height;
        private readonly byte[] _bytes;
        private readonly byte[] _empty;
        private readonly Texture _texture;
        private readonly Sprite _sprite;


        public int Width => _width;

        public int Height => _height;


        public Canvas(int width, int height)
        {
            _width = width;
            _height = height;
            _bytes = new byte[width * height * BYTES_PER_PIXEL];
            _empty = new byte[width * height * BYTES_PER_PIXEL];

            uint uWidth = (uint)_width;
            uint uHeight = (uint)_height;

            _texture = new(uWidth, uHeight);
            _texture.Update(_bytes);
            _sprite = new(_texture);
        }

        public void SetPixel(int x, int y, Color color)
        {
            int i = (y * _width + x) * BYTES_PER_PIXEL;

            _bytes[i] = color.R;
            _bytes[i + 1] = color.G;
            _bytes[i + 2] = color.B;
            _bytes[i + 3] = color.A;
        }

        public void ClearPixels() => Array.Copy(_empty, 0, _bytes, 0, _bytes.Length);

        public void Draw(RenderTarget target, RenderStates states)
        {
            _texture.Update(_bytes);
            _sprite.Draw(target, states);
        }
    }
}
