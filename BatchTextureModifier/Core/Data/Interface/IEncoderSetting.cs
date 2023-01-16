//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：图像编码接口，会创建一个 ImageSharp 编码器
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;

namespace BatchTextureModifier
{
    public interface IEncoderSetting
    {
        public IImageEncoder CraeteEncoder();

    }
}