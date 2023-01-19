//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Gif;

namespace BatchTextureModifier
{
    /// <summary>
    /// ���°汾 ImageSharp  ���Ծ��ѱ����Ϊ init����֧�ֳ�ʼ�����ⷽʽ�޸� Encoder �ˣ�ֻ���½�һ�� Wrap ���롣˳���װһ�����
    /// </summary>
    public sealed class GifEncoderSetting : IEncoderSetting
    {

        IImageEncoder IEncoderSetting.CraeteEncoder()
        {
            return new GifEncoder();
        }

        string IEncoderSetting.GetFileSuffix()
        {
            return ".gif";
        }
    }
}