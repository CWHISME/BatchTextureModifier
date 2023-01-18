//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace BatchTextureModifier
{
    /// <summary>
    /// 最新版本 ImageSharp  属性均已被标记为 init，不支持初始化意外方式修改 Encoder 了，只能新建一个 Wrap 代码。顺便封装一层操作
    /// </summary>
    public sealed class WebpEncoderSetting : IEncoderSetting, ISupportAlphaSetting, IQualitySetting
    {
        public WebpFileFormatType? FileFormat { get; set; } = WebpFileFormatType.Lossy;
        public int Quality { get; set; } = 75;
        public WebpEncodingMethod Method { get; set; } = WebpEncodingMethod.Default;
        public int EntropyPasses { get; set; } = 1;
        public int SpatialNoiseShaping { get; set; } = 50;
        public int FilterStrength { get; set; } = 60;
        public WebpTransparentColorMode TransparentColorMode { get; set; } = WebpTransparentColorMode.Clear;

        /// <summary>
        /// 是否无损，默认有损
        /// </summary>
        public bool IsLossless { get { return FileFormat == WebpFileFormatType.Lossless; } set { FileFormat = value ? WebpFileFormatType.Lossless : WebpFileFormatType.Lossy; } }
        ///// <summary>
        ///// 编码方式，等级
        ///// </summary>
        //public string[] WebpEncodingMethodNames { get { return _webpEncodingMethodNames; } }
        ///// <summary>
        ///// 选中编码 index
        ///// </summary>
        //public int WebpEncodingMethodIndex { get { return (int)Method; } set { Method = (WebpEncodingMethod)value; } }

        //private string[] _webpEncodingMethodNames;

        public WebpEncoderSetting()
        {
            //_webpEncodingMethodNames = new string[(int)WebpEncodingMethod.BestQuality + 1];
            //for (int i = 0; i < _webpEncodingMethodNames.Length; i++)
            //{
            //    _webpEncodingMethodNames[i] = ((WebpEncodingMethod)i).ToString();
            //}
            FileFormat.EnumToNames();
            Method.EnumToNames();
        }

        IImageEncoder IEncoderSetting.CraeteEncoder()
        {
            return new WebpEncoder()
            {
                FileFormat = FileFormat,
                Quality = Quality,
                Method = Method,
                EntropyPasses = EntropyPasses,
                SpatialNoiseShaping = SpatialNoiseShaping,
                FilterStrength = FilterStrength,
                TransparentColorMode = TransparentColorMode,
                UseAlphaCompression = false
            };
        }

        bool ISupportAlphaSetting.IsAllowAlpha()
        {
            return TransparentColorMode == WebpTransparentColorMode.Preserve;
        }

        void ISupportAlphaSetting.SetAllowAlpha(bool allow)
        {
            TransparentColorMode = allow ? WebpTransparentColorMode.Preserve : WebpTransparentColorMode.Clear;
        }
    }
}