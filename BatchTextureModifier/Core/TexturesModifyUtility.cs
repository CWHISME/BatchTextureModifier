using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Linq;
using System.Windows;

namespace BatchTextureModifier
{
    public static class TexturesModifyUtility
    {

        public readonly static string[] Filter = new string[] { "*.png", "*.jpg", "*.webp", "*.tga", "*.bmp", "*.gif" };
        public readonly static IImageEncoder[] _encoder = new IImageEncoder[] {
            new JpegEncoder(),
            new PngEncoder(),
            new WebpEncoder(),
            new TgaEncoder(),
            new BmpEncoder(),
            new GifEncoder()
        };

        /// <summary>
        /// 获取目录下第一张图片
        /// </summary>
        /// <param name="dirPath"></param>
        /// <returns></returns>
        public static string GetDirectoryFirstTextures(string dirPath)
        {
            try
            {
                EnumerationOptions opt = new EnumerationOptions();
                opt.RecurseSubdirectories = true;
                foreach (var pattern in Filter)
                {
                    foreach (var item in Directory.EnumerateFiles(dirPath, pattern, opt))
                    {
                        return item;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// 通过下标取格式
        /// </summary>
        /// <returns></returns>
        public static IImageEncoder? GetFormatByIndex(int index)
        {
            if (index < 0 || index >= _encoder.Length) return null;
            return _encoder[index];
        }

        public static void ResizeTextures(string path, TexturesModifyData data)
        {
            using (Image image = Image.Load(path))
            {
                image.Mutate(x => x.Resize(data.Width, data.Height));
                image.Save(Path.Combine(Path.GetDirectoryName(path), "[Resize]" + Path.GetFileName(path)));
            }
        }

        public static byte[] ResizeTextures(byte[] bytes, TexturesModifyData data)
        {
            using (Image image = Image.Load(bytes))
            {
                image.Mutate(x => x.Resize(data.Width, data.Height));
                using (MemoryStream ms = new MemoryStream())
                {
                    image.Save(ms, Image.DetectFormat(bytes));
                    return ms.ToArray();
                }
                //image.Save(Path.Combine(Path.GetDirectoryName(path), "[Resize]" + Path.GetFileName(path)));
            }
        }
    }
}