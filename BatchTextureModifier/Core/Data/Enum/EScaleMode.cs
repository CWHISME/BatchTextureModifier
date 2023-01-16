//=========================================
//作者：wangjiaying@cwhisme
//日期：
//描述：从 TexturesModifyData  单独分离
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System.ComponentModel;

namespace BatchTextureModifier
{
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

}