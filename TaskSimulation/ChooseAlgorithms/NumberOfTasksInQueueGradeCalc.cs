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
                NumberOfTasksGrade = 0,
            };

            return grade;
        }

        public Grade UpdateOnTaskAdd(Grade grade)
        {
            // TODO add to metadata as NumberOfTasks
            grade.NumberOfTasksGrade++;
            UpdateQueueGrade(ref grade);

            return grade;
        }
        public Grade UpdateOnTaskArrival(Grade grade)
        {
            
            return grade;

        }
        public Grade UpdateOnTaskRemoved(Worker worker, Task task) //WORKER TASK
        {
            var grade = worker.Grade;
            grade.NumberOfTasksGrade--;
            UpdateQueueGrade(ref grade);

            return grade;
        }

        public IChooseWorkerAlgo ChooseMethod()
        {
            return new ChooseLowestGrade();
        }

        
        private void UpdateQueueGrade(ref Grade grade)
        {
        
            grade.ResponseGrade = grade.NumberOfTasksGrade;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade
          

        }
    }
}

