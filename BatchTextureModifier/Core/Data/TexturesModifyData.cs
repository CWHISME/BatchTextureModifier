//=========================================
//���ߣ�wangjiaying@cwhisme
//���ڣ�
//�����������˶�ͼƬ�޸ĺ����ò�����
//��;��https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Processing;
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
        public bool IsDirectOverideFile = false;
        /// <summary>
        /// ����Դ�ļ�
        /// </summary>
        public bool IsBackupInputFile = true;

        /// <summary>
        /// �����ʽ
        /// </summary>
        public IEncoderSetting? OutputEncoder;
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public EScaleMode ScaleMode = EScaleMode.NotScale;
        /// <summary>
        /// ����ͼƬʱ����׼λ��
        /// </summary>
        public AnchorPositionMode ImageScaleAnchorPositionMode = AnchorPositionMode.Center;
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
        public bool StayPixel = true;

        /// <summary>
        /// �����͸��ͼƬ������͸��ͨ�����䣬����͸��������
        /// </summary>
        public bool? StayAlpha { get { return (OutputEncoder as ISupportAlphaSetting)?.IsAllowAlpha(); } set { (OutputEncoder as ISupportAlphaSetting)?.SetAllowAlpha(value == null ? false : (bool)value); } }
        /// <summary>
        /// �Ƿ���֧�� Alpha �ĸ�ʽ
        /// </summary>
        public bool IsSupportAlphaFormat { get { return OutputEncoder is ISupportAlphaSetting; } }


        public TexturesModifyData()
        {
            ResamplerAlgorithm = TexturesModifyUtility.ResamplerAlgorithms[0];
        }
    }
}