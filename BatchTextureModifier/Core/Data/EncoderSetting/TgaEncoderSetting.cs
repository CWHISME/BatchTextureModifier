//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Tga;

namespace BatchTextureModifier
{
    /// <summary>
    /// 最新版本 ImageSharp  属性均已被标记为 init，不支持初始化意外方式修改 Encoder 了，只能新建一个 Wrap 代码。顺便封装一层操作
    /// </summary>
    public sealed class TgaEncoderSetting : IEncoderSetting
    {

        IImageEncoder IEncoderSetting.CraeteEncoder()
        {
            return new TgaEncoder();
        }

        string IEncoderSetting.GetFileSuffix()
        {
            return ".tga";
        }
    }
}