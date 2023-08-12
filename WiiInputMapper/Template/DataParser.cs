using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiInputMapper.Template
{
    public class DataParser
    {
        private Dictionary<String, MovingAverage> dataSmoother = new Dictionary<string, MovingAverage>();
        public void AddData(String varname, Double value)
        {
            if (!dataSmoother.ContainsKey(varname))
            {
                dataSmoother.Add(varname, new MovingAverage(10));
            }
            dataSmoother[varname].ComputeAverage(value);
        }
        public double GetData(String varname)
        {
            if (dataSmoother.ContainsKey(varname))
            {
                return dataSmoother[varname].Average;
            } 
            else
            {
                return 0.0;
            }
        }
    }
}
