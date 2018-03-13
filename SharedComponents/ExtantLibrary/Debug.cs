using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Extant
{
    /// <summary>
    /// Thread-safe class for saving debugging logs.
    /// </summary>
    public class DebugLogger
    {
        /////////////
        public static readonly DebugLogger Global = new DebugLogger();
        /////////////

        private List<String> log = new List<String>();
        private object log_lock = new object();

        public delegate void DebugLogMessageDelegate(String message);
        public event DebugLogMessageDelegate MessageLogged;
        public event DebugLogMessageDelegate WarningLogged;
        public event DebugLogMessageDelegate ErrorLogged;
        public event DebugLogMessageDelegate AnyLogged;

        public DebugLogger()
        {  }

        private String MakeLog(String title, String s)
        {
            String m;
            lock (log_lock)
            {
                m = DateTime.Now.ToString("HH:mm:ss tt");
                if (title != String.Empty)
                    m += "[" + title + "]";
                m += ": " + s;

                log.Add(m);
            }
            return m;
        }

        public void Log(string s)
        {
            string l = MakeLog(String.Empty, s);
            if (MessageLogged != null)
                MessageLogged(l);
            if (AnyLogged != null)
                AnyLogged(l);
        }

        public void LogWarning(string s)
        {
            string l = MakeLog("Warning", s);
            if (WarningLogged != null)
                WarningLogged(l);
            if (AnyLogged != null)
                AnyLogged(l);
        }

        public void LogError(string s)
        {
            string l = MakeLog("ERROR", s);
            if (ErrorLogged != null)
                ErrorLogged(l);
            if (AnyLogged != null)
                AnyLogged(l);
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
