//bs"d
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
                NumberOfTasksGrade = 0,
            };

            return grade;
        }

        public Grade UpdateOnTaskAdd(Grade grade, Worker worker)
        {
            // TODO add to metadata as NumberOfTasks

            UpdateQueueGrade(ref grade, worker);
            grade.NumberOfTasksGrade++;

            return grade;
        }
        public Grade UpdateOnTaskArrival(Grade grade, Worker worker )
        {
            UpdateQueueGrade(ref grade, worker);
            return grade;

        }
        public Grade UpdateOnTaskRemoved(Worker worker, Task task) //WORKER TASK
        {
            var grade = worker.Grade;

            UpdateQueueGrade(ref grade, worker);

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
        private void UpdateQueueGrade(ref Grade grade, Worker w)
        {
            // TODO ask, is this Queue length or Total execution time
            var currentTime = Simulator.SimulateServer.SimulationClock;

            var workingTime = grade.Meta.WorkingTime;

            var sum = grade.Meta.tl;
            var currentQeueuValue = (grade.NumberOfTasksGrade - TASKS_IN_PROSS);
            if (currentQeueuValue < 0)
                currentQeueuValue = 0;
            var newDeltaTime = currentTime - grade.Meta.LastModifiedAt;
            if (newDeltaTime <= 0)
                return;
            sum += currentQeueuValue * newDeltaTime;

            grade.Meta.WorkingTime += newDeltaTime;


            //grade.ResponseGrade = LMath.AverageIncrementalSize(grade.ResponseGrade, workingTime, currentQeueuValue, newDeltaTime);

            if (currentTime<window)

                 grade.ResponseGrade = Simulator.SimulateServer.GetWorkerWindowTL(w,0) / currentTime;
            else
                grade.ResponseGrade = Simulator.SimulateServer.GetWorkerWindowTL(w, currentTime-window) / window;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade
            grade.Meta.tl = sum;

            grade.Meta.LastModifiedAt = Simulator.SimulateServer.SimulationClock;

        }
    }
}




