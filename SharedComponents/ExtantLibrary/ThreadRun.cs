/* ThreadRun.cs
 * Author: Blake Scherschel
 * Last Update: 2015/2/3
 * 
 * Purpose: Class used as a means of cleanly running a process on
 * a separate thread wrapped into one object.
 * 
 *   The author just needs to inherit ThreadRun and override three functions:
 * Begin() -> Called when the object is instructed to begin running.
 * RunLoop() -> A non-halting function that is called between iterations of defined time periods.
 * Finish() -> Called when the thread has ended either by an exception or by instruction.
 * These functions all operate on the same thread so they are thread safe.
 * 
 * Notes:
 * -Avoid intensive processes in the constructor. Instead, use Begin.
 * -For functions that are not thread safe, use the public Invoke function.
 * -After instantiation of this class, use Start and Stop to dictate when it executes.
 * -If an object throws an UnhandledException then the Exception is saved in  the public 
 *  UnhandledException and the OnHandledException event is executed.
 */

using System;
using System.Collections.Generic;
using System.Threading;

namespace Extant
{
    //Base class used as a framework for classes that will run on a separate
    //thread. When inheritted, the inheritting class must override a RunLoop method
    //that will be called until requested to stop.
    public abstract class ThreadRun : IDisposable , ILogging
    {
        private static Int32 runningIDIterator = 0;
        private static object runningIDIterator_lock = new object();

        private Thread thisThread;
        private object thisThread_lock = new object();
        private bool thisThread_killSwitch;
        private Int32 thisThread_pauseDelay;
        private Int32 runningID;

        private List< ThreadTask<object> > invoked_ThreadTask = new List< ThreadTask<object> >();
        private List<Action> invoked_Action = new List<Action>();
        private object invoked_lock = new object();

        private Exception unhandledException = null;
        private String stopMessage = String.Empty;
        private Boolean isDisposed = false;

        private DebugLogger log = new DebugLogger();

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
            thisThread.Name = "<" + threadName + "::" + runningID.ToString() + ">";
            thisThread_killSwitch = false;
        }

        ~ThreadRun()
        {
            this.Stop("Destructor.");
            Dispose();
        }

        /// <summary>
        /// Main run function for this thread.
        /// </summary>
        private void Run()
        {
            Begin();
            while (!thisThread_killSwitch)
            {
                try
                {
                    //Run main loop.
                    lock (thisThread_lock)
                    {
                        RunLoop();
                    }

                    //Execute all invoked methods.
                    lock (invoked_lock)
                    {
                        //ThreadTasks
                        foreach (ThreadTask<object> a in invoked_ThreadTask)
                        {
                            a.Start();
                        }
                        invoked_ThreadTask.Clear();
                        //Actions
                        foreach (Action a in invoked_Action)
                        {
                            a();
                        }
                        invoked_Action.Clear();
                    }
                }
                catch (Exception e)
                {
                    log.LogWarning("ThreadRun experienced an unexpected Exception! (" + this.RunningID + ")\n" + e.ToString() + "\n-");
                    unhandledException = e;
                    this.Stop("Unhandled exception.");
                    if (OnUnhandledException != null)
                        OnUnhandledException(this, new UnhandledExceptionEventArgs(e, true));
                }
                Thread.Sleep(thisThread_pauseDelay);
            }
            if (stopMessage == String.Empty)
                stopMessage = "Finished successfully.";
            Finish((unhandledException == null));
        }

        #region Override these functions!

        /// <summary>
        /// Called when thread instructed to start.
        /// </summary>
        protected virtual void Begin()
        {
            //Begin method is not necessary.
        }

        /// <summary>
        /// Non-halting function used as the loop call.
        /// </summary>
        protected abstract void RunLoop();

        /// <summary>
        /// Called when thread is finished, fails via instruction, or unhandled exception.
        /// </summary>
        /// <param name="success">Is true when the thread finished without any errors.</param>
        protected virtual void Finish(bool success)
        {
            //Finish method is not necessary.
        }

        protected virtual void OnDispose()
        {
            //Triggered after the thread is stopped and disposed.
        }

        #endregion Override these functions!

        /// <summary>
        /// Starts the thread controlling this ThreadRun class.
        /// </summary>
        public void Start()
        {
            if (thisThread.ThreadState == ThreadState.Running || thisThread_killSwitch == true)
                throw new InvalidOperationException("ThreadRun has already been started! " + this.runningID.ToString());
            thisThread.Start();
        }

        /// <summary>
        /// Instructs the thread to stop.
        /// </summary>
        public void Stop(String reason)
        {
            if (thisThread_killSwitch == false)
            {
                thisThread_killSwitch = true;

                stopMessage = reason;
                log.LogError("Stopped with this reason: " + reason);
            }
        }

        /// <summary>
        /// Instructs the thread to stop and clean up.
        /// </summary>
        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// Instructs the thread to stop and clean up.
        /// </summary>
        /// <param name="waitForDispose">If this thread to wait for the object's thread to dispose.</param>
        public void Dispose(Boolean waitForDispose)
        {
            if (!isDisposed)
            {
                if (thisThread_killSwitch == false)
                {
                    this.Stop("Disposed.");
                }

                this.isDisposed = true;

                if (waitForDispose)
                {
                    thisThread.Join();
                }
            }
        }

        /// <summary>
        /// Adds a Task to be executed during this object's main thread.
        /// </summary>
        /// <param name="a">ThreadTask to be run on this object's main thread.</param>
        public void Invoke(ThreadTask<object> a)
        {
            lock (invoked_lock)
            {
                invoked_ThreadTask.Add(a);
            }
        }

        /// <summary>
        /// Adds an Action to be executed during this object's main thread.
        /// </summary>
        /// <param name="a">Action to be run on this object's main thread.</param>
        public void Invoke(Action a)
        {
            lock (invoked_lock)
            {
                invoked_Action.Add(a);
            }
        }

        /// <summary>
        /// Subscribed to this object's log.
        /// </summary>
        /// <param name="func"></param>
        public void SubscribeToLogs(DebugLogger.DebugLogMessageDelegate func)
        {
            this.Log.AnyLogged += func;
        }

        /// <summary>
        /// Returns the log for this object.
        /// </summary>
        public DebugLogger Log
        {
            get
            {
                return log;
            }
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
        /// Lock to block execution until RunLoop is finished.
        /// Prefer Invoke(..) over this approach.
        /// </summary>
        protected object ThreadLock
        {
            get
            {
                return thisThread_lock;
            }
        }

        /// <summary>
        /// If the thread throws an UnhandledException, it is stored here.
        /// </summary>
        public Exception UnhandledException
        {
            get
            {
                return unhandledException;
            }
        }

        /// <summary>
        /// Code that symbolizes how the thread stopped.
        /// </summary>
        public String StopMessage
        {
            get
            {
                return stopMessage;
            }
        }

        /// <summary>
        /// Executed when the thread throws an UnhandledException and
        /// does not gracefully finish.
        /// </summary>
        public event UnhandledExceptionEventHandler OnUnhandledException;

        /// <summary>
        /// Code that symbolizes how the thread stopped.
        /// </summary>
        public enum ThreadStopCode
        {
            Null,

            Default,
            Success,
            Failure
        }
    }
}
