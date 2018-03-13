using System;
using System.Net;
using System.Threading;

using Extant;

using WorldServer.Networking;
using SharedComponents.GameProperties;

namespace WorldServer
{
    class Program
    {
        static void Main(string[] args)
        {
            DebugLogger.GlobalDebug.MessageLogged += Console.WriteLine;

            WorldHost host = new WorldHost(new IPEndPoint(IPAddress.Any, 3000));
            host.Start();

            while(!host.IsStopped)
            {
                Thread.Sleep(100);
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }
    }
}
