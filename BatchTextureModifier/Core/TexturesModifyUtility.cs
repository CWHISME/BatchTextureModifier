using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace BatchTextureModifier
{
    public static class TexturesModifyUtility
    {

        /// <summary>
        /// ��ʽ
        /// </summary>
        public readonly static string[] Filter = new string[] { "*.png", "*.jpg", "*.webp", "*.tga", "*.bmp", "*.gif" };
        /// <summary>
        /// ������
        /// </summary>
        public readonly static IImageEncoder[] _encoder = new IImageEncoder[] {
            new JpegEncoder(),
            new PngEncoder(),
            new WebpEncoder(),
            new TgaEncoder(),
            new BmpEncoder(),
            new GifEncoder()
        };
        /// <summary>
        /// �����㷨
        /// </summary>
        public readonly static List<IResampler> ResamplerAlgorithms;
        /// <summary>
        /// �����㷨��Ӧ������
        /// </summary>
        public readonly static List<string> ResamplerAlgorithmNames;
        //public readonly static IResampler[] ResamplerAlgorithms = new IResampler[] {
        //    KnownResamplers.Bicubic,
        //    KnownResamplers.Box,
        //    KnownResamplers.CatmullRom,
        //    KnownResamplers.Hermite,
        //    KnownResamplers.Lanczos2,
        //    KnownResamplers.Lanczos3,
        //    KnownResamplers.Lanczos5,
        //    KnownResamplers.Lanczos8,
        //    KnownResamplers.MitchellNetravali,
        //    KnownResamplers.NearestNeighbor,
        //    KnownResamplers.Robidoux,
        //    KnownResamplers.RobidouxSharp,
        //    KnownResamplers.Spline,
        //    KnownResamplers.Triangle,
        //    KnownResamplers.Welch,
        //};

        static TexturesModifyUtility()
        {
            PropertyInfo[] prt = typeof(SixLabors.ImageSharp.Processing.KnownResamplers).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
            ResamplerAlgorithms = new List<IResampler>(prt.Length);
            ResamplerAlgorithmNames = new List<string>(prt.Length);
            foreach (var item in prt)
            {
                IResampler resampler = item.GetValue(null) as IResampler;
                if (resampler == null) continue;
                ResamplerAlgorithms.Add(resampler);
                ResamplerAlgorithmNames.Add(item.Name);
            }
        }

        /// <summary>
        /// ��ȡĿ¼�µ�һ��ͼƬ
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
        /// ͨ���±�ȡ��ʽ
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
            if (data.ScaleMode == EScaleMode.NotScale && data.OutputFormat == null) return bytes;
            using (Image image = Image.Load(bytes))
            {
                ResizeOptions opt;
                switch (data.ScaleMode)
                {
                    case EScaleMode.NotScale:
                        break;
                    case EScaleMode.DirectScale_Min:
                        ResizeByMode(data, image, SixLabors.ImageSharp.Processing.ResizeMode.Min);
                        break;
                    case EScaleMode.DirectScale_Max:
                        ResizeByMode(data, image, SixLabors.ImageSharp.Processing.ResizeMode.Max);
                        break;
                    case EScaleMode.StretchScale:
                        ResizeByMode(data, image, SixLabors.ImageSharp.Processing.ResizeMode.Stretch);
                        break;
                    case EScaleMode.ScaleBased:
                        ResizeByScaleBased(data, image);
                        break;
                    case EScaleMode.DirectCut:
                        image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), false));
                        break;
                    case EScaleMode.ScaleBasedByCut:
                        ResizeByMode(data, image, SixLabors.ImageSharp.Processing.ResizeMode.Crop);
                        break;
                    case EScaleMode.WidthBase:

                        break;
                    case EScaleMode.HeightBase:

                        break;
                    case EScaleMode.Fill:
                        ResizeByMode(data, image, SixLabors.ImageSharp.Processing.ResizeMode.BoxPad);
                        break;
                    case EScaleMode.POT:
                        break;
                    case EScaleMode.Max:
                        break;
                    default:
                        break;
                }
                using (MemoryStream ms = new MemoryStream())
                {
                    //�����ԭ��ʽ����ת����ʽ
                    if (data.OutputFormat == null)
                        image.Save(ms, Image.DetectFormat(bytes));
                    else
                        image.Save(ms, data.OutputFormat);
                    return ms.ToArray();
                }
                //image.Save(Path.Combine(Path.GetDirectoryName(path), "[Resize]" + Path.GetFileName(path)));
            }
        }

        private static void ResizeByMode(TexturesModifyData data, Image image, SixLabors.ImageSharp.Processing.ResizeMode mode)
        {
            ResizeOptions opt = new ResizeOptions();
            opt.Position = AnchorPositionMode.Center;
            opt.Sampler = data.ResamplerAlgorithm;
            opt.Size = new SixLabors.ImageSharp.Size(data.Width, data.Height);
            opt.Mode = mode;
            image.Mutate(x => x.Resize(opt));
        }

        private static void ResizeByScaleBased(TexturesModifyData data, Image image)
        {
            int startX = 0, startY = 0;
            //�ȼ���ԭͼ����
            float scaleRatio = image.Width / (float)image.Height;
            //�ٰ��ձ���ӳ������ͼʱ��ʲô�ֱ��ʺ���
            //���ֲ��������£������ڿ��
            int newHeight = (int)(data.Width / scaleRatio);
            //���ֲ��������£������ڸ߶�
            int newWidth = (int)(data.Height * scaleRatio);
            //�µ���ȷ��������£��ĸ���ȷ��Խ��
            if (newHeight > data.Height)
            {
                //���ڿ�ȣ��µĸ߶�Խ������ô��ȡ���ڸ߶�
                newHeight = data.Height;
            }
            else
            {
                //���ڸ߶ȣ��µĿ��Խ������ô��ȡ���ڿ��
                newWidth = data.Width;
            }
            startX = (data.Width - newWidth) / 2;
            startY = (data.Height - newHeight) / 2;

            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(startX, startY, newWidth, newHeight), false));
        }
    }
}