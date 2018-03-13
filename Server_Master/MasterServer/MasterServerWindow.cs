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
        private MasterHost host;

        public MasterServerWindow(ClientAcceptor clientAcceptor, WorldServer[] worldServers, GeneralServer[] generalServers)
        {
            InitializeComponent();

            //World servers
            listBox_WorldSevers.Items.Clear();
            foreach (var w in worldServers)
            {
                w.OnStateChange += OnWorldServerStateChanged;
                listBox_WorldSevers.Items.Add(w);
            }

            //General servers
            listBox_GeneralServers.Items.Clear();
            foreach (var g in generalServers)
            {
                g.OnStateChange += OnGeneralServerStateChanged;
                listBox_GeneralServers.Items.Add(g);
            }

            //Start host
            this.host = new MasterHost(clientAcceptor, new ServerHub(worldServers, generalServers));
            this.host.ClientUpdated += OnClientUpdated;
            this.host.ClientRemoved += OnClientRemoved;
            this.host.Start();

            //Form defaults
            SetClientInfo(null);
            SetInstServerInfo(null);
        }

        private void MasterServerWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            host.Stop("Window closed.");
        }

        public void OnWorldServerStateChanged(ServerLink serverLink)
        {
            WorldServer world = serverLink as WorldServer;

            this.Invoke(new MethodInvoker(() =>
            {
                if (world == listBox_WorldSevers.SelectedItem)
                {
                    SetInstServerInfo(world);
                }
            }));
        }

        public void OnGeneralServerStateChanged(ServerLink serverLink)
        {
            GeneralServer general = serverLink as GeneralServer;

            this.Invoke(new MethodInvoker(() =>
            {
                if (general == listBox_GeneralServers.SelectedItem)
                {
                    SetInstServerInfo(general);
                }
            }));
        }

        public void OnClientUpdated(ClientLink client)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (listBox_Clients.Items.Contains(client))
                {
                    if (client == listBox_Clients.SelectedItem)
                        SetClientInfo(client);
                }
                else
                {
                    listBox_Clients.Items.Add(client);
                }
            }));
        }

        public void OnClientRemoved(ClientLink client)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                if (listBox_Clients.Items.Contains(client))
                {
                    if (client == listBox_Clients.SelectedItem)
                        listBox_Clients.SelectedItem = null;
                    listBox_Clients.Items.Remove(client);
                }
            }));
        }

        private void SetClientInfo(ClientLink cl)
        {
            bool isConnected = false;
            string name = "";
            string state = "";
            string type = "";

            if (cl != null)
            {
                isConnected = cl.IsConnected;
                name = cl.AccountInfo.Name;
                state = cl.State.ToString();
                type = cl.AccountInfo.Type.ToString();
            }

            checkBox_ClientConnected.Checked = isConnected;
            label_ClientName.Text = name;
            label_ClientState.Text = state;
            label_ClientAccountType.Text = type;
        }

        private void SetInstServerInfo(ServerLink link)
        {
            bool isConnected = false;
            string name = "";

            if (link != null)
            {
                isConnected = link.IsConnected;
                name = link.ServerName;
            }

            checkBox_instServIsConnected.Checked = isConnected;
            label_InstServName.Text = name;
        }

        private void listBox_WorldSevers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_WorldSevers.SelectedItem != null)
            {
                listBox_GeneralServers.SelectedItem = null;

                SetInstServerInfo(listBox_WorldSevers.SelectedItem as WorldServer);
            }
            else
            {
                SetInstServerInfo(null);
            }
        }

        private void listBox_WorldSevers_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                listBox_WorldSevers.SelectedItem = null;
        }

        private void listBox_GeneralServers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_GeneralServers.SelectedItem != null)
            {
                listBox_WorldSevers.SelectedItem = null;

                SetInstServerInfo(listBox_GeneralServers.SelectedItem as GeneralServer);
            }
            else
            {
                SetInstServerInfo(null);
            }
        }

        private void listBox_GeneralServers_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                listBox_GeneralServers.SelectedItem = null;
        }

        private void listBox_Clients_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Clients.SelectedItem != null)
            {
                SetClientInfo(listBox_Clients.SelectedItem as ClientLink);
            }
            else
            {
                SetClientInfo(null);
            }
        }

        private void listBox_Clients_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                listBox_Clients.SelectedItem = null;
        }
    }
}
