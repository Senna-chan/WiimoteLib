namespace WiimoteTest
{
    partial class InputMapper
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
            this.txtCode = new System.Windows.Forms.RichTextBox();
            this.btnCompile = new System.Windows.Forms.Button();
            this.txtCompileOutput = new System.Windows.Forms.RichTextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtCode
            // 
            this.txtCode.AcceptsTab = true;
            this.txtCode.Location = new System.Drawing.Point(0, 0);
            this.txtCode.Name = "txtCode";
            this.txtCode.ShowSelectionMargin = true;
            this.txtCode.Size = new System.Drawing.Size(1249, 465);
            this.txtCode.TabIndex = 1;
            this.txtCode.Text = "";
            // 
            // btnCompile
            // 
            this.btnCompile.Location = new System.Drawing.Point(0, 471);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(103, 36);
            this.btnCompile.TabIndex = 2;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // txtCompileOutput
            // 
            this.txtCompileOutput.AcceptsTab = true;
            this.txtCompileOutput.Location = new System.Drawing.Point(0, 518);
            this.txtCompileOutput.Name = "txtCompileOutput";
            this.txtCompileOutput.ReadOnly = true;
            this.txtCompileOutput.ShowSelectionMargin = true;
            this.txtCompileOutput.Size = new System.Drawing.Size(1249, 185);
            this.txtCompileOutput.TabIndex = 3;
            this.txtCompileOutput.Text = "";
            this.txtCompileOutput.WordWrap = false;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(119, 471);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(95, 36);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // InputMapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1252, 703);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtCompileOutput);
            this.Controls.Add(this.btnCompile);
            this.Controls.Add(this.txtCode);
            this.Name = "InputMapper";
            this.Text = "InputMapper";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.RichTextBox txtCode;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.RichTextBox txtCompileOutput;
        private System.Windows.Forms.Button btnRun;
    }
}