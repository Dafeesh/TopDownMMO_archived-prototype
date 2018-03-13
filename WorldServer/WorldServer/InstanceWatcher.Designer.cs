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
            this.button_RefreshList = new System.Windows.Forms.Button();
            this.panel_InstanceView = new System.Windows.Forms.Panel();
            this.label_InstanceName = new System.Windows.Forms.Label();
            this.backgroundWorker_InstanceWatch = new System.ComponentModel.BackgroundWorker();
            this.label_Players = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // listBox_Instances
            // 
            this.listBox_Instances.FormattingEnabled = true;
            this.listBox_Instances.Location = new System.Drawing.Point(12, 92);
            this.listBox_Instances.Name = "listBox_Instances";
            this.listBox_Instances.Size = new System.Drawing.Size(126, 225);
            this.listBox_Instances.TabIndex = 0;
            this.listBox_Instances.SelectedIndexChanged += new System.EventHandler(this.listBox_Instances_SelectedIndexChanged);
            // 
            // button_RefreshList
            // 
            this.button_RefreshList.Location = new System.Drawing.Point(13, 61);
            this.button_RefreshList.Name = "button_RefreshList";
            this.button_RefreshList.Size = new System.Drawing.Size(75, 23);
            this.button_RefreshList.TabIndex = 1;
            this.button_RefreshList.Text = "Refresh";
            this.button_RefreshList.UseVisualStyleBackColor = true;
            this.button_RefreshList.Click += new System.EventHandler(this.button_RefreshList_Click);
            // 
            // panel_InstanceView
            // 
            this.panel_InstanceView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel_InstanceView.Location = new System.Drawing.Point(220, 72);
            this.panel_InstanceView.Name = "panel_InstanceView";
            this.panel_InstanceView.Size = new System.Drawing.Size(200, 200);
            this.panel_InstanceView.TabIndex = 2;
            // 
            // label_InstanceName
            // 
            this.label_InstanceName.AutoSize = true;
            this.label_InstanceName.Location = new System.Drawing.Point(220, 53);
            this.label_InstanceName.Name = "label_InstanceName";
            this.label_InstanceName.Size = new System.Drawing.Size(88, 13);
            this.label_InstanceName.TabIndex = 3;
            this.label_InstanceName.Text = "_InstanceName_";
            // 
            // label_Players
            // 
            this.label_Players.AutoSize = true;
            this.label_Players.Location = new System.Drawing.Point(367, 53);
            this.label_Players.Name = "label_Players";
            this.label_Players.Size = new System.Drawing.Size(53, 13);
            this.label_Players.TabIndex = 4;
            this.label_Players.Text = "_Players_";
            // 
            // InstanceWatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 365);
            this.Controls.Add(this.label_Players);
            this.Controls.Add(this.label_InstanceName);
            this.Controls.Add(this.panel_InstanceView);
            this.Controls.Add(this.button_RefreshList);
            this.Controls.Add(this.listBox_Instances);
            this.Name = "InstanceWatcher";
            this.Text = "InstanceWatcher";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InstanceWatcher_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_Instances;
        private System.Windows.Forms.Button button_RefreshList;
        private System.Windows.Forms.Panel panel_InstanceView;
        private System.Windows.Forms.Label label_InstanceName;
        private System.ComponentModel.BackgroundWorker backgroundWorker_InstanceWatch;
        private System.Windows.Forms.Label label_Players;
    }
}