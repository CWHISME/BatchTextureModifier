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

        public MainWindow()
        {
            InitializeComponent();

            _helper = new ViewHelper();
            DataContext = _helper;

            //输出格式选择
            //OutputFormatComboBox.ItemsSource = TexturesConvertUtility.Filter;
            //OutputFormatComboBox.SelectedIndex = 0;
            //OutputFormatComboBox.

            //保持图片输入格式
            //StayInputFormatToggle.Checked += OnStayInputFormatToggleCheck;
            //StayInputFormatToggle.Unchecked += OnStayInputFormatToggleCheck;
            //OnStayInputFormatToggleCheck(null, null);
        }

        //private void OnStayInputFormatToggleCheck(object sender, RoutedEventArgs e)
        //{
        //    //bool stayCheck = (bool)StayInputFormatToggle.IsChecked;
        //    OutputFormatComboBox.Visibility = (bool)StayInputFormatToggle.IsChecked ? Visibility.Collapsed : Visibility.Visible;
        //    //if (stayCheck)
        //    //{
        //    //    OutputFormatComboBox.Text = "?";
        //    //}
        //    //else OutputFormatComboBox.SelectedIndex = 0;

        //}

        private void DoProcessBtn_Click(object sender, RoutedEventArgs e)
        {
            //TexturesCutCore.ResizeTextures(@"D:\MyOthers\BlogTexturesBackup\新建文件夹\6833939bly1giciusoyjnj219g0u0x56.jpg");
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

        /// <summary>
        /// 改变输入路径
        /// </summary>
        private void OnInputPathChange(object sender, TextChangedEventArgs e)
        {
            ValidePath(TexturesInputPathText.Text, OpenInputPathBtn);
        }

        /// <summary>
        /// 改变输出路径
        /// </summary>
        private void OnOutputPathChange(object sender, TextChangedEventArgs e)
        {
            ValidePath(TexturesOutputPathText.Text, OpenOutputPathBtn);
        }

        private void ValidePath(string path, Button effectBtn)
        {
            if (Directory.Exists(path))
            {
                effectBtn.IsEnabled = true;
            }
            else
            {
                effectBtn.IsEnabled = false;
            }
        }

        private void OpenInputPathBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", TexturesInputPathText.Text);
        }

        private void OpenOutputPathBtn_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("explorer.exe", TexturesOutputPathText.Text);
        }

        private void ScaleWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_helper.ConvertData.Width = _helper.CheckTextNumberInput(ScaleWidth);
            //_helper.PreviewOutputImage(PreviewOutputImage);
        }

        private void ScaleHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            //_helper.ConvertData.Height = _helper.CheckTextNumberInput(ScaleHeight);
            //_helper.PreviewOutputImage(PreviewOutputImage);
        }
    }
}
