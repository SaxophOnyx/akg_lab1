using SFML.Graphics;

namespace GraphicsLabSFML.Render
{
    public class SimpleText : Drawable
    {
        private readonly Text _text;

        public SimpleText()
        {
          _text = new Text();
        }

        public void Draw(RenderTarget target, RenderStates states) => _text.Draw(target, states);
    }
}
