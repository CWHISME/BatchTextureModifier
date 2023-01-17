//=========================================
//作者：wangjiaying@cwhisme
//日期：
//描述：从 TexturesModifyData  单独分离
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BatchTextureModifier
{
    public enum EScaleMode
    {
        [Display(Name = "不缩放", Description = "不缩放：不会改变原本分辨率，如果你连格式也选择保持不变，那么就没什么用")]
        NotScale,
        [Display(Name = "比例缩放", Description = "比例缩放II：将原图缩放至指定分辨率时尽量保持比例不变，多余处则填充")]
        ScaleBased,
        [Display(Name = "直接裁剪", Description = "直接裁剪：直接裁剪至目标分辨率")]
        DirectCut,
        [Display(Name = "基于宽度", Description = "基于宽度：以宽度为基准进行缩放，高度不足则填充，高度超过之处则裁剪")]
        WidthBase,
        [Display(Name = "基于高度", Description = "基于高度：以高度为基准进行缩放，宽度不足则填充，宽度超过之处则裁剪")]
        HeightBase,
        [Display(Name = "比例缩放II", Description = "比例缩放：如果图片小于设定分辨率，则比例放大，不足之处则填充；如果图片大于设定分辨率，则比例缩小")]
        Pad,
        [Display(Name = "比例裁剪", Description = "比例裁剪：以高度或宽度最大者为基准进行缩放，尽量保持比例不变，多余处直接裁剪")]
        ScaleBasedByCut,
        [Display(Name = "直接缩放(向下)", Description = "直接缩放(向下)：[与设定分辨率不会保持一致]基于最小边长达到给定分辨率直接缩小，不会改变原本图片比例，也不会放大")]
        DirectScale_Min,
        [Display(Name = "直接缩放(向上)", Description = "直接缩放(向上)：[与设定分辨率不会保持一致]基于原本图片比例，缩放至新分辨率的比例，缩放后分辨率一般比设定分辨率更小")]
        DirectScale_Max,
        [Display(Name = "拉伸缩放", Description = "拉伸缩放：原图直接填充至新的分辨率，不足直接拉伸，会导致图片变形")]
        StretchScale,
        [Display(Name = "POT缩放", Description = "POT缩放：高宽缩放至最接近2N次方的分辨率，尽量保持比例不变，不足处则填充")]
        POT,
        [Display(Name = "POT方形缩放", Description = "POT方形缩放：高宽缩放至最接近2N次方的方形分辨率，尽量保持比例不变，不足处则填充")]
        POT_Cube
    }

}