//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��1��15��
//������ͼ�����ӿڣ��ᴴ��һ�� ImageSharp ������
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;

namespace BatchTextureModifier
{
    public interface IEncoderSetting
    {
        public IImageEncoder CraeteEncoder();

    }
}