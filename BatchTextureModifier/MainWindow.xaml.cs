using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BatchTextureModifier
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private ViewHelper _helper;

        private ImageCompareViewWindow? _imageCompareView;
        private WindowState _imageCompareViewState;

        public MainWindow()
        {
            InitializeComponent();

            _helper = new ViewHelper();
            DataContext = _helper;

            LogViewer.ScrollChanged += OnViewerChange;
        }

        private void OnViewerChange(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange <= 0) return;
            LogViewer.ScrollToEnd();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _imageCompareView?.Close();
        }

        private void DoProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            OutPutTabControl.SelectedIndex = 1;
            BatchProgressView view = new BatchProgressView();
            view.Owner = this;
            view.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            view.DataContext = _helper;
            _helper.StartBatchModify(() =>
            {
                try
                {
                    view.ForceClose();
                }
                catch (Exception ex)
                {
                    LogManager.GetInstance.LogError(ex.Message);
                }
            });
            if (_helper.CanBatchProcess) return;
            try
            {
                view.ShowDialog();
                this.Activate();
            }
            catch (Exception ex)
            {
                LogManager.GetInstance.LogError(ex.Message);
            }
            //SyncTextures();
        }

        /// <summary>
        /// 选择输入目录按钮
        /// </summary>
        private void ChooseInputPathBtn_Click(object sender, RoutedEventArgs e)
        {
            _helper.SelectInputPath();// (TexturesInputPathText, PreviewInputPathImage);
        }

        /// <summary>
        /// 选择输出目录按钮
        /// </summary>
        private void ChooseOutputPathBtn_Click(object sender, RoutedEventArgs e)
        {
            //_helper.SelectPath(TexturesOutputPathText);
            _helper.SelectOutputtPath();
        }

        /// <summary>
        /// 限制只能输入数字——这个只能限制英文键盘，聊胜于无
        /// </summary>
        private void OnPreviewTextInputLimitNumber(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int x);
        }

        /// <summary>
        /// 最新一个日志按钮
        /// </summary>
        private void NewestLogBtn_Click(object sender, RoutedEventArgs e)
        {
            OutPutTabControl.SelectedIndex = 1;
        }

        /// <summary>
        /// 关于按钮
        /// </summary>
        private void AboutBtn_Click(object sender, RoutedEventArgs e)
        {
            _helper.DisplayAboutInfo();
        }

        /// <summary>
        /// Git 按钮
        /// </summary>
        private void OpensourceBtn_Click(object sender, RoutedEventArgs e)
        {
            _helper.DisplayGitInfo();
        }

        ///// <summary>
        ///// 改变输入路径
        ///// </summary>
        //private void OnInputPathChange(object sender, TextChangedEventArgs e)
        //{
        //    ValidePath(TexturesInputPathText.Text, OpenInputPathBtn);
        //}

        ///// <summary>
        ///// 改变输出路径
        ///// </summary>
        //private void OnOutputPathChange(object sender, TextChangedEventArgs e)
        //{
        //    ValidePath(TexturesOutputPathText.Text, OpenOutputPathBtn);
        //}

        //private void ValidePath(string path, Button effectBtn)
        //{
        //    if (Directory.Exists(path))
        //    {
        //        effectBtn.IsEnabled = true;
        //    }
        //    else
        //    {
        //        effectBtn.IsEnabled = false;
        //    }
        //}

        private void OpenInputPathBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", TexturesInputPathText.Text);
        }

        private void OpenOutputPathBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", TexturesOutputPathText.Text);
        }

        private void OpenImageCompareViewBtn_Click(object sender, RoutedEventArgs e)
        {
            if (_imageCompareView == null)
            {
                _imageCompareView = new ImageCompareViewWindow();
                _imageCompareView.DataContext = _helper;
                _imageCompareView.Closed += (o, x) => _imageCompareView = null;
                _imageCompareView.StateChanged += (o, x) => _imageCompareViewState = (o as Window)?.WindowState == WindowState.Minimized ? _imageCompareViewState : (o as Window)?.WindowState ?? WindowState.Normal;
                _imageCompareViewState = _imageCompareView.WindowState;
                _imageCompareView.Show();
                return;
            }
            _imageCompareView.WindowState = _imageCompareViewState;
            _imageCompareView.Activate();
        }

        private void ChooseSingleImageBtn_Click(object sender, RoutedEventArgs e)
        {
            _helper.SelectSingleImage();
        }

        private void SaveSingleProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            //保存文件
            _helper.SaveSinglePreviewImage();
        }

        private void SyncTextures()
        {
            //根据名字同步两个目录的图片
            //原始目录
            string pathOrigin = @"E:\项目\Blog\HexoBlog\Hexo\source\images\coverimages\large";
            //会被修改的目录
            string pathModify = @"E:\项目\Blog\BlogTexturesBackup\large_";
            //pathModify 中多余的图片会被删除

            string[] orgs = Directory.GetFiles(pathOrigin);
            string[] mods = Directory.GetFiles(pathModify);
            foreach (var item in mods)
            {
                string name = System.IO.Path.GetFileNameWithoutExtension(item);
                if (Array.FindIndex(orgs, x => name == System.IO.Path.GetFileNameWithoutExtension(x)) == -1)
                {
                    File.Delete(item);
                    LogManager.GetInstance.Log("删除：" + item);
                }
            }
        }
    }
}
