using MathNet.Numerics.Distributions;
using System.Collections.Generic;
using static TaskSimulation.Utiles.InputXmlShema;

namespace TaskSimulation.Simulator.Workers
{
    public class WorkersQualityDistribution
    {
       

        public ExecutionWorkersQualityDistributionFeedback Feedback { get; set; } 
                      
        public ExecutionWorkersQualityDistributionQuality Quality { get; set; }

        public ExecutionWorkersQualityDistributionResponseTime ProcessingTime { get; set; }
        
        public long numOfWorkers { get; set; }

        public long counter { get; set; }




        public bool Validate()
        {
            
            return Feedback != null &&
                   Quality != null &&
                   ProcessingTime != null ;
        }

        public WorkerQualies GenerateQualies()
        {
            var ratio = 1.0;
            long groupSize = numOfWorkers / ProcessingTime.Ratio.Length;
            if (groupSize == 0)
                groupSize = 1;

            var index = (int)(counter / groupSize);
            if (index >= ProcessingTime.Ratio.Length)
                index = (int)(counter % groupSize);
            ratio = ProcessingTime.Ratio[index];
            counter++;

            List<double> tmp = new List<double>();

            foreach (var p in ProcessingTime.Params)
            {
                tmp.Add(p * ratio);
                Log.I("////////////           " + ratio + "               " + p, System.ConsoleColor.Magenta);
            }

              
            return new WorkerQualies(
                Feedback.Type,Feedback.Params, 
                Quality.Type, Quality.Params,
                ProcessingTime.Type, tmp.ToArray());
        }
    }
}