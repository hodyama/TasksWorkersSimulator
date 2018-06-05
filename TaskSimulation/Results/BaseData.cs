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
        private const int BIN_LENGTH = 1;
        private readonly Dictionary<Worker, Dictionary<double, int>> _workersQueue;
        private readonly Dictionary<double, Dictionary<Worker, double>> _workersGradesAtArrivalTask;
        private readonly Dictionary<Worker, List<Task>> _workersFinishedTasks;
        private readonly Dictionary<Worker, List<Task>> _workersTasks;
        private StreamWriter _sw;
        
        private  double WARM_UP_TIME ;
        
        public BaseData()
        {
            _workersQueue = new Dictionary<Worker , Dictionary<double, int>>();
            _workersGradesAtArrivalTask = new Dictionary<double, Dictionary<Worker, double>>();
            _workersFinishedTasks = new Dictionary<Worker, List<Task>>();
            _workersTasks = new Dictionary<Worker, List<Task>>();
            WARM_UP_TIME = Simulator.SimulateServer.WARM_UP_TIME;
        }

        public void Update(TaskArrivalEvent @event)
        {
            var time = @event.ArriveTime;
            var task = @event.Task;
            var existingWorker = task.GetWorker();
          
            task.OnAddedToWorker += w =>
            {

                var queueLength = w.Grade.Meta.NumberOfTasks;
                _workersQueue[w].Add(time, queueLength + 1);
                _workersTasks[w].Add(task);
                _workersGradesAtArrivalTask.Add(time, new Dictionary<Worker, double>());


                foreach (var keyValuePair in _workersQueue)
                {
                    _workersGradesAtArrivalTask[time].Add(keyValuePair.Key, keyValuePair.Key.Grade.TotalGrade);

                }
            };

           
        }

        public void Update(TaskFinishedEvent @event)
        {
            var time = @event.ArriveTime;
            var task = @event.Task;
            var worker = @event.Worker;
            
            var queueLength = worker.Grade.Meta.NumberOfTasks;

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
            String dir =  "Simulation_"+DateTime.Now.ToFileTime().ToString() ;
            System.IO.Directory.CreateDirectory(dir);
            _sw = new StreamWriter($"{dir}/WorkersGradesAtArrivalTask.csv");
            _sw.WriteLine(GetWorkersGradesAtArrivalTask());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/WorkersQueueAtTime.csv");
            _sw.WriteLine(GetWorkersQueueAtTime());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/AvgQueueLength.csv");
            _sw.WriteLine(GetAvgQueueLength());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/WorkersUtilization.csv");
            _sw.WriteLine(GetUtilization());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/WorkersBusyAtTime.csv");
            _sw.WriteLine(GetWorkersBusyAtTime());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/AvgTasksProcessingTime.csv");
            _sw.WriteLine(GetAvgTasksProcessingTime());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/TasksProcessingTime.csv");
            _sw.WriteLine(GetTasksProcessingTime());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/AvgTasksWaitingTimePerWorker.csv");
            _sw.WriteLine(GetAvgTasksWaitingTimePerWorker());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/TasksWaitingTime.csv");
            _sw.WriteLine(GetTasksWaitingTime());
            _sw.Close();
            _sw = new StreamWriter($"{dir}/SumOfWorkerFinishedTsks.csv");
            _sw.WriteLine(GetSumOfWorkerFinishedTsks());
            _sw.Close();

            _sw = new StreamWriter($"{dir}/ArrivalRates{DateTime.Now.ToFileTime()}.csv");
            _sw.WriteLine("Average arrivel rate to worker");
            _sw.WriteLine(GetTasksArrivalRateByBins());
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
                        tmp += $"{kv.Key + "Score"},";
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
            var tmptt = new List<string>();
            var len = 0;
            foreach (var wDic in _workersQueue.Values)
            {
                if (wDic.Count() > len)
                    len = wDic.Count();
            }

            for (var i = 0; i < len * 3; i++)
                tmptt.Add(".");
            sb.AppendLine(string.Join(",",tmptt));

            foreach (var keyValuePair in _workersQueue)
            {
                var time = new List<double>();
                var wq = new List<double>();
                var tmp = 0;
                

                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                   
                        var queueLength = keyValuePair2.Value;
                       
                        time.Add(keyValuePair2.Key);
                        wq.Add(tmp);
                        time.Add(keyValuePair2.Key);
                        wq.Add(queueLength);
                        
                        tmp = queueLength;
                    
                }
                sb.AppendLine("time," + string.Join(",", time));
                sb.AppendLine(keyValuePair.Key + "queueLength," + string.Join(",", wq));
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
                   
                        var tmpq = keyValuePair2.Value;

                        
                        if (tmpq != q)
                        {


                            tl += q * (t1 - t0);
                            t0 = keyValuePair2.Key;

                            q = tmpq;
                        }
                   
                }
                sb.AppendLine($"{keyValuePair.Key.GetWorkerID()},{tl/( Simulator.SimulateServer.SimulationClock-WARM_UP_TIME)}");

            }

            return sb.ToString();
        }
       

        public string GetUtilization()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{"worker id"},{"worker utilization"}");

            foreach (var keyValuePair in _workersQueue)
            {
                var q = 0;
                var t0 = WARM_UP_TIME;
                var t1 = WARM_UP_TIME;
                var tl = 0.0;

                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                    t1 = keyValuePair2.Key;
                   
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
                sb.AppendLine($"{keyValuePair.Key.GetWorkerID()},{tl / (Simulator.SimulateServer.SimulationClock-WARM_UP_TIME)}");

            }

            return sb.ToString();
        }
       
        private string GetWorkersBusyAtTime()
        {
            StringBuilder sb = new StringBuilder();
            var tmptt = new List<string>();
            var len = 0;
            foreach (var wDic in _workersQueue.Values)
            {
                if (wDic.Count() > len)
                    len = wDic.Count();
            }

            for (var i = 0; i < len * 3; i++)
                tmptt.Add(".");
            sb.AppendLine(string.Join(",", tmptt));

            foreach (var keyValuePair in _workersQueue)
            {
                var time = new List<double>();
                var wb = new List<double>();
                var tmp = 0;
                
                foreach (var keyValuePair2 in keyValuePair.Value)
                {
                   
                        var busy = keyValuePair2.Value;
                        if (busy != 0)

                            busy = 1;
                        time.Add(keyValuePair2.Key);
                        wb.Add(tmp);
                        time.Add(keyValuePair2.Key);
                        wb.Add(busy);
                        
                        tmp = busy;
                    
                }
                sb.AppendLine("time," + string.Join(",", time));
                sb.AppendLine(keyValuePair.Key + "busy," + string.Join(",", wb));
            }
            return sb.ToString();
        }
        private string GetTasksProcessingTime()
        {
            
            StringBuilder sb = new StringBuilder();
            var tmptt = new List<string>();
            var len = 0;
            foreach (var wDic in _workersFinishedTasks.Values)
            {
                if (wDic.Count() > len)
                    len = wDic.Count();
            }

            for (var i = 0; i < len * 3; i++)
                tmptt.Add(".");
            sb.AppendLine(string.Join(",", tmptt));

            foreach (var keyValuePair in _workersFinishedTasks)
            {
                var tasks = new List<double>();
                var pr = new List<double>();
                
                foreach (var item in keyValuePair.Value)
                {
                   
                        tasks.Add(item.GetTaskId());
                        pr.Add(item.EndTime - item.StartTime);

                   
                }

                sb.AppendLine("task," + string.Join(",", tasks));
                sb.AppendLine(keyValuePair.Key + "tasks proccessing time," + string.Join(",", pr));

            }
            
            return sb.ToString();
        }
        private string GetAvgTasksProcessingTime()
        {
           
            StringBuilder sbt = new StringBuilder();
            sbt.AppendLine($"{"worker id"},{"avg tasks processing time"}");

            foreach (var keyValuePair in _workersFinishedTasks)
            {
                var avgP = 0.0;
                
              
                foreach (var item in keyValuePair.Value)
                
                        avgP += item.EndTime - item.StartTime;
                 
                sbt.AppendLine($"{keyValuePair.Key.GetWorkerID()},{avgP / keyValuePair.Value.Count}");


            }
           
            return sbt.ToString();
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
            sb.AppendLine($"{"worker id"},{"Allocated Tasks"},{"Finished Tasks"}");

            foreach (var keyValuePair in _workersTasks)
            

                sb.AppendLine($"{keyValuePair.Key.GetWorkerID()},{keyValuePair.Value.Count},{_workersFinishedTasks[keyValuePair.Key].Count}");

             
            return sb.ToString();
        }
        private string GetMeanRateOfTasksArrival()
        {

            StringBuilder sb = new StringBuilder();
            foreach (var keyValuePair in _workersTasks)
            {
                var sum = 0.0;
                
                sb.AppendLine($"{keyValuePair.Key },{"MeanRateOfTasksArrival"}");
                foreach (var item in keyValuePair.Value)
                 
                        sum += item.CreatedTime;
               
                sb.AppendLine($"{sum/ keyValuePair.Value.Count}");
                

            }
            return sb.ToString();
        }
        private string GetTasksWaitingTime()
        {
            
            StringBuilder sb = new StringBuilder();
            var tmptt = new List<string>();
            var len = 0;
            foreach (var wDic in _workersFinishedTasks.Values)
            {
                if (wDic.Count() > len)
                    len = wDic.Count();
            }

            for (var i = 0; i < len * 3; i++)
                tmptt.Add(".");
            sb.AppendLine(string.Join(",", tmptt));

            foreach (var keyValuePair in _workersFinishedTasks)
            {
                var tasks = new List<double>();
                var wt = new List<double>();
                
                foreach (var item in keyValuePair.Value)
                {
                   
                        tasks.Add(item.GetTaskId());
                        wt.Add(item.StartTime - item.CreatedTime);

                   
                }

                sb.AppendLine("task," + string.Join(",", tasks));
                sb.AppendLine(keyValuePair.Key + "tasks waiting time," + string.Join(",", wt));


            
            }
                
            return sb.ToString();
        }
        private string GetAvgTasksWaitingTimePerWorker()
        {
            
            StringBuilder sbt = new StringBuilder();
            sbt.AppendLine($"{"worker id"},{"avg tasks waiting time"}");

            foreach (var keyValuePair in _workersFinishedTasks)
            {
                var avgW = 0.0;
                
               
                foreach (var item in keyValuePair.Value)
                     avgW += item.StartTime - item.CreatedTime;
                    
               
                sbt.AppendLine($"{keyValuePair.Key.GetWorkerID()},{avgW / keyValuePair.Value.Count}");

            }
            
            return sbt.ToString();
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
                        avgW += item.StartTime - item.CreatedTime;
                        counter++;
                    
                }




            }
            sb.AppendLine($"{avgW / counter}");
            return sb.ToString();
        }


    }
}

