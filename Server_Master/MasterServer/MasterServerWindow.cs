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
using MasterServer.Host;

namespace MasterServer
{
    public partial class MasterServerWindow : Form
    {
        private MasterServerHost host;

        public MasterServerWindow(MasterServerHost _host)
        {
            InitializeComponent();
            this.host = _host;

            //Zone servers
            listBox_ZoneSevers.Items.Clear();
            foreach (var z in host.ZoneServerLinks)
            {
                z.OnStateChange += ZoneServerStateChanged;
                listBox_ZoneSevers.Items.Add(z);
            }

            //Instance servers
            listBox_InstServers.Items.Clear();
            foreach (var i in host.InstanceServerLinks)
            {
                i.OnStateChange += InstanceServerStateChanged;
                listBox_InstServers.Items.Add(i);
            }

            //Start host
            host.Start();
        }

        private void MasterServerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            host.Stop("Window closed.");
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

        public void ClientUpdated(ClientLink client)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (listBox_Clients.Items.Contains(client))
                {
                    if (client == listBox_Clients.SelectedItem)
                    {
                        listBox_Clients_SelectedIndexChanged(null, null);
                    }
                }
                else
                {
                    listBox_Clients.Items.Add(client);
                }
            }));
        }

        private void listBox_ZoneSevers_SelectedIndexChanged(object sender, EventArgs e)
        {
            InstanceServerLink zone = listBox_ZoneSevers.SelectedItem as InstanceServerLink;
            if (zone == null)
                return;

            label_ZoneServerID.Text = zone.ServerId.ToString();
            label_ZoneName.Text = zone.Name;
        }

        private void listBox_InstServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            InstanceServerLink inst = listBox_InstServers.SelectedItem as InstanceServerLink;
            if (inst == null)
                return;

            label_InstServerID.Text = inst.ServerId.ToString();
            label_InstName.Text = inst.Name;
        }

        private void listBox_Clients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
