//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Webp;

namespace BatchTextureModifier
{
    /// <summary>
    /// ���°汾 ImageSharp  ���Ծ��ѱ����Ϊ init����֧�ֳ�ʼ�����ⷽʽ�޸� Encoder �ˣ�ֻ���½�һ�� Wrap ���롣˳���װһ�����
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
        /// �Ƿ�����Ĭ������
        /// </summary>
        public bool IsLossless { get { return FileFormat == WebpFileFormatType.Lossless; } set { FileFormat = value ? WebpFileFormatType.Lossless : WebpFileFormatType.Lossy; } }
        ///// <summary>
        ///// ���뷽ʽ���ȼ�
        ///// </summary>
        //public string[] WebpEncodingMethodNames { get { return _webpEncodingMethodNames; } }
        ///// <summary>
        ///// ѡ�б��� index
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