//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年2月2日
//描述：限制图片最大大小的，确定是否支持限制大小以及最大大小
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats;

namespace BatchTextureModifier
{
    public interface IFileSizeSetting : IQualitySetting
    {
        /// <summary>
        /// 文件最大大小，以 KB 为单位
        /// </summary>
        public int MaxFileSize { get; set; }

        /// <summary>
        /// 创建一个文件大小限制
        /// </summary>
        /// <param name="tryIndex">尝试次数</param>
        /// <returns>返回空则代表尝试失败且无法再作处理，调用者请中断调用</returns>
        public IImageEncoder? CreateFileSizeLimitEncoder(int tryIndex);
    }
}