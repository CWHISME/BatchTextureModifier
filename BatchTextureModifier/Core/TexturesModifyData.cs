using SixLabors.ImageSharp.Formats;

namespace BatchTextureModifier
{
    public class TexturesModifyData
    {
        /// <summary>
        /// �޸ĺ�Ŀ��
        /// </summary>
        public int Width = 1280;
        /// <summary>
        /// �޸ĺ�ĸ߶�
        /// </summary>
        public int Height = 720;

        /// <summary>
        /// ����Ŀ¼
        /// </summary>
        public string? InputPath;
        /// <summary>
        /// ���Ŀ¼
        /// </summary>
        public string? OutputPath;

        /// <summary>
        /// �����ʽ��Ϊ�����ʾ����
        /// </summary>
        public IImageEncoder? OutputFormat;
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public EScaleMode ScaleMode;
    }

    public enum EScaleMode
    {
        NotScale,
        DirectScale,
        WidthBaseScale,
        HeightBaseScale,
        Max
    }
}