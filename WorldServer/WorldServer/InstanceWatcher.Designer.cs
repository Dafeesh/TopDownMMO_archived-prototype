namespace WorldServer
{
    partial class InstanceWatcher
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
            this.listBox_Instances = new System.Windows.Forms.ListBox();
            this.button_RefreshInstanceList = new System.Windows.Forms.Button();
            this.panel_InstanceView = new System.Windows.Forms.Panel();
            this.label_InstanceName = new System.Windows.Forms.Label();
            this.backgroundWorker_InstanceWatch = new System.ComponentModel.BackgroundWorker();
            this.label_Players = new System.Windows.Forms.Label();
            this.listBox_Players = new System.Windows.Forms.ListBox();
            this.label_InstancesTitle = new System.Windows.Forms.Label();
            this.label_PlayersTitle = new System.Windows.Forms.Label();
            this.button_RefreshPlayerList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox_Instances
            // 
            this.listBox_Instances.FormattingEnabled = true;
            this.listBox_Instances.Location = new System.Drawing.Point(294, 37);
            this.listBox_Instances.Name = "listBox_Instances";
            this.listBox_Instances.Size = new System.Drawing.Size(126, 225);
            this.listBox_Instances.TabIndex = 0;
            this.listBox_Instances.SelectedIndexChanged += new System.EventHandler(this.listBox_Instances_SelectedIndexChanged);
            // 
            // button_RefreshInstanceList
            // 
            this.button_RefreshInstanceList.Location = new System.Drawing.Point(294, 268);
            this.button_RefreshInstanceList.Name = "button_RefreshInstanceList";
            this.button_RefreshInstanceList.Size = new System.Drawing.Size(75, 23);
            this.button_RefreshInstanceList.TabIndex = 1;
            this.button_RefreshInstanceList.Text = "Refresh";
            this.button_RefreshInstanceList.UseVisualStyleBackColor = true;
            this.button_RefreshInstanceList.Click += new System.EventHandler(this.button_RefreshList_Click);
            // 
            // panel_InstanceView
            // 
            this.panel_InstanceView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_InstanceView.Location = new System.Drawing.Point(429, 37);
            this.panel_InstanceView.Name = "panel_InstanceView";
            this.panel_InstanceView.Size = new System.Drawing.Size(200, 200);
            this.panel_InstanceView.TabIndex = 2;
            // 
            // label_InstanceName
            // 
            this.label_InstanceName.AutoSize = true;
            this.label_InstanceName.Location = new System.Drawing.Point(426, 240);
            this.label_InstanceName.Name = "label_InstanceName";
            this.label_InstanceName.Size = new System.Drawing.Size(88, 13);
            this.label_InstanceName.TabIndex = 3;
            this.label_InstanceName.Text = "_InstanceName_";
            // 
            // label_Players
            // 
            this.label_Players.AutoSize = true;
            this.label_Players.Location = new System.Drawing.Point(576, 240);
            this.label_Players.Name = "label_Players";
            this.label_Players.Size = new System.Drawing.Size(53, 13);
            this.label_Players.TabIndex = 4;
            this.label_Players.Text = "_Players_";
            // 
            // listBox_Players
            // 
            this.listBox_Players.FormattingEnabled = true;
            this.listBox_Players.Location = new System.Drawing.Point(12, 37);
            this.listBox_Players.Name = "listBox_Players";
            this.listBox_Players.Size = new System.Drawing.Size(126, 225);
            this.listBox_Players.TabIndex = 5;
            // 
            // label_InstancesTitle
            // 
            this.label_InstancesTitle.AutoSize = true;
            this.label_InstancesTitle.Location = new System.Drawing.Point(291, 21);
            this.label_InstancesTitle.Name = "label_InstancesTitle";
            this.label_InstancesTitle.Size = new System.Drawing.Size(56, 13);
            this.label_InstancesTitle.TabIndex = 6;
            this.label_InstancesTitle.Text = "Instances:";
            // 
            // label_PlayersTitle
            // 
            this.label_PlayersTitle.AutoSize = true;
            this.label_PlayersTitle.Location = new System.Drawing.Point(12, 21);
            this.label_PlayersTitle.Name = "label_PlayersTitle";
            this.label_PlayersTitle.Size = new System.Drawing.Size(44, 13);
            this.label_PlayersTitle.TabIndex = 7;
            this.label_PlayersTitle.Text = "Players:";
            // 
            // button_RefreshPlayerList
            // 
            this.button_RefreshPlayerList.Location = new System.Drawing.Point(12, 268);
            this.button_RefreshPlayerList.Name = "button_RefreshPlayerList";
            this.button_RefreshPlayerList.Size = new System.Drawing.Size(75, 23);
            this.button_RefreshPlayerList.TabIndex = 8;
            this.button_RefreshPlayerList.Text = "Refresh";
            this.button_RefreshPlayerList.UseVisualStyleBackColor = true;
            this.button_RefreshPlayerList.Click += new System.EventHandler(this.button_RefreshPlayerList_Click);
            // 
            // InstanceWatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 503);
            this.Controls.Add(this.button_RefreshPlayerList);
            this.Controls.Add(this.label_PlayersTitle);
            this.Controls.Add(this.label_InstancesTitle);
            this.Controls.Add(this.listBox_Players);
            this.Controls.Add(this.label_Players);
            this.Controls.Add(this.label_InstanceName);
            this.Controls.Add(this.panel_InstanceView);
            this.Controls.Add(this.button_RefreshInstanceList);
            this.Controls.Add(this.listBox_Instances);
            this.Name = "InstanceWatcher";
            this.Text = "InstanceWatcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstanceWatcher_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_Instances;
        private System.Windows.Forms.Button button_RefreshInstanceList;
        private System.Windows.Forms.Panel panel_InstanceView;
        private System.Windows.Forms.Label label_InstanceName;
        private System.ComponentModel.BackgroundWorker backgroundWorker_InstanceWatch;
        private System.Windows.Forms.Label label_Players;
        private System.Windows.Forms.ListBox listBox_Players;
        private System.Windows.Forms.Label label_InstancesTitle;
        private System.Windows.Forms.Label label_PlayersTitle;
        private System.Windows.Forms.Button button_RefreshPlayerList;
    }
}