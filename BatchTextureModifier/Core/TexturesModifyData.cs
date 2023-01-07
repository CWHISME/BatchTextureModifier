using SixLabors.ImageSharp.Formats;

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
        /// 输出格式，为空则表示不变
        /// </summary>
        public IImageEncoder? OutputFormat;
        /// <summary>
        /// 缩放模式
        /// </summary>
        public EScaleMode ScaleMode;
    }

    public enum EScaleMode
    {
        NotScale,
        DirectScale,
        WidthBaseScale,
        HeightBaseScale,
        Max
    }
}