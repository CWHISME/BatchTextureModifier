//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�
//��������װ ImageSharp ����
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BatchTextureModifier
{
    public static class TexturesModifyUtility
    {
        /// <summary>
        /// ��ʽ
        /// </summary>
        public readonly static string[] Filter = new string[] { "*.png", "*.jpg", "*.webp", "*.tga", "*.bmp", "*.gif" };
        /// <summary>
        /// ����������
        /// </summary>
        public readonly static IEncoderSetting[] Encoder = new IEncoderSetting[] {
            new PngEncoderSetting(),
            new JpegEncoderSetting(),
            new WebpEncoderSetting(),
            new TgaEncoderSetting(),
            new BmpEncoderSetting(),
            new GifEncoderSetting()
        };

        /// <summary>
        /// �����㷨
        /// </summary>
        public readonly static List<IResampler> ResamplerAlgorithms;
        /// <summary>
        /// �����㷨��Ӧ������
        /// </summary>
        public readonly static List<string> ResamplerAlgorithmNames;

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
                LogManager.GetInstance.LogError(ex.Message);
            }

            return string.Empty;
        }

        /// <summary>
        /// ͨ���±�ȡ��ʽ
        /// </summary>
        /// <returns></returns>
        public static IEncoderSetting? GetFormatByIndex(int index)
        {
            if (index < 0 || index >= Encoder.Length) return null;
            return Encoder[index];
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
            if (data.ScaleMode == EScaleMode.NotScale && data.OutputEncoder == null) return bytes;
            try
            {
                //Image<Rgba32> 
                SixLabors.ImageSharp.Processing.ResizeMode padMode = data.StayPixel ? SixLabors.ImageSharp.Processing.ResizeMode.BoxPad : SixLabors.ImageSharp.Processing.ResizeMode.Pad;
                using (Image image = Image.Load(bytes))
                {
                    if (data.ScaleMode != EScaleMode.NotScale)
                        switch (data.ScaleMode)
                        {
                            case EScaleMode.ScaleBased:
                                ResizeByScaleBased(data, image);
                                break;
                            case EScaleMode.DirectCut:
                                image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, image.Width, image.Height), false));
                                break;
                            case EScaleMode.WidthBase:
                                //���ڿ�ȣ������µĸ߶�
                                int newHeight = (int)(data.Width / (image.Width / (float)image.Height));
                                image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, (data.Height - newHeight) / 2, data.Width, newHeight), false));
                                break;
                            case EScaleMode.HeightBase:
                                //���ڸ߶ȣ������µĿ��
                                int newWidth = (int)(data.Height * (image.Width / (float)image.Height));
                                image.Mutate(x => x.Resize(data.Width, data.Height, data.ResamplerAlgorithm, new Rectangle(0, 0, image.Width, image.Height), new Rectangle((data.Width - newWidth) / 2, 0, newWidth, data.Height), false));
                                break;
                            case EScaleMode.ScaleBasedByCut:
                                ResizeByMode(image, data, SixLabors.ImageSharp.Processing.ResizeMode.Crop);
                                break;
                            case EScaleMode.Pad:
                                ResizeByMode(image, data, padMode);
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
                            case EScaleMode.POT:
                                ResizeByMode(image, CalcPot(image.Width, data.PotMode), CalcPot(image.Height, data.PotMode), data, padMode);
                                break;
                            case EScaleMode.POT_Cube:
                                //ȡ�����Ϊ�µķ��α䳤
                                int pot = CalcPot(image.Width > image.Height ? image.Width : image.Height, data.PotMode);
                                ResizeByMode(image, pot, pot, data, padMode);
                                break;
                        }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        //�����ԭ��ʽ����ת����ʽ
                        IImageFormat? format;
                        if (data.OutputEncoder == null)
                        {
                            if (Image.TryDetectFormat(bytes, out format))
                                image.Save(ms, format);
                            else image.Save(ms, GetDefaultEncoder());
                        }
                        else
                            image.Save(ms, data.OutputEncoder.CraeteEncoder());
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

        private static IImageEncoder GetDefaultEncoder()
        {
            return Encoder[0].CraeteEncoder();
        }

        private static void ResizeByMode(Image image, TexturesModifyData data, ResizeMode mode)
        {
            ResizeByMode(image, data.Width, data.Height, data, mode);
        }

        private static void ResizeByMode(Image image, int width, int height, TexturesModifyData data, ResizeMode mode)
        {
            ResizeOptions opt = new ResizeOptions();
            opt.Position = data.ImageScaleAnchorPositionMode;
            opt.Sampler = data.ResamplerAlgorithm;
            opt.Size = new SixLabors.ImageSharp.Size(width, height);
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

        /// <summary>
        /// ���������ֵ�� POT����������ģʽ���ظ�����ֵ�ӽ���2N��ֵ��
        /// ���֧�� 65535
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
                    //�ж��ĸ����ӽ� X
                    return x - oldX > oldX - minPot ? minPot : x;
                case EPotMode.ToLarger:
                    //��X��� 2N ��ֵ
                    return x;
                case EPotMode.ToSmaller:
                    //��XС�� 2N ��ֵ
                    return minPot;
            }
            //�������ˣ�
            return oldX;
        }
    }
}