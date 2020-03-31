namespace WiiInputMapper.Template
{
    partial class ucVarShow
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblVarName = new System.Windows.Forms.Label();
            this.lblVarValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblVarName
            // 
            this.lblVarName.AutoSize = true;
            this.lblVarName.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblVarName.Location = new System.Drawing.Point(3, 0);
            this.lblVarName.Name = "lblVarName";
            this.lblVarName.Size = new System.Drawing.Size(89, 20);
            this.lblVarName.TabIndex = 0;
            this.lblVarName.Text = "VARNAME";
            // 
            // lblVarValue
            // 
            this.lblVarValue.AutoSize = true;
            this.lblVarValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.lblVarValue.Location = new System.Drawing.Point(426, 0);
            this.lblVarValue.Name = "lblVarValue";
            this.lblVarValue.Size = new System.Drawing.Size(97, 20);
            this.lblVarValue.TabIndex = 1;
            this.lblVarValue.Text = "VARVALUE";
            // 
            // ucVarShow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblVarValue);
            this.Controls.Add(this.lblVarName);
            this.Name = "ucVarShow";
            this.Size = new System.Drawing.Size(730, 22);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblVarName;
        private System.Windows.Forms.Label lblVarValue;
    }
}
