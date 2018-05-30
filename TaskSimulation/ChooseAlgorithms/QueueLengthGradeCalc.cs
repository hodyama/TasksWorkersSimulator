using TaskSimulation.Distribution;
using TaskSimulation.Simulator.Tasks;
using TaskSimulation.Simulator.Workers;
using TaskSimulation.Utiles;

namespace TaskSimulation.ChooseAlgorithms
{
    public class QueueLengthGradeCalc : IGradeCalcAlgo
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

        public Grade UpdateOnTaskAdd(Grade grade, Worker w)
        {
            // TODO add to metadata as NumberOfTasks

            UpdateQueueGrade(ref grade);
            grade.NumberOfTasksGrade++;

            return grade;
        }
        public Grade UpdateOnTaskArrival(Grade grade, Worker w)
        {
            
            UpdateQueueGrade(ref grade);
            return grade;

        }
        public Grade UpdateOnTaskRemoved(Worker worker, Task task) //WORKER TASK
        {
            var grade = worker.Grade;
            
            UpdateQueueGrade(ref grade);

            grade.NumberOfTasksGrade--; 

            return grade;
        }
        

        public IChooseWorkerAlgo ChooseMethod()
        {
            return new ChooseLowestGrade();
        }

        /// <summary>
        /// Queue grade re-calculations
        /// QueueTime = (Delta time * Queue length) 
        /// QueueAvarage = TODO Need to normalize the QueueTime
        /// </summary>
        /// <param name="grade"></param>
        private void UpdateQueueGrade(ref Grade grade)
        {
            // TODO ask, is this Queue length or Total execution time
            var currentTime = Simulator.SimulateServer.SimulationClock;
            
            var workingTime = grade.Meta.WorkingTime;

            var sum = grade.Meta.tl;
            //var currentQeueuValue = (grade.NumberOfTasksGrade - TASKS_IN_PROSS);
            var currentQeueuValue = (grade.NumberOfTasksGrade );
            if (currentQeueuValue < 0)
                 currentQeueuValue = 0;
            var newDeltaTime = currentTime - grade.Meta.LastModifiedAt;
            if (newDeltaTime <= 0)
                return;
            sum += currentQeueuValue * newDeltaTime;

            grade.Meta.WorkingTime += newDeltaTime;

            
            //grade.ResponseGrade = LMath.AverageIncrementalSize(grade.ResponseGrade, workingTime, currentQeueuValue, newDeltaTime);

           
            grade.ResponseGrade = sum / currentTime;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade
            grade.Meta.tl = sum;

            grade.Meta.LastModifiedAt = Simulator.SimulateServer.SimulationClock;
            
        }
    }
}
