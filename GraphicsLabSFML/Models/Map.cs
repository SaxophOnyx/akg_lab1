using System.Runtime.CompilerServices;

namespace GraphicsLabSFML.Models
{
    public class Map<T>
    {
        private readonly T[,] _data;

        public int Width => _data.GetLength(0);

        public int Height => _data.GetLength(1);

        public T this[int x, int y]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _data[x, y];

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _data[x, y] = value;
        }


        public Map(T[,] data)
        {
            _data = data;
        }

        public Map<T> Copy()
        {
            T[,] data = (T[,])_data.Clone();
            return new Map<T>(data);
        }

        public static Map<T> Empty()
        {
            return new Map<T>(new T[0, 0]);
        }
    }
}
