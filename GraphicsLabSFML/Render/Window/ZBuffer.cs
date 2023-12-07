using System.Runtime.CompilerServices;

namespace GraphicsLabSFML.Render.Window
{
    public class ZBuffer
    {
        private readonly float[,] _buffer;
        private readonly float[,] _empty;

        public int Width { get; }

        public int Height { get; }

        public float this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer[x, y];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _buffer[x, y] = value;
        }


        public ZBuffer(int width, int height)
        {
            Width = width;
            Height = height;

            _buffer = new float[Width, Height];
            _empty = new float[Width, Height];

            for (int i = 0; i < width; ++i)
            {
                for (int j = 0; j < height; ++j)
                {
                    _empty[i, j] = float.PositiveInfinity;
                    _buffer[i, j] = float.PositiveInfinity;
                }
            }
        }


        public void Clear()
        {
            Array.Copy(_empty, 0, _buffer, 0, _buffer.Length);
        }
    }
}
