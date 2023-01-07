﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace BatchTextureModifier
{
    internal class ViewHelper : System.ComponentModel.INotifyPropertyChanged
    {

        //界面
        //private MainWindow _view;
        //public MainWindow View { get { return _view; } }
        //数据
        private TexturesModifyData _convertData = new TexturesModifyData();
        //预览图片
        private byte[]? _previewImageBytes;

        #region 数据绑定
        public int Width { get { return _convertData.Width; } set { _convertData.Width = CheckHeightWidthOutbound(value); Notify("Width"); PreviewOutputImage(); } }
        public int Height { get { return _convertData.Height; } set { _convertData.Height = CheckHeightWidthOutbound(value); Notify("Height"); PreviewOutputImage(); } }
        public string? InputPath { get { return _convertData.InputPath; } set { _convertData.InputPath = value; PreviewInputPathImage(value); Notify("InputPath", "HasExistInputPath"); } }
        public string? OutputPath { get { return _convertData.OutputPath; } set { _convertData.OutputPath = value; Notify("OutputPath", "HasExistOutputPath"); } }

        //输出格式列表
        public string[] OutputFormats { get { return TexturesModifyUtility.Filter; } }
        //选择的输出格式下标
        private int _outputFormatIndex;
        public int OutputFormatIndex { get { return _outputFormatIndex; } set { _outputFormatIndex = value; _convertData.OutputFormat = TexturesModifyUtility.GetFormatByIndex(value); } }

        //缩放模式
        private string[] _scaleModes = new string[] { "不缩放", "直接缩放", "基于宽度", "基于高度" };
        public string[] ScaleModes { get { return _scaleModes; } }
        //选择的缩放模式下标
        private int _scaleModeIndex;
        public int ScaleModeIndex { get { return _scaleModeIndex; } set { _scaleModeIndex = value; _convertData.ScaleMode = (EScaleMode)value; Notify("ScaleModeIndex"); } }
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
        #endregion
        //public ViewHelper()
        //{
        //    _view = view;
        //}

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

        public void DisplayAboutInfo()
        {
            if (MessageBox.Show("批量图片处理工具 \n\nMade by wangjiaying\n个人博客：wangjiaying.top\n\n=>点击确认前往<=", "关于", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start("explorer.exe", "https://wangjiaying.top");
            }
        }

        public void DisplayGitInfo()
        {
            if (MessageBox.Show("Git地址：wangjiaying.top\n(点个赞吧('ω'))\n\n=>点击确认前往<=", "源码地址", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start("explorer.exe", "https://wangjiaying.top");
            }
        }

        //==================Private=====================

        #region Private
        private string SelectPath()
        {
            //Microsoft.Win32.OpenFileDialog fileDialog = new Microsoft.Win32.OpenFileDialog();
            //fileDialog.CheckFileExists = false;
            //if ((bool)fileDialog.ShowDialog())
            //{
            //    TexturesOriginPathText.Text = fileDialog.FileName;
            //}
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
        private void PreviewInputPathImage(string? texturePath)
        {
            if (string.IsNullOrEmpty(OutputPath))
                OutputPath = texturePath + "_Output";
            //获取输入目录第一张图作为预览图
            string imagePath = TexturesModifyUtility.GetDirectoryFirstTextures(texturePath);
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }
            _previewImageBytes = File.ReadAllBytes(imagePath);
            if (_previewImageBytes == null)
            {
                PreviewInputBitmap = null;
                PreviewOutputBitmap = null;
            }
            else
            {
                //加载预览的原图
                PreviewInputBitmap = LoadImage(_previewImageBytes);
                //检测并显示输出结果图
                PreviewOutputImage();
            }
        }

        /// <summary>
        /// 检测是否显示被修改后的预览图
        /// </summary>
        public void PreviewOutputImage()
        {
            if (_previewImageBytes == null) return;
            byte[] bytes;
            try
            {
                bytes = TexturesModifyUtility.ResizeTextures(_previewImageBytes, _convertData);
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                return;
            }
            PreviewOutputBitmap = LoadImage(bytes);
        }

        public BitmapImage LoadImage(byte[] texBytes)
        {
            //释放旧图资源
            //if (bitmap.StreamSource != null)
            //    bitmap.StreamSource.Dispose();

            //加载图片
            //_inputPreviewBitmap.UriSource = new Uri(imagePath);
            //using (MemoryStream ms = new MemoryStream(texBytes))
            //{
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = new MemoryStream(texBytes);
            bitmap.EndInit();
            return bitmap;
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
        #endregion
    }
}
