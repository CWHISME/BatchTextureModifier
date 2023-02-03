//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;

namespace BatchTextureModifier
{
    /// <summary>
    /// ���°汾 ImageSharp  ���Ծ��ѱ����Ϊ init����֧�ֳ�ʼ�����ⷽʽ�޸� Encoder �ˣ�ֻ���½�һ�� Wrap ���롣˳���װһ�����
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