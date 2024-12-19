using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WiiInputMapper.Properties;
using WiimoteLib.DataTypes.Enums;
using WiiInputMapper.Scripting;
using WiiInputMapper.Template;
using WiiInputMapper.Views;

namespace WiiInputMapper
{
	public partial class InputMapper : Form
	{
		String codeFileName = "";
		bool fileSaved = false;
		public MethodInfo scriptStopInfo { private get; set; }
		public object inputMapperScript { private get; set; }
		Scripting.CodeParser codeParser;
		public InputMapper()
		{
			InitializeComponent();
            codeParser = new Scripting.CodeParser(this);
        }

		private void btnCompile_Click(object sender, EventArgs e)
		{
			try
			{
				String code = codeParser.ParseCode(txtCode.Text);
                if (code == "") return;
				if (code.StartsWith("ERROR: "))
				{
					txtCompileOutput.Text = code;
					return;
				}
                txtCompileOutput.Text = codeParser.Compile(code, false);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnRun_Click(object sender, EventArgs e)
		{
			btnRun.Visible = false;
			btnRunPrecompiled.Visible = false;
			btnStopScript.Visible = true;
			try
			{
				string code = codeParser.ParseCode(txtCode.Text);
				if (code == "") return;
                txtCompileOutput.Text = codeParser.Compile(code, true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnRunPrecompiled_Click(object sender, EventArgs e)
		{
			loadPreCompiled(new PrevCompiledScript());
		}

        private void btnRunPrecompiled1_Click(object sender, EventArgs e)
        {
            loadPreCompiled(new PreCompiledScriptV1());
        }

        private void btnRunPrecompiled2_Click(object sender, EventArgs e)
        {

            loadPreCompiled(new PreCompiledScriptV2());
        }
        private void loadPreCompiled(object script)
		{
            btnRun.Visible = false;
            btnRunPrecompiled.Visible = false;
            btnRunPrecompiled1.Visible = false;
            btnRunPrecompiled2.Visible = false;
            btnStopScript.Visible = true;
            inputMapperScript = script;
            txtCode.Text = inputMapperScript.GetType().GetMethod("GetSourceCode").Invoke(inputMapperScript, new object[] { }).ToString();
            scriptStopInfo = inputMapperScript.GetType().GetMethod("StopScript");
        }

		private void btnStopScript_Click(object sender, EventArgs e)
		{
			btnRun.Visible = true;
			btnRunPrecompiled.Visible = true;
			btnStopScript.Visible = false;
			scriptStopInfo?.Invoke(inputMapperScript, null);
		}
		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog()
			{
				FileName = "Select a text file",
				Filter = "Text files (*.txt)|*.txt",
				Title = "Open text file"
			};
			if(fileDialog.ShowDialog() == DialogResult.OK)
			{
				codeFileName = fileDialog.FileName;
				if (!File.Exists(codeFileName)) File.Create(codeFileName);
				txtCode.Text = File.ReadAllText(codeFileName);
				txtCompileOutput.Clear();
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (codeFileName == "")
			{
				SaveFileDialog fileDialog = new SaveFileDialog()
				{
					Filter = "Text files (*.txt)|*.txt",
					Title = "Save text file"
				};
				if (fileDialog.ShowDialog() == DialogResult.OK)
				{
					codeFileName = fileDialog.FileName;
				}
			}
			File.WriteAllText(codeFileName, txtCode.Text);
			fileSaved = true;
		}

		private void quitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!fileSaved)
			{
				if(MessageBox.Show("The code is not saved. Do you want to save it?","Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					saveToolStripMenuItem_Click(null, null);
				}
			}
			Application.Exit();
		}

		private void closeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!fileSaved)
			{
				if (MessageBox.Show("The code is not saved. Do you want to save it?", "Unsaved changes", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					saveToolStripMenuItem_Click(null, null);
				}
			}
			txtCode.Clear();
			txtCompileOutput.Clear();
		}

		private void txtCode_TextChanged(object sender, EventArgs e)
		{
			fileSaved = false;
		}

        private void showProgrammingHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {
			new ProgrammerHelp().Show();
        }
    }
}
