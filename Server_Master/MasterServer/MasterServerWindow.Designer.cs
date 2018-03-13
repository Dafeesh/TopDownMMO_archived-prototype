namespace MasterServer
{
    partial class MasterServerWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBox_InstServers = new System.Windows.Forms.ListBox();
            this.label_static_ISs = new System.Windows.Forms.Label();
            this.listBox_Clients = new System.Windows.Forms.ListBox();
            this.label_static_Clients = new System.Windows.Forms.Label();
            this.label_static_ISID = new System.Windows.Forms.Label();
            this.label_InstServerID = new System.Windows.Forms.Label();
            this.checkBox_instServIsConnected = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label_InstName = new System.Windows.Forms.Label();
            this.label_static_InstName = new System.Windows.Forms.Label();
            this.listBox_WorldSevers = new System.Windows.Forms.ListBox();
            this.label_static_WorldServers = new System.Windows.Forms.Label();
            this.listBox_Instances = new System.Windows.Forms.ListBox();
            this.label_static_InstanceList = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox_InstServers
            // 
            this.listBox_InstServers.FormattingEnabled = true;
            this.listBox_InstServers.Location = new System.Drawing.Point(139, 19);
            this.listBox_InstServers.Name = "listBox_InstServers";
            this.listBox_InstServers.Size = new System.Drawing.Size(120, 316);
            this.listBox_InstServers.TabIndex = 0;
            this.listBox_InstServers.SelectedIndexChanged += new System.EventHandler(this.listBox_InstServers_SelectedIndexChanged);
            // 
            // label_static_ISs
            // 
            this.label_static_ISs.AutoSize = true;
            this.label_static_ISs.Location = new System.Drawing.Point(136, 4);
            this.label_static_ISs.Name = "label_static_ISs";
            this.label_static_ISs.Size = new System.Drawing.Size(90, 13);
            this.label_static_ISs.TabIndex = 1;
            this.label_static_ISs.Text = "Instance Servers:";
            // 
            // listBox_Clients
            // 
            this.listBox_Clients.FormattingEnabled = true;
            this.listBox_Clients.Location = new System.Drawing.Point(6, 21);
            this.listBox_Clients.Name = "listBox_Clients";
            this.listBox_Clients.Size = new System.Drawing.Size(120, 316);
            this.listBox_Clients.TabIndex = 2;
            this.listBox_Clients.SelectedIndexChanged += new System.EventHandler(this.listBox_Clients_SelectedIndexChanged);
            // 
            // label_static_Clients
            // 
            this.label_static_Clients.AutoSize = true;
            this.label_static_Clients.Location = new System.Drawing.Point(3, 5);
            this.label_static_Clients.Name = "label_static_Clients";
            this.label_static_Clients.Size = new System.Drawing.Size(41, 13);
            this.label_static_Clients.TabIndex = 3;
            this.label_static_Clients.Text = "Clients:";
            // 
            // label_static_ISID
            // 
            this.label_static_ISID.AutoSize = true;
            this.label_static_ISID.Location = new System.Drawing.Point(267, 39);
            this.label_static_ISID.Name = "label_static_ISID";
            this.label_static_ISID.Size = new System.Drawing.Size(21, 13);
            this.label_static_ISID.TabIndex = 4;
            this.label_static_ISID.Text = "ID:";
            // 
            // label_InstServerID
            // 
            this.label_InstServerID.AutoSize = true;
            this.label_InstServerID.Location = new System.Drawing.Point(311, 39);
            this.label_InstServerID.Name = "label_InstServerID";
            this.label_InstServerID.Size = new System.Drawing.Size(30, 13);
            this.label_InstServerID.TabIndex = 5;
            this.label_InstServerID.Text = "_ID_";
            // 
            // checkBox_instServIsConnected
            // 
            this.checkBox_instServIsConnected.AutoSize = true;
            this.checkBox_instServIsConnected.Enabled = false;
            this.checkBox_instServIsConnected.Location = new System.Drawing.Point(280, 19);
            this.checkBox_instServIsConnected.Name = "checkBox_instServIsConnected";
            this.checkBox_instServIsConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_instServIsConnected.TabIndex = 6;
            this.checkBox_instServIsConnected.Text = "Connected";
            this.checkBox_instServIsConnected.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox_Clients);
            this.panel1.Controls.Add(this.label_static_Clients);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(139, 341);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label_static_InstanceList);
            this.panel2.Controls.Add(this.listBox_Instances);
            this.panel2.Controls.Add(this.label_static_WorldServers);
            this.panel2.Controls.Add(this.listBox_WorldSevers);
            this.panel2.Controls.Add(this.label_InstName);
            this.panel2.Controls.Add(this.label_static_InstName);
            this.panel2.Controls.Add(this.listBox_InstServers);
            this.panel2.Controls.Add(this.label_static_ISs);
            this.panel2.Controls.Add(this.label_InstServerID);
            this.panel2.Controls.Add(this.checkBox_instServIsConnected);
            this.panel2.Controls.Add(this.label_static_ISID);
            this.panel2.Location = new System.Drawing.Point(157, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(687, 341);
            this.panel2.TabIndex = 8;
            // 
            // label_InstName
            // 
            this.label_InstName.AutoSize = true;
            this.label_InstName.Location = new System.Drawing.Point(311, 61);
            this.label_InstName.Name = "label_InstName";
            this.label_InstName.Size = new System.Drawing.Size(47, 13);
            this.label_InstName.TabIndex = 10;
            this.label_InstName.Text = "_Name_";
            // 
            // label_static_InstName
            // 
            this.label_static_InstName.AutoSize = true;
            this.label_static_InstName.Location = new System.Drawing.Point(267, 61);
            this.label_static_InstName.Name = "label_static_InstName";
            this.label_static_InstName.Size = new System.Drawing.Size(38, 13);
            this.label_static_InstName.TabIndex = 9;
            this.label_static_InstName.Text = "Name:";
            // 
            // listBox_WorldSevers
            // 
            this.listBox_WorldSevers.FormattingEnabled = true;
            this.listBox_WorldSevers.Location = new System.Drawing.Point(13, 20);
            this.listBox_WorldSevers.Name = "listBox_WorldSevers";
            this.listBox_WorldSevers.Size = new System.Drawing.Size(120, 316);
            this.listBox_WorldSevers.TabIndex = 0;
            this.listBox_WorldSevers.SelectedIndexChanged += new System.EventHandler(this.listBox_WorldSevers_SelectedIndexChanged);
            // 
            // label_static_WorldServers
            // 
            this.label_static_WorldServers.AutoSize = true;
            this.label_static_WorldServers.Location = new System.Drawing.Point(7, 4);
            this.label_static_WorldServers.Name = "label_static_WorldServers";
            this.label_static_WorldServers.Size = new System.Drawing.Size(77, 13);
            this.label_static_WorldServers.TabIndex = 1;
            this.label_static_WorldServers.Text = "World Servers:";
            // 
            // listBox_Instances
            // 
            this.listBox_Instances.FormattingEnabled = true;
            this.listBox_Instances.Location = new System.Drawing.Point(440, 20);
            this.listBox_Instances.Name = "listBox_Instances";
            this.listBox_Instances.Size = new System.Drawing.Size(120, 316);
            this.listBox_Instances.TabIndex = 11;
            // 
            // label_static_InstanceList
            // 
            this.label_static_InstanceList.AutoSize = true;
            this.label_static_InstanceList.Location = new System.Drawing.Point(437, 6);
            this.label_static_InstanceList.Name = "label_static_InstanceList";
            this.label_static_InstanceList.Size = new System.Drawing.Size(56, 13);
            this.label_static_InstanceList.TabIndex = 12;
            this.label_static_InstanceList.Text = "Instances:";
            // 
            // MasterServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 363);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MasterServerWindow";
            this.Text = "Master Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterServerWindow_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_InstServers;
        private System.Windows.Forms.Label label_static_ISs;
        private System.Windows.Forms.ListBox listBox_Clients;
        private System.Windows.Forms.Label label_static_Clients;
        private System.Windows.Forms.Label label_static_ISID;
        private System.Windows.Forms.Label label_InstServerID;
        private System.Windows.Forms.CheckBox checkBox_instServIsConnected;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox listBox_WorldSevers;
        private System.Windows.Forms.Label label_static_WorldServers;
        private System.Windows.Forms.Label label_InstName;
        private System.Windows.Forms.Label label_static_InstName;
        private System.Windows.Forms.Label label_static_InstanceList;
        private System.Windows.Forms.ListBox listBox_Instances;
    }
}

