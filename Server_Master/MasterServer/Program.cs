﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharedComponents.Global;
using SharedComponents.Server;
using MasterServer.Host;
using MasterServer.Links;

using Extant;

namespace MasterServer
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DebugLogger.Global.MessageLogged += Console.WriteLine;

            //ClientAcceptor
            ClientAcceptor clientAcceptor = new ClientAcceptor(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3000));

            //World servers
            WorldServer[] worldServers = new WorldServer[]
            {
                   new WorldServer(1, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000)),
                   new WorldServer(2, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001))
            };

            //General servers
            GeneralServer[] generalServers = new GeneralServer[]
            {
                   new GeneralServer(1, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500)),
                   new GeneralServer(2, new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4501), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4501))
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MasterServerWindow(clientAcceptor, worldServers, generalServers));
        }
    }
}
