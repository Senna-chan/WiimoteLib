using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiInputMapper.Template
{
    public class RunningAverage
    {
        private Queue<Double> samples = new Queue<Double>();
        private int sampleSize = 10;
        private Double sampleAccumulator;
        public Double Average { get; private set; }

        public RunningAverage()
        {
        }

        public RunningAverage(int queueSize)
        {
            sampleSize = queueSize;
        }

        public void SetSampleSize(int sampleSize)
        {
            this.sampleSize = sampleSize;
        }

        /// <summary>
        /// Computes a new windowed average each time a new sample arrives
        /// </summary>
        /// <param name="newSample"></param>
        public void AddSample(Double newSample)
        {
            sampleAccumulator += newSample;
            samples.Enqueue(newSample);

            if (samples.Count > sampleSize)
            {
                sampleAccumulator -= samples.Dequeue();
            }

            Average = sampleAccumulator / samples.Count;
        }
    }
}
