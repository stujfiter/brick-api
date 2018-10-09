using System;
using SixLabors.Primitives;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace Brick.Images.ImageSharp {
    public class ImageSharpProcessor : ImageProcessor
    {
        public string ResizeImage(string image64, int maxWidth, int maxHeight)
        {
            Configuration.Default.ImageFormatsManager.SetEncoder(ImageFormats.Jpeg, new JpegEncoder() 
            {
                Quality = 50
            });

            string imageData64 = image64.Replace("data:image/jpeg;base64,", "");
            byte[] imageBytes = Convert.FromBase64String(imageData64);

            using (Image<Rgba32> image = Image.Load(imageBytes))
            {
                Rectangle bounds = image.Bounds();
                float scale = 1;
                float ratio = (float)bounds.Width / (float)bounds.Height;

                if (ratio > 1 && bounds.Width > maxWidth)
                {
                    scale = maxWidth / bounds.Width;
                    int newHeight = (int) (bounds.Height * scale);
                    image.Mutate(i => i.Resize(maxWidth, newHeight));
                }

                if (ratio < 1 && bounds.Height > maxHeight)
                {
                    scale = maxHeight / bounds.Height;
                    int newWidth = (int) (bounds.Width * scale);
                    image.Mutate(i => i.Resize(newWidth, maxHeight));
                }

                return image.ToBase64String(ImageFormats.Jpeg);
            }


        }
    }
}