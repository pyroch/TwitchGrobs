
namespace TwitchGrobs
{
    partial class TwitchGrobsForm
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
            this.status = new System.Windows.Forms.Label();
            this.currentStreamerText = new System.Windows.Forms.Label();
            this.currStreamer = new System.Windows.Forms.Label();
            this.version = new System.Windows.Forms.Label();
            this.verText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // status
            // 
            this.status.AutoSize = true;
            this.status.Location = new System.Drawing.Point(12, 9);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(52, 13);
            this.status.TabIndex = 0;
            this.status.Text = "Starting...";
            // 
            // currentStreamerText
            // 
            this.currentStreamerText.AutoSize = true;
            this.currentStreamerText.Location = new System.Drawing.Point(12, 57);
            this.currentStreamerText.Name = "currentStreamerText";
            this.currentStreamerText.Size = new System.Drawing.Size(87, 13);
            this.currentStreamerText.TabIndex = 1;
            this.currentStreamerText.Text = "Current streamer:";
            // 
            // currStreamer
            // 
            this.currStreamer.AutoSize = true;
            this.currStreamer.Location = new System.Drawing.Point(106, 57);
            this.currStreamer.Name = "currStreamer";
            this.currStreamer.Size = new System.Drawing.Size(33, 13);
            this.currStreamer.TabIndex = 2;
            this.currStreamer.Text = "None";
            // 
            // version
            // 
            this.version.AutoSize = true;
            this.version.Location = new System.Drawing.Point(97, 155);
            this.version.Name = "version";
            this.version.Size = new System.Drawing.Size(31, 13);
            this.version.TabIndex = 4;
            this.version.Text = "0.0.0";
            // 
            // verText
            // 
            this.verText.AutoSize = true;
            this.verText.Location = new System.Drawing.Point(12, 155);
            this.verText.Name = "verText";
            this.verText.Size = new System.Drawing.Size(79, 13);
            this.verText.TabIndex = 6;
            this.verText.Text = "Current Version";
            // 
            // TwitchGrobsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 177);
            this.Controls.Add(this.verText);
            this.Controls.Add(this.version);
            this.Controls.Add(this.currStreamer);
            this.Controls.Add(this.currentStreamerText);
            this.Controls.Add(this.status);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TwitchGrobsForm";
            this.Text = "TwitchGrobs";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label status;
        private System.Windows.Forms.Label currentStreamerText;
        private System.Windows.Forms.Label currStreamer;
        private System.Windows.Forms.Label version;
        private System.Windows.Forms.Label verText;
    }
}

