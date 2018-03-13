/* ThreadRun.cs
 * Author: Blake Scherschel
 * 
 *   Class used as a means of cleanly running a process on
 * another thread in one object.
 * 
 *   The author just needs to override three functions:
 * Begin() -> Called when the object is instructed to begin running.
 * RunLoop() -> A non-halting function that is called between iterations of defined time periods.
 * Finish() -> Called when the thread has ended either by an exception or by instruction.
 * These functions all operate on the same thread so they are thread safe.
 */

using System;
using System.Collections.Generic;
using System.Threading;

//TEST
namespace Extant
{
    //Base class used as a framework for classes that will run on a separate
    //thread. When inheritted, the inheritting class just writes a RunLoop method
    //that will be called until the requested to stop.
    public abstract class ThreadRun
    {
        private static Int32 runningIDIterator = 0;
        private static object runningIDIterator_lock = new object();

        private Thread thisThread;
        private bool thisThread_killSwitch;
        private Int32 thisThread_pauseDelay;
        private Int32 runningID;

        private String errorMessage = "No error.";

        /// <summary>
        /// Creates and starts a thread to run an inheritted class.
        /// </summary>
        /// <param name="threadName">Name of the thread.</param>
        /// <param name="pauseDelay">Amount to pause after every RunLoop.</param>
        public ThreadRun(String threadName, Int32 pauseDelay = 1)
        {
            //PauseDelay
            if (pauseDelay < 0)
                throw new ArgumentException("PauseDelay cannot be less than zero.");
            thisThread_pauseDelay = pauseDelay;

            //Get a running ID
            lock (runningIDIterator_lock)
            {
                runningID = ++runningIDIterator;
            }

            //Create Thread
            thisThread = new Thread(new ThreadStart(Run));
            thisThread.Name = "<" + threadName + ":" + runningID.ToString() + ">";
            thisThread_killSwitch = false;
        }

        private void Run()
        {
            Begin();
            while (!thisThread_killSwitch)
            {
                try
                {
                    RunLoop();
                }
                catch (Exception e)
                {
                    DebugLogger.GlobalDebug.LogError("ThreadRun experienced an unexpected Exception! (" + this.RunningID + ")\n" + e.ToString());
                    errorMessage = "Unhandled exception:\n" + e.ToString() + "\n-";
                    this.Stop();
                }
                Thread.Sleep(thisThread_pauseDelay);
            }
            Finish();
        }

        /////////// Override these ///////////

        /// <summary>
        /// Called when thread instructed to start.
        /// </summary>
        protected abstract void Begin();
        /// <summary>
        /// Non-halting function used as the loop call.
        /// </summary>
        protected abstract void RunLoop();
        /// <summary>
        /// Called when thread fails via instruction or unhandled exception.
        /// </summary>
        protected abstract void Finish();

        /////////// ~Override these ///////////


        /// <summary>
        /// Starts the thread controlling this ThreadRun class.
        /// </summary>
        public void Start()
        {
            if (thisThread.ThreadState == ThreadState.Running)
                throw new ThreadStateException("ThreadRun has already been started! " + this.runningID.ToString());
            thisThread.Start();
        }

        /// <summary>
        /// Stops the thread running this object.
        /// </summary>
        public void Stop()
        {
            //if (thisThread.ThreadState == ThreadState.Stopped)
            //    throw new ThreadStateException("ThreadRun has already been stopped! " + this.runningID.ToString());

            thisThread_killSwitch = true;
        }

        /// <summary>
        /// Returns if the thread as been requested to stop.
        /// </summary>
        public bool IsStopped
        {
            get
            {
                return thisThread_killSwitch;
            }
        }

        /// <summary>
        /// Returns the unique thread ID.
        /// </summary>
        public Int32 RunningID
        {
            get
            {
                return runningID;
            }
        }

        /// <summary>
        /// Returns the error message saved if the thread failed.
        /// </summary>
        public String ErrorMessage
        {
            get
            {
                return errorMessage;
            }
        }
    }
}
