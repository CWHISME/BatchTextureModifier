//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BatchTextureModifier
{
    /// <summary>
    /// 最新版本 ImageSharp  属性均已被标记为 init，不支持初始化意外方式修改 Encoder 了，只能新建一个 Wrap 代码。顺便封装一层操作
    /// </summary>
    public sealed class JpegEncoderSetting : IEncoderSetting, IQualitySetting, IFileSizeSetting
    {

        public int Quality { get; set; } = 100;
        public bool Interleaved { get; set; } = false;

        public bool IsSupportQuality => true;

        int IFileSizeSetting.MaxFileSize { get; set; }

        IImageEncoder IEncoderSetting.CraeteEncoder()
        {
            return new JpegEncoder()
            {
                Quality = Quality,
                Interleaved = Interleaved
            };
        }

        string IEncoderSetting.GetFileSuffix()
        {
            return ".jpg";
        }

        IImageEncoder? IFileSizeSetting.CreateFileSizeLimitEncoder(int tryIndex)
        {
            int quality = Quality - tryIndex * 5;
            if (quality < 1) return null;
            return new JpegEncoder()
            {
                Quality = quality,
                Interleaved = Interleaved
            };
        }
    }
}