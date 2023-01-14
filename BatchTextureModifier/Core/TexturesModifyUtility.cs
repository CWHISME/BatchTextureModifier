using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tga;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
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
        /// 格式
        /// </summary>
        public readonly static string[] Filter = new string[] { "*.png", "*.jpg", "*.webp", "*.tga", "*.bmp", "*.gif" };
        /// <summary>
        /// 编码器
        /// </summary>
        public readonly static IImageEncoder[] _encoder = new IImageEncoder[] {
            new PngEncoder() ,
            new JpegEncoder(){ Quality=100},
            new WebpEncoder(){ TransparentColorMode=WebpTransparentColorMode.Preserve},
            new TgaEncoder(),
            new BmpEncoder(){ SupportTransparency=true},
            new GifEncoder()
        };
        /// <summary>
        /// 缩放算法
        /// </summary>
        public readonly static List<IResampler> ResamplerAlgorithms;
        /// <summary>
        /// 缩放算法对应的名字
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

        #region 日志显示



        #endregion

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
                LogManager.GetInstance.LogError(ex.Message);
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
            if (data.ScaleMode == EScaleMode.NotScale && data.OutputFormat == null) return bytes;
            try
            {
                //Image<Rgba32> 
                SixLabors.ImageSharp.Processing.ResizeMode padMode = data.StayPixel ? SixLabors.ImageSharp.Processing.ResizeMode.BoxPad : SixLabors.ImageSharp.Processing.ResizeMode.Pad;
                using (Image image = Image.Load(bytes))
                {
                    switch (data.ScaleMode)
                    {
                        case EScaleMode.NotScale:
                            break;
                        case EScaleMode.DirectScale_Min:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Min);
                            break;
                        case EScaleMode.DirectScale_Max:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Max);
                            break;
                        case EScaleMode.StretchScale:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Stretch);
                            break;
                        case EScaleMode.ScaleBased:
                            ResizeByScaleBased(data, image);
                            break;
                        case EScaleMode.DirectCut:
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), false));
                            break;
                        case EScaleMode.ScaleBasedByCut:
                            ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Crop);
                            break;
                        case EScaleMode.WidthBase:
                            //基于宽度，计算新的高度
                            int newHeight = (int)(data.Width / (image.Width / (float)image.Height));
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, (data.Height - newHeight) / 2, data.Width, newHeight), false));
                            break;
                        case EScaleMode.HeightBase:
                            //基于高度，计算新的宽度
                            int newWidth = (int)(data.Height * (image.Width / (float)image.Height));
                            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle((data.Width - newWidth) / 2, 0, newWidth, data.Height), false));
                            break;
                        case EScaleMode.Fill:
                            ResizeByMode(image, data, padMode);
                            break;
                        case EScaleMode.POT:
                            ResizeByMode(image, CalcPot(image.Width, data.PotMode), CalcPot(image.Height, data.PotMode), data.ResamplerAlgorithm, padMode);
                            break;
                        case EScaleMode.POT_Cube:
                            //取最长边作为新的方形变长
                            int pot = CalcPot(image.Width > image.Height ? image.Width : image.Height, data.PotMode);
                            ResizeByMode(image, pot, pot, data.ResamplerAlgorithm, padMode);
                            break;
                        case EScaleMode.Max:
                            break;
                        default:
                            break;
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //检测是原格式还是转换格式
                        if (data.OutputFormat == null)
                            image.Save(ms, Image.DetectFormat(bytes));
                        else
                            image.Save(ms, data.OutputFormat);
                        return ms.ToArray();
                    }
                    //image.Save(Path.Combine(Path.GetDirectoryName(path), "[Resize]" + Path.GetFileName(path)));
                }
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 自动检查图片格式
        /// </summary>
        public static IImageFormat DetectImageFormat(byte[] bytes)
        {
            return Image.DetectFormat(bytes);
        }

        private static void ResizeByMode(Image image, TexturesModifyData data, SixLabors.ImageSharp.Processing.ResizeMode mode)
        {
            ResizeByMode(image, data.Width, data.Height, data.ResamplerAlgorithm, mode);
        }

        private static void ResizeByMode(Image image, int width, int height, IResampler resampler, SixLabors.ImageSharp.Processing.ResizeMode mode)
        {
            ResizeOptions opt = new ResizeOptions();
            opt.Position = AnchorPositionMode.Center;
            opt.Sampler = resampler;
            opt.Size = new SixLabors.ImageSharp.Size(width, height);
            opt.Mode = mode;
            image.Mutate(x => x.Resize(opt));
        }

        private static void ResizeByScaleBased(TexturesModifyData data, Image image)
        {
            int startX = 0, startY = 0;
            //先计算原图比例
            float scaleRatio = image.Width / (float)image.Height;
            //再按照比例映射至新图时，什么分辨率合适
            //保持不变的情况下，若基于宽度
            int newHeight = (int)(data.Width / scaleRatio);
            //保持不变的情况下，若基于高度
            int newWidth = (int)(data.Height * scaleRatio);
            //新的正确比例情况下，哪个正确不越界
            if (newHeight > data.Height)
            {
                //基于宽度，新的高度越界了那么，取基于高度
                newHeight = data.Height;
            }
            else
            {
                //基于高度，新的宽度越界了那么，取基于宽度
                newWidth = data.Width;
            }
            startX = (data.Width - newWidth) / 2;
            startY = (data.Height - newHeight) / 2;

            image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(startX, startY, newWidth, newHeight), false));
        }

        /// <summary>
        /// 计算给定数值的 POT（根据缩放模式返回给定数值接近的2N数值）
        /// 最大支持 65535
        /// </summary>
        /// <returns></returns>
        private static int CalcPot(int x, EPotMode mode)
        {
            int oldX = x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x += 1;
            int minPot = x >> 1;
            switch (mode)
            {
                case EPotMode.ToNearest:
                    //判断哪个更接近 X
                    return x - oldX > oldX - minPot ? minPot : x;
                case EPotMode.ToLarger:
                    //比X大的 2N 数值
                    return x;
                case EPotMode.ToSmaller:
                    //比X小的 2N 数值
                    return minPot;
            }
            //出问题了？
            return oldX;
        }
    }
}