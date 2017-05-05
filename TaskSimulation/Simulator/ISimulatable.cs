﻿using TaskSimulation.Simulator;
using TaskSimulation.Simulator.Events;

namespace TaskSimulation
{
    public interface ISimulatable
    {
        /// <summary>
        /// Update on Simulation step
        /// </summary>
        //void Update();

        void Update(TaskArrivalEvent @event);
        void Update(TaskFinishedEvent @event);
        void Update(WorkerArrivalEvent @event);
        void Update(WorkerLeaveEvent @event);

    }

}