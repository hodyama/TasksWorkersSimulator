﻿using System;
using MathNet.Numerics.Distributions;
using TaskSimulation.ChooseAlgorithms;
using TaskSimulation.Simulator.Events;
using TaskSimulation.Simulator.Workers;
using TaskSimulation.Utiles;


namespace TaskSimulation.Distribution
{
    public class SimDistribution
    {
        private static SimDistribution _instance;
        public static SimDistribution Instance { get { return _instance = _instance ?? new SimDistribution(); } }
        public static SimDistribution I => Instance;
        private SimDistribution() { }

        public int GlobalSeed { get; private set; }
        public Random GlobalRandom { get; private set; }

        public void Initialize(int globalSeed)
        {
            GlobalSeed = globalSeed;
            GlobalRandom = new Random(GlobalSeed);
        }

        public bool LoadData(int execution, InputXmlShema shema)
        {
            var execData = shema.Executions[execution];
            TaskArrivalRate = ReflectIContinuousDistribution.GetDistribution(execData.TaskArrivalRate.Type, execData.TaskArrivalRate.Params, GlobalRandom);
            WorkerArrivalRate = ReflectIContinuousDistribution.GetDistribution(execData.WorkerArrivalRate.Type, execData.WorkerArrivalRate.Params, GlobalRandom);
            WorkerLeaveRate = ReflectIContinuousDistribution.GetDistribution(execData.WorkerLeaveRate.Type, execData.WorkerLeaveRate.Params, GlobalRandom);

            var wqd = execData.WorkersQualityDistribution;
            WorkersQualityDistribution = new WorkersQualityDistribution()
            {

                Feedback = wqd.Feedback,

                Quality = wqd.Quality,


                ProcessingTime = wqd.ProcessingTime,

                numOfWorkers = execData.InitialNumOfWorkers,

                counter = 0
            };

            return WorkersQualityDistribution.Validate();
        }

        public IContinuousDistribution TaskArrivalRate { get; private set; }
        public IContinuousDistribution WorkerArrivalRate { get; private set; }
        public IContinuousDistribution WorkerLeaveRate { get; private set; }
        

        public IGradeCalcAlgo GradeSystem { get; set; }
        public IChooseWorkerAlgo GradeSystemChooseMethod { get; set; }


        public WorkersQualityDistribution WorkersQualityDistribution { get; private set; }

    }
}
