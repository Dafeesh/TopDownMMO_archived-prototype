﻿using System;
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

            DebugLogger.GlobalDebug.LogNetworking("Untitled Game Server\nPre-Alpha Stage\nCreated by Blake Scherschel\n-----------------------------");

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
