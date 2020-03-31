namespace WiimoteTest
{
	partial class MultipleWiimoteForm
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
            this.tabWiimotes = new System.Windows.Forms.TabControl();
            this.SuspendLayout();
            // 
            // tabWiimotes
            // 
            this.tabWiimotes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabWiimotes.Location = new System.Drawing.Point(0, 0);
            this.tabWiimotes.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabWiimotes.Name = "tabWiimotes";
            this.tabWiimotes.SelectedIndex = 0;
            this.tabWiimotes.Size = new System.Drawing.Size(1317, 780);
            this.tabWiimotes.TabIndex = 0;
            // 
            // MultipleWiimoteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1317, 780);
            this.Controls.Add(this.tabWiimotes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.Name = "MultipleWiimoteForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Multiple Wiimote Tester";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MultipleWiimoteForm_FormClosing);
            this.Load += new System.EventHandler(this.MultipleWiimoteForm_Load);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tabWiimotes;
	}
}