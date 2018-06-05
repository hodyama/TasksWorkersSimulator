using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaskSimulation.Simulator.Tasks;
using TaskSimulation.Simulator.Workers;

namespace TaskSimulation.ChooseAlgorithms
{
    public class NumberOfTasksInQueueGradeCalc : IGradeCalcAlgo
    {
        const int TASKS_IN_PROSS = 1;

        public Grade InitialGrade()
        {

            var grade = new Grade()
            {
                FeedbackGrade = 0,
                QualityGrade = 0,
                ResponseGrade = 0,
                
            };

            return grade;
        }

        public Grade UpdateOnTaskAdd(Grade grade)
        {
            
            grade.Meta.NumberOfTasks++;
            UpdateQueueGrade(ref grade);

            return grade;
        }
        public Grade UpdateOnTaskArrival(Grade grade)
        {
            
            return grade;

        }
        public Grade UpdateOnTaskRemoved(Worker worker, Task task)
        {
            var grade = worker.Grade;

            grade.Meta.NumberOfTasks--;
            UpdateQueueGrade(ref grade);

            return grade;
        }

        public IChooseWorkerAlgo ChooseMethod()
        {
            return new ChooseLowestGrade();
        }

        
        private void UpdateQueueGrade(ref Grade grade)
        {
        
            grade.ResponseGrade = grade.Meta.NumberOfTasks;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade


            var currentTime = Simulator.SimulateServer.SimulationClock;
           

            var newDeltaTime = currentTime - grade.Meta.LastModifiedAt;
            if (newDeltaTime <= 0)
                return;
          

            grade.Meta.LastModifiedAt = Simulator.SimulateServer.SimulationClock;


            grade.Meta.WorkingTime += newDeltaTime;
        }
    }
}

