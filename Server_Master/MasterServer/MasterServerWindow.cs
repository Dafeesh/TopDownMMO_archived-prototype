using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using MasterServer.Links;
using System.Net;

namespace MasterServer
{
    public partial class MasterServerWindow : Form
    {
        private MasterServerHost host;

        public MasterServerWindow()
        {
            InitializeComponent();

            //Forward server
            ForwardServerLink forwardServer = new ForwardServerLink(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 3991));
            forwardServer.OnStateChange += ForwardServerStateChanged;

            //Zone servers
            InstanceServerLink[] zoneServers = new InstanceServerLink[]
            {
                   new InstanceServerLink(1, "ZoneServer0", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000)),
                   new InstanceServerLink(1, "ZoneServer1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4001))
            };
            listBox_ZoneSevers.Items.Clear();
            foreach (var z in zoneServers)
            {
                z.OnStateChange += ZoneServerStateChanged;
                listBox_ZoneSevers.Items.Add(z);
            }

            //Instance servers
            InstanceServerLink[] instanceServers = new InstanceServerLink[]
            {
                   new InstanceServerLink(1, "InstServer0", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4500)),
                   new InstanceServerLink(1, "InstServer1", new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4501), new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4501))
            };
            listBox_InstServers.Items.Clear();
            foreach (var i in instanceServers)
            {
                i.OnStateChange += InstanceServerStateChanged;
                listBox_InstServers.Items.Add(i);
            }

            //MasterServerHost
            this.host = new MasterServerHost(forwardServer, zoneServers, instanceServers);
            this.host.Start();
        }

        private void MasterServerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            host.Stop("Window closed.");
        }

        public void ForwardServerStateChanged(ForwardServerLink forwardServer)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                checkBox_fwdIsConnected.Checked = forwardServer.ConnectedState;
            }));
        }

        public void ZoneServerStateChanged(InstanceServerLink zone)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (zone == listBox_ZoneSevers.SelectedItem)
                {
                    listBox_ZoneSevers_SelectedIndexChanged(null, null);
                }
            }));
        }

        public void InstanceServerStateChanged(InstanceServerLink zone)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (zone == listBox_InstServers.SelectedItem)
                {
                    listBox_InstServers_SelectedIndexChanged(null, null);
                }
            }));
        }

        private void listBox_ZoneSevers_SelectedIndexChanged(object sender, EventArgs e)
        {
            InstanceServerLink zone = listBox_ZoneSevers.SelectedItem as InstanceServerLink;

            label_ZoneServerID.Text = zone.ServerId.ToString();
            label_ZoneName.Text = zone.Name;
        }

        private void listBox_InstServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            InstanceServerLink inst = listBox_InstServers.SelectedItem as InstanceServerLink;

            label_InstServerID.Text = inst.ServerId.ToString();
            label_InstName.Text = inst.Name;
        }

        private void listBox_Players_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
