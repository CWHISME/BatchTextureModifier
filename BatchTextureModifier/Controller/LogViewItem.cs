using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchTextureModifier
{
    public class LogViewItem
    {

        private LogItem _log;
        private Brush _color;

        public string Content { get { return _log.LogString; } }
        public Brush ForgroundColor { get { return _color; } }

        public LogViewItem(LogItem log)
        {
            _log = log;
            switch (_log.LogType)
            {
                case LogItem.ELogType.Warning:
                    _color = new SolidBrush(Color.Yellow);
                    break;
                case LogItem.ELogType.Error:
                    _color = new SolidBrush(Color.Red);
                    break;
                case LogItem.ELogType.Log:
                default:
                    _color = new SolidBrush(Color.Black);
                    break;
            }
        }
    }
}
