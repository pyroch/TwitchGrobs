
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TwitchGrobsForm));
            this.status = new System.Windows.Forms.Label();
            this.currentStreamerText = new System.Windows.Forms.Label();
            this.currStreamer = new System.Windows.Forms.Label();
            this.streamersList = new System.Windows.Forms.ListView();
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
            this.currentStreamerText.Location = new System.Drawing.Point(12, 239);
            this.currentStreamerText.Name = "currentStreamerText";
            this.currentStreamerText.Size = new System.Drawing.Size(87, 13);
            this.currentStreamerText.TabIndex = 1;
            this.currentStreamerText.Text = "Current streamer:";
            // 
            // currStreamer
            // 
            this.currStreamer.AutoSize = true;
            this.currStreamer.Location = new System.Drawing.Point(105, 239);
            this.currStreamer.Name = "currStreamer";
            this.currStreamer.Size = new System.Drawing.Size(0, 13);
            this.currStreamer.TabIndex = 2;
            // 
            // streamersList
            // 
            this.streamersList.Enabled = false;
            this.streamersList.HideSelection = false;
            this.streamersList.Location = new System.Drawing.Point(13, 26);
            this.streamersList.Name = "streamersList";
            this.streamersList.Size = new System.Drawing.Size(259, 210);
            this.streamersList.TabIndex = 3;
            this.streamersList.UseCompatibleStateImageBehavior = false;
            this.streamersList.View = System.Windows.Forms.View.Details;
            // 
            // TwitchGrobsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.streamersList);
            this.Controls.Add(this.currStreamer);
            this.Controls.Add(this.currentStreamerText);
            this.Controls.Add(this.status);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
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
        private System.Windows.Forms.ListView streamersList;
    }
}

