namespace WiiInputMapper.Views
{
    partial class ProgrammerHelp
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
            this.webHelp = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webHelp
            // 
            this.webHelp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webHelp.Location = new System.Drawing.Point(0, 0);
            this.webHelp.MinimumSize = new System.Drawing.Size(20, 20);
            this.webHelp.Name = "webHelp";
            this.webHelp.Size = new System.Drawing.Size(800, 450);
            this.webHelp.TabIndex = 0;
            // 
            // ProgrammerHelp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.webHelp);
            this.Name = "ProgrammerHelp";
            this.Text = "ProgrammerHelp";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webHelp;
    }
}