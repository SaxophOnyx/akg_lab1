using SFML.Graphics;
using SFML.System;

namespace GraphicsLabSFML.Render.Components
{
    public abstract class ValueDisplay<T> : Drawable where T : struct
    {
        private readonly Text _text = new();
        private string _label = string.Empty;
        private T _value;


        public T Value
        {
            get => _value;
            set
            {
                if (!_value.Equals(value))
                {
                    _value = value;
                    _text.DisplayedString = BuildDisplayString();
                }
            }
        }

        public string Label
        {
            get => _label;
            set
            {
                _label = value;
                _text.DisplayedString = BuildDisplayString();
            }
        }

        public Vector2f Position
        {
            get => _text.Position;
            set => _text.Position = value;
        }

        public Font Font
        {
            get => _text.Font;
            set => _text.Font = value;
        }

        public uint CharacterSize
        {
            get => _text.CharacterSize;
            set => _text.CharacterSize = value;
        }

        public Color TextColor
        {
            get => _text.FillColor;
            set => _text.FillColor = value;
        }

        public float Height => _text.GetLocalBounds().Height;

        public float Width => _text.GetLocalBounds().Width;


        public void Draw(RenderTarget target, RenderStates states) => _text.Draw(target, states);

        protected abstract string StringifyValue(T value);

        private string BuildDisplayString()
        {
            if (_label == string.Empty)
                return StringifyValue(_value);

            return $"{_label}: {StringifyValue(_value)}";
        }
    }
}
