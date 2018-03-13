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

using WorldServer.Control;

namespace WorldServer
{
    public partial class InstanceWatcher : Form
    {
        private Dictionary<string, Instance> instances = new Dictionary<string, Instance>();
        private Instance selectedInstance = null;
        private Thread workerThread = null;
        private WorldController wc;

        public InstanceWatcher(WorldController wc)
        {
            InitializeComponent();

            this.wc = wc;
        }

        private void button_RefreshList_Click(object sender, EventArgs e)
        {
            var insts = wc.GetInstances();

            listBox_Instances.Items.Clear();
            instances.Clear();

            foreach (var i in insts)
            {
                listBox_Instances.Items.Add(i.Name);
                instances.Add(i.Name, i);
            }
        }

        private void listBox_Instances_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox_Instances.SelectedItem.ToString() != String.Empty)
            {
                selectedInstance = instances[listBox_Instances.SelectedItem.ToString()];
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

        }
    }
}
