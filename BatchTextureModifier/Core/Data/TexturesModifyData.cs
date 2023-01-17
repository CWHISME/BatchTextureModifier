//=========================================
//作者：wangjiaying@cwhisme
//日期：
//描述：包含了对图片修改后设置参数类
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace BatchTextureModifier
{
    public class TexturesModifyData
    {
        /// <summary>
        /// 修改后的宽度
        /// </summary>
        public int Width = 1280;
        /// <summary>
        /// 修改后的高度
        /// </summary>
        public int Height = 720;

        /// <summary>
        /// 输入目录
        /// </summary>
        public string? InputPath;
        /// <summary>
        /// 输出目录
        /// </summary>
        public string? OutputPath;

        /// <summary>
        /// 直接覆盖源文件
        /// </summary>
        public bool IsDirectOverideFile = false;
        /// <summary>
        /// 备份源文件
        /// </summary>
        public bool IsBackupInputFile = true;

        /// <summary>
        /// 输出格式
        /// </summary>
        public IEncoderSetting? OutputEncoder;
        /// <summary>
        /// 缩放模式
        /// </summary>
        public EScaleMode ScaleMode = EScaleMode.NotScale;
        /// <summary>
        /// 缩放图片时，标准位置
        /// </summary>
        public AnchorPositionMode ImageScaleAnchorPositionMode = AnchorPositionMode.Center;
        /// <summary>
        /// 缩放算法
        /// </summary>
        public IResampler ResamplerAlgorithm;
        /// <summary>
        /// 如果选择POT缩放，POT计算模式
        /// </summary>
        public EPotMode PotMode = EPotMode.ToNearest;
        /// <summary>
        /// 如果是放大，保持像素不变
        /// </summary>
        public bool StayPixel = true;

        /// <summary>
        /// 如果是透明图片，保持透明通道不变，否则将透明度填充掉
        /// </summary>
        public bool? StayAlpha { get { return (OutputEncoder as ISupportAlphaSetting)?.IsAllowAlpha(); } set { (OutputEncoder as ISupportAlphaSetting)?.SetAllowAlpha(value == null ? false : (bool)value); } }
        /// <summary>
        /// 是否是支持 Alpha 的格式
        /// </summary>
        public bool IsSupportAlphaFormat { get { return OutputEncoder is ISupportAlphaSetting; } }


        public TexturesModifyData()
        {
            ResamplerAlgorithm = TexturesModifyUtility.ResamplerAlgorithms[0];
        }
    }
}