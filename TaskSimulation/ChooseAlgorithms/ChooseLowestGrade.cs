using System;
using System.Collections.Generic;
using System.Linq;
using TaskSimulation.Distribution;
using TaskSimulation.Simulator;
using TaskSimulation.Simulator.Workers;

namespace TaskSimulation.ChooseAlgorithms
{
    public class ChooseLowestGrade : IChooseWorkerAlgo
    {
        public List<Worker> ChooseWorkers(List<Worker> activeWorkers, int chooseNum)
        {
            var r = new Random(SimDistribution.I.GlobalSeed);
            var workers = activeWorkers
                .OrderBy(w => w.Grade.TotalGrade)
                .ThenBy(w => w.Grade.NumberOfTasksGrade)
                .ThenBy(w => r.Next())
                .Take(chooseNum).ToList();

            return workers;
        }
    }
}
