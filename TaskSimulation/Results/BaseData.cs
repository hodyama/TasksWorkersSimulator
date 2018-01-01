﻿//bs"d
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TaskSimulation.Simulator;
using TaskSimulation.Simulator.Events;
using TaskSimulation.Simulator.Tasks;
using TaskSimulation.Simulator.Workers;
using System.IO;

namespace TaskSimulation.Results
{
    class BaseData : ISimulatable
    {
        private readonly Dictionary<Worker, Dictionary<double, int>> _workersQueue;
        private readonly Dictionary<double, Dictionary<Worker, double>> _workersGradesAtArrivalTask;
        private readonly Dictionary<Worker, List<Task>> _workersFinishedTasks;
        private readonly Dictionary<Worker, List<Task>> _workersTasks;
        private StreamWriter _sw;
        const int TASK_IN_PROCCESS = 1;
        public BaseData()
        {
            _workersQueue = new Dictionary<Worker , Dictionary<double, int>>();
            _workersGradesAtArrivalTask = new Dictionary<double, Dictionary<Worker, double>>();
            _workersFinishedTasks = new Dictionary<Worker, List<Task>>();
            _workersTasks = new Dictionary<Worker, List<Task>>();
        }

        public void Update(TaskArrivalEvent @event)
        {
            var time = @event.ArriveTime;
            var task = @event.Task;
            var existingWorker = task.GetWorker();
            
           /* if (existingWorker != null)
            {
               
                var queueLength = existingWorker.Grade.NumberOfTasksGrade;
              
                _workersQueue[existingWorker].Add(time,queueLength+1);
                
            }
            else
            {
                _workersGradesAtArrivalTask.Add(time, new Dictionary<Worker, double>());


                foreach (var keyValuePair in _workersQueue)
                   _workersGradesAtArrivalTask[time].Add(keyValuePair.Key, keyValuePair.Key.Grade.TotalGrade);
                 
            }*/
            
            task.OnAddedToWorker += w =>
            {
               
               var queueLength = w.Grade.NumberOfTasksGrade;
               _workersQueue[w].Add(time, queueLength+1);
                _workersTasks[w].Add(task);
                _workersGradesAtArrivalTask.Add(time, new Dictionary<Worker, double>());


                foreach (var keyValuePair in _workersQueue)
                    _workersGradesAtArrivalTask[time].Add(keyValuePair.Key, keyValuePair.Key.Grade.TotalGrade);

            };

            task.OnTaskAssigned += w =>
            {

                
            };
        }

        public void Update(TaskFinishedEvent @event)
        {
            var time = @event.ArriveTime;
            var task = @event.Task;
            var worker = @event.Worker;
            
            var queueLength = worker.Grade.NumberOfTasksGrade;

            _workersQueue[worker].Add(time, queueLength-1);
            _workersFinishedTasks[worker].Add(task);
           
        }

        public void Update(WorkerArrivalEvent @event)
        {
            _workersQueue.Add(@event.Worker, new Dictionary<double, int>());
            _workersFinishedTasks.Add(@event.Worker, new List<Task>());
            _workersTasks.Add(@event.Worker, new List<Task>());

        }

        public void Update(WorkerLeaveEvent @event)
        {
            
        }

       public void CreateBaseDataFile()
        {
           
            _sw = new StreamWriter($"baseData{DateTime.Now.ToFileTime()}.csv");
            _sw.WriteLine("WorkersGradesAtArrivalTask");
            _sw.WriteLine(GetWorkersGradesAtArrivalTask());
            _sw.WriteLine();
            _sw.WriteLine("WorkersQueueAtTime");
            _sw.WriteLine(GetWorkersQueueAtTime());
            _sw.WriteLine();
            _sw.WriteLine("WorkersBusyAtTime");
            _sw.WriteLine(GetWorkersBusyAtTime());
            _sw.WriteLine();
            _sw.WriteLine("TasksProcessingTime");
            _sw.WriteLine(GetTasksProcessingTime());
            _sw.WriteLine();
            _sw.WriteLine("TasksWaitingTime");
            _sw.WriteLine(GetTasksWaitingTime());
            _sw.WriteLine();
            _sw.WriteLine("AvgQueueLength");
            _sw.WriteLine(GetAvgQueueLength());
            _sw.WriteLine();
            _sw.WriteLine("SumOfWorkerFinishedTasks");
            _sw.WriteLine(GetSumOfWorkerFinishedTsks());
            _sw.WriteLine();
            _sw.WriteLine("TimeOfStartProcessingTaskForWorker");
            _sw.WriteLine(GetTimeOfStartProcessingTaskForWorker());
            _sw.WriteLine();

            _sw.Close();
            
        }
        private string GetWorkersGradesAtArrivalTask()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder title = new StringBuilder();
            title.Append($"{"time"},");
            string tmp = "";
            
            foreach (var keyValuePair in _workersGradesAtArrivalTask)
            {
                sb.Append($"{keyValuePair.Key},");
                tmp = "";
               
                foreach (var kv in keyValuePair.Value)
                {
                    tmp+=$"{kv.Key+"Score"},";
                    sb.Append($"{kv.Value},");
                }
                sb.AppendLine();
            }
            title.AppendLine(tmp);
            title.AppendLine(sb.ToString());
                    
           
            return title.ToString();
        }
        private string GetWorkersQueueAtTime()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var keyValuePair in _workersQueue)
            {
                var tmp = 0;
                sb.AppendLine($"{"time"},{keyValuePair.Key+" queueLength"}");
                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                    var queueLength = keyValuePair2.Value - TASK_IN_PROCCESS;
                    if (queueLength < 0)
                        queueLength = 0;
                    sb.AppendLine($"{keyValuePair2.Key},{tmp}");
                    sb.AppendLine($"{keyValuePair2.Key},{queueLength}");
                    tmp = queueLength;
                }
            }
            return sb.ToString();
        }
        private string GetAvgQueueLength()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var keyValuePair in _workersQueue)
            {
                sb.AppendLine($"{"worker id"},{"Avg Queue Length"}");
                
                sb.AppendLine($"{keyValuePair.Key.GetWorkerID()},{keyValuePair.Key.Grade.TotalGrade}");
                   
                
            }
            return sb.ToString();
        }
        private string GetWorkersBusyAtTime()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var keyValuePair in _workersQueue)
            {
                var tmp = 0;
                sb.AppendLine($"{"time"},{keyValuePair.Key + "Busy"}");
                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                    var busy= keyValuePair2.Value ;
                    if (busy == 0)
                        busy = 0;
                    else
                        busy = 1;
                    sb.AppendLine($"{keyValuePair2.Key},{tmp}");
                    sb.AppendLine($"{keyValuePair2.Key},{busy}");
                    tmp = busy;
                }
            }
            return sb.ToString();
        }
        private string GetTasksProcessingTime()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var keyValuePair in _workersFinishedTasks)
            {
                sb.AppendLine($"{keyValuePair.Key+"tasks"},{"task processing time"}");
                foreach (var item in keyValuePair.Value)
                
                    sb.AppendLine($"{item.GetTaskId()},{item.EndTime-item.StartTime}");
                
            }
            return sb.ToString();
        }
        
         private string GetTimeOfStartProcessingTaskForWorker()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var keyValuePair in _workersFinishedTasks)
            {
                sb.AppendLine($"{keyValuePair.Key + "tasks"},{"time of start processing task"}");
                foreach (var item in keyValuePair.Value)

                    sb.AppendLine($"{item.GetTaskId()},{ item.StartTime}");

            }
            return sb.ToString();
        }
        private string GetSumOfWorkerFinishedTsks()
        {
            StringBuilder sb = new StringBuilder();

            foreach (var keyValuePair in _workersTasks)
            {
                sb.AppendLine($"{keyValuePair.Key + "AllocatedTasks"},{"FinishedTasks"}");
                sb.AppendLine($"{keyValuePair.Value.Count},{_workersFinishedTasks[keyValuePair.Key].Count}");

               

            }
            return sb.ToString();
        }
        private string GetTasksWaitingTime()
        {
            StringBuilder sb = new StringBuilder();
            
            foreach (var keyValuePair in _workersFinishedTasks)
            {
                sb.AppendLine($"{keyValuePair.Key + "tasks"},{"task waiting time"}");
                foreach (var item in keyValuePair.Value)

                    sb.AppendLine($"{item.GetTaskId()},{item.StartTime-item.CreatedTime}");

            }
            return sb.ToString();
        }
      
    }
}
