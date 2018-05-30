//bs"d
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
        private const double WARM_UP_TIME = 0;
        private const int BIN_LENGTH = 1;
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
            //move from here
           _sw= new StreamWriter($"ArrivalRates{DateTime.Now.ToFileTime()}.csv");
            _sw.WriteLine("Average arrivel rate to worker");
            _sw.WriteLine(GetTasksArrivalRateByBins());
            _sw.WriteLine();
            _sw.Close();

            _sw = new StreamWriter($"AvgTasksWaitingTime{DateTime.Now.ToFileTime()}.csv");
            _sw.WriteLine("Avg Tasks Waiting Time");
            _sw.WriteLine(GetAvgTasksWaitingTime());
            _sw.WriteLine();
            _sw.Close();


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
            _sw.WriteLine("MaenOfTasksArrivalTimesPerWorker");
            _sw.WriteLine(GetMeanRateOfTasksArrival());
            _sw.WriteLine("Workers Utilization");
            _sw.WriteLine(GetUtilization());
            _sw.WriteLine("Avg Tasks Waiting Time");
            _sw.WriteLine(GetAvgTasksWaitingTime());
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
                if (keyValuePair.Key > WARM_UP_TIME)
                {
                    sb.Append($"{keyValuePair.Key},");
                    tmp = "";

                    foreach (var kv in keyValuePair.Value)
                    {
                        tmp += $"{kv.Key + "Score"},";
                        sb.Append($"{kv.Value},");
                    }
                    sb.AppendLine();
                }
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
                    if (keyValuePair2.Key > WARM_UP_TIME)
                    {
                        var queueLength = keyValuePair2.Value - TASK_IN_PROCCESS;
                        if (queueLength < 0)
                            queueLength = 0;
                        sb.AppendLine($"{keyValuePair2.Key},{tmp}");
                        sb.AppendLine($"{keyValuePair2.Key},{queueLength}");
                        tmp = queueLength;
                    }
                }
            }
            return sb.ToString();
        }
        private string GetAvgQueueLength()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{"worker id"},{"Avg Queue Length"}");
            
            foreach (var keyValuePair in _workersQueue)
            {
                var q = 0;
                var t0 = WARM_UP_TIME;
                var t1 = WARM_UP_TIME;
                var tl = 0.0;
                
                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                    t1 = keyValuePair2.Key;
                    if (t1 > t0)
                    {
                        var tmpq = keyValuePair2.Value - TASK_IN_PROCCESS;
                        if (tmpq < 0)
                            tmpq = 0;
                        if (tmpq != q)
                        {


                            tl += q * (t1 - t0);
                            t0 = keyValuePair2.Key;

                            q = tmpq;
                        }
                    }


                }
                sb.AppendLine($"{keyValuePair.Key},{tl/( Simulator.SimulateServer.SimulationClock-WARM_UP_TIME)}");

            }

            return sb.ToString();
        }
        public double GetWorkerWindowTL( Worker w, double start_time)
        {
           
                var q = 0;
                var t0 = start_time;
                var t1 = start_time;
                var tl = 0.0;

                foreach (var keyValuePair2 in _workersQueue[w])
                {
                    t1 = keyValuePair2.Key;
                    if (t1 > t0)
                    {
                         var tmpq = keyValuePair2.Value - TASK_IN_PROCCESS;
                         if (tmpq < 0)
                             tmpq = 0;
                        if (tmpq != q)
                        {


                            tl += q * (t1 - t0);
                            t0 = keyValuePair2.Key;

                            q = tmpq;
                        }
                     }


                }
               

             return tl;
        }

        public string GetUtilization()
        {
            StringBuilder sb = new StringBuilder();
            //sb.AppendLine($"{"worker id"},{"worker utilization"}");

            foreach (var keyValuePair in _workersQueue)
            {
                var q = 0;
                var t0 = WARM_UP_TIME;
                var t1 = WARM_UP_TIME;
                var tl = 0.0;

                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                    t1 = keyValuePair2.Key;
                    if (t1 > t0)
                    {
                        var tmpq = keyValuePair2.Value;
                        if (tmpq != 0)
                            tmpq = 1;
                        if (tmpq != q)
                        {


                            tl += q * (t1 - t0);
                            t0 = keyValuePair2.Key;

                            q = tmpq;
                        }

                    }
                }
                sb.AppendLine($"{keyValuePair.Key},{tl / (Simulator.SimulateServer.SimulationClock-WARM_UP_TIME)}");

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
                    if (keyValuePair2.Key > WARM_UP_TIME)
                    {
                        var busy = keyValuePair2.Value;
                        if (busy != 0)

                            busy = 1;
                        sb.AppendLine($"{keyValuePair2.Key},{tmp}");
                        sb.AppendLine($"{keyValuePair2.Key},{busy}");
                        tmp = busy;
                    }
                }
            }
            return sb.ToString();
        }
        private string GetTasksProcessingTime()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();
            sbt.AppendLine($"{"worker id"},{"avg tasks processing time"}");
            
            foreach (var keyValuePair in _workersFinishedTasks)
            {
                var avgP = 0.0;
                var counter = 0;
                sb.AppendLine($"{keyValuePair.Key+"tasks"},{"task processing time"}");
                foreach (var item in keyValuePair.Value)
                {
                    if (item.CreatedTime > WARM_UP_TIME)
                    {
                        counter++;
                        avgP += item.EndTime - item.StartTime;
                        sb.AppendLine($"{item.GetTaskId()},{item.EndTime - item.StartTime}");
                    }
                }
                sbt.AppendLine($"{keyValuePair.Key.GetWorkerID()},{avgP/counter}");


            }
            sb.AppendLine(sbt.ToString());
            return sb.ToString();
        }
        
         private string GetTimeOfStartProcessingTaskForWorker()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var keyValuePair in _workersFinishedTasks)
            {
                sb.AppendLine($"{keyValuePair.Key + "tasks"},{"time of start processing task"}");
                foreach (var item in keyValuePair.Value)
                    if(item.CreatedTime>WARM_UP_TIME)
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
                var alloc= 0;
                var finish = 0;
                foreach( var item in keyValuePair.Value)
                {
                    if (item.CreatedTime > WARM_UP_TIME)
                        alloc++;
                }
                foreach (var item in _workersFinishedTasks[keyValuePair.Key])
                {
                    if (item.CreatedTime > WARM_UP_TIME)
                        finish++;
                }

                sb.AppendLine($"{alloc},{finish}");

               

            }
            return sb.ToString();
        }
        private string GetMeanRateOfTasksArrival()
        {

            StringBuilder sb = new StringBuilder();
            foreach (var keyValuePair in _workersTasks)
            {
                var sum = 0.0;
                var counter = 0;
                sb.AppendLine($"{keyValuePair.Key },{"MeanRateOfTasksArrival"}");
                foreach (var item in keyValuePair.Value)
                {
                    if (item.CreatedTime > WARM_UP_TIME)
                    {
                        sum += item.CreatedTime;
                        counter++;
                    }
                }
                sb.AppendLine($"{sum/counter}");
                

            }
            return sb.ToString();
        }
        private string GetTasksWaitingTime()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbt = new StringBuilder();
            sbt.AppendLine($"{"worker id"},{"avg tasks waiting time"}");

            foreach (var keyValuePair in _workersFinishedTasks)
            {
                var avgW = 0.0;
                var counter = 0;
                sb.AppendLine($"{keyValuePair.Key + "tasks"},{"task waiting time"}");
                foreach (var item in keyValuePair.Value)
                {
                    if (item.CreatedTime > WARM_UP_TIME)
                    {
                        avgW += item.StartTime - item.CreatedTime;
                        sb.AppendLine($"{item.GetTaskId()},{item.StartTime - item.CreatedTime}");
                        counter++;
                    }
                }
                sbt.AppendLine($"{keyValuePair.Key.GetWorkerID()},{avgW / counter}");

            }
            sb.AppendLine(sbt.ToString());
            return sb.ToString();
        }
        private string GetTasksArrivalRateByBins()
        {
            StringBuilder sbt = new StringBuilder();
            Dictionary<Worker, List<int>> bins = new Dictionary<Worker, List<int>>();

            
            foreach (var w in _workersTasks)
            {
                bins.Add(w.Key, new List<int>());
                for (var i = 0; i < (Simulator.SimulateServer.SimulationClock - WARM_UP_TIME) / BIN_LENGTH; i++)
                    bins[w.Key].Add(0);
                
                
              foreach (var t in w.Value)
                if(t.CreatedTime>WARM_UP_TIME)
                   bins[w.Key][(int)(t.CreatedTime-WARM_UP_TIME)]++;///if bins !=1?
                    
            }
            sbt.Append($"{""},");
            foreach (var w in bins)
                sbt.Append($"{w.Key.ToString()},");
            sbt.AppendLine();
            for (var i = 0; i < (Simulator.SimulateServer.SimulationClock - WARM_UP_TIME) / BIN_LENGTH; i++)
            {
                sbt.Append($"{i+1},");
                foreach (var w in bins)
                    sbt.Append($"{bins[w.Key][i]},");
                sbt.AppendLine();

            }
            
               
                return sbt.ToString();

        }
        private string GetAvgTasksWaitingTime()
        {
            StringBuilder sb = new StringBuilder();

            var avgW = 0.0;
            int counter = 0;

            foreach (var keyValuePair in _workersFinishedTasks)
            {

                foreach (var item in keyValuePair.Value)
                {
                    if (item.CreatedTime > WARM_UP_TIME)
                    {
                        avgW += item.StartTime - item.CreatedTime;
                        counter++;
                    }
                }




            }
            sb.AppendLine($"{avgW / counter}");
            return sb.ToString();
        }


    }
}

