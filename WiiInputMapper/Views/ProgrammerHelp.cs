using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiiInputMapper.Views
{
    public partial class ProgrammerHelp : Form
    {
        public ProgrammerHelp()
        {
            InitializeComponent();
            var assembly = Assembly.GetExecutingAssembly();
            var manifestResources = assembly.GetManifestResourceNames();
            string resourceName = manifestResources.Single(str => str.EndsWith("programming.md"));
            string result = "";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }
            var html = Markdig.Markdown.ToHtml(result);
            webHelp.DocumentText = html;
        }
    }
}
