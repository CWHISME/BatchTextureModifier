//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月18日
//描述：与界面绑定的语言
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System.Windows.Media;

namespace BatchTextureModifier
{
    /// <summary>
    /// 语言提示绑定
    /// </summary>
    public class LanguageConfig
    {

        //ToopTips 显示延迟时间
        public float TooTipsTime { get { return 0; } }

        public string StayInputFormatTips { get { return "在处理完毕后，保存的图片与输入格式保持一致。例如修改前是 *.png，修改后也是 *.png"; } }
        public string OutputFormatComboBoxTips { get { return "选择输出的新图片格式，注意：TGA 不支持预览！"; } }
        public string LangResamplerAlgorithmTips { get { return "缩放算法将会影响图片的缩放质量，默认的 Bicubic 就不错"; } }
        public string LangOverideTips { get { return "该选项会直接覆盖源文件，并将源文件备份至『输出目录』"; } }
        //private string[] _langScaleModeTips;
        public string[] LangScaleModeTips { get { return EScaleMode.NotScale.EnumToDescriptions(); } }
        public string LangStayPixelTips { get { return "如果是放大操作，保持像素不变，否则等比放大"; } }
        public string LangStayAlphaTips { get { return "如果是透明图片，保持透明通道精确像素值不变，否则清理不可见像素以获得更好的压缩效果"; } }
        public string CompressionTips { get { return "压缩等级越高，最终文件大小越小(但是会更慢)"; } }
        public string WebpEncodingMethodTips { get { return "编码等级越高，质量越好(但是会更慢)"; } }
        public string QualityTips { get { return "质量等级，范围 0~100，越高质量越好，但是文件也会更大"; } }
        public string PngFilterAlgorithmTips { get { return "会影响文件大小"; } }

    }
}
