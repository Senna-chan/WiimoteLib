namespace WiiInputMapper
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
            this.txtCompileOutput = new System.Windows.Forms.RichTextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnRunPrecompiled = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCtrlOToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.quitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCompile = new System.Windows.Forms.Button();
            this.txtCode = new System.Windows.Forms.RichTextBox();
            this.btnStopScript = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtCompileOutput
            // 
            this.txtCompileOutput.AcceptsTab = true;
            this.txtCompileOutput.Location = new System.Drawing.Point(3, 545);
            this.txtCompileOutput.Name = "txtCompileOutput";
            this.txtCompileOutput.ReadOnly = true;
            this.txtCompileOutput.ShowSelectionMargin = true;
            this.txtCompileOutput.Size = new System.Drawing.Size(1249, 188);
            this.txtCompileOutput.TabIndex = 3;
            this.txtCompileOutput.Text = "";
            this.txtCompileOutput.WordWrap = false;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(120, 500);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(95, 39);
            this.btnRun.TabIndex = 4;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnRunPrecompiled
            // 
            this.btnRunPrecompiled.Location = new System.Drawing.Point(230, 500);
            this.btnRunPrecompiled.Name = "btnRunPrecompiled";
            this.btnRunPrecompiled.Size = new System.Drawing.Size(95, 39);
            this.btnRunPrecompiled.TabIndex = 5;
            this.btnRunPrecompiled.Text = "Run precompiled";
            this.btnRunPrecompiled.UseVisualStyleBackColor = true;
            this.btnRunPrecompiled.Click += new System.EventHandler(this.btnRunPrecompiled_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1252, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCtrlOToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.closeToolStripMenuItem,
            this.toolStripSeparator1,
            this.quitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openCtrlOToolStripMenuItem
            // 
            this.openCtrlOToolStripMenuItem.Name = "openCtrlOToolStripMenuItem";
            this.openCtrlOToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.openCtrlOToolStripMenuItem.Text = "Open";
            this.openCtrlOToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(100, 6);
            // 
            // quitToolStripMenuItem
            // 
            this.quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            this.quitToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.quitToolStripMenuItem.Text = "Quit";
            this.quitToolStripMenuItem.Click += new System.EventHandler(this.quitToolStripMenuItem_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Location = new System.Drawing.Point(3, 500);
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(103, 39);
            this.btnCompile.TabIndex = 2;
            this.btnCompile.Text = "Compile";
            this.btnCompile.UseVisualStyleBackColor = true;
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
            // 
            // txtCode
            // 
            this.txtCode.AcceptsTab = true;
            this.txtCode.Location = new System.Drawing.Point(3, 27);
            this.txtCode.Name = "txtCode";
            this.txtCode.ShowSelectionMargin = true;
            this.txtCode.Size = new System.Drawing.Size(1249, 468);
            this.txtCode.TabIndex = 1;
            this.txtCode.Text = "";
            this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
            // 
            // btnStopScript
            // 
            this.btnStopScript.Location = new System.Drawing.Point(120, 500);
            this.btnStopScript.Name = "btnStopScript";
            this.btnStopScript.Size = new System.Drawing.Size(95, 39);
            this.btnStopScript.TabIndex = 7;
            this.btnStopScript.Text = "Stop";
            this.btnStopScript.UseVisualStyleBackColor = true;
            this.btnStopScript.Visible = false;
            this.btnStopScript.Click += new System.EventHandler(this.btnStopScript_Click);
            // 
            // InputMapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1252, 740);
            this.Controls.Add(this.btnStopScript);
            this.Controls.Add(this.btnRunPrecompiled);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtCompileOutput);
            this.Controls.Add(this.btnCompile);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "InputMapper";
            this.Text = "InputMapper";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox txtCompileOutput;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnRunPrecompiled;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCtrlOToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.Button btnCompile;
        private System.Windows.Forms.RichTextBox txtCode;
        private System.Windows.Forms.Button btnStopScript;
    }
}