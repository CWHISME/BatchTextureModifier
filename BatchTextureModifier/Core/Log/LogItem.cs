//=========================================
//作者：wangjiaying@cwhisme
//日期：2023年1月10日
//描述：日志对象
//用途：https://github.com/CWHISME/BatchTextureModifier.git
//=========================================

using System;

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
            _logStr = DateTime.Now.ToString("[HH:mm:ss]") + str;
            _logType = type;
        }

        public enum ELogType { Log, Warning, Error }
    }
}
