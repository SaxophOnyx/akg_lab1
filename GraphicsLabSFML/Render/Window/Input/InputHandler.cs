using System.Numerics;

using static SFML.Window.Keyboard;

namespace GraphicsLabSFML.Render.Window.Input
{
    public class InputHandler : IInputHandler
    {
        public event Action<Vector3>? OnCameraMoved;
        public event Action<Vector3>? OnLightSourceWorldPosChanged;
        public event Action<Vector3>? OnModelRotated;
        public event Action<float>? OnModelScaled;


        public void DispatchEvent(Key key)
        {
            switch (key)
            {
                // Model rotation
                case Key.W:
                {
                    OnModelRotated?.Invoke(Vector3.UnitY);
                    break;
                }
                case Key.S:
                {
                    OnModelRotated?.Invoke(-Vector3.UnitY);
                    break;
                }
                case Key.A:
                {
                    OnModelRotated?.Invoke(-Vector3.UnitX);
                    break;
                }
                case Key.D:
                {
                    OnModelRotated?.Invoke(Vector3.UnitX);
                    break;
                }
                case Key.E:
                {
                    OnModelRotated?.Invoke(Vector3.UnitZ);
                    break;
                }
                case Key.Q:
                {
                    OnModelRotated?.Invoke(-Vector3.UnitZ);
                    break;
                }

                // Model scale
                case Key.Equal:
                {
                    OnModelScaled?.Invoke(1.5f);
                    break;
                }
                case Key.Hyphen:
                {
                    OnModelScaled?.Invoke(1 / 1.5f);
                    break;
                }

                // Camera movement
                case Key.Up:
                {
                    OnCameraMoved?.Invoke(Vector3.UnitY);
                    break;
                }
                case Key.Down:
                {
                    OnCameraMoved?.Invoke(-Vector3.UnitY);
                    break;
                }
                case Key.Left:
                {
                    OnCameraMoved?.Invoke(Vector3.UnitX);
                    break;
                }
                case Key.Right:
                {
                    OnCameraMoved?.Invoke(-Vector3.UnitX);
                    break;
                }
                case Key.Space:
                {
                    OnCameraMoved?.Invoke(Vector3.UnitZ);
                    break;
                }
                case Key.Z:
                {
                    OnCameraMoved?.Invoke(-Vector3.UnitZ);
                    break;
                }

                // Lighg direction changed
                case Key.Numpad7:
                {
                    OnLightSourceWorldPosChanged?.Invoke(-Vector3.UnitX);
                    break;
                }
                case Key.Numpad9:
                {
                    OnLightSourceWorldPosChanged?.Invoke(Vector3.UnitX);
                    break;
                }
                case Key.Numpad4:
                {
                    OnLightSourceWorldPosChanged?.Invoke(-Vector3.UnitY);
                    break;
                }
                case Key.Numpad6:
                {
                    OnLightSourceWorldPosChanged?.Invoke(Vector3.UnitY);
                    break;
                }
                case Key.Numpad1:
                {
                    OnLightSourceWorldPosChanged?.Invoke(-Vector3.UnitZ);
                    break;
                }
                case Key.Numpad3:
                {
                    OnLightSourceWorldPosChanged?.Invoke(Vector3.UnitZ);
                    break;
                }
            }
        }
    }
}
