namespace GraphicsLabSFML.Render.Components
{
    public class FloatDisplay : ValueDisplay<float>
    {
        protected override string StringifyValue(float value)
        {
            return value.ToString();
        }
    }
}
