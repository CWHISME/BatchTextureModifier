using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BatchTextureModifier.LogItem;

namespace BatchTextureModifier
{
    public class LogManager
    {

        private static LogManager _instance;
        private static readonly object _locker = new object();
        private static SpinLock _spinLocker = new SpinLock();
        public static LogManager GetInstance
        {
            get
            {
                if (_instance == null)
                    lock (_locker)
                    {
                        if (_instance == null) _instance = new LogManager();
                        return _instance;
                    }
                return _instance;
            }
        }

        private List<LogItem> _logItems = new List<LogItem>(30);
        public List<LogItem> LogItems { get { return _logItems; } }

        /// <summary>
        /// 日志改变事件，bool 表示是否是错误信息，可以进行后续处理如是否弹窗提示
        /// </summary>
        public event Action<LogItem> OnLogChange;
        public void Log(string str)
        {
            AddLog(ELogType.Log, str);
        }

        public void LogWarning(string str)
        {
            AddLog(ELogType.Warning, str);
        }

        public void LogError(string str)
        {
            AddLog(ELogType.Error, str);
        }

        public void ClearLog()
        {
            bool islock = false;
            _spinLocker.Enter(ref islock);
            _logItems.Clear();
            OnLogChange?.Invoke(new LogItem(ELogType.Log, "Clear Log!"));
            _spinLocker.Exit();
        }

        private void AddLog(ELogType type, string str)
        {
            bool islock = false;
            _spinLocker.Enter(ref islock);
            LogItem item = new LogItem(type, str);
            _logItems.Add(item);
            OnLogChange?.Invoke(item);
            _spinLocker.Exit();
        }
    }
}
