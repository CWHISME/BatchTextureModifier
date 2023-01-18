//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;

namespace BatchTextureModifier
{
    /// <summary>
    /// ���°汾 ImageSharp  ���Ծ��ѱ����Ϊ init����֧�ֳ�ʼ�����ⷽʽ�޸� Encoder �ˣ�ֻ���½�һ�� Wrap ���롣˳���װһ�����
    /// </summary>
    public sealed class PngEncoderSetting : IEncoderSetting, ISupportAlphaSetting
    {
        public PngBitDepth? BitDepth { get; set; }
        public PngColorType? ColorType { get; set; }
        public PngFilterMethod? FilterMethod { get; set; }
        public PngCompressionLevel CompressionLevel { get; set; } = PngCompressionLevel.DefaultCompression;
        public PngTransparentColorMode TransparentColorMode { get; set; } = PngTransparentColorMode.Clear;

        ////pngѹ���ȼ�
        //private string[] _pngCompressionLevelNames;
        //public string[] PngCompressionLevelNames { get { return _pngCompressionLevelNames; } }
        //public int PngCompressionLevelsIndex { get { return Array.IndexOf(_pngCompressionLevels, TexturesModifyUtility.PngSetting.CompressionLevel.ToString()); } set { TexturesModifyUtility.PngSetting.CompressionLevel = (PngCompressionLevel)value; PreviewOutputImage(); } }
        ////Png �����㷨
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