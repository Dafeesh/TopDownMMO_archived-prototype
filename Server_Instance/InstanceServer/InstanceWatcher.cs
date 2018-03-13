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
using WorldServer.Control;
using WorldServer.World;

namespace WorldServer
{
    public partial class InstanceWatcher : Form
    {
        private Instance selectedInstance;

        private LogItem[] logItems = new LogItem[0];
        private object logItems_lock = new object();

        private Thread workerThread = null;
        private WorldController wc;

        public InstanceWatcher(WorldController wc)
        {
            InitializeComponent();

            this.wc = wc;

            button_RefreshList_Click(null, null);
            button_RefreshPlayerList_Click(null, null);
        }

        private void button_RefreshList_Click(object sender, EventArgs e)
        {
            var insts = wc.GetInstances();

            listBox_Instances.Items.Clear();
            foreach (var i in insts)
            {
                listBox_Instances.Items.Add(i);
            }
        }

        private void listBox_Instances_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Instances.SelectedItem.ToString() != String.Empty)
            {
                selectedInstance = (Instance)listBox_Instances.SelectedItem;
                label_InstanceName.Text = selectedInstance.Name;

                if (workerThread != null)
                {
                    workerThread.Abort();
                    workerThread.Join();
                }
                workerThread = new Thread(new ThreadStart(_AutoUpdateInstanceWatch));
                workerThread.Name = "(InstanceWatcher)";
                workerThread.Start();
            }
        }

        private void _AutoUpdateInstanceWatch()
        {
            try
            {
                Graphics graphics = panel_InstanceView.CreateGraphics();
                Pen pen = new Pen(Brushes.Black, 2);
                while (true)
                {
                    //Draw map
                    panel_InstanceView.Invalidate();

                    var characters = selectedInstance.GetCharacters();
                    this.Invoke(new MethodInvoker(() =>
                    {
                        label_Players.Text = characters.Count.ToString();
                    }));

                    foreach (var c in characters) //<<<
                    {
                        graphics.DrawRectangle(pen, new Rectangle((int)c.Position.x, (int)c.Position.y, 2, 2));
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

        private void InstanceWatcher_FormClosing(object sender, FormClosingEventArgs e)
        {
            workerThread.Abort();
            workerThread.Join();
        }

        private void button_RefreshPlayerList_Click(object sender, EventArgs e)
        {
            listBox_Players.Items.Clear();
            foreach (var p in wc.GetPlayerList())
            {
                listBox_Players.Items.Add(p);
            }
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
