namespace ForwardServer
{
    partial class ForwardServerWindow
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBox_ConToMaster = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox_IsPlrConnected = new System.Windows.Forms.CheckBox();
            this.label_static_PlayerName = new System.Windows.Forms.Label();
            this.label_PlayerName = new System.Windows.Forms.Label();
            this.listBox_Players = new System.Windows.Forms.ListBox();
            this.label_static_Players = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBox_ConToMaster);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(134, 27);
            this.panel1.TabIndex = 1;
            // 
            // checkBox_ConToMaster
            // 
            this.checkBox_ConToMaster.AutoSize = true;
            this.checkBox_ConToMaster.Enabled = false;
            this.checkBox_ConToMaster.Location = new System.Drawing.Point(3, 3);
            this.checkBox_ConToMaster.Name = "checkBox_ConToMaster";
            this.checkBox_ConToMaster.Size = new System.Drawing.Size(125, 17);
            this.checkBox_ConToMaster.TabIndex = 2;
            this.checkBox_ConToMaster.Text = "Connected to Master";
            this.checkBox_ConToMaster.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.checkBox_IsPlrConnected);
            this.panel2.Controls.Add(this.label_static_PlayerName);
            this.panel2.Controls.Add(this.label_PlayerName);
            this.panel2.Controls.Add(this.listBox_Players);
            this.panel2.Controls.Add(this.label_static_Players);
            this.panel2.Location = new System.Drawing.Point(12, 45);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(293, 266);
            this.panel2.TabIndex = 2;
            // 
            // checkBox_IsPlrConnected
            // 
            this.checkBox_IsPlrConnected.AutoSize = true;
            this.checkBox_IsPlrConnected.Enabled = false;
            this.checkBox_IsPlrConnected.Location = new System.Drawing.Point(137, 21);
            this.checkBox_IsPlrConnected.Name = "checkBox_IsPlrConnected";
            this.checkBox_IsPlrConnected.Size = new System.Drawing.Size(78, 17);
            this.checkBox_IsPlrConnected.TabIndex = 3;
            this.checkBox_IsPlrConnected.Text = "Connected";
            this.checkBox_IsPlrConnected.UseVisualStyleBackColor = true;
            // 
            // label_static_PlayerName
            // 
            this.label_static_PlayerName.AutoSize = true;
            this.label_static_PlayerName.Location = new System.Drawing.Point(196, 44);
            this.label_static_PlayerName.Name = "label_static_PlayerName";
            this.label_static_PlayerName.Size = new System.Drawing.Size(47, 13);
            this.label_static_PlayerName.TabIndex = 5;
            this.label_static_PlayerName.Text = "_Name_";
            // 
            // label_PlayerName
            // 
            this.label_PlayerName.AutoSize = true;
            this.label_PlayerName.Location = new System.Drawing.Point(134, 44);
            this.label_PlayerName.Name = "label_PlayerName";
            this.label_PlayerName.Size = new System.Drawing.Size(38, 13);
            this.label_PlayerName.TabIndex = 4;
            this.label_PlayerName.Text = "Name:";
            // 
            // listBox_Players
            // 
            this.listBox_Players.FormattingEnabled = true;
            this.listBox_Players.Location = new System.Drawing.Point(5, 21);
            this.listBox_Players.Name = "listBox_Players";
            this.listBox_Players.Size = new System.Drawing.Size(120, 238);
            this.listBox_Players.TabIndex = 3;
            // 
            // label_static_Players
            // 
            this.label_static_Players.AutoSize = true;
            this.label_static_Players.Location = new System.Drawing.Point(4, 5);
            this.label_static_Players.Name = "label_static_Players";
            this.label_static_Players.Size = new System.Drawing.Size(44, 13);
            this.label_static_Players.TabIndex = 3;
            this.label_static_Players.Text = "Players:";
            // 
            // ForwardServerWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(316, 323);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "ForwardServerWindow";
            this.Text = "Forward Server";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ForwardServerWindow_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox checkBox_ConToMaster;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ListBox listBox_Players;
        private System.Windows.Forms.Label label_static_Players;
        private System.Windows.Forms.Label label_PlayerName;
        private System.Windows.Forms.CheckBox checkBox_IsPlrConnected;
        private System.Windows.Forms.Label label_static_PlayerName;
    }
}