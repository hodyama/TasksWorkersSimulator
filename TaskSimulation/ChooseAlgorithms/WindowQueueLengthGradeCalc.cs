//bs"d
using System;
using System.Collections.Generic;
using TaskSimulation.Distribution;
using TaskSimulation.Simulator.Tasks;
using TaskSimulation.Simulator.Workers;
using TaskSimulation.Utiles;

namespace TaskSimulation.ChooseAlgorithms
{
    public class WindowQueueLengthGradeCalc : IGradeCalcAlgo
    {
       const int TASKS_IN_PROSS = 1;
       private double window;
       public  WindowQueueLengthGradeCalc(double w)
        {
            window = w;
        }

        public Grade InitialGrade()
        {

            var grade = new Grade()
            {
                FeedbackGrade = 0,
                QualityGrade = 0,
                ResponseGrade = 0,
                
            };
            grade.Meta._workerQueue = new SortedDictionary<double, int>();
            return grade;
        }

        public Grade UpdateOnTaskAdd(Grade grade)
        {
            

            UpdateQueueGrade(ref grade);
            grade.Meta.NumberOfTasks++;
            grade.Meta._workerQueue.Add(Simulator.SimulateServer.SimulationClock, grade.Meta.NumberOfTasks);

           

            return grade;
        }
        public Grade UpdateOnTaskArrival(Grade grade )
        {
            UpdateQueueGrade(ref grade);
            return grade;

        }
        public Grade UpdateOnTaskRemoved(Worker worker, Task task)
        {
            var grade = worker.Grade;


            UpdateQueueGrade(ref grade);

            grade.Meta.NumberOfTasks--;
            grade.Meta._workerQueue.Add(Simulator.SimulateServer.SimulationClock, grade.Meta.NumberOfTasks);

            return grade;
        }

        public IChooseWorkerAlgo ChooseMethod()
        {
            return new ChooseLowestGrade();
        }

        
        private void UpdateQueueGrade(ref Grade grade)
        {
            
            var currentTime = Simulator.SimulateServer.SimulationClock;
            var newDeltaTime = currentTime - grade.Meta.LastModifiedAt;
            if (newDeltaTime <= 0)
                return;
            if (currentTime < window)

                grade.ResponseGrade = GetWorkerWindowTL( ref grade, 0) / currentTime;
            else
                grade.ResponseGrade = GetWorkerWindowTL(ref grade, currentTime - window) / window;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade

           
           
            

            grade.Meta.LastModifiedAt = Simulator.SimulateServer.SimulationClock;

           grade.Meta.WorkingTime += newDeltaTime;


        }
        public double GetWorkerWindowTL(ref Grade grade, double start_time)
        {

            var q = 0;
            var t0 = start_time;
            var t1 = start_time;
            var tl = 0.0;
            List<double> tmp = new List<double>();
            

            foreach (var keyValuePair2 in grade.Meta._workerQueue)
            {
                t1 = keyValuePair2.Key;
                if (t1 <= t0)

                {
                    q = keyValuePair2.Value;
                   
                    tmp.Add(t1);


                }


                else
                {
                    
                    var tmpq = keyValuePair2.Value;
                   
                    if (tmpq != q)
                    {


                        tl += q * (t1 - t0);
                        t0 = keyValuePair2.Key;

                        q = tmpq;
                    }
                }


            }
           
            for (var i=0; i<tmp.Count-1;i++)
                grade.Meta._workerQueue.Remove(tmp[i]);

           
            if (t1 < start_time)
                t1 = start_time;

            return tl + q * (Simulator.SimulateServer.SimulationClock - t1);
        }


    }
}




