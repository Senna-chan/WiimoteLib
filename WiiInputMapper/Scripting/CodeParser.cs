using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using WiiInputMapper.Properties;
using WiimoteLib.DataTypes.Enums;
using WiiInputMapper.Utils;
using System.Reflection.Emit;
using WiimoteLib.DataTypes;
using WiimoteLib;

namespace WiiInputMapper.Scripting
{
    internal class CodeParser
    {
        private InputMapper inputMapper;
        public CodeParser(InputMapper inputMapper)
        {
            this.inputMapper = inputMapper;
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

        private string checkWholeCodeForMistakesAndShorthands(string code)
        {
            string checkedCode = code;
            checkedCode = checkedCode.Replace("xbox", "XBox", StringComparison.CurrentCultureIgnoreCase);
            checkedCode = checkedCode.Replace("udraw", "Tablet", StringComparison.CurrentCultureIgnoreCase);

            return checkedCode;
        }

        private string checkCodeLineForMistakesAndShorthands(string codeLine)
        {
            string checkedCodeLine = codeLine;
            var strings = checkedCodeLine.Split(' ');
            if (!checkedCodeLine.Contains("Wiimote.Buttons") && checkedCodeLine.Contains("Wiimote."))
            {
                var fields = new Wiimote().WiimoteState.Buttons.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach(var field in fields)
                {
                    //Console.WriteLine(field.Name);
                }
            }
            return checkedCodeLine;
        }

        ExtensionType validateExtensionUsed(string codeLine, ExtensionType currentExtension)
        {
            var requestedExtension = currentExtension;
            if (codeLine.Contains("Wiimote.Nunchuk"))
            {
                requestedExtension = ExtensionType.Nunchuk;
            }
            if (codeLine.Contains("Wiimote.Tablet"))
            {
                requestedExtension = ExtensionType.UDraw;
            }

            if (requestedExtension != ExtensionType.None && currentExtension != ExtensionType.None && currentExtension != requestedExtension)
            {
                requestedExtension = ExtensionType.Unknown;
            }
            return requestedExtension;
        }

        public string ParseCode(string code)
        {
            variables.Clear();
            initCode.Clear();
            userCode.Clear();
            rawCode = false;
            string unparsedCode = code;

            unparsedCode = checkWholeCodeForMistakesAndShorthands(unparsedCode);

            ExtensionType requiredExtensionType = ExtensionType.None;

            string[] userCodeLines = unparsedCode.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            // Code changing
            bool isCodeBlock = false; // When true this will make sure the code is between accolades({ and })
            int codeBlockAmount = 0;

            foreach (string userCodeLine in userCodeLines)
            {
                string userCodeStr = checkCodeLineForMistakesAndShorthands(userCodeLine);
                requiredExtensionType = validateExtensionUsed(unparsedCode, requiredExtensionType);
                if(requiredExtensionType == ExtensionType.Unknown)
                {
                    return "ERROR: MultipleExtensionsUsed";
                }

                if (userCodeStr.Equals("rawcode") && !rawCode) // No editing of the codeline
                {
                    rawCode = true;
                    continue;
                }

                if (userCodeStr.Equals("endrawcode") && rawCode) // No editing of the codeline
                {
                    rawCode = false;
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
                    if (codeBlockAmount == 0)
                    {
                        isCodeBlock = false;
                    }
                }
                if (!userCodeStr.StartsWith("show("))
                {
                    if (userCodeStr.Contains("Accel.IMU.Pitch"))  // We need to validate the pitch.
                    {
                        string nonPitchInstruction = userCodeStr.Substring(0, userCodeStr.IndexOf("Wiimote"));
                        string pitchInstruction = userCodeStr.Substring(userCodeStr.IndexOf("Wiimote"));
                        RegexOptions options = RegexOptions.None;
                        Regex regex = new Regex("[ ]{2,}", options);
                        pitchInstruction = regex.Replace(pitchInstruction, " ");
                        char compareChar = ' ';
                        if (pitchInstruction.Contains(">")) compareChar = '>';
                        if (pitchInstruction.Contains("<")) compareChar = '<';
                        if (pitchInstruction.Contains("=")) compareChar = '=';
                        string[] pitchInstructionparts = pitchInstruction.Split(' ');
                        string newPitchInstruction = $"DataValidater.ValidatePitch({pitchInstructionparts[0]}, {pitchInstructionparts[0].Replace(".Pitch", ".Roll")}, {pitchInstructionparts[2].Replace(";", "")}, '{compareChar}')";
                        userCodeStr = $"{nonPitchInstruction}{newPitchInstruction}";
                    }
                }

                checkAndInitBool(userCodeStr, "XBox.", "initscp");
                checkAndInitBool(userCodeStr, "Wiimote.IR", "usingIR");
                checkAndInitBool(userCodeStr, "IRDifference", "usingIR");


                if (rawCode)
                {
                    userCode.AppendLine(userCodeLine);
                    continue;
                }

                if (userCodeStr.StartsWith("Mouse") || userCodeStr.StartsWith("AbsMouse"))
                {
                    //Mouse.X = Wiimote.Nunchuk.Joystick.X;
                    //Mouse = Wiimote.Nunchuck.Joystick;
                    string mouseMoveCommand = userCodeStr.StartsWith("AbsMouse") ? "MoveAbs" : "MoveRel";
                    string MouseInstruction = userCodeStr.Substring(0, userCodeStr.IndexOf("=")).Trim();
                    string WiimoteInstruction = userCodeStr.Substring(userCodeStr.IndexOf("=") + 1).Replace(";", "").Trim();
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
                    string varvalue = userCodeStr.Replace("var ", "").Split('=')[1].Trim().Replace(";", "");
                    try
                    {
                        variables.Add(varname, varvalue);
                    }
                    catch (ArgumentException ex)
                    {
                        return $"ERROR: Variable '{varname}' already exists";
                    }
                    continue;
                }
                else if (userCodeStr.StartsWith("show("))
                {
                    string varname = userCodeStr.Replace("show(", "").Replace(");", "").Trim();
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
                    string KeyboardInstruction = userCodeStr.Substring(0, userCodeStr.IndexOf('=')).Trim();
                    string keyInstruction = userCodeStr.Substring(userCodeStr.IndexOf('=') + 1).Replace(';', ' ').Trim();
                    if (userCodeStr.Contains("Keyboard.SinglePress"))
                    {
                        userCodeStr = $"Keyboard.SinglePress({KeyboardInstruction.Replace("Keyboard.SinglePress.", "KeyCode.")});";
                    }
                    else if (userCodeStr.Contains("Keyboard.Press"))
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

                if (userCodeStr.Contains("Wiimote.IR"))
                {
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
                    {
                        userCodeStr = $"if(Wiimote.IR.Midpoint.ValidPosition) {userCodeStr}";
                    }
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


            if (isCodeBlock)
            {
                appendUserCodeLine("}");
            }
            if (!variables.ContainsKey("mousespeed")) variables.Add("mousespeed", "1.0");
            StringBuilder variableString = new StringBuilder();

            foreach (KeyValuePair<String, String> varKV in variables)
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
            initString.AppendLine($"requiredExtension = ExtensionType.{requiredExtensionType.ToString()};");
            foreach (String initStr in initCode.Values)
            {
                initString.AppendLine(initStr);
            }
            String parsedcode = Resources.InputMapperTemplate;
            parsedcode = parsedcode.Replace("//USERCODE", userCode.ToString());
            parsedcode = parsedcode.Replace("//GLOBALS", variableString.ToString());
            parsedcode = parsedcode.Replace("//INIT", initString.ToString());
            parsedcode = parsedcode.Replace("SOURCECODE", code.Replace("\"", "\\\""));


            return parsedcode;
        }

        void appendUserCodeLine(String codeToAppend)
        {
            if (!rawCode) userCode.AppendLine(codeToAppend);
        }
        void appendUserCode(String codeToAppend)
        {
            if (!rawCode) userCode.Append(codeToAppend);
        }

        public string Compile(string code, bool run)
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
                File.WriteAllText("../../Scripting/ErrorScript.cs", code.Replace("InputMapperTemplate", "ErrorScript")); // Save code to build folder to be able to debug it
                StringBuilder text = new StringBuilder();
                text.AppendLine("Compile error: ");
                foreach (CompilerError ce in compile.Errors)
                {
                    text.AppendLine(ce.ToString());
                }
                return text.ToString();
            }
            else
            {
                File.WriteAllText("../../Scripting/PrevCompiledScript.cs", code.Replace("InputMapperTemplate", "PrevCompiledScript")); // Save code to build folder to be able to debug it
                if (!run) return "Compile success";
                Module[] modules = compile.CompiledAssembly.GetModules();
                Module module = modules[0];
                Type mt = null;
                ConstructorInfo constructorInfo = null;
                if (module != null)
                {
                    mt = module.GetType("WiiInputMapper.Scripting.InputMapperTemplate");
                }

                if (mt != null)
                {
                    inputMapper.scriptStopInfo = mt.GetMethod("StopScript");
                    constructorInfo = mt.GetConstructors()[0];
                }
                else
                {
                    throw new Exception("Uhm... There is something wrong with the template. Could not find the class");
                }

                if (constructorInfo != null)
                {
                    inputMapper.inputMapperScript = constructorInfo.Invoke(null);
                }
                else
                {
                    throw new Exception("Uhm... There is something wrong with the template. Could not find a constructor");
                }
                return "Running";
            }
        }

    }
}
