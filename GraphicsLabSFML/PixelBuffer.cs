using SFML.Graphics;

namespace GraphicsLabSFML
{
    public class PixelBuffer
    {
        private const int BYTES_PER_PIXEL = 4;
        
        private readonly uint _width;
        private readonly uint _height;
        private readonly byte[] _bytes;
        private readonly byte[] _empty;


        public byte[] Bytes => _bytes;

        public uint Width => _width;

        public uint Height => _height;


        public PixelBuffer(uint width, uint height)
        {
            _width = width;
            _height = height;
            _bytes = new byte[width * height * BYTES_PER_PIXEL];
            _empty = new byte[width * height * BYTES_PER_PIXEL];
        }


        public void SetPixel(uint x, uint y, Color color)
        {
            uint i = (y * _width + x) * BYTES_PER_PIXEL;
            if (x < 0 || x > Width || y < 0 || y > Height) return;
            try
            {

                _bytes[i] = color.R;
                _bytes[i + 1] = color.G;
                _bytes[i + 2] = color.B;
                _bytes[i + 3] = color.A;
            }
            catch { }
        }

        public void Clear()
        {
            Array.Copy(_empty, 0, _bytes, 0, _bytes.Length);
        }
    }
}
