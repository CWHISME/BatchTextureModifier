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
    public sealed class JpegEncoderSetting : IEncoderSetting, IQualitySetting
    {

        public int Quality { get; set; }
        public bool Interleaved { get; set; }

        IImageEncoder IEncoderSetting.CraeteEncoder()
        {
            return new JpegEncoder()
            {
                Quality = Quality,
                Interleaved = Interleaved
            };
        }
    }
}