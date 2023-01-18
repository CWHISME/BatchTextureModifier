//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;

namespace BatchTextureModifier
{
    /// <summary>
    /// 最新版本 ImageSharp  属性均已被标记为 init，不支持初始化意外方式修改 Encoder 了，只能新建一个 Wrap 代码。顺便封装一层操作
    /// </summary>
    public sealed class PngEncoderSetting : IEncoderSetting, ISupportAlphaSetting
    {
        public PngBitDepth? BitDepth { get; set; }
        public PngColorType? ColorType { get; set; }
        public PngFilterMethod? FilterMethod { get; set; }
        public PngCompressionLevel CompressionLevel { get; set; } = PngCompressionLevel.DefaultCompression;
        public PngTransparentColorMode TransparentColorMode { get; set; } = PngTransparentColorMode.Clear;

        ////png压缩等级
        //private string[] _pngCompressionLevelNames;
        //public string[] PngCompressionLevelNames { get { return _pngCompressionLevelNames; } }
        //public int PngCompressionLevelsIndex { get { return Array.IndexOf(_pngCompressionLevels, TexturesModifyUtility.PngSetting.CompressionLevel.ToString()); } set { TexturesModifyUtility.PngSetting.CompressionLevel = (PngCompressionLevel)value; PreviewOutputImage(); } }
        ////Png 过滤算法
        //private string[] _pngPngFilterMethods;
        //public string[] PngPngFilterMethods { get { return _pngPngFilterMethods; } }
        //public int PngPngFilterMethodsIndex { get { return Array.IndexOf(_pngPngFilterMethods, TexturesModifyUtility.PngSetting.FilterMethod.ToString()); } set { TexturesModifyUtility.PngSetting.FilterMethod = (PngFilterMethod)value; PreviewOutputImage(); } }


        public PngEncoderSetting()
        {
            //string[] names = System.Enum.GetNames<PngBitDepth>();
            //_pngCompressionLevels = new string[(int)PngCompressionLevel.BestCompression + 1];
            //for (int i = 0; i < _pngCompressionLevels.Length; i++)
            //{
            //    _pngCompressionLevels[i] = ((PngCompressionLevel)i).ToString();
            //}
            //_pngPngFilterMethods = new string[(int)PngFilterMethod.Adaptive + 1];
            //for (int i = 0; i < _pngPngFilterMethods.Length; i++)
            //{
            //    _pngPngFilterMethods[i] = ((PngFilterMethod)i).ToString();
            //}
            //LogManager.GetInstance.Log(CompressionLevel.EnumToIndex().ToString());
            BitDepth.EnumToNames<PngBitDepth>(true);
            ColorType.EnumToNames<PngColorType>(true);
            FilterMethod.EnumToNames<PngFilterMethod>(true);
            CompressionLevel.EnumToNames();
        }



        IImageEncoder IEncoderSetting.CraeteEncoder()
        {
            return new PngEncoder()
            {
                BitDepth = BitDepth,
                ColorType = ColorType,
                FilterMethod = FilterMethod,
                CompressionLevel = CompressionLevel,
                TransparentColorMode = TransparentColorMode
            };
        }

        bool ISupportAlphaSetting.IsAllowAlpha()
        {
            return TransparentColorMode == PngTransparentColorMode.Preserve;
        }

        void ISupportAlphaSetting.SetAllowAlpha(bool support)
        {
            TransparentColorMode = support ? PngTransparentColorMode.Preserve : PngTransparentColorMode.Clear;
        }
    }
}