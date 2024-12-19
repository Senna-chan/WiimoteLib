using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiInputMapper.Template
{
    public class DataSmoother
    {
        private Dictionary<String, RunningAverage> dataSmoother = new Dictionary<string, RunningAverage>();
        public void AddData(String varname, Double value, int sampleSize = 10)
        {
            if (!dataSmoother.ContainsKey(varname))
            {
                dataSmoother.Add(varname, new RunningAverage(sampleSize));
            }
            else
            {
                dataSmoother[varname].SetSampleSize(sampleSize);
            }
            dataSmoother[varname].AddSample(value);
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
