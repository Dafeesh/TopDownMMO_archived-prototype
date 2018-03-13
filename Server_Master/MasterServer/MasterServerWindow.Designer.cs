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
            this.listBox_Players = new System.Windows.Forms.ListBox();
            this.label_static_Players = new System.Windows.Forms.Label();
            this.label_static_ISID = new System.Windows.Forms.Label();
            this.label_InstServerID = new System.Windows.Forms.Label();
            this.checkBox_instServIsConnected = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label_InstName = new System.Windows.Forms.Label();
            this.label_static_InstName = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.checkBox_fwdIsConnected = new System.Windows.Forms.CheckBox();
            this.label_static_FwdServer = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label_ZoneName = new System.Windows.Forms.Label();
            this.label_static_nameName = new System.Windows.Forms.Label();
            this.listBox_ZoneSevers = new System.Windows.Forms.ListBox();
            this.label_static_ZoneServers = new System.Windows.Forms.Label();
            this.label_ZoneServerID = new System.Windows.Forms.Label();
            this.checkBox_zoneServIsConnected = new System.Windows.Forms.CheckBox();
            this.label_static_ZSID = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox_InstServers
            // 
            this.listBox_InstServers.FormattingEnabled = true;
            this.listBox_InstServers.Location = new System.Drawing.Point(6, 20);
            this.listBox_InstServers.Name = "listBox_InstServers";
            this.listBox_InstServers.Size = new System.Drawing.Size(120, 251);
            this.listBox_InstServers.TabIndex = 0;
            this.listBox_InstServers.SelectedIndexChanged += new System.EventHandler(this.listBox_InstServers_SelectedIndexChanged);
            // 
            // label_static_ISs
            // 
            this.label_static_ISs.AutoSize = true;
            this.label_static_ISs.Location = new System.Drawing.Point(3, 4);
            this.label_static_ISs.Name = "label_static_ISs";
            this.label_static_ISs.Size = new System.Drawing.Size(90, 13);
            this.label_static_ISs.TabIndex = 1;
            this.label_static_ISs.Text = "Instance Servers:";
            // 
            // listBox_Players
            // 
            this.listBox_Players.FormattingEnabled = true;
            this.listBox_Players.Location = new System.Drawing.Point(6, 21);
            this.listBox_Players.Name = "listBox_Players";
            this.listBox_Players.Size = new System.Drawing.Size(120, 251);
            this.listBox_Players.TabIndex = 2;
            this.listBox_Players.SelectedIndexChanged += new System.EventHandler(this.listBox_Players_SelectedIndexChanged);
            // 
            // label_static_Players
            // 
            this.label_static_Players.AutoSize = true;
            this.label_static_Players.Location = new System.Drawing.Point(3, 5);
            this.label_static_Players.Name = "label_static_Players";
            this.label_static_Players.Size = new System.Drawing.Size(44, 13);
            this.label_static_Players.TabIndex = 3;
            this.label_static_Players.Text = "Players:";
            // 
            // label_static_ISID
            // 
            this.label_static_ISID.AutoSize = true;
            this.label_static_ISID.Location = new System.Drawing.Point(129, 24);
            this.label_static_ISID.Name = "label_static_ISID";
            this.label_static_ISID.Size = new System.Drawing.Size(21, 13);
            this.label_static_ISID.TabIndex = 4;
            this.label_static_ISID.Text = "ID:";
            // 
            // label_InstServerID
            // 
            this.label_InstServerID.AutoSize = true;
            this.label_InstServerID.Location = new System.Drawing.Point(173, 24);
            this.label_InstServerID.Name = "label_InstServerID";
            this.label_InstServerID.Size = new System.Drawing.Size(30, 13);
            this.label_InstServerID.TabIndex = 5;
            this.label_InstServerID.Text = "_ID_";
            // 
            // checkBox_instServIsConnected
            // 
            this.checkBox_instServIsConnected.AutoSize = true;
            this.checkBox_instServIsConnected.Enabled = false;
            this.checkBox_instServIsConnected.Location = new System.Drawing.Point(142, 4);
            this.checkBox_instServIsConnected.Name = "checkBox_instServIsConnected";
            this.checkBox_instServIsConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_instServIsConnected.TabIndex = 6;
            this.checkBox_instServIsConnected.Text = "Connected";
            this.checkBox_instServIsConnected.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox_Players);
            this.panel1.Controls.Add(this.label_static_Players);
            this.panel1.Location = new System.Drawing.Point(532, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 341);
            this.panel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label_InstName);
            this.panel2.Controls.Add(this.label_static_InstName);
            this.panel2.Controls.Add(this.listBox_InstServers);
            this.panel2.Controls.Add(this.label_static_ISs);
            this.panel2.Controls.Add(this.label_InstServerID);
            this.panel2.Controls.Add(this.checkBox_instServIsConnected);
            this.panel2.Controls.Add(this.label_static_ISID);
            this.panel2.Location = new System.Drawing.Point(272, 55);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(254, 341);
            this.panel2.TabIndex = 8;
            // 
            // label_InstName
            // 
            this.label_InstName.AutoSize = true;
            this.label_InstName.Location = new System.Drawing.Point(173, 46);
            this.label_InstName.Name = "label_InstName";
            this.label_InstName.Size = new System.Drawing.Size(47, 13);
            this.label_InstName.TabIndex = 10;
            this.label_InstName.Text = "_Name_";
            // 
            // label_static_InstName
            // 
            this.label_static_InstName.AutoSize = true;
            this.label_static_InstName.Location = new System.Drawing.Point(129, 46);
            this.label_static_InstName.Name = "label_static_InstName";
            this.label_static_InstName.Size = new System.Drawing.Size(38, 13);
            this.label_static_InstName.TabIndex = 9;
            this.label_static_InstName.Text = "Name:";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.checkBox_fwdIsConnected);
            this.panel3.Controls.Add(this.label_static_FwdServer);
            this.panel3.Location = new System.Drawing.Point(12, 12);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(185, 37);
            this.panel3.TabIndex = 9;
            // 
            // checkBox_fwdIsConnected
            // 
            this.checkBox_fwdIsConnected.AutoSize = true;
            this.checkBox_fwdIsConnected.Enabled = false;
            this.checkBox_fwdIsConnected.Location = new System.Drawing.Point(92, 5);
            this.checkBox_fwdIsConnected.Name = "checkBox_fwdIsConnected";
            this.checkBox_fwdIsConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_fwdIsConnected.TabIndex = 10;
            this.checkBox_fwdIsConnected.Text = "Connected";
            this.checkBox_fwdIsConnected.UseVisualStyleBackColor = true;
            // 
            // label_static_FwdServer
            // 
            this.label_static_FwdServer.AutoSize = true;
            this.label_static_FwdServer.Location = new System.Drawing.Point(4, 5);
            this.label_static_FwdServer.Name = "label_static_FwdServer";
            this.label_static_FwdServer.Size = new System.Drawing.Size(82, 13);
            this.label_static_FwdServer.TabIndex = 0;
            this.label_static_FwdServer.Text = "Forward Server:";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.label_ZoneName);
            this.panel4.Controls.Add(this.label_static_nameName);
            this.panel4.Controls.Add(this.listBox_ZoneSevers);
            this.panel4.Controls.Add(this.label_static_ZoneServers);
            this.panel4.Controls.Add(this.label_ZoneServerID);
            this.panel4.Controls.Add(this.checkBox_zoneServIsConnected);
            this.panel4.Controls.Add(this.label_static_ZSID);
            this.panel4.Location = new System.Drawing.Point(12, 55);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(254, 341);
            this.panel4.TabIndex = 9;
            // 
            // label_ZoneName
            // 
            this.label_ZoneName.AutoSize = true;
            this.label_ZoneName.Location = new System.Drawing.Point(176, 46);
            this.label_ZoneName.Name = "label_ZoneName";
            this.label_ZoneName.Size = new System.Drawing.Size(47, 13);
            this.label_ZoneName.TabIndex = 8;
            this.label_ZoneName.Text = "_Name_";
            // 
            // label_static_nameName
            // 
            this.label_static_nameName.AutoSize = true;
            this.label_static_nameName.Location = new System.Drawing.Point(132, 46);
            this.label_static_nameName.Name = "label_static_nameName";
            this.label_static_nameName.Size = new System.Drawing.Size(38, 13);
            this.label_static_nameName.TabIndex = 7;
            this.label_static_nameName.Text = "Name:";
            // 
            // listBox_ZoneSevers
            // 
            this.listBox_ZoneSevers.FormattingEnabled = true;
            this.listBox_ZoneSevers.Location = new System.Drawing.Point(6, 20);
            this.listBox_ZoneSevers.Name = "listBox_ZoneSevers";
            this.listBox_ZoneSevers.Size = new System.Drawing.Size(120, 251);
            this.listBox_ZoneSevers.TabIndex = 0;
            this.listBox_ZoneSevers.SelectedIndexChanged += new System.EventHandler(this.listBox_ZoneSevers_SelectedIndexChanged);
            // 
            // label_static_ZoneServers
            // 
            this.label_static_ZoneServers.AutoSize = true;
            this.label_static_ZoneServers.Location = new System.Drawing.Point(3, 4);
            this.label_static_ZoneServers.Name = "label_static_ZoneServers";
            this.label_static_ZoneServers.Size = new System.Drawing.Size(74, 13);
            this.label_static_ZoneServers.TabIndex = 1;
            this.label_static_ZoneServers.Text = "Zone Servers:";
            // 
            // label_ZoneServerID
            // 
            this.label_ZoneServerID.AutoSize = true;
            this.label_ZoneServerID.Location = new System.Drawing.Point(175, 24);
            this.label_ZoneServerID.Name = "label_ZoneServerID";
            this.label_ZoneServerID.Size = new System.Drawing.Size(30, 13);
            this.label_ZoneServerID.TabIndex = 5;
            this.label_ZoneServerID.Text = "_ID_";
            // 
            // checkBox_zoneServIsConnected
            // 
            this.checkBox_zoneServIsConnected.AutoSize = true;
            this.checkBox_zoneServIsConnected.Enabled = false;
            this.checkBox_zoneServIsConnected.Location = new System.Drawing.Point(142, 4);
            this.checkBox_zoneServIsConnected.Name = "checkBox_zoneServIsConnected";
            this.checkBox_zoneServIsConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_zoneServIsConnected.TabIndex = 6;
            this.checkBox_zoneServIsConnected.Text = "Connected";
            this.checkBox_zoneServIsConnected.UseVisualStyleBackColor = true;
            // 
            // label_static_ZSID
            // 
            this.label_static_ZSID.AutoSize = true;
            this.label_static_ZSID.Location = new System.Drawing.Point(132, 24);
            this.label_static_ZSID.Name = "label_static_ZSID";
            this.label_static_ZSID.Size = new System.Drawing.Size(21, 13);
            this.label_static_ZSID.TabIndex = 4;
            this.label_static_ZSID.Text = "ID:";
            // 
            // MasterServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(771, 408);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "MasterServerWindow";
            this.Text = "Master Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterServerWindow_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_InstServers;
        private System.Windows.Forms.Label label_static_ISs;
        private System.Windows.Forms.ListBox listBox_Players;
        private System.Windows.Forms.Label label_static_Players;
        private System.Windows.Forms.Label label_static_ISID;
        private System.Windows.Forms.Label label_InstServerID;
        private System.Windows.Forms.CheckBox checkBox_instServIsConnected;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.CheckBox checkBox_fwdIsConnected;
        private System.Windows.Forms.Label label_static_FwdServer;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ListBox listBox_ZoneSevers;
        private System.Windows.Forms.Label label_static_ZoneServers;
        private System.Windows.Forms.Label label_ZoneServerID;
        private System.Windows.Forms.CheckBox checkBox_zoneServIsConnected;
        private System.Windows.Forms.Label label_static_ZSID;
        private System.Windows.Forms.Label label_static_nameName;
        private System.Windows.Forms.Label label_InstName;
        private System.Windows.Forms.Label label_static_InstName;
        private System.Windows.Forms.Label label_ZoneName;
    }
}

