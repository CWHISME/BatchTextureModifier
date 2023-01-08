using SixLabors.ImageSharp.Formats;
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
        public bool IsDirectOverideFile;
        /// <summary>
        /// 备份源文件
        /// </summary>
        public bool IsBackupInputFile = true;

        /// <summary>
        /// 输出格式，为空则表示不变
        /// </summary>
        public IImageEncoder? OutputFormat;
        /// <summary>
        /// 缩放模式
        /// </summary>
        public EScaleMode ScaleMode;
        /// <summary>
        /// 缩放算法
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
        /// 不缩放：不会改变原本分辨率，如果你连格式也选择保持不变，那么就没什么用
        /// </summary>
        NotScale,
        /// <summary>
        /// 直接缩放：直接缩放为设置的分辨率，不足之处直接拉伸，会造成变形
        /// </summary>
        DirectScale,
        /// <summary>
        /// 比例缩放：将原图缩放至指定分辨率时尽量保持比例不变，多余处填充透明度
        /// </summary>
        ScaleBased,
        /// <summary>
        /// 比例裁剪：以高度或宽度最大者为基准进行缩放，尽量保持比例不变，多余处直接裁剪
        /// </summary>
        ScaleBasedByCut,
        /// <summary>
        /// 基于宽度：以宽度为基准进行缩放，高度不足则以透明度填充，高度超过之处则裁剪
        /// </summary>
        WidthBase,
        /// <summary>
        /// 基于高度：以高度为基准进行缩放，宽度不足则以透明度填充，宽度超过之处则裁剪
        /// </summary>
        HeightBase,
        /// <summary>
        /// 填充缩放：如果图片小于设定分辨率，则不改变图片原有像素大小，不足之处以透明度填充；如果图片大于设定分辨率，则比例缩放
        /// </summary>
        Fill,
        /// <summary>
        /// POT缩放：高宽缩放至最接近2N次方的分辨率，尽量保持比例不变，不足处进行透明度填充
        /// </summary>
        POT,
        Max
    }
}