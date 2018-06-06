using System;
using MathNet.Numerics.Distributions;
using TaskSimulation.Distribution;

namespace TaskSimulation.Simulator.Workers
{
    public class WorkerDistribution
    {
        public IContinuousDistribution Feedback { get; }

        public IContinuousDistribution JobQuality { get; }

        public IContinuousDistribution ResponseTime { get; }
        

        public WorkerDistribution(WorkerQualies qualies, long id )
        {
            // Generate private random for worker
            var privateRandom = new Random(SimDistribution.I.GlobalRandom.Next());

            
            Feedback = ReflectIContinuousDistribution.GetDistribution(qualies.FeedbackType, qualies.FeedbackParams, privateRandom);
            JobQuality      = ReflectIContinuousDistribution.GetDistribution(qualies.QualityType, qualies.QualityParams,  privateRandom);
            ResponseTime    = ReflectIContinuousDistribution.GetDistribution(qualies.ResponseType,  qualies.ResponseParams,  privateRandom);


        }

        public WorkerDistribution(IContinuousDistribution feedback, IContinuousDistribution jobQuality, IContinuousDistribution responseTime)
        {
            Feedback = feedback;
            JobQuality = jobQuality;
            ResponseTime = responseTime;
           
        }

    }
}