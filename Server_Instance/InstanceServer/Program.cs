using System;
using System.Net;
using System.Threading;
using System.Windows.Forms;

using Extant;

using SharedComponents.Global.GameProperties;

namespace InstanceServer
{
    class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new InstanceServerHostWindow());
        }
    }
}
