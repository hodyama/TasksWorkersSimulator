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

            //TODO get type from input file
            Feedback        = new Normal(qualies.FeedbackMean, qualies.FeedbackStd, privateRandom);
            JobQuality      = new Normal(qualies.QualityMean,  qualies.QualityStd,  privateRandom);
            // ResponseTime    = new Normal(qualies.ResponseMean,  qualies.ResponseStd,  privateRandom);

            // ResponseTime = new Exponential(qualies.ResponseMean,  privateRandom);
            ResponseTime = new Exponential(0.416, privateRandom);

            // ResponseTime = new Exponential((id)*0.21,  privateRandom);
           /* if (id ==1)
                ResponseTime = new Exponential(0.21, privateRandom);
            if (id == 2)
                ResponseTime = new Exponential(2*0.21, privateRandom);
            if (id == 3)
                ResponseTime = new Exponential(4*0.21, privateRandom);
            /* else
             {
                 if (id < 68)
                     ResponseTime = new Exponential(0.0125, privateRandom);
                 else
                     ResponseTime = new Exponential(0.0125 * 2, privateRandom);

                 /*  if (id < 35)
                       ResponseTime = new Exponential(0.0125/2, privateRandom);
                   else
                   {
                       if (id < 68)
                           ResponseTime = new Exponential(0.0125, privateRandom);
                       else
                           ResponseTime = new Exponential(0.0125*2, privateRandom);
                   }*/



        }

        public WorkerDistribution(IContinuousDistribution feedback, IContinuousDistribution jobQuality, IContinuousDistribution responseTime)
        {
            Feedback = feedback;
            JobQuality = jobQuality;
            ResponseTime = responseTime;
           
        }

    }
}