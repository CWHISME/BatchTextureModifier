//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月15日
//描述：支持透明通道的图片格式编码器设置所用
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

namespace BatchTextureModifier
{
    public interface ISupportAlphaSetting
    {
        //当前设置是否支持透明度设置
        public bool IsAllowAlpha();
        //设置不保留透明度
        public void SetAllowAlpha(bool allow);
    }
}