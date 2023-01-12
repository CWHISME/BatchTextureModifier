using SixLabors.ImageSharp.Processing.Processors.Transforms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

        #region 数据绑定
        public int Width { get { return _convertData.ScaleMode == EScaleMode.NotScale && PreviewInputBitmap != null ? (PreviewInputBitmap.PixelWidth) : _convertData.Width; } set { _convertData.Width = CheckHeightWidthOutbound(value); Notify("Width"); PreviewOutputImage(); } }
        public int Height { get { return _convertData.ScaleMode == EScaleMode.NotScale && PreviewInputBitmap != null ? (PreviewInputBitmap.PixelHeight) : _convertData.Height; } set { _convertData.Height = CheckHeightWidthOutbound(value); Notify("Height"); PreviewOutputImage(); } }
        public string? InputPath { get { return _convertData.InputPath; } set { _convertData.InputPath = value; PreviewInputPathImage(); Notify("InputPath", "HasExistInputPath", "Width", "Height"); } }
        public string? OutputPath { get { return _convertData.OutputPath; } set { _convertData.OutputPath = value; IsDirectOverideFile = OutputPath == InputPath; Notify("OutputPath", "HasExistOutputPath"); } }
        public bool IsDirectOverideFile { get { return _convertData.IsDirectOverideFile; } set { _convertData.IsDirectOverideFile = value; if (value && OutputPath != InputPath) OutputPath = InputPath; else if (!value && OutputPath == InputPath) { OutputPath = string.Empty; CheckOutputPath(); } Notify("IsDirectOverideFile"); } }
        public bool IsBackupInputFile { get { return _convertData.IsBackupInputFile; } set { if (!value && !ShowConfirmMessage("你已经选择了直接覆盖源文件！\n如果选择不备份的话，将会导致源文件直接丢失！\n\n确定要不备份吗？")) { IsBackupInputFile = true; return; } _convertData.IsBackupInputFile = value; } }

        //输出格式列表
        public string[] OutputFormats { get { return TexturesModifyUtility.Filter; } }
        //保留原图片格式
        private bool _stayOldFormat = true;
        public bool StayOldFormat { get { return _stayOldFormat; } set { _stayOldFormat = value; OutputFormatIndex = value ? -1 : 0; Notify("StayOldFormat", "OutputFormatIndex"); } }
        //选择的输出格式下标
        private int _outputFormatIndex;
        public int OutputFormatIndex { get { return _outputFormatIndex; } set { _outputFormatIndex = value; _convertData.OutputFormat = TexturesModifyUtility.GetFormatByIndex(value); } }

        //==================缩放模式==================
        private string[] _scaleModes;// = new string[] { "不缩放", "直接缩放", "比例缩放", "直接裁剪", "比例裁剪", "基于宽度", "基于高度", "填充缩放", "POT缩放" };
        public string[] ScaleModes { get { return _scaleModes; } }
        //选择的缩放模式下标
        private int _scaleModeIndex;
        public int ScaleModeIndex { get { return _scaleModeIndex; } set { _scaleModeIndex = value; _convertData.ScaleMode = (EScaleMode)value; Notify("ScaleModeIndex", "ShowPixelSetting", "Width", "Height", "IsPotScaleMode"); PreviewOutputImage(); } }
        public bool IsPotScaleMode { get { return _convertData.ScaleMode == EScaleMode.POT || _convertData.ScaleMode == EScaleMode.POT_Cube; } }
        //============================================

        //==================POT 缩放算法==================
        private string[] _potMode;
        public string[] PotMode { get { return _potMode; } }
        //选择的POT算法下标
        private int _potModeIndex;
        public int PotModeIndex { get { return _potModeIndex; } set { _potModeIndex = value; _convertData.PotMode = (EPotMode)value; PreviewOutputImage(); } }
        public bool PotStayPixel { get { return _convertData.PotStayPixel; } set { _convertData.PotStayPixel = value; PreviewOutputImage(); } }
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
        public string LangResamplerAlgorithmTips { get { return "缩放算法将会影响图片的缩放质量，默认 Bicubic 就不错，如果想要更好的效果可以选 Lanczos8(但是更慢)"; } }
        public string LangOverideTips { get { return "该选项会直接覆盖源文件，并将源文件备份至『输出目录』"; } }
        private string[] _langScaleModeTips;
        public string[] LangScaleModeTips { get { return _langScaleModeTips; } }
        public string LangPotStayPixelTips { get { return "如果是放大操作，保持像素不变，否则以最短边进行填充"; } }
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
            //清空旧图
            _previewImageBytes = null;
            if (!string.IsNullOrEmpty(imagePath))
            {
                try
                {
                    _previewImageBytes = File.ReadAllBytes(imagePath);
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
            PreviewOutputBitmap = LoadImage(bytes);
        }

        public BitmapImage LoadImage(byte[] texBytes)
        {
            if (texBytes == null) return null;
            //释放旧图资源
            //if (bitmap.StreamSource != null)
            //    bitmap.StreamSource.Dispose();

            //加载图片
            //_inputPreviewBitmap.UriSource = new Uri(imagePath);
            //using (MemoryStream ms = new MemoryStream(texBytes))
            //{
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
            //}
            //释放原始资源
            //texStream.Dispose();
            //赋值UI
            //ui.Source = bitmap;
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
