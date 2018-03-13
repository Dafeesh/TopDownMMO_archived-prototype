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

        public MasterServerWindow(ClientAcceptor clientAcceptor, WorldServerLink[] worldServers, InstanceServerLink[] instanceServers)
        {
            InitializeComponent();

            //World servers
            listBox_WorldSevers.Items.Clear();
            foreach (var w in worldServers)
            {
                w.ServerLink.OnStateChange += WorldServerStateChanged;
                listBox_WorldSevers.Items.Add(w);
            }

            //Instance servers
            listBox_InstServers.Items.Clear();
            foreach (var i in instanceServers)
            {
                i.OnStateChange += InstanceServerStateChanged;
                listBox_InstServers.Items.Add(i);
            }

            //Start host
            this.host = new MasterServerHost(clientAcceptor, new ServerHub(worldServers, instanceServers));
            this.host.Start();
        }

        private void MasterServerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            host.Stop("Window closed.");
        }

        public void WorldServerStateChanged(InstanceServerLink world)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (world == listBox_WorldSevers.SelectedItem)
                {
                    listBox_WorldSevers_SelectedIndexChanged(null, null);
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

        private void listBox_WorldSevers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_WorldSevers.SelectedItem != null)
            {
                listBox_InstServers.SelectedItem = null;

                WorldServerLink world = listBox_WorldSevers.SelectedItem as WorldServerLink;
                {
                    label_InstServerID.Text = world.ServerLink.ServerId.ToString();
                    label_InstName.Text = "World " + world.WorldNumber;

                    listBox_Instances.Items.Clear();
                    foreach (var gi in world.ServerLink.GetInstances())
                    {
                        listBox_Instances.Items.Add(gi);
                    }
                }
            }
        }

        private void listBox_InstServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_InstServers.SelectedItem != null)
            {
                listBox_WorldSevers.SelectedItem = null;

                InstanceServerLink inst = listBox_InstServers.SelectedItem as InstanceServerLink;
                {
                    label_InstServerID.Text = inst.ServerId.ToString();
                    label_InstName.Text = "Instance Server";

                    listBox_Instances.Items.Clear();
                    foreach (var gi in inst.GetInstances())
                    {
                        listBox_Instances.Items.Add(gi);
                    }
                }
            }
        }

        private void listBox_Clients_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
