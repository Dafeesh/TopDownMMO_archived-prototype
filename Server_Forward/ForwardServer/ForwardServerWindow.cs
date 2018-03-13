using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

using MasterServer.Links;

namespace ForwardServer
{
    public partial class ForwardServerWindow : Form
    {
        ForwardServerHost forwardServerHost;

        public ForwardServerWindow()
        {
            InitializeComponent();

            //Start host
            IPEndPoint clientLocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3990);

            IPEndPoint masterLocalEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3991);
            IPAddress expectedMasterIPAddress = IPAddress.Parse("127.0.0.1");
            MasterServerLink masterServerLink = new MasterServerLink(masterLocalEndPoint, expectedMasterIPAddress);
            masterServerLink.OnStateChange += OnMasterServerStateChange;
            masterServerLink.Start();

            forwardServerHost = new ForwardServerHost(clientLocalEndPoint, masterServerLink);
            forwardServerHost.Start();
        }

        private void ForwardServerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            forwardServerHost.Stop("Window closed.");
        }

        public void OnMasterServerStateChange(MasterServerLink msl)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                checkBox_ConToMaster.Checked = msl.ConnectedState;
            }));
        }
    }
}
