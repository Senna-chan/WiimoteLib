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

namespace WiiInputMapper
{
	public partial class InputMapper : Form
	{
		String codeFileName = null;
		bool fileSaved = false;
		MethodInfo scriptStopInfo;
		object inputMapperScript;
		public InputMapper()
		{
			InitializeComponent();
		}

		private void btnCompile_Click(object sender, EventArgs e)
		{
			try
			{
				String code = parseCode();
				if (code == "") return;
				Compile(code, false);
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
				String code = parseCode();
				if (code == "") return;
				Compile(code, true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnRunPrecompiled_Click(object sender, EventArgs e)
		{
			btnRun.Visible = false;
			btnRunPrecompiled.Visible = false;
			btnStopScript.Visible = true;
			inputMapperScript = new InputMapperTemplate();
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

		private void checkAndInitBool(String userCodeStr, String contains, String mapKey)
		{
			if (userCodeStr.Contains(contains))
			{
				if (!initCode.ContainsKey(mapKey))
				{
					initCode.Add(mapKey, $"{mapKey} = true;");
				}
			}
		}

		Dictionary<String, String> variables = new Dictionary<string, string>();
		Dictionary<String, String> initCode = new Dictionary<string, string>();
		StringBuilder userCode = new StringBuilder();
		bool rawCode = false;
		private string parseCode()
		{
			variables.Clear();
			initCode.Clear();
			userCode.Clear();
			rawCode = false;
			string[] userCodeLines = txtCode.Text.Split(new[] { "\r\n", "\r","\n" },StringSplitOptions.None);
			// Code changing
			bool isCodeBlock = false; // When true this will make sure the code is between accolades({ and })
			int codeBlockAmount = 0;
			foreach(string userCodeLine in userCodeLines) {
				string userCodeStr = userCodeLine;

				if(userCodeStr.Equals("rawcode") && !rawCode) // No editing of the codeline
				{
					rawCode = true;
					continue;
				}

				userCodeStr = userCodeStr.Replace("log(", "Console.WriteLine(");
				userCodeStr = userCodeStr.Replace("Accel.X", "Accel.Values.X");
				userCodeStr = userCodeStr.Replace("Accel.Y", "Accel.Values.Y");
				userCodeStr = userCodeStr.Replace("Accel.Z", "Accel.Values.Z");
				userCodeStr = userCodeStr.Replace("Accel.Pitch", "Accel.IMU.Pitch");
				userCodeStr = userCodeStr.Replace("Accel.Roll", "Accel.IMU.Roll");
				userCodeStr = userCodeStr.Replace("Accel.Yaw", "Accel.IMU.Yaw");
				//userCodeStr = userCodeStr.Replace("Wiimote.IR.", "Wiimote.IR.Midpoint.");

				var tabAmounts = userCodeStr.Length - userCodeStr.Replace("\t", "").Length;

				userCodeStr = userCodeStr.Replace("\t", ""); // We add tabs ourself and this is beter for StartsWith
				if (isCodeBlock)
				{
					if (tabAmounts != codeBlockAmount)
					{
						appendUserCode("\t\t\t"); // Normal spacing
						if (codeBlockAmount != 0)
						{
							for (int i = codeBlockAmount; i > 1; i--)
							{
								appendUserCode("\t");
							}
						}
						appendUserCodeLine("}");
						codeBlockAmount--;
					}
					if(codeBlockAmount == 0)
					{
						isCodeBlock = false;
					}
				}

				if (userCodeStr.Contains("Accel.IMU.Pitch") && !userCodeStr.StartsWith("show("))  // We need to validate the pitch.
				{
					string nonPitchInstruction = userCodeStr.Substring(0,userCodeStr.IndexOf("Wiimote"));
					string pitchInstruction = userCodeStr.Substring(userCodeStr.IndexOf("Wiimote"));
					RegexOptions options = RegexOptions.None;
					Regex regex = new Regex("[ ]{2,}", options);
					pitchInstruction = regex.Replace(pitchInstruction, " ");
					char compareChar = ' ';
					if (pitchInstruction.Contains(">")) compareChar = '>';
					if (pitchInstruction.Contains("<")) compareChar = '<';
					if (pitchInstruction.Contains("=")) compareChar = '=';
					string[] pitchInstructionparts = pitchInstruction.Split(' ');
					string newPitchInstruction = $"DataValidater.ValidatePitch({pitchInstructionparts[0]}, {pitchInstructionparts[0].Replace(".Pitch", ".Roll")}, {pitchInstructionparts[2].Replace(";","")}, '{compareChar}')";
					userCodeStr = $"{nonPitchInstruction}{newPitchInstruction}";
				}
				
				checkAndInitBool(userCodeStr, "XBox.", "initscp");
				checkAndInitBool(userCodeStr, "Wiimote.IR", "usingIR");
				checkAndInitBool(userCodeStr, "IRDifference", "usingIR");

				if (userCodeStr.StartsWith("Mouse"))
				{
					//Mouse.X = Wiimote.Nunchuk.Joystick.X;
					//Mouse = Wiimote.Nunchuck.Joystick;
					string MouseInstruction = userCodeStr.Substring(0, userCodeStr.IndexOf("=")).Trim();
					string WiimoteInstruction = userCodeStr.Substring(userCodeStr.IndexOf("=") + 1).Replace(";","").Trim();
					if (MouseInstruction.Contains("."))
					{
						if (MouseInstruction.Contains("X"))
						{
							userCodeStr = $"Mouse.MoveRel({WiimoteInstruction} * mousespeed, 0);";
						}
						else if (MouseInstruction.Contains("Y"))
						{
							userCodeStr = $"Mouse.MoveRel(0, {WiimoteInstruction} * mousespeed);";
						}
					}
					else
					{
						userCodeStr = $"Mouse.MoveRel({WiimoteInstruction}.X * mousespeed, {WiimoteInstruction}.Y * mousespeed);";
					}
				}
				else if (userCodeStr.StartsWith("var "))
				{
					string varname = userCodeStr.Replace("var ", "").Split('=')[0].Trim();
					string varvalue = userCodeStr.Replace("var ", "").Split('=')[1].Trim().Replace(";","");
					try {
						variables.Add(varname, varvalue);
					} catch(ArgumentException ex)
					{
						txtCompileOutput.Text = $"ERROR: Variable '{varname}' already exists";
						return "";
					}
					continue;
				}
				else if (userCodeStr.StartsWith("show("))
				{
					string varname = userCodeStr.Replace("show(", "").Replace(")", "").Replace(";","").Trim();
					userCodeStr = $"varShower.ShowVar(\"{varname}\", {varname});";
					if (!initCode.ContainsKey("initvarshower"))
					{
						initCode.Add("initvarshower", "initvarshower = true;");
					}
				}
				else if (userCodeStr.StartsWith("if(") && !userCodeStr.EndsWith("{"))
				{
					isCodeBlock = true;
					userCodeStr = userCodeStr + "{";
					codeBlockAmount++;
				}
				
				if (userCodeStr.Contains("Keyboard."))
				{
					string KeyboardInstruction = userCodeStr.Substring(0,userCodeStr.IndexOf('=')).Trim();
					string keyInstruction = userCodeStr.Substring(userCodeStr.IndexOf('=') + 1).Replace(';', ' ').Trim();
					if (userCodeStr.Contains("Keyboard.Press"))
					{
						userCodeStr = $"Keyboard.Press({KeyboardInstruction.Replace("Keyboard.Press.", "KeyCode.")}, {keyInstruction});";
					} 
					else if (userCodeStr.Contains("Keyboard.KeyUp"))
					{
						userCodeStr = $"Keyboard.Up({KeyboardInstruction.Replace("Keyboard.KeyUp.", "KeyCode.")}, {keyInstruction});";
					} 
					else if (userCodeStr.Contains("Keyboard.KeyDown"))
					{
						userCodeStr = $"Keyboard.Down({KeyboardInstruction.Replace("Keyboard.KeyDown.", "KeyCode.")}, {keyInstruction});";
					}
					else
					{
						userCodeStr = $"Keyboard.Press({KeyboardInstruction.Replace("Keyboard.", "KeyCode.")}, {keyInstruction});";
					}
				}

				if (userCodeStr.Contains("Wiimote.IR")) {
					if (!userCodeStr.Contains("Wiimote.IR.")) userCodeStr = userCodeStr.Replace("Wiimote.IR", "Wiimote.IR.Midpoint");
					userCodeStr = userCodeStr.Replace("Wiimote.IR.X", "Wiimote.IR.Midpoint.RawPosition.X");
					userCodeStr = userCodeStr.Replace("Wiimote.IR.Y", "Wiimote.IR.Midpoint.RawPosition.Y");
					userCodeStr = userCodeStr.Replace("Wiimote.IR.Position", "Wiimote.IR.Midpoint.Position");
					userCodeStr = userCodeStr.Replace("Wiimote.IR.RawPosition", "Wiimote.IR.Midpoint.RawPosition");
					

					if (!(
						userCodeStr.Contains("varShower") ||
						userCodeStr.StartsWith("if(") ||
						variables.ContainsKey(userCodeStr.Split(' ')[0])
						)) 
					userCodeStr = $"if(Wiimote.IR.Midpoint.ValidPosition) {userCodeStr}";
				}

				appendUserCode("\t\t\t"); // Normal spacing
				for (; tabAmounts > 0; tabAmounts--)
				{
					appendUserCode("\t");
				}
				appendUserCodeLine(userCodeStr);
				if (rawCode)
				{
					userCode.AppendLine(userCodeLine);
				}
			}
			if(isCodeBlock)
				appendUserCodeLine("}");
			if (!variables.ContainsKey("mousespeed")) variables.Add("mousespeed", "1.0");
			StringBuilder variableString = new StringBuilder();
			foreach(KeyValuePair<String, String> varKV in variables)
			{
				string varName = varKV.Key;
				string varValue = varKV.Value;
				string varType = "dynamic";
				Regex numberRegex = new Regex("[^a-z ]*([0-9])*\\d", RegexOptions.None);
				if (numberRegex.IsMatch(varKV.Value))
				{
					varType = "float";
					varValue += "f";
				}
				if (varKV.Value.Contains("true") || varKV.Value.Contains("false")) varType = "bool";
				variableString.AppendLine($"{varType} {varName} = {varValue};");
			}
			StringBuilder initString = new StringBuilder();
			foreach (String initStr in initCode.Values)
			{
				initString.AppendLine(initStr);
			}
			String code = Resources.InputMapperTemplate;
			code = code.Replace("//USERCODE", userCode.ToString());
			code = code.Replace("//GLOBALS", variableString.ToString());
			code = code.Replace("//INIT", initString.ToString());
			code = code.Replace("SOURCECODE", txtCode.Text.Replace("\"","\\\""));

            File.WriteAllText("../../script.cs", code.Replace("namespace WiiInputMapper.Template", "namespace WiiInputMapper")); // Save code to build folder to be able to debug it

            return code;
		}

		void appendUserCodeLine(String codeToAppend)
		{
			if (!rawCode) userCode.AppendLine(codeToAppend);
		}
		void appendUserCode(String codeToAppend)
		{
			if (!rawCode) userCode.Append(codeToAppend);
		}

		void Compile(string code, bool andRun)
		{

			CompilerParameters CompilerParams = new CompilerParameters();
			string outputDirectory = Directory.GetCurrentDirectory();

			CompilerParams.GenerateInMemory = true;
			CompilerParams.TreatWarningsAsErrors = false;
			CompilerParams.GenerateExecutable = false;
			CompilerParams.CompilerOptions = "/optimize";
			CompilerParams.IncludeDebugInformation = true;
			if (Directory.Exists("./tempfiles"))
			{
				Directory.Delete("./tempfiles", true);
			}
			Directory.CreateDirectory("./tempfiles");


			string[] references = { "System.dll", "System.Data.dll" };
			CompilerParams.ReferencedAssemblies.AddRange(references);
			Assembly currentAssembly = Assembly.GetExecutingAssembly();
			CompilerParams.ReferencedAssemblies.Add(currentAssembly.ManifestModule.Name);
			CompilerParams.ReferencedAssemblies.AddRange(Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(a => a.Name + ".dll").ToArray());

			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code);

			

			if (compile.Errors.HasErrors)
			{
				StringBuilder text = new StringBuilder();
				text.AppendLine("Compile error: ");
				foreach (CompilerError ce in compile.Errors)
				{
					text.AppendLine(ce.ToString());
				}
				txtCompileOutput.Text = text.ToString();
			}
			else
			{
				txtCompileOutput.Text = "Compile success";
                if (!andRun) return;
				Module[] modules = compile.CompiledAssembly.GetModules();
				Module module = modules[0];
				Type mt = null;
				ConstructorInfo constructorInfo = null;
				if (module != null)
				{
					mt = module.GetType("WiiInputMapper.Template.InputMapperTemplate");
				}

				if (mt != null)
				{
					scriptStopInfo = mt.GetMethod("StopScript");
					constructorInfo = mt.GetConstructors()[0];
				}
				else {
					throw new Exception("Uhm... There is something wrong with the template. Could not find the class");
				}

				if (constructorInfo != null)
				{
					inputMapperScript = constructorInfo.Invoke(null);
				}
				else
				{
					throw new Exception("Uhm... There is something wrong with the template. Could not find a constructor");
				}
			}
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
	}
}
