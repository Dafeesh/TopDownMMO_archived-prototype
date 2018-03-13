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
        public static readonly DebugLogger Global = new DebugLogger("Global");
        /////////////

        private String sourceName;

        private List<String> log = new List<String>();
        private object log_lock = new object();

        public delegate void DebugLogMessageDelegate(String message);
        public event DebugLogMessageDelegate MessageLogged;

        public DebugLogger(String sourceName)
        {
            this.sourceName = sourceName;
        }

        public void Log(string s)
        {
            String m;
            lock (log_lock)
            {
                m = DateTime.Now.ToString("HH:mm:ss tt") +
                    "[" + sourceName + "]: " + s;

                log.Add(m);
            }

            if (MessageLogged != null)
                MessageLogged(m);
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
