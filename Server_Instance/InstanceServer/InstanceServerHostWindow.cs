using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Extant;
using InstanceServer.Control;
using InstanceServer.World;
using InstanceServer.Links;

namespace InstanceServer
{
    public partial class InstanceServerHostWindow : Form
    {
        GameInstance selectedInstance;

        private LogItem[] logItems = new LogItem[0];
        private object logItems_lock = new object();

        private InstanceServerHost instHost;

        public InstanceServerHostWindow(ClientAccepter clientAccepter, MasterServerLink msLink)
        {
            InitializeComponent();

            msLink.StateChanged += OnStateChange_MasterServerLink;

            this.instHost = new InstanceServerHost(clientAccepter, msLink);
            this.instHost.Start();

            button_RefreshList_Click(null, null);
            button_RefreshPlayerList_Click(null, null);
        }

        private void InstanceServerHostWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            instHost.Dispose();
        }

        private void OnStateChange_MasterServerLink(MasterServerLink.ConnectionState state)
        {
            this.Invoke(new MethodInvoker(() =>
            {
                this.checkBox_MasterConnected.Checked = (state == MasterServerLink.ConnectionState.Connected);
            }));
        }

        private void button_RefreshList_Click(object sender, EventArgs e)
        {
            listBox_Instances.Items.Clear();
        }

        private void listBox_Instances_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Instances.SelectedItem != null)
            {
                
            }
        }

        private void _AutoUpdateInstanceWatch()
        {
            try
            {
                Graphics graphics = panel_InstanceView.CreateGraphics();
                Pen pen_npc = new Pen(Brushes.Black, 2);
                Pen pen_plr = new Pen(Brushes.Red, 2);
                while (true)
                {
                    //Draw map
                    panel_InstanceView.Invalidate();

                    var npcs = selectedInstance.GetNpcs();
                    var plrs = selectedInstance.GetPlayers();
                    this.Invoke(new MethodInvoker(() =>
                    {
                        label_Players.Text = plrs.Length.ToString();
                    }));
                    foreach (var c in npcs)
                    {
                        graphics.DrawRectangle(pen_npc, new Rectangle((int)c.Position.x, (int)c.Position.y, 2, 2));
                    }
                    foreach (var p in plrs)
                    {
                        graphics.DrawRectangle(pen_plr, new Rectangle((int)p.Position.x, (int)p.Position.y, 2, 2));
                    }

                    //Handle log
                    if (!checkBox_logPause.Checked)
                    {
                        this.Invoke(new MethodInvoker(() =>
                        {
                            lock (logItems_lock)
                            {
                                try
                                {
                                    listBox_log.SelectedIndex = -1;
                                    textBox_logSelect.Text = String.Empty;

                                    logItems = selectedInstance.Log.GetLog(10);

                                    listBox_log.Items.Clear();
                                    foreach (LogItem li in logItems)
                                    {
                                        if (li.Message.Length > 28)
                                            listBox_log.Items.Add(li.Message.Substring(0, 28));
                                        else
                                            listBox_log.Items.Add(li.Message);
                                    }
                                }
                                catch (IndexOutOfRangeException)
                                { }
                            }
                        }));
                    }

                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {

            }
        }

        private void button_RefreshPlayerList_Click(object sender, EventArgs e)
        {
            listBox_Players.Items.Clear();
        }

        private void listBox_log_SelectedIndexChanged(object sender, EventArgs e)
        {
            lock (logItems_lock)
            {
                textBox_logSelect.Text = String.Empty;
                if (listBox_log.SelectedIndex < logItems.Length)
                {
                    textBox_logSelect.Text = logItems[listBox_log.SelectedIndex].ToString();
                }
            }
        }
    }
}
