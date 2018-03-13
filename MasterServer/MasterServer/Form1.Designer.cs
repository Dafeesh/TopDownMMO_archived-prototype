namespace MasterServer
{
    partial class MasterServer
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
            this.SuspendLayout();
            // 
            // listBox_GameServers
            // 
            this.listBox_GameServers.FormattingEnabled = true;
            this.listBox_GameServers.Location = new System.Drawing.Point(12, 68);
            this.listBox_GameServers.Name = "listBox_GameServers";
            this.listBox_GameServers.Size = new System.Drawing.Size(120, 160);
            this.listBox_GameServers.TabIndex = 0;
            // 
            // label_static_GSs
            // 
            this.label_static_GSs.AutoSize = true;
            this.label_static_GSs.Location = new System.Drawing.Point(12, 49);
            this.label_static_GSs.Name = "label_static_GSs";
            this.label_static_GSs.Size = new System.Drawing.Size(77, 13);
            this.label_static_GSs.TabIndex = 1;
            this.label_static_GSs.Text = "Game Servers:";
            // 
            // MasterServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(514, 398);
            this.Controls.Add(this.label_static_GSs);
            this.Controls.Add(this.listBox_GameServers);
            this.Name = "MasterServer";
            this.Text = "Master Server";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox_GameServers;
        private System.Windows.Forms.Label label_static_GSs;
    }
}

