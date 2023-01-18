//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月10日
//描述：与 WPF 耦合的日志对象，封装实际日志对象类
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System.Windows.Media;

namespace BatchTextureModifier
{
    public class LogViewItem
    {

        private LogItem _log;

        public string Content { get { return _log.LogString; } }
        public Brush ForgroundColor
        {
            get
            {
                switch (_log.LogType)
                {
                    case LogItem.ELogType.Warning:
                        return Brushes.Orange;
                    case LogItem.ELogType.Error:
                        return Brushes.Red;
                    case LogItem.ELogType.Log:
                    default:
                        return Brushes.Black;
                }
            }
        }

        public LogViewItem(LogItem log)
        {
            _log = log;
        }
    }
}
