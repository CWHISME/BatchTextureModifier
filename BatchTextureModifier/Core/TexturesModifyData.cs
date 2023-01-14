using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System.ComponentModel;

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
        public IImageEncoder OutputFormat;
        /// <summary>
        /// 缩放模式
        /// </summary>
        public EScaleMode ScaleMode;
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

        private bool _stayAlpha = true;
        /// <summary>
        /// 如果是透明图片，保持透明通道不变，否则将透明度填充掉
        /// </summary>
        public bool StayAlpha
        {
            get { return _stayAlpha; }
            set
            {
                if (OutputFormat is PngEncoder)
                    (OutputFormat as PngEncoder).TransparentColorMode = value ? PngTransparentColorMode.Preserve : PngTransparentColorMode.Clear;
                else if (OutputFormat is WebpEncoder)
                    (OutputFormat as WebpEncoder).TransparentColorMode = value ? WebpTransparentColorMode.Preserve : WebpTransparentColorMode.Clear;
                else if (OutputFormat is BmpEncoder)
                    (OutputFormat as BmpEncoder).SupportTransparency = value;
                _stayAlpha = value;
            }
        }

        public bool IsHaveAlphaFormat { get { return OutputFormat is PngEncoder || OutputFormat is WebpEncoder || OutputFormat is BmpEncoder; } }


        public TexturesModifyData()
        {
            ResamplerAlgorithm = TexturesModifyUtility.ResamplerAlgorithms[0];
        }
    }

    public enum EScaleMode
    {
        [Description("NotScale：不缩放，不会改变原本分辨率，如果你连格式也选择保持不变，那么就没什么用")]
        NotScale,
        [Description("Fill(填充缩放)：如果图片小于设定分辨率，则比例放大，不足之处则填充；如果图片大于设定分辨率，则比例缩小")]
        Fill,
        [Description("DirectScale_Min(直接缩放)：[与设定分辨率不会保持一致]基于最小边长达到给定分辨率直接缩小，不会改变原本图片比例，也不会放大")]
        DirectScale_Min,
        [Description("DirectScale_Max(直接缩放)：[与设定分辨率不会保持一致]基于原本图片比例，缩放至新分辨率的比例，缩放后分辨率一般比设定分辨率更小")]
        DirectScale_Max,
        [Description("StretchScale(拉伸缩放)：原图直接填充至新的分辨率，不足直接拉伸，会导致图片变形")]
        StretchScale,
        [Description("ScaleBased(比例缩放)：将原图缩放至指定分辨率时尽量保持比例不变，多余处则填充")]
        ScaleBased,
        [Description("ScaleBasedByCut(比例裁剪)：以高度或宽度最大者为基准进行缩放，尽量保持比例不变，多余处直接裁剪")]
        ScaleBasedByCut,
        [Description("DirectCut(直接裁剪)：直接裁剪至目标分辨率")]
        DirectCut,
        [Description("WidthBase(基于宽度)：以宽度为基准进行缩放，高度不足则填充，高度超过之处则裁剪")]
        WidthBase,
        [Description("HeightBase(基于高度)：以高度为基准进行缩放，宽度不足则填充，宽度超过之处则裁剪")]
        HeightBase,
        [Description("POT缩放：高宽缩放至最接近2N次方的分辨率，尽量保持比例不变，不足处则填充")]
        POT,
        [Description("POT方形缩放：高宽缩放至最接近2N次方的方形分辨率，尽量保持比例不变，不足处则填充")]
        POT_Cube,
        Max
    }

    /// <summary>
    /// POT 缩放模式
    /// </summary>

    public enum EPotMode
    {
        ToNearest,
        ToLarger,
        ToSmaller,
        Max
    }
}