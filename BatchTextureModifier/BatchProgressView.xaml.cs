using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BatchTextureModifier
{
    /// <summary>
    /// BatchProgressView.xaml 的交互逻辑
    /// </summary>
    public partial class BatchProgressView : Window
    {
        public BatchProgressView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            if (!TexturesModifyUtility.RegisterCancelCallback(ForceClose))
                ForceClose();
            LogViewer.ScrollChanged += OnViewerChange;
        }

        private void OnViewerChange(object sender, ScrollChangedEventArgs e)
        {
            if (e.ExtentHeightChange <= 0) return;
            LogViewer.ScrollToEnd();
        }

        private bool _autoClose = false;
        private void ForceClose()
        {
            _autoClose = true;
            Close();
        }

        private void CancelBatch_Click(object sender, RoutedEventArgs e)
        {
            if (CheckCancel()) Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!CheckCancel()) e.Cancel = true;
        }

        private bool CheckCancel()
        {
            if (_autoClose) return true;
            if (MessageBox.Show("是否确定取消处理？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                TexturesModifyUtility.CancelBatch();
                return true;
            }
            return false;
        }
    }
}
