using static SFML.Window.Keyboard;

namespace GraphicsLabSFML.Render.Window.Input
{
    public interface IInputHandler
    {
        void DispatchEvent(Key key);
    }
}
