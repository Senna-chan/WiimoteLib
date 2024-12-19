using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiiInputMapper.Template
{
    public partial class VarShower : Form
    {
        Dictionary<string, ucVarShow> variables = new Dictionary<string, ucVarShow>();
        public VarShower()
        {
            InitializeComponent();
        }

        public void ShowVar(string name, object value)
        {
            try
            {
                ucVarShow varShow;
                if (!variables.TryGetValue(name, out varShow))
                {
                    varShow = new ucVarShow(name);
                    variables.Add(name, varShow);
                    if (pnlVarShower.InvokeRequired)
                    {
                        pnlVarShower.Invoke((Action)(() =>
                        {
                            pnlVarShower.Controls.Add(varShow);
                        }));
                    }
                    else {
                        pnlVarShower.Controls.Add(varShow);
                    }
                }
                varShow.setValue(value);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
