using GraphicsLabSFML.Models;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Numerics;

using Color = SFML.Graphics.Color;

namespace GraphicsLabSFML.Parsing
{
    public static class MapWriter
    {
        public static void WriteToFile(Map<Vector3> map, string emptyPath, string destPath)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(emptyPath))
            {
                for (int x = 0; x < map.Width; ++x)
                {
                    for (int y = 0; y < map.Height; ++y)
                    {
                        Color color = map[x, y].ToClampedColor();
                        Rgba32 pixel = new Rgba32(color.R, color.G, color.B);
                        image[x, y] = pixel;
                    }
                }

                image.SaveAsJpeg(destPath);
            }
        }
    }
}
