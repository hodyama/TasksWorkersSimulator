﻿using System;
using TaskSimulation.Results;
using TaskSimulation.Simulator.Events;
using TaskSimulation.Simulator.Tasks;
using TaskSimulation.Simulator.Workers;

namespace TaskSimulation.Simulator
{
    public class SimulateServer
    {
        public static double SimulationClock { get; private set; } // TNOW

        private TasksJournal _tasksJournal;
        private WorkersJournal _workersJournal;
        public static double SimulatorMaxRunTime { get; private set; }
        private readonly SimulationEventMan _simulationEvents;
        public Utilization Utilization { get; private set; }

        public SimulateServer(double maxSimulationTime = Int32.MaxValue)
        {
            _simulationEvents = new SimulationEventMan();
            SimulationClock = 0;
            SimulatorMaxRunTime = maxSimulationTime;
            
            Utilization = new Utilization();
        }

        public void Initialize(long initialNumOfWorkers)
        {
            Log.D("* * * * * * * Init * * * * * * *");
            Task.TASK_ID = 0;

            _simulationEvents.InitializeGenesisEvents();

            _tasksJournal = new TasksJournal();

            _workersJournal = new WorkersJournal();
            //Utilization.AddWorkers(_workersJournal.ActiveWorkers);

            Log.D("* * * * * * * Init * * * * * * *");
        }

        public void Start()
        {
            var nextEvent = _simulationEvents.GetNextEvent();

            while (nextEvent.ArriveTime < SimulatorMaxRunTime)
            {
                // Update the simulation clock to the new event time
                if (nextEvent.ArriveTime < 0)
                    Log.Err("Time is < 0");

                SimulationClock = nextEvent.ArriveTime;
                Log.I();
                Log.Event( $"{nextEvent} at time {SimulationClock,-5:##.##}");

                if (nextEvent is TaskArrivalEvent || nextEvent is TaskFinishedEvent)
                {
                    nextEvent.Accept(_tasksJournal);
                    nextEvent.Accept(_workersJournal);
                }
                else
                {
                    nextEvent.Accept(_workersJournal);
                    nextEvent.Accept(_tasksJournal);
                }

                nextEvent.Accept(Utilization);

                #if DEBUG
                PrintSimulationState();
                #endif

                nextEvent = _simulationEvents.GetNextEvent();
            }

            PrintSimulationState();
        }

        public void PrintSimulationState()
        {
            Log.I($"* * * * * * * {SimulationClock:###0.##}/{SimulatorMaxRunTime:####0.##} * * * * * * *", ConsoleColor.Yellow);

            _workersJournal.ActiveWorkers.ForEach(w =>
            {
                Log.I($"{w} {w.Grade}");
            });
        }
 
    }
}

