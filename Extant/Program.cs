using System;
using System.Threading;
using System.Windows.Forms;

using Extant;

namespace GameServer
{
    static class Program
    {
        static void Main()
        {
            DebugLogger.GlobalDebug.MessageLogged += PostConsole;

            DebugLogger.GlobalDebug.LogBlank("Untitled Game Server\n" +
                                             "Pre-Alpha Stage v" + GameServer.Shared.GameVersion.Version + "\n" + 
                                             GameServer.Shared.GameVersion.VersionDescription + "\n" +
                                             "Created by Blake Scherschel\n" + 
                                             "-----------------------------");

            GameHandler handler = new GameHandler("127.0.0.1",3000,5);
            handler.Start();

            while (!handler.IsStopped)
            {
                
                Thread.Sleep(100);
            }
        }

        static void PostConsole(string s)
        {
            Console.WriteLine(s);
        }
    }
}
