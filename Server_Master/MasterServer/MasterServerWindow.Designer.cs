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
            this.listBox_GeneralServers = new System.Windows.Forms.ListBox();
            this.label_static_GSs = new System.Windows.Forms.Label();
            this.listBox_Clients = new System.Windows.Forms.ListBox();
            this.label_static_Clients = new System.Windows.Forms.Label();
            this.checkBox_instServIsConnected = new System.Windows.Forms.CheckBox();
            this.panel_Clients = new System.Windows.Forms.Panel();
            this.panel_Servers = new System.Windows.Forms.Panel();
            this.label_static_PlayerList = new System.Windows.Forms.Label();
            this.listBox_Instances = new System.Windows.Forms.ListBox();
            this.label_static_WorldServers = new System.Windows.Forms.Label();
            this.listBox_WorldSevers = new System.Windows.Forms.ListBox();
            this.label_InstServName = new System.Windows.Forms.Label();
            this.label_static_InstServName = new System.Windows.Forms.Label();
            this.label_ClientName = new System.Windows.Forms.Label();
            this.label_static_ClientName = new System.Windows.Forms.Label();
            this.checkBox_ClientConnected = new System.Windows.Forms.CheckBox();
            this.label_ClientState = new System.Windows.Forms.Label();
            this.label_static_ClientState = new System.Windows.Forms.Label();
            this.label_ClientAccountType = new System.Windows.Forms.Label();
            this.label_static_ClientAccountType = new System.Windows.Forms.Label();
            this.panel_Clients.SuspendLayout();
            this.panel_Servers.SuspendLayout();
            this.SuspendLayout();
            // 
            // listBox_GeneralServers
            // 
            this.listBox_GeneralServers.FormattingEnabled = true;
            this.listBox_GeneralServers.Location = new System.Drawing.Point(139, 19);
            this.listBox_GeneralServers.Name = "listBox_GeneralServers";
            this.listBox_GeneralServers.Size = new System.Drawing.Size(120, 316);
            this.listBox_GeneralServers.TabIndex = 0;
            this.listBox_GeneralServers.SelectedIndexChanged += new System.EventHandler(this.listBox_GeneralServers_SelectedIndexChanged);
            this.listBox_GeneralServers.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_GeneralServers_KeyUp);
            // 
            // label_static_GSs
            // 
            this.label_static_GSs.AutoSize = true;
            this.label_static_GSs.Location = new System.Drawing.Point(136, 3);
            this.label_static_GSs.Name = "label_static_GSs";
            this.label_static_GSs.Size = new System.Drawing.Size(86, 13);
            this.label_static_GSs.TabIndex = 1;
            this.label_static_GSs.Text = "General Servers:";
            // 
            // listBox_Clients
            // 
            this.listBox_Clients.FormattingEnabled = true;
            this.listBox_Clients.Location = new System.Drawing.Point(6, 21);
            this.listBox_Clients.Name = "listBox_Clients";
            this.listBox_Clients.Size = new System.Drawing.Size(120, 316);
            this.listBox_Clients.TabIndex = 2;
            this.listBox_Clients.SelectedIndexChanged += new System.EventHandler(this.listBox_Clients_SelectedIndexChanged);
            this.listBox_Clients.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_Clients_KeyUp);
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
            // checkBox_instServIsConnected
            // 
            this.checkBox_instServIsConnected.AutoSize = true;
            this.checkBox_instServIsConnected.Enabled = false;
            this.checkBox_instServIsConnected.Location = new System.Drawing.Point(265, 21);
            this.checkBox_instServIsConnected.Name = "checkBox_instServIsConnected";
            this.checkBox_instServIsConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_instServIsConnected.TabIndex = 6;
            this.checkBox_instServIsConnected.Text = "Connected";
            this.checkBox_instServIsConnected.UseVisualStyleBackColor = true;
            // 
            // panel_Clients
            // 
            this.panel_Clients.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Clients.Controls.Add(this.label_ClientAccountType);
            this.panel_Clients.Controls.Add(this.label_static_ClientAccountType);
            this.panel_Clients.Controls.Add(this.label_ClientState);
            this.panel_Clients.Controls.Add(this.label_static_ClientState);
            this.panel_Clients.Controls.Add(this.label_ClientName);
            this.panel_Clients.Controls.Add(this.label_static_ClientName);
            this.panel_Clients.Controls.Add(this.checkBox_ClientConnected);
            this.panel_Clients.Controls.Add(this.listBox_Clients);
            this.panel_Clients.Controls.Add(this.label_static_Clients);
            this.panel_Clients.Location = new System.Drawing.Point(19, 12);
            this.panel_Clients.Name = "panel_Clients";
            this.panel_Clients.Size = new System.Drawing.Size(259, 348);
            this.panel_Clients.TabIndex = 7;
            // 
            // panel_Servers
            // 
            this.panel_Servers.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_Servers.Controls.Add(this.label_static_PlayerList);
            this.panel_Servers.Controls.Add(this.listBox_Instances);
            this.panel_Servers.Controls.Add(this.label_static_WorldServers);
            this.panel_Servers.Controls.Add(this.listBox_WorldSevers);
            this.panel_Servers.Controls.Add(this.label_InstServName);
            this.panel_Servers.Controls.Add(this.label_static_InstServName);
            this.panel_Servers.Controls.Add(this.listBox_GeneralServers);
            this.panel_Servers.Controls.Add(this.label_static_GSs);
            this.panel_Servers.Controls.Add(this.checkBox_instServIsConnected);
            this.panel_Servers.Location = new System.Drawing.Point(284, 12);
            this.panel_Servers.Name = "panel_Servers";
            this.panel_Servers.Size = new System.Drawing.Size(687, 348);
            this.panel_Servers.TabIndex = 8;
            // 
            // label_static_PlayerList
            // 
            this.label_static_PlayerList.AutoSize = true;
            this.label_static_PlayerList.Location = new System.Drawing.Point(416, 5);
            this.label_static_PlayerList.Name = "label_static_PlayerList";
            this.label_static_PlayerList.Size = new System.Drawing.Size(61, 13);
            this.label_static_PlayerList.TabIndex = 12;
            this.label_static_PlayerList.Text = "Characters:";
            // 
            // listBox_Instances
            // 
            this.listBox_Instances.FormattingEnabled = true;
            this.listBox_Instances.Location = new System.Drawing.Point(419, 19);
            this.listBox_Instances.Name = "listBox_Instances";
            this.listBox_Instances.Size = new System.Drawing.Size(120, 316);
            this.listBox_Instances.TabIndex = 11;
            // 
            // label_static_WorldServers
            // 
            this.label_static_WorldServers.AutoSize = true;
            this.label_static_WorldServers.Location = new System.Drawing.Point(10, 4);
            this.label_static_WorldServers.Name = "label_static_WorldServers";
            this.label_static_WorldServers.Size = new System.Drawing.Size(77, 13);
            this.label_static_WorldServers.TabIndex = 1;
            this.label_static_WorldServers.Text = "World Servers:";
            // 
            // listBox_WorldSevers
            // 
            this.listBox_WorldSevers.FormattingEnabled = true;
            this.listBox_WorldSevers.Location = new System.Drawing.Point(13, 20);
            this.listBox_WorldSevers.Name = "listBox_WorldSevers";
            this.listBox_WorldSevers.Size = new System.Drawing.Size(120, 316);
            this.listBox_WorldSevers.TabIndex = 0;
            this.listBox_WorldSevers.SelectedIndexChanged += new System.EventHandler(this.listBox_WorldSevers_SelectedIndexChanged);
            this.listBox_WorldSevers.KeyUp += new System.Windows.Forms.KeyEventHandler(this.listBox_WorldSevers_KeyUp);
            // 
            // label_InstServName
            // 
            this.label_InstServName.AutoSize = true;
            this.label_InstServName.Location = new System.Drawing.Point(304, 41);
            this.label_InstServName.Name = "label_InstServName";
            this.label_InstServName.Size = new System.Drawing.Size(47, 13);
            this.label_InstServName.TabIndex = 10;
            this.label_InstServName.Text = "_Name_";
            // 
            // label_static_InstServName
            // 
            this.label_static_InstServName.AutoSize = true;
            this.label_static_InstServName.Location = new System.Drawing.Point(260, 41);
            this.label_static_InstServName.Name = "label_static_InstServName";
            this.label_static_InstServName.Size = new System.Drawing.Size(38, 13);
            this.label_static_InstServName.TabIndex = 9;
            this.label_static_InstServName.Text = "Name:";
            // 
            // label_ClientName
            // 
            this.label_ClientName.AutoSize = true;
            this.label_ClientName.Location = new System.Drawing.Point(171, 41);
            this.label_ClientName.Name = "label_ClientName";
            this.label_ClientName.Size = new System.Drawing.Size(47, 13);
            this.label_ClientName.TabIndex = 13;
            this.label_ClientName.Text = "_Name_";
            // 
            // label_static_ClientName
            // 
            this.label_static_ClientName.AutoSize = true;
            this.label_static_ClientName.Location = new System.Drawing.Point(127, 41);
            this.label_static_ClientName.Name = "label_static_ClientName";
            this.label_static_ClientName.Size = new System.Drawing.Size(38, 13);
            this.label_static_ClientName.TabIndex = 12;
            this.label_static_ClientName.Text = "Name:";
            // 
            // checkBox_ClientConnected
            // 
            this.checkBox_ClientConnected.AutoSize = true;
            this.checkBox_ClientConnected.Enabled = false;
            this.checkBox_ClientConnected.Location = new System.Drawing.Point(132, 21);
            this.checkBox_ClientConnected.Name = "checkBox_ClientConnected";
            this.checkBox_ClientConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_ClientConnected.TabIndex = 11;
            this.checkBox_ClientConnected.Text = "Connected";
            this.checkBox_ClientConnected.UseVisualStyleBackColor = true;
            // 
            // label_ClientState
            // 
            this.label_ClientState.AutoSize = true;
            this.label_ClientState.Location = new System.Drawing.Point(171, 63);
            this.label_ClientState.Name = "label_ClientState";
            this.label_ClientState.Size = new System.Drawing.Size(44, 13);
            this.label_ClientState.TabIndex = 15;
            this.label_ClientState.Text = "_State_";
            // 
            // label_static_ClientState
            // 
            this.label_static_ClientState.AutoSize = true;
            this.label_static_ClientState.Location = new System.Drawing.Point(127, 63);
            this.label_static_ClientState.Name = "label_static_ClientState";
            this.label_static_ClientState.Size = new System.Drawing.Size(35, 13);
            this.label_static_ClientState.TabIndex = 14;
            this.label_static_ClientState.Text = "State:";
            // 
            // label_ClientAccountType
            // 
            this.label_ClientAccountType.AutoSize = true;
            this.label_ClientAccountType.Location = new System.Drawing.Point(171, 85);
            this.label_ClientAccountType.Name = "label_ClientAccountType";
            this.label_ClientAccountType.Size = new System.Drawing.Size(43, 13);
            this.label_ClientAccountType.TabIndex = 17;
            this.label_ClientAccountType.Text = "_Type_";
            // 
            // label_static_ClientAccountType
            // 
            this.label_static_ClientAccountType.AutoSize = true;
            this.label_static_ClientAccountType.Location = new System.Drawing.Point(127, 85);
            this.label_static_ClientAccountType.Name = "label_static_ClientAccountType";
            this.label_static_ClientAccountType.Size = new System.Drawing.Size(34, 13);
            this.label_static_ClientAccountType.TabIndex = 16;
            this.label_static_ClientAccountType.Text = "Type:";
            // 
            // MasterServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1072, 387);
            this.Controls.Add(this.panel_Servers);
            this.Controls.Add(this.panel_Clients);
            this.Name = "MasterServerWindow";
            this.Text = "Master Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MasterServerWindow_FormClosing);
            this.panel_Clients.ResumeLayout(false);
            this.panel_Clients.PerformLayout();
            this.panel_Servers.ResumeLayout(false);
            this.panel_Servers.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_GeneralServers;
        private System.Windows.Forms.Label label_static_GSs;
        private System.Windows.Forms.ListBox listBox_Clients;
        private System.Windows.Forms.Label label_static_Clients;
        private System.Windows.Forms.CheckBox checkBox_instServIsConnected;
        private System.Windows.Forms.Panel panel_Clients;
        private System.Windows.Forms.Panel panel_Servers;
        private System.Windows.Forms.ListBox listBox_WorldSevers;
        private System.Windows.Forms.Label label_static_WorldServers;
        private System.Windows.Forms.Label label_InstServName;
        private System.Windows.Forms.Label label_static_InstServName;
        private System.Windows.Forms.Label label_static_PlayerList;
        private System.Windows.Forms.ListBox listBox_Instances;
        private System.Windows.Forms.Label label_ClientName;
        private System.Windows.Forms.Label label_static_ClientName;
        private System.Windows.Forms.CheckBox checkBox_ClientConnected;
        private System.Windows.Forms.Label label_ClientState;
        private System.Windows.Forms.Label label_static_ClientState;
        private System.Windows.Forms.Label label_ClientAccountType;
        private System.Windows.Forms.Label label_static_ClientAccountType;
    }
}

