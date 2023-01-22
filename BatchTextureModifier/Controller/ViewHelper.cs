//=========================================
//作者：wangjiaying@cwhisme
//日期：
//描述：与 WPF UI 绑定的控制类
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BatchTextureModifier
{
    internal class ViewHelper : System.ComponentModel.INotifyPropertyChanged
    {

        //数据
        private TexturesModifyData _convertData = new TexturesModifyData();
        private LanguageConfig _lang = new LanguageConfig();
        //预览图片
        private byte[]? _previewImageBytes;
        //预览图片后缀
        private string _previewImageSuffix = String.Empty;
        //预览的图片大小缓存
        private float _inputPreviewImageSize;
        private float _outputPreviewImageSize;

        #region 数据绑定
        public TexturesModifyData ConvertData { get { return _convertData; } }
        public LanguageConfig LangData { get { return _lang; } }

        public int Width { get { return _convertData.ScaleMode == EScaleMode.NotScale && PreviewInputBitmap != null ? (PreviewInputBitmap.PixelWidth) : _convertData.Width; } set { _convertData.Width = CheckHeightWidthOutbound(value); Notify("Width"); PreviewOutputImage(); } }
        public int Height { get { return _convertData.ScaleMode == EScaleMode.NotScale && PreviewInputBitmap != null ? (PreviewInputBitmap.PixelHeight) : _convertData.Height; } set { _convertData.Height = CheckHeightWidthOutbound(value); Notify("Height"); PreviewOutputImage(); } }
        public string? InputPath { get { return _convertData.InputPath; } set { _convertData.InputPath = value; PreviewInputPathImage(); Notify("InputPath", "HasExistInputPath", "Width", "Height"); } }
        public string? OutputPath { get { return _convertData.OutputPath; } set { _convertData.OutputPath = value; IsDirectOverideFile = OutputPath == InputPath; Notify("OutputPath", "HasExistOutputPath"); } }
        public bool IsDirectOverideFile { get { return _convertData.IsDirectOverideFile; } set { _convertData.IsDirectOverideFile = value; if (value && OutputPath != InputPath) OutputPath = InputPath; else if (!value && OutputPath == InputPath) { OutputPath = string.Empty; CheckOutputPath(); } Notify("IsDirectOverideFile"); } }
        public bool IsBackupInputFile { get { return _convertData.IsBackupInputFile; } set { if (!value && !ShowConfirmMessage("你已经选择了直接覆盖源文件！\n如果选择不备份的话，将会导致源文件直接丢失！\n\n确定要不备份吗？")) { IsBackupInputFile = true; return; } _convertData.IsBackupInputFile = value; } }
        //输入图片大小
        public float InputImageSize { get { return _inputPreviewImageSize; } set { _inputPreviewImageSize = value; Notify("InputImageSize"); } }
        //输出图片大小
        public float OutputImageSize { get { return _outputPreviewImageSize; } set { _outputPreviewImageSize = value; Notify("OutputImageSize"); } }

        //输出格式列表
        public string[] OutputFormats { get { return TexturesModifyUtility.Filter; } }
        //保留原图片格式
        private bool _stayOldFormat = true;
        public bool StayOldFormat { get { return _stayOldFormat; } set { _stayOldFormat = value; OutputFormatIndex = value ? -1 : 0; Notify("StayOldFormat", "OutputFormatIndex"); } }
        //选择的输出格式下标
        private int _outputFormatIndex;
        public int OutputFormatIndex { get { return _outputFormatIndex; } set { _outputFormatIndex = value; _convertData.OutputEncoder = TexturesModifyUtility.GetFormatByIndex(value); Notify("IsShowQualityTextBox", "IsSupportQuality", "Quality", "IsWebp", "IsPng", "IsJpegEncoder", "WebpEncodingMethodsIndex", "PngCompressionLevelsIndex", "PngEncodingBitDepthIndex", "PngPngColorTypeIndex", "IsShowStayAlpha", "StayAlpha"); PreviewOutputImage(); } }

        //==================图像导出设置==================
        public PngEncoderSetting PngSetting { get { return (PngEncoderSetting)TexturesModifyUtility.Encoder[0]; } }
        public JpegEncoderSetting JpegSetting { get { return (JpegEncoderSetting)TexturesModifyUtility.Encoder[1]; } }
        public WebpEncoderSetting WebpSetting { get { return (WebpEncoderSetting)TexturesModifyUtility.Encoder[2]; } }
        public TgaEncoderSetting TgaSetting { get { return (TgaEncoderSetting)TexturesModifyUtility.Encoder[3]; } }
        public BmpEncoderSetting BmpSetting { get { return (BmpEncoderSetting)TexturesModifyUtility.Encoder[4]; } }
        public GifEncoderSetting GifSetting { get { return (GifEncoderSetting)TexturesModifyUtility.Encoder[5]; } }

        //Webp格式转换方法==========
        //private string[] _webpEncodingMethods;
        public bool IsWebp { get { return _convertData.OutputEncoder is WebpEncoderSetting; } }
        public string[] WebpEncodingMethods { get { return WebpSetting.Method.EnumToNames(); } }
        public int WebpEncodingMethodsIndex { get { return WebpSetting.Method.EnumToIndex(); } set { WebpSetting.Method = value.IndexToEnum<WebpEncodingMethod>(); PreviewOutputImage(); } }
        public bool WebpIsLossless { get { return WebpSetting.IsLossless; } set { WebpSetting.IsLossless = value; Notify("IsSupportQuality"); PreviewOutputImage(); } }
        //public int WebpSettingEntropyPasses { get { return WebpSetting.EntropyPasses; } set { WebpSetting.EntropyPasses = value; PreviewOutputImage(); } }
        //public int WebpSettingSpatialNoiseShaping { get { return WebpSetting.SpatialNoiseShaping; } set { WebpSetting.SpatialNoiseShaping = value; PreviewOutputImage(); } }
        //public int WebpSettingFilterStrength { get { return WebpSetting.FilterStrength; } set { WebpSetting.FilterStrength = value; PreviewOutputImage(); } }


        //JPEG格式转换方法==========
        public bool IsJpegEncoder { get { return _convertData.OutputEncoder is JpegEncoderSetting; } }
        public bool JpgIsInterleaved { get { return JpegSetting.Interleaved; } set { JpegSetting.Interleaved = value; PreviewOutputImage(); } }

        //Png格式转换方法==========
        public bool IsPng { get { return _convertData.OutputEncoder is PngEncoderSetting; } }
        //png压缩等级
        public string[] PngCompressionLevels { get { return PngSetting.CompressionLevel.EnumToNames(); } }
        public int PngCompressionLevelsIndex { get { return PngSetting.CompressionLevel.EnumToIndex(); } set { PngSetting.CompressionLevel = value.IndexToEnum<PngCompressionLevel>(); PreviewOutputImage(); } }
        //Png 颜色深度
        //private string[] _pngEncodingBitDepth = new string[] { "Bit1", "Bit2", "Bit4", "Bit8", "Bit16" };
        public string[] PngEncodingBitDepth { get { return PngSetting.BitDepth.EnumToNames<PngBitDepth>(); } }
        public int PngEncodingBitDepthIndex { get { return PngSetting.BitDepth.EnumToIndex(); } set { PngSetting.BitDepth = value.IndexToEnumNullable<PngBitDepth>(); PreviewOutputImage(); } }
        //Png 过滤算法
        //private string[] _pngPngFilterMethods;
        public string[] PngPngFilterMethods { get { return PngSetting.FilterMethod.EnumToNames<PngFilterMethod>(); } }
        public int PngPngFilterMethodsIndex { get { return PngSetting.FilterMethod.EnumToIndex(); } set { PngSetting.FilterMethod = value.IndexToEnumNullable<PngFilterMethod>(); PreviewOutputImage(); } }
        //Png 颜色类型
        //private string[] _pngPngColorType = new string[] { "自动", "Grayscale", "Rgb", "Palette", "GrayscaleWithAlpha", "RgbWithAlpha" };
        public string[] PngPngColorType { get { return PngSetting.ColorType.EnumToNames<PngColorType>(); } }
        public int PngPngColorTypeIndex { get { return PngSetting.ColorType.EnumToIndex(); } set { PngSetting.ColorType = value.IndexToEnumNullable<PngColorType>(); PreviewOutputImage(); } }

        //是否显示质量设置
        public bool IsSupportQuality { get { return (_convertData.OutputEncoder as IQualitySetting)?.IsSupportQuality ?? false; } }
        public int Quality { get { return IsSupportQuality ? ((IQualitySetting)_convertData.OutputEncoder!).Quality : -1; } set { value = Math.Clamp(value, 1, 100); ((IQualitySetting)_convertData.OutputEncoder!).Quality = value; PreviewOutputImage(); } }

        //============================================

        //==================缩放模式==================
        public string[] ScaleModes { get { return _convertData.ScaleMode.EnumToNames(); } }
        //选择的缩放模式下标
        public int ScaleModeIndex { get { return _convertData.ScaleMode.EnumToIndex(); } set { _convertData.ScaleMode = value.IndexToEnum<EScaleMode>(); Notify("ScaleModeIndex", "ShowPixelSetting", "Width", "Height", "IsPotScaleMode", "IsShowStayPixel", "IsShowStayAlpha", "IsDisplayImageScaleAnchorPositionModes"); PreviewOutputImage(); } }
        //是否报保持像素不变，仅 Pad 模式有效
        public bool StayPixel { get { return _convertData.StayPixel; } set { _convertData.StayPixel = value; PreviewOutputImage(); } }
        //缩放时是否保持透明不变
        public bool? StayAlpha { get { return _convertData.StayAlpha; } set { _convertData.StayAlpha = value; PreviewOutputImage(); } }
        //保持像素
        public bool IsShowStayPixel { get { return IsPotScaleMode || _convertData.ScaleMode == EScaleMode.Pad; } }
        public bool IsShowStayAlpha { get { return _convertData.IsSupportAlphaFormat /*&& ScaleModeIndex > 0*/; } }
        //是否POT缩放
        public bool IsPotScaleMode { get { return _convertData.ScaleMode == EScaleMode.POT || _convertData.ScaleMode == EScaleMode.POT_Cube; } }
        //停靠标准
        public bool IsDisplayImageScaleAnchorPositionModes { get { return _convertData.ScaleMode >= EScaleMode.Pad; } }
        public string[] ImageScaleAnchorPositionModes { get { return _convertData.ImageScaleAnchorPositionMode.EnumToNames(); } }
        public int ImageScaleAnchorPositionModeIndex { get { return _convertData.ImageScaleAnchorPositionMode.EnumToIndex(); } set { _convertData.ImageScaleAnchorPositionMode = value.IndexToEnum<SixLabors.ImageSharp.Processing.AnchorPositionMode>(); PreviewOutputImage(); } }
        //============================================

        //==================POT 缩放算法==================
        //private string[] _potMode;
        public string[] PotMode { get { return _convertData.PotMode.EnumToNames(); } }
        //选择的POT算法下标
        //private int _potModeIndex;
        public int PotModeIndex { get { return _convertData.PotMode.EnumToIndex(); } set { _convertData.PotMode = value.IndexToEnum<EPotMode>(); PreviewOutputImage(); } }
        //============================================

        //==================图像缩放算法==================
        //分辨率设置是否显示
        public bool ShowPixelSetting { get { return _convertData.ScaleMode != EScaleMode.NotScale && !IsPotScaleMode; } }
        //缩放算法
        //private string[] _resamplerAlgorithmNames;
        public List<string> ResamplerAlgorithmNames { get { return TexturesModifyUtility.ResamplerAlgorithmNames; } }
        //选择的缩放算法下标
        private int _resamplerAlgorithmIndex;
        public int ResamplerAlgorithmIndex { get { return _resamplerAlgorithmIndex; } set { _resamplerAlgorithmIndex = value; _convertData.ResamplerAlgorithm = TexturesModifyUtility.ResamplerAlgorithms[_resamplerAlgorithmIndex]; Notify("ResamplerAlgorithmIndex"); PreviewOutputImage(); } }
        //============================================
        #endregion

        #region UI绑定
        //目录是否合法
        //输入目录存在
        public bool HasExistInputPath { get { return Directory.Exists(InputPath); } }
        //输出目录存在
        public bool HasExistOutputPath { get { return Directory.Exists(OutputPath); } }
        //预览图
        private BitmapImage? _previewInputBitmap;
        public BitmapImage? PreviewInputBitmap { get { return _previewInputBitmap; } set { _previewInputBitmap = value; Notify("PreviewInputBitmap", "PreviewImageVisibility"); } }
        private BitmapImage? _previewOutputBitmap;
        public BitmapImage? PreviewOutputBitmap { get { return _previewOutputBitmap; } set { _previewOutputBitmap = value; Notify("PreviewOutputBitmap"); } }
        public Visibility PreviewImageVisibility { get { return PreviewInputBitmap == null ? Visibility.Hidden : Visibility.Visible; } }

        //日志显示
        private System.Collections.ObjectModel.ObservableCollection<LogViewItem> _outputLogs = new System.Collections.ObjectModel.ObservableCollection<LogViewItem>();
        public System.Collections.ObjectModel.ObservableCollection<LogViewItem> OutputLogs { get { return _outputLogs; } }
        public LogViewItem LastLogItem { get { return _outputLogs[_outputLogs.Count - 1]; } }
        #endregion

        private SynchronizationContext? _synchronizationContext;

        public ViewHelper()
        {
            _synchronizationContext = DispatcherSynchronizationContext.Current;
            //注释日志
            LogManager.GetInstance.OnLogChange += OnLogChange;
            LogManager.GetInstance.LogWarning("核心初始化...");
            StringBuilder builder = new StringBuilder("系统信息：", 32);
            builder.Append(Environment.OSVersion.ToString());
            builder.Append("  ");
            builder.Append(Environment.MachineName);
            builder.Append("  ");
            builder.AppendLine(Environment.Version.ToString());
            builder.Append("初始化内存占用：");
            builder.Append(TexturesModifyUtility.CalcSize(Environment.WorkingSet).ToString());
            builder.AppendLine("MB");
            builder.Append("核心数：");
            builder.Append(Environment.ProcessorCount.ToString());
            builder.AppendLine("个");
            builder.Append("批量处理图片时，最高将会开启 ");
            builder.Append(Environment.ProcessorCount.ToString());
            builder.AppendLine(" 个线程同时进行处理");
            ConvertData.ProcessCount = Environment.ProcessorCount - 1;
            LogManager.GetInstance.LogWarning(builder.ToString());
        }

        private void OnLogChange(LogItem log)
        {
            if (_synchronizationContext == null)
            {
                ShowErrorMessage("线程回调出现问题：SynchronizationContext 为空！");
                return;
            }
            _synchronizationContext.Post((x) =>
            {
                _outputLogs.Add(new LogViewItem(log));
                if (log.LogType == LogItem.ELogType.Error)
                    ShowErrorMessage(log.LogString);
                Notify("OutputLogs", "LastLogItem");
            }, null);
        }

        #region 绑定通知
        private event PropertyChangedEventHandler? PropertyChanged;
        event PropertyChangedEventHandler? INotifyPropertyChanged.PropertyChanged
        {
            add
            {
                PropertyChanged += value;
            }

            remove
            {
                PropertyChanged -= value;
            }
        }

        public void Notify(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Notify(params string[] propertyName)
        {
            foreach (var item in propertyName)
            {
                Notify(item);
            }
        }
        #endregion

        #region 提供于界面接口
        /// <summary>
        /// 选择输入目录
        /// </summary>
        public void SelectInputPath()
        {
            InputPath = SelectPath();
        }

        /// <summary>
        /// 选择输出目录
        /// </summary>
        public void SelectOutputtPath()
        {
            OutputPath = SelectPath();
        }

        /// <summary>
        /// 选择某个单图进行处理
        /// </summary>
        public void SelectSingleImage()
        {
            Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            fileDialog.DefaultExt = string.Join("|", TexturesModifyUtility.Filter);
            fileDialog.Multiselect = false;
            if (fileDialog.ShowDialog() == true)
            {
                PreviewInputPathImage(fileDialog.FileName);
            }
        }

        /// <summary>
        /// 将当前预览图保存
        /// </summary>
        public void SaveSinglePreviewImage()
        {
            if (_previewImageBytes == null) return;
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            //如果没有选择转换格式，则保持原图片后缀不变
            fileDialog.DefaultExt = _convertData.OutputEncoder == null ? _previewImageSuffix : TexturesModifyUtility.Filter[_outputFormatIndex];
            fileDialog.Filter = string.Concat(fileDialog.DefaultExt, "|", fileDialog.DefaultExt);
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    byte[] bytes = TexturesModifyUtility.ModifyTextures(_previewImageBytes, _convertData);
                    File.WriteAllBytes(fileDialog.FileName, bytes);
                    LogManager.GetInstance.Log("手动保存图片：" + fileDialog.FileName);
                    ShowMessage("保存成功！路径：" + fileDialog.FileName);
                }
                catch (Exception ex)
                {
                    LogManager.GetInstance.LogError(ex.Message);
                }
            }
        }

        private bool _isNoneBatchProcess = true;
        public bool CanBatchProcess { get { return _isNoneBatchProcess; } }
        /// <summary>
        /// 开始执行批量处理
        /// </summary>
        public async void StartBatchModify(Action onEnd)
        {
            _isNoneBatchProcess = false;
            Notify("CanBatchProcess");
            await Task.Run(async () =>
            {
                await TexturesModifyUtility.StartBatchModify(ConvertData);
            });
            _isNoneBatchProcess = true;
            Notify("CanBatchProcess");
            onEnd?.Invoke();
        }

        public void DisplayAboutInfo()
        {
            if (MessageBox.Show("批量图片处理工具 \n\nMade by wangjiaying\n个人博客：wangjiaying.top\n\n=>点击确认前往<=", "关于", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start("explorer.exe", "https://wangjiaying.top");
            }
        }

        public void DisplayGitInfo()
        {
            if (MessageBox.Show("Git地址：github.com/CWHISME/BatchTextureModifier\n(点个赞吧('ω'))\n\n=>点击确认前往<=", "源码地址", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start("explorer.exe", "https://github.com/CWHISME/BatchTextureModifier.git");
            }
        }
        #endregion

        //==================Private and tools=====================

        private string SelectPath()
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo info = new DirectoryInfo(folderDialog.SelectedPath);
                return folderDialog.SelectedPath;
            }
            return string.Empty;
        }

        /// <summary>
        /// 当选择了输入目录，使用其第一张图作为预览
        /// </summary>
        /// <param name="texturePath"></param>
        private void PreviewInputPathImage()
        {
            if (string.IsNullOrEmpty(InputPath)) return;
            CheckOutputPath();
            //获取输入目录第一张图作为预览图
            PreviewInputPathImage(TexturesModifyUtility.GetDirectoryFirstTextures(InputPath));
        }

        /// <summary>
        /// 给定一张图片路径，加载为预览
        /// </summary>
        /// <param name="imagePath"></param>
        private void PreviewInputPathImage(string imagePath)
        {
            LogManager.GetInstance.Log("加载预览图片：" + imagePath);
            //清空旧图
            _previewImageBytes = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    _previewImageBytes = File.ReadAllBytes(imagePath);
                    InputImageSize = TexturesModifyUtility.CalcSize(_previewImageBytes);
                }
                catch (Exception ex)
                {
                    ShowErrorMessage(ex.Message);
                }
            }
            //读取失败，则清空显示
            if (_previewImageBytes == null)
            {
                PreviewInputBitmap = null;
                PreviewOutputBitmap = null;
            }
            else
            {
                //存储后缀
                _previewImageSuffix = "*" + Path.GetExtension(imagePath);
                //加载预览的原图
                PreviewInputBitmap = LoadImage(_previewImageBytes);
                //检测并显示输出结果图
                if (PreviewInputBitmap != null)
                    PreviewOutputImage();
            }
        }

        private void CheckOutputPath()
        {
            if (string.IsNullOrEmpty(InputPath)) return;
            if (string.IsNullOrEmpty(OutputPath))
                OutputPath = InputPath + "_Output";
        }

        /// <summary>
        /// 检测是否显示被修改后的预览图
        /// </summary>
        private void PreviewOutputImage()
        {
            if (_previewImageBytes == null) return;
            byte[]? bytes = null;
            try
            {
                bytes = TexturesModifyUtility.ModifyTextures(_previewImageBytes, _convertData);
                OutputImageSize = TexturesModifyUtility.CalcSize(bytes);
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogError(ex.Message);
            }
            PreviewOutputBitmap = LoadImage(bytes);
        }

        private BitmapImage? LoadImage(byte[]? texBytes)
        {
            if (texBytes == null) return null;
            BitmapImage bitmap = new BitmapImage();
            try
            {
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(texBytes);
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogError("LoadImage Failed:" + ex.Message);
                return PreviewInputBitmap;
            }
        }

        /// <summary>
        /// 检测高度与宽度是否设置过大
        /// </summary>
        /// <returns></returns>
        private int CheckHeightWidthOutbound(int val)
        {
            if (val > 30000)
            {
                ShowErrorMessage("我想你应该不会想用这种批量工具处理分辨率超过 30000 这么的图吧？\n为了避免卡死，所以我限制了修改后的图长宽不能超过 30000 分辨率。");
                return 30000;
            }
            return val;
        }


        private void ShowErrorMessage(string str)
        {
            MessageBox.Show(str, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool ShowConfirmMessage(string str)
        {
            return MessageBox.Show(str, "提示", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK;
        }

        private void ShowMessage(string str)
        {
            MessageBox.Show(str, "提示", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
