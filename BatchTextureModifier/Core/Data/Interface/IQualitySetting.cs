//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：支持质量设置的图片编码接口
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

namespace BatchTextureModifier
{
    public interface IQualitySetting
    {
        public int Quality { get; set; }
    }
}