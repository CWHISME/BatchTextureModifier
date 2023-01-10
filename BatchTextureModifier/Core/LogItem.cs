using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchTextureModifier
{
    public class LogItem
    {
        private string _logStr;
        public string LogString { get { return _logStr; } }
        private ELogType _logType;
        public ELogType LogType { get { return _logType; } }

        public LogItem(ELogType type, string str)
        {
            _logStr = str;
            _logType = type;
        }

        public enum ELogType { Log, Warning, Error }
    }
}
