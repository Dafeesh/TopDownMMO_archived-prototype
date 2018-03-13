using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using Extant;

using SharedComponents.Global.GameProperties;
using InstanceServer.Control;
using InstanceServer.Links;

namespace InstanceServer
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            //MasterServerLink
            IPEndPoint msLink_hostLocal = new IPEndPoint(IPAddress.Loopback, 4000);
            IPAddress msLink_expectedAddress = IPAddress.Loopback;
            MasterServerLink msLink = new MasterServerLink(msLink_hostLocal, msLink_expectedAddress);

            //ClientAcceptor
            IPEndPoint clientAcceptor_hostLocal = new IPEndPoint(IPAddress.Loopback, 5000);
            ClientAccepter clientAcceptor = new ClientAccepter(clientAcceptor_hostLocal);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstanceServerHostWindow(clientAcceptor, msLink));
        }
    }
}
