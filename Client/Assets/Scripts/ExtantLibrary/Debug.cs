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
        public static readonly DebugLogger GlobalDebug = new DebugLogger();
        /////////////

        private List<String> log = new List<String>();
        private object thisLock = new object();

        public delegate void DebugLogMessageDelegate(String message);
        public event DebugLogMessageDelegate MessageLogged;

        public DebugLogger()
        {  }

        private void Log(String title, String s)
        {
            lock (thisLock)
            {
                String m = DateTime.Now.ToString("HH:mm:ss tt") + "[" + title + "]: " + s;
                log.Add(m);
                MessageLogged(m);
            }
        }

        public void LogBlank(String m)
        {
            lock (thisLock)
            {
                log.Add(m);
                MessageLogged(m);
            }
        }

        public void LogGame(String gameId, long gt, String s)
        {
            Log("Game:" + gameId + ":" + gt / 1000, s);
        }

        public void LogThisGame(String s)
        {
            Log("Game", s);
        }

        public void LogCatch(String s)
        {
            Log("Catch", s);
        }

        public void LogError(String s)
        {
            Log("Error", s);
        }

        public void LogNetworking(String s)
        {
            Log("Net", s);
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
