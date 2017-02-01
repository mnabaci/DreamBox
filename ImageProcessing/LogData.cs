using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ImageProcessing
{
    public enum LogType
    {
        [Description("Normal")]
        Normal,
        [Description("Uyarı")]
        Warning,
        [Description("Hata")]
        Error,
    }
    public class LogData
    {
        LogType _logType;
        public LogType LogType
        {
            get
            {
                return _logType;
            }
            set
            {
                _logType = value;
            }
        }
        DateTime _logTime;
        public DateTime LogTime
        {
            get
            {
                return _logTime;
            }
            private set
            {
                _logTime = value;
            }
        }
        string _logMessage;
        public string LogMessage
        {
            get
            {
                return _logMessage;
            }
            set
            {
                LogTime = DateTime.Now;
                _logMessage = value;
            }
        }
        public LogData(string logMessage, LogType logType = LogType.Normal)
        {
            LogMessage = logMessage;
            LogType = logType;
        }
    }

}
