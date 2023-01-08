using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

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
        /// ֱ�Ӹ���Դ�ļ�
        /// </summary>
        public bool IsDirectOverideFile;
        /// <summary>
        /// ����Դ�ļ�
        /// </summary>
        public bool IsBackupInputFile = true;

        /// <summary>
        /// �����ʽ��Ϊ�����ʾ����
        /// </summary>
        public IImageEncoder? OutputFormat;
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public EScaleMode ScaleMode;
        /// <summary>
        /// �����㷨
        /// </summary>
        public IResampler ResamplerAlgorithm;

        public TexturesModifyData()
        {
            ResamplerAlgorithm = TexturesModifyUtility.ResamplerAlgorithms[0];
        }
    }

    public enum EScaleMode
    {
        /// <summary>
        /// �����ţ�����ı�ԭ���ֱ��ʣ����������ʽҲѡ�񱣳ֲ��䣬��ô��ûʲô��
        /// </summary>
        NotScale,
        /// <summary>
        /// ֱ�����ţ�ֱ������Ϊ���õķֱ��ʣ�����֮��ֱ�����죬����ɱ���
        /// </summary>
        DirectScale,
        /// <summary>
        /// �������ţ���ԭͼ������ָ���ֱ���ʱ�������ֱ������䣬���ദ���͸����
        /// </summary>
        ScaleBased,
        /// <summary>
        /// �����ü����Ը߶Ȼ��������Ϊ��׼�������ţ��������ֱ������䣬���ദֱ�Ӳü�
        /// </summary>
        ScaleBasedByCut,
        /// <summary>
        /// ���ڿ�ȣ��Կ��Ϊ��׼�������ţ��߶Ȳ�������͸������䣬�߶ȳ���֮����ü�
        /// </summary>
        WidthBase,
        /// <summary>
        /// ���ڸ߶ȣ��Ը߶�Ϊ��׼�������ţ���Ȳ�������͸������䣬��ȳ���֮����ü�
        /// </summary>
        HeightBase,
        /// <summary>
        /// ������ţ����ͼƬС���趨�ֱ��ʣ��򲻸ı�ͼƬԭ�����ش�С������֮����͸������䣻���ͼƬ�����趨�ֱ��ʣ����������
        /// </summary>
        Fill,
        /// <summary>
        /// POT���ţ��߿���������ӽ�2N�η��ķֱ��ʣ��������ֱ������䣬���㴦����͸�������
        /// </summary>
        POT,
        Max
    }
}