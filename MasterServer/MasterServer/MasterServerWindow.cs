using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MasterServer.Host;
using System.Net;

namespace MasterServer
{
    public partial class MasterServerWindow : Form
    {
        private MasterServerHost host;

        public MasterServerWindow()
        {
            InitializeComponent();

            WorldServerLink[] gsLinks = new WorldServerLink[]
            {
                   new WorldServerLink(1, "Server1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001)),
                   new WorldServerLink(2, "Server2", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4002))
            };
            this.host = new MasterServerHost(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000), gsLinks);
            this.host.Start();

            listBox_GameServers.Items.Clear();
            foreach (WorldServerLink gsl in gsLinks)
            {
                listBox_GameServers.Items.Add(gsl);
            }
        }

        private void listBox_GameServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            WorldServerLink gsl = listBox_GameServers.SelectedItem as WorldServerLink;

            label_gsID.Text = gsl.ServerId.ToString();
            checkBox_gsIsConnected.Checked = gsl.IsConnected;
        }
    }
}
