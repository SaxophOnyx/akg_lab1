using SFML.Graphics;

namespace GraphicsLabSFML.Render.Window
{
    public struct CustomWindowOptions
    {
        private static CustomWindowOptions _default = new(1280, 720, "Window", Color.Green, Color.Red, true);

        public static CustomWindowOptions Default => _default;

        public int Width;

        public int Height;

        public string Name;

        public bool VSync;

        public Color RenderColor;

        public Color TechRenderColor;


        public CustomWindowOptions(int width, int height, string name, Color renderColor, Color techRenderColor, bool vsync)
        {
            Width = width;
            Height = height;
            Name = name;
            RenderColor = renderColor;
            TechRenderColor = techRenderColor;
            VSync = vsync;
        }
    }
}
