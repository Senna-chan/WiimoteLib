namespace WiiInputMapper.Template
{
    partial class VarShower
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
            this.pnlVarShower = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // pnlVarShower
            // 
            this.pnlVarShower.Location = new System.Drawing.Point(0, 2);
            this.pnlVarShower.Name = "pnlVarShower";
            this.pnlVarShower.Size = new System.Drawing.Size(780, 450);
            this.pnlVarShower.TabIndex = 0;
            // 
            // VarShower
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 451);
            this.Controls.Add(this.pnlVarShower);
            this.Name = "VarShower";
            this.Text = "VarShower";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel pnlVarShower;
    }
}