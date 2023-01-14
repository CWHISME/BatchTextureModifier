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
                        return Brushes.Yellow;
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
