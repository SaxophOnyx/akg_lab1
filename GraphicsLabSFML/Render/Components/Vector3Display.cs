using System.Numerics;

namespace GraphicsLabSFML.Render.Components
{
    public class Vector3Display : ValueDisplay<Vector3>
    {
        protected override string StringifyValue(Vector3 value)
        {
            return $"{value.X}, {value.Y}, {value.Z}";
        }
    }
}
