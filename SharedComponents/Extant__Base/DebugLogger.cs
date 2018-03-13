using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Extant
{
    /// <summary>
    /// Thread-safe class for saving debugging logs.
    /// </summary>
    public class DebugLogger
    {
        /////////////
        public static readonly DebugLogger Global;
        static DebugLogger()
        { Global = new DebugLogger("Global"); }
        /////////////

        private String sourceName;

        private List<LogItem> log = new List<LogItem>();
        private object log_lock = new object();

        public delegate void DebugLogMessageDelegate(String message);
        public event DebugLogMessageDelegate MessageLogged;
        public event DebugLogMessageDelegate WarningMessageLogged;
        public event DebugLogMessageDelegate ErrorMessageLogged;

        public DebugLogger(String sourceName)
        {
            this.sourceName = sourceName;
        }

        public void Log(string s)
        {
            if (String.IsNullOrEmpty(s))
                return;

            _Log(new LogItem(DateTime.Now, sourceName, LogItem.LogItemType.Normal, s));
        }
        public void LogWarning(string s)
        {
            if (String.IsNullOrEmpty(s))
                return;

            _Log(new LogItem(DateTime.Now, sourceName, LogItem.LogItemType.Warning, s));
        }

        public void LogError(string s)
        {
            if (String.IsNullOrEmpty(s))
                return;

            _Log(new LogItem(DateTime.Now, sourceName, LogItem.LogItemType.Error, s));
        }

        private void _Log(LogItem li)
        {
            lock (log_lock)
            {
                log.Add(li);

                if (MessageLogged != null)
                    MessageLogged(li.ToString());

                switch (li.LogType)
                {
                    case (LogItem.LogItemType.Warning):
                        if (WarningMessageLogged != null)
                            WarningMessageLogged(li.ToString());
                        break;

                    case (LogItem.LogItemType.Error):
                        if (ErrorMessageLogged != null)
                            ErrorMessageLogged(li.ToString());
                        break;
                }
            }
        }

        public LogItem[] GetLog(int numLines)
        {
            lock (log_lock)
            {
                if (numLines <= 0)
                    return new LogItem[0];

                if (numLines > log.Count)
                    numLines = log.Count;

                return log.Take(numLines).ToArray();
            }
        }
    }

    public class LogItem
    {
        public DateTime Time;
        public String Source;
        public LogItemType LogType;
        public String Message;

        public LogItem(DateTime time, String source, LogItemType logType, String message)
        {
            this.Time = time;
            this.Source = source;
            this.LogType = logType;
            this.Message = message;
        }

        public override string ToString()
        {
            switch (this.LogType)
            {
                default: //Normal
                    return Time.ToString("HH:mm:ss tt") + "[" + Source + "]: " + Message;

                case (LogItemType.Warning):
                    return Time.ToString("HH:mm:ss tt") + "<Warning> [" + Source + "]: " + Message;

                case (LogItemType.Error):
                    return Time.ToString("HH:mm:ss tt") + "<ERROR> [" + Source + "]: " + Message;
            }
        }

        public enum LogItemType
        {
            Normal,
            Warning,
            Error
        }
    }

    /// <summary>
    /// Used to record byte usage and returns analytics about the data.
    /// </summary>
    public class ByteRecord
    {
        /////////////
        public static readonly ByteRecord GlobalByteRecord_In = new ByteRecord();
        public static readonly ByteRecord GlobalByteRecord_Out = new ByteRecord();
        /////////////

        private Stopwatch elapsed;
        private int bytes;
        private int bytes_last; //Used to signify the last record of total bytes to calculate BPS.
        private float kiloBytesPerSecond;

        public ByteRecord(int b = 0)
        {
            elapsed = new Stopwatch();
            elapsed.Start();

            bytes = b;
            bytes_last = bytes;
            kiloBytesPerSecond = 0;
        }

        public int Bytes
        {
            set
            {
                bytes = value;
            }

            get
            {
                return bytes;
            }
        }

        public float KiloBytesPerSecond
        {
            get
            {
                if (elapsed.ElapsedMilliseconds > 1000)
                {
                    kiloBytesPerSecond = ((bytes - bytes_last) / 1024.0f) / (elapsed.ElapsedMilliseconds * 1000.0f);

                    bytes_last = bytes;
                    elapsed.Reset();
                    elapsed.Start();
                }
                return kiloBytesPerSecond;
            }
        }
    }
}
