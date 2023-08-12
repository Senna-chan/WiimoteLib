using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiInputMapper.Template
{
    public class MovingAverage
    {
        private Queue<Double> samples = new Queue<Double>();
        private int windowSize = 10;
        private Double sampleAccumulator;
        public Double Average { get; private set; }

        public MovingAverage()
        {
        }

        public MovingAverage(int queueSize)
        {
            windowSize = queueSize;
        }

        /// <summary>
        /// Computes a new windowed average each time a new sample arrives
        /// </summary>
        /// <param name="newSample"></param>
        public void ComputeAverage(Double newSample)
        {
            sampleAccumulator += newSample;
            samples.Enqueue(newSample);

            if (samples.Count > windowSize)
            {
                sampleAccumulator -= samples.Dequeue();
            }

            Average = sampleAccumulator / samples.Count;
        }
    }
}
