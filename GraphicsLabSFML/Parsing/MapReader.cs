using GraphicsLabSFML.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace GraphicsLabSFML.Parsing
{
    public class MapReader
    {
        public static Map<Vector3> ReadDiffuse(string imagePath)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                Vector3[,] data = new Vector3[image.Width, image.Height];
                Rgba32[] buffer = new Rgba32[image.Width * image.Height];
                image.CopyPixelDataTo(buffer);

                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        Rgba32 pixel = buffer[x + y * image.Height];

                        float _x = pixel.R * 1.0f / 255;
                        float _y = pixel.G * 1.0f / 255;
                        float z = pixel.B * 1.0f / 255;

                        data[x, y] = new Vector3(_x, _y, z);
                    }
                }

                return new Map<Vector3>(data);
            }
        }

        public static Map<Vector3> ReadNormal(string imagePath)
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                Vector3[,] data = new Vector3[image.Width, image.Height];
                Rgba32[] buffer = new Rgba32[image.Width * image.Height];
                image.CopyPixelDataTo(buffer);

                for (int y = 0; y < image.Height; ++y)
                {
                    for (int x = 0; x < image.Width; ++x)
                    {
                        Rgba32 pixel = buffer[x + y * image.Height];

                        float _x = pixel.R * 1.0f / 255;
                        float _y = pixel.G * 1.0f / 255;
                        float z = pixel.B * 1.0f / 255;

                        Vector3 normal = new Vector3(_x, _y, z);
                        Vector3 transformed = normal * 2 - Vector3.One;

                        data[x, y] = transformed;
                    }
                }

                return new Map<Vector3>(data);
            }
        }
    }
}
