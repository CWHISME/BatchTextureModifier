using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BatchTextureModifier
{
    internal class ViewHelper : System.ComponentModel.INotifyPropertyChanged
    {

        //数据
        private TexturesModifyData _convertData = new TexturesModifyData();
        //预览图片
        private byte[] _previewImageBytes;
        //预览图片后缀
        private string _previewImageSuffix;
        //预览的图片大小缓存
        private float _inputPreviewImageSize;
        private float _outputPreviewImageSize;

        #region 数据绑定
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
        public int OutputFormatIndex { get { return _outputFormatIndex; } set { _outputFormatIndex = value; _convertData.OutputFormat = TexturesModifyUtility.GetFormatByIndex(value); Notify("IsShowQualityTextBox", "Quality", "IsWebp", "IsPng", "WebpEncodingMethodsIndex", "PngCompressionLevelsIndex", "PngEncodingBitDepthIndex", "PngPngColorTypeIndex", "IsShowStayAlpha"); PreviewOutputImage(); } }
        //==================图像导出质量==================
        //Webp格式转换方法
        private string[] _webpEncodingMethods;
        public string[] WebpEncodingMethods { get { return _webpEncodingMethods; } }
        public int WebpEncodingMethodsIndex { get { return _convertData.OutputFormat is WebpEncoder ? Array.IndexOf(_webpEncodingMethods, (_convertData.OutputFormat as WebpEncoder).Method.ToString()) : 0; } set { (_convertData.OutputFormat as WebpEncoder).Method = (WebpEncodingMethod)value; PreviewOutputImage(); } }
        //png压缩等级
        private string[] _pngCompressionLevels;
        public string[] PngCompressionLevels { get { return _pngCompressionLevels; } }
        public int PngCompressionLevelsIndex { get { return _convertData.OutputFormat is PngEncoder ? Array.IndexOf(_pngCompressionLevels, (_convertData.OutputFormat as PngEncoder).CompressionLevel.ToString()) : 0; } set { (_convertData.OutputFormat as PngEncoder).CompressionLevel = (PngCompressionLevel)value; PreviewOutputImage(); } }
        //Png 颜色深度
        private string[] _pngEncodingBitDepth = new string[] { "Bit1", "Bit2", "Bit4", "Bit8", "Bit16" };
        public string[] PngEncodingBitDepth { get { return _pngEncodingBitDepth; } }
        public int PngEncodingBitDepthIndex { get { return _convertData.OutputFormat is PngEncoder ? Array.IndexOf(_pngEncodingBitDepth, (_convertData.OutputFormat as PngEncoder).BitDepth.ToString()) : 0; } set { (_convertData.OutputFormat as PngEncoder).BitDepth = (PngBitDepth)Enum.Parse(typeof(PngBitDepth), _pngEncodingBitDepth[value]); PreviewOutputImage(); } }
        //Png 过滤算法
        private string[] _pngPngFilterMethods;
        public string[] PngPngFilterMethods { get { return _pngPngFilterMethods; } }
        public int PngPngFilterMethodsIndex { get { return _convertData.OutputFormat is PngEncoder ? Array.IndexOf(_pngPngFilterMethods, (_convertData.OutputFormat as PngEncoder).FilterMethod.ToString()) : 0; } set { (_convertData.OutputFormat as PngEncoder).FilterMethod = (PngFilterMethod)value; PreviewOutputImage(); } }
        //Png 颜色类型
        private string[] _pngPngColorType = new string[] { "自动", "Grayscale", "Rgb", "Palette", "GrayscaleWithAlpha", "RgbWithAlpha" };
        public string[] PngPngColorType { get { return _pngPngColorType; } }
        public int PngPngColorTypeIndex { get { return _convertData.OutputFormat is PngEncoder ? Array.IndexOf(_pngPngColorType, (_convertData.OutputFormat as PngEncoder).ColorType == null ? "自动" : (_convertData.OutputFormat as PngEncoder).ColorType.ToString()) : 0; } set { (_convertData.OutputFormat as PngEncoder).ColorType = value == 0 ? null : (PngColorType)Enum.Parse(typeof(PngColorType), _pngPngColorType[value]); PreviewOutputImage(); } }


        //是否显示质量设置
        public bool IsShowQualityTextBox { get { return _convertData.OutputFormat is JpegEncoder || _convertData.OutputFormat is WebpEncoder; } }
        public bool IsWebp { get { return _convertData.OutputFormat is WebpEncoder; } }
        public bool IsPng { get { return _convertData.OutputFormat is PngEncoder; } }

        public int Quality
        {
            get
            {
                if (_convertData.OutputFormat is JpegEncoder)
                    return (int)(_convertData.OutputFormat as JpegEncoder).Quality;
                if (_convertData.OutputFormat is WebpEncoder)
                    return (_convertData.OutputFormat as WebpEncoder).Quality;
                return -1;
            }
            set
            {
                if (value > 100)
                    value = 100;
                if (value < 0)
                    value = 0;
                if (_convertData.OutputFormat is JpegEncoder)
                    (_convertData.OutputFormat as JpegEncoder).Quality = value;
                if (_convertData.OutputFormat is WebpEncoder)
                    (_convertData.OutputFormat as WebpEncoder).Quality = value;
                PreviewOutputImage();
            }
        }

        //============================================

        //==================缩放模式==================
        private string[] _scaleModes;// = new string[] { "不缩放", "直接缩放", "比例缩放", "直接裁剪", "比例裁剪", "基于宽度", "基于高度", "填充缩放", "POT缩放" };
        public string[] ScaleModes { get { return _scaleModes; } }
        //选择的缩放模式下标
        private int _scaleModeIndex;
        public int ScaleModeIndex { get { return _scaleModeIndex; } set { _scaleModeIndex = value; _convertData.ScaleMode = (EScaleMode)value; Notify("ScaleModeIndex", "ShowPixelSetting", "Width", "Height", "IsPotScaleMode", "IsShowStayPixel", "IsShowStayAlpha"); PreviewOutputImage(); } }
        //是否报保持像素不变，仅 Pad 模式有效
        public bool StayPixel { get { return _convertData.StayPixel; } set { _convertData.StayPixel = value; PreviewOutputImage(); } }
        //缩放时是否保持透明不变
        public bool StayAlpha { get { return _convertData.StayAlpha; } set { _convertData.StayAlpha = value; PreviewOutputImage(); } }
        public bool IsShowStayPixel { get { return IsPotScaleMode || _convertData.ScaleMode == EScaleMode.Fill; } }
        public bool IsShowStayAlpha { get { return _convertData.IsHaveAlphaFormat && ScaleModeIndex > 0; } }
        public bool IsPotScaleMode { get { return _convertData.ScaleMode == EScaleMode.POT || _convertData.ScaleMode == EScaleMode.POT_Cube; } }
        //============================================

        //==================POT 缩放算法==================
        private string[] _potMode;
        public string[] PotMode { get { return _potMode; } }
        //选择的POT算法下标
        private int _potModeIndex;
        public int PotModeIndex { get { return _potModeIndex; } set { _potModeIndex = value; _convertData.PotMode = (EPotMode)value; PreviewOutputImage(); } }
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
        private BitmapImage _previewInputBitmap;
        public BitmapImage PreviewInputBitmap { get { return _previewInputBitmap; } set { _previewInputBitmap = value; Notify("PreviewInputBitmap", "PreviewImageVisibility"); } }
        private BitmapImage _previewOutputBitmap;
        public BitmapImage PreviewOutputBitmap { get { return _previewOutputBitmap; } set { _previewOutputBitmap = value; Notify("PreviewOutputBitmap"); } }
        public Visibility PreviewImageVisibility { get { return PreviewInputBitmap == null ? Visibility.Hidden : Visibility.Visible; } }

        //日志显示
        private System.Collections.ObjectModel.ObservableCollection<LogViewItem> _outputLogs = new System.Collections.ObjectModel.ObservableCollection<LogViewItem>();
        public System.Collections.ObjectModel.ObservableCollection<LogViewItem> OutputLogs { get { return _outputLogs; } }
        #endregion

        #region 语言提示绑定
        public float TooTipsTime { get { return 0; } }
        public string StayInputFormatTips { get { return "在处理完毕后，保存的图片与输入格式保持一致。例如修改前是 *.png，修改后也是 *.png"; } }
        public string LangResamplerAlgorithmTips { get { return "缩放算法将会影响图片的缩放质量，默认的 Bicubic 就不错"; } }
        public string LangOverideTips { get { return "该选项会直接覆盖源文件，并将源文件备份至『输出目录』"; } }
        private string[] _langScaleModeTips;
        public string[] LangScaleModeTips { get { return _langScaleModeTips; } }
        public string LangStayPixelTips { get { return "如果是放大操作，保持像素不变，否则等比放大"; } }
        public string LangStayAlphalTips { get { return "如果是透明图片，保持透明通道不变，否则将透明度填充掉"; } }
        public string CompressionTips { get { return "压缩等级越高，最终文件大小越小(但是会更慢)"; } }
        public string WebpEncodingMethodTips { get { return "编码等级越高，质量越好(但是会更慢)"; } }
        public string QualityTips { get { return "质量等级，范围 0~100，越高质量越好，但是文件也会更大"; } }
        public string PngFilterAlgorithmTips { get { return "会影响文件大小"; } }
        #endregion

        public ViewHelper()
        {
            //注释日志
            LogManager.GetInstance.OnLogChange += OnLogChange;
            //写每个缩放模式的说明太麻烦了，而且之前还用中文名，还要加注释，三边难以同步
            //直接反射吧算了
            _scaleModes = new string[(int)EScaleMode.Max];
            _langScaleModeTips = new string[_scaleModes.Length];
            Type scaleModeType = typeof(EScaleMode);
            for (int i = 0; i < _scaleModes.Length; i++)
            {
                EScaleMode mode = (EScaleMode)i;
                _scaleModes[i] = mode.ToString();
                DescriptionAttribute des = scaleModeType.GetField(_scaleModes[i]).GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
                _langScaleModeTips[i] = des?.Description;
            }

            _potMode = new string[(int)EPotMode.Max];
            for (int i = 0; i < _potMode.Length; i++)
            {
                _potMode[i] = ((EPotMode)i).ToString();
            }

            //格式转换
            _webpEncodingMethods = new string[(int)WebpEncodingMethod.BestQuality + 1];
            for (int i = 0; i < _webpEncodingMethods.Length; i++)
            {
                _webpEncodingMethods[i] = ((WebpEncodingMethod)i).ToString();
            }
            _pngCompressionLevels = new string[(int)PngCompressionLevel.BestCompression + 1];
            for (int i = 0; i < _pngCompressionLevels.Length; i++)
            {
                _pngCompressionLevels[i] = ((PngCompressionLevel)i).ToString();
            }
            _pngPngFilterMethods = new string[(int)PngFilterMethod.Adaptive + 1];
            for (int i = 0; i < _pngPngFilterMethods.Length; i++)
            {
                _pngPngFilterMethods[i] = ((PngFilterMethod)i).ToString();
            }
        }

        private void OnLogChange(LogItem log)
        {
            //System.Windows.Data.CollectionViewSource view = new System.Windows.Data.CollectionView();
            //view.pus
            _outputLogs.Add(new LogViewItem(log));
            if (log.LogType == LogItem.ELogType.Error)
                ShowErrorMessage(log.LogString);
            Notify("OutputLogs");
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
            Microsoft.Win32.SaveFileDialog fileDialog = new Microsoft.Win32.SaveFileDialog();
            //如果没有选择转换格式，则保持原图片后缀不变
            fileDialog.DefaultExt = _convertData.OutputFormat == null ? _previewImageSuffix : TexturesModifyUtility.Filter[_outputFormatIndex];
            fileDialog.Filter = string.Concat(fileDialog.DefaultExt, "|", fileDialog.DefaultExt);
            if (fileDialog.ShowDialog() == true)
            {
                try
                {
                    byte[] bytes = TexturesModifyUtility.ResizeTextures(_previewImageBytes, _convertData);
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

        //==================Private=====================

        #region Private
        private string SelectPath()
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Directory.GetAccessControl(folderDialog.SelectedPath);
                //System.Security.AccessControl.DirectorySecurity ds = new DirectoryInfo(folderDialog.SelectedPath).GetAccessControl();
                //var currentUserIdentity = System.IO.Path.Combine(Environment.UserDomainName, Environment.UserName);
                //var userAccessRules = ds.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)).OfType<System.Security.AccessControl.FileSystemAccessRule>();//.Where(i => i.IdentityReference.Value == currentUserIdentity).ToList();
                DirectoryInfo info = new DirectoryInfo(folderDialog.SelectedPath);
                return folderDialog.SelectedPath;
                //callback?.Invoke(folderDialog.SelectedPath);
            }
            return string.Empty;
        }

        /// <summary>
        /// 当选择了输入目录，使用其第一张图作为预览
        /// </summary>
        /// <param name="texturePath"></param>
        private void PreviewInputPathImage()
        {
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
                    InputImageSize = CalcSize(_previewImageBytes);
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
            byte[] bytes = TexturesModifyUtility.ResizeTextures(_previewImageBytes, _convertData);
            OutputImageSize = CalcSize(bytes);
            PreviewOutputBitmap = LoadImage(bytes);
        }

        private float CalcSize(byte[] bytes)
        {
            if (bytes == null) return 0;
            return (float)Math.Round(bytes.Length / 1024f / 1024, 2);
        }

        private BitmapImage LoadImage(byte[] texBytes)
        {
            if (texBytes == null) return null;
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(texBytes);
                bitmap.EndInit();
                return bitmap;
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogError("LoadImage Failed:" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 检测 TextBox 输入是否是纯数字
        /// </summary>
        /// <param name="text"></param>
        public int CheckTextNumberInput(TextBox text)
        {
            int val;
            if (int.TryParse(text.Text, out val))
            {
                return val;
            }
            else
            {
                if (!string.IsNullOrEmpty(text.Text))
                    ShowErrorMessage("请输入数字！");
                text.Clear();
            }
            return val;
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
        #endregion
    }
}
