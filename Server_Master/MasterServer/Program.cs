using System;
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

            //Zone servers
            InstanceServerLink[] zoneServers = new InstanceServerLink[]
            {
                   new InstanceServerLink(1, "ZoneServer0", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000)),
                   new InstanceServerLink(2, "ZoneServer1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001))
            };

            //Instance servers
            InstanceServerLink[] instanceServers = new InstanceServerLink[]
            {
                   new InstanceServerLink(3, "InstServer0", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500)),
                   new InstanceServerLink(4, "InstServer1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4501), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4501))
            };

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MasterServerWindow(clientAcceptor, zoneServers, instanceServers));
        }
    }
}
