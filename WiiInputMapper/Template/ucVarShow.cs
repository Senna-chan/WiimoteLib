using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WiiInputMapper.Template
{
    public partial class ucVarShow : UserControl
    {
        public ucVarShow(string name)
        {
            InitializeComponent();
            lblVarName.Text = name;
        }

        internal void setValue(object value)
        {
            if (lblVarName.InvokeRequired)
            {
                lblVarValue.BeginInvoke((Action)(() =>
                {
                    lblVarValue.Text = value.ToString();
                }));
            }
            else
            {
                lblVarValue.Text = value.ToString();
            }
        }
    }
}
