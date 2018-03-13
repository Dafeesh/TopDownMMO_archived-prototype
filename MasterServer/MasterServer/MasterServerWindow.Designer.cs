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
            this.listBox_GameServers = new System.Windows.Forms.ListBox();
            this.label_static_GSs = new System.Windows.Forms.Label();
            this.listBox_Players = new System.Windows.Forms.ListBox();
            this.label_static_Players = new System.Windows.Forms.Label();
            this.label_static_ID = new System.Windows.Forms.Label();
            this.label_gsID = new System.Windows.Forms.Label();
            this.checkBox_gsIsConnected = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // listBox_GameServers
            // 
            this.listBox_GameServers.FormattingEnabled = true;
            this.listBox_GameServers.Location = new System.Drawing.Point(12, 68);
            this.listBox_GameServers.Name = "listBox_GameServers";
            this.listBox_GameServers.Size = new System.Drawing.Size(120, 251);
            this.listBox_GameServers.TabIndex = 0;
            this.listBox_GameServers.SelectedIndexChanged += new System.EventHandler(this.listBox_GameServers_SelectedIndexChanged);
            // 
            // label_static_GSs
            // 
            this.label_static_GSs.AutoSize = true;
            this.label_static_GSs.Location = new System.Drawing.Point(9, 49);
            this.label_static_GSs.Name = "label_static_GSs";
            this.label_static_GSs.Size = new System.Drawing.Size(77, 13);
            this.label_static_GSs.TabIndex = 1;
            this.label_static_GSs.Text = "Game Servers:";
            // 
            // listBox_Players
            // 
            this.listBox_Players.FormattingEnabled = true;
            this.listBox_Players.Location = new System.Drawing.Point(180, 68);
            this.listBox_Players.Name = "listBox_Players";
            this.listBox_Players.Size = new System.Drawing.Size(120, 160);
            this.listBox_Players.TabIndex = 2;
            // 
            // label_static_Players
            // 
            this.label_static_Players.AutoSize = true;
            this.label_static_Players.Location = new System.Drawing.Point(176, 49);
            this.label_static_Players.Name = "label_static_Players";
            this.label_static_Players.Size = new System.Drawing.Size(44, 13);
            this.label_static_Players.TabIndex = 3;
            this.label_static_Players.Text = "Players:";
            // 
            // label_static_ID
            // 
            this.label_static_ID.AutoSize = true;
            this.label_static_ID.Location = new System.Drawing.Point(306, 68);
            this.label_static_ID.Name = "label_static_ID";
            this.label_static_ID.Size = new System.Drawing.Size(21, 13);
            this.label_static_ID.TabIndex = 4;
            this.label_static_ID.Text = "ID:";
            // 
            // label_gsID
            // 
            this.label_gsID.AutoSize = true;
            this.label_gsID.Location = new System.Drawing.Point(345, 68);
            this.label_gsID.Name = "label_gsID";
            this.label_gsID.Size = new System.Drawing.Size(30, 13);
            this.label_gsID.TabIndex = 5;
            this.label_gsID.Text = "_ID_";
            // 
            // checkBox_gsIsConnected
            // 
            this.checkBox_gsIsConnected.AutoSize = true;
            this.checkBox_gsIsConnected.Location = new System.Drawing.Point(309, 45);
            this.checkBox_gsIsConnected.Name = "checkBox_gsIsConnected";
            this.checkBox_gsIsConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_gsIsConnected.TabIndex = 6;
            this.checkBox_gsIsConnected.Text = "Connected";
            this.checkBox_gsIsConnected.UseVisualStyleBackColor = true;
            // 
            // MasterServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 398);
            this.Controls.Add(this.checkBox_gsIsConnected);
            this.Controls.Add(this.label_gsID);
            this.Controls.Add(this.label_static_ID);
            this.Controls.Add(this.label_static_Players);
            this.Controls.Add(this.listBox_Players);
            this.Controls.Add(this.label_static_GSs);
            this.Controls.Add(this.listBox_GameServers);
            this.Name = "MasterServerWindow";
            this.Text = "Master Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_GameServers;
        private System.Windows.Forms.Label label_static_GSs;
        private System.Windows.Forms.ListBox listBox_Players;
        private System.Windows.Forms.Label label_static_Players;
        private System.Windows.Forms.Label label_static_ID;
        private System.Windows.Forms.Label label_gsID;
        private System.Windows.Forms.CheckBox checkBox_gsIsConnected;
    }
}

