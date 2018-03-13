using System;
using System.Net;
using System.Threading;

using Extant;

using WorldServer.Networking;
using SharedComponents.GameProperties;
using System.Windows.Forms;

namespace WorldServer
{
    class Program
    {
        static WorldHost host;
        static InstanceWatcher watcher = null;

        static void Main(string[] args)
        {
            DebugLogger.Global.MessageLogged += Console.WriteLine;

            host = new WorldHost(new IPEndPoint(IPAddress.Any, 3000));
            host.Start();

            while(!host.IsStopped)
            {
                if (Console.KeyAvailable)
                    if (Console.ReadKey(true).KeyChar == ' ')
                        if (!RunWatcher())
                            host.Stop();
                Thread.Sleep(100);
            }

            //Console.WriteLine("\nPress any key to continue...");
            //Console.ReadKey();
        }

        static bool RunWatcher()
        {
            if (watcher == null)
            {
                watcher = new InstanceWatcher(host.WorldController);
                Application.Run(watcher);
                watcher.Dispose();
                watcher = null;
                return false;
            }
            return true;
        }
    }
}
