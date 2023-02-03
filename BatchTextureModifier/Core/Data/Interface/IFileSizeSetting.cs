//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�2023��2��2��
//����������ͼƬ����С�ģ�ȷ���Ƿ�֧�����ƴ�С�Լ�����С
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;

namespace BatchTextureModifier
{
    public interface IFileSizeSetting : IQualitySetting
    {
        /// <summary>
        /// �ļ�����С���� KB Ϊ��λ
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// ����һ���ļ���С����
        /// </summary>
        /// <param name="tryIndex">���Դ���</param>
        /// <returns>���ؿ��������ʧ�����޷������������������жϵ���</returns>
        public IImageEncoder? CreateFileSizeLimitEncoder(int tryIndex);
    }
}