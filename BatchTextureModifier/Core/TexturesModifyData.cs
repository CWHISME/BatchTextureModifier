using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.ComponentModel;

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
        /// <summary>
        /// ���ѡ��POT���ţ�POT����ģʽ
        /// </summary>
        public EPotMode PotMode = EPotMode.ToNearest;
        /// <summary>
        /// ����ǷŴ󣬱������ز���
        /// </summary>
        public bool PotStayPixel = true;


        public TexturesModifyData()
        {
            ResamplerAlgorithm = TexturesModifyUtility.ResamplerAlgorithms[0];
        }
    }

    public enum EScaleMode
    {
        [Description("NotScale�������ţ�����ı�ԭ���ֱ��ʣ����������ʽҲѡ�񱣳ֲ��䣬��ô��ûʲô��")]
        NotScale,
        [Description("DirectScale_Min(ֱ������)��[���趨�ֱ��ʲ��ᱣ��һ��]������С�߳��ﵽ�����ֱ���ֱ����С������ı�ԭ��ͼƬ������Ҳ����Ŵ�")]
        DirectScale_Min,
        [Description("DirectScale_Max(ֱ������)��[���趨�ֱ��ʲ��ᱣ��һ��]����ԭ��ͼƬ�������������·ֱ��ʵı��������ź�ֱ���һ����趨�ֱ��ʸ�С")]
        DirectScale_Max,
        [Description("StretchScale(��������)��ԭͼֱ��������µķֱ��ʣ�����ֱ�����죬�ᵼ��ͼƬ����")]
        StretchScale,
        [Description("ScaleBased(��������)����ԭͼ������ָ���ֱ���ʱ�������ֱ������䣬���ദ���͸����")]
        ScaleBased,
        [Description("ScaleBasedByCut(�����ü�)���Ը߶Ȼ��������Ϊ��׼�������ţ��������ֱ������䣬���ദֱ�Ӳü�")]
        ScaleBasedByCut,
        [Description("Fill(�������)�����ͼƬС���趨�ֱ��ʣ��򲻸ı�ͼƬԭ�����ش�С������֮����͸������䣻���ͼƬ�����趨�ֱ��ʣ����������")]
        Fill,
        [Description("DirectCut(ֱ�Ӳü�)��ֱ�Ӳü���Ŀ��ֱ���")]
        DirectCut,
        [Description("WidthBase(���ڿ��)���Կ��Ϊ��׼�������ţ��߶Ȳ�������͸������䣬�߶ȳ���֮����ü�")]
        WidthBase,
        [Description("HeightBase(���ڸ߶�)���Ը߶�Ϊ��׼�������ţ���Ȳ�������͸������䣬��ȳ���֮����ü�")]
        HeightBase,
        [Description("POT���ţ��߿���������ӽ�2N�η��ķֱ��ʣ��������ֱ������䣬���㴦����͸�������")]
        POT,
        [Description("POT�������ţ��߿���������ӽ�2N�η��ķ��ηֱ��ʣ��������ֱ������䣬���㴦����͸�������")]
        POT_Cube,
        Max
    }

    /// <summary>
    /// POT ����ģʽ
    /// </summary>

    public enum EPotMode
    {
        ToNearest,
        ToLarger,
        ToSmaller,
        Max
    }
}