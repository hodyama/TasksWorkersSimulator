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
                
            };

            return grade;
        }

        public Grade UpdateOnTaskAdd(Grade grade, Worker worker)
        {
            // TODO add to metadata as NumberOfTasks

            UpdateQueueGrade(ref grade, worker);
            grade.Meta.NumberOfTasks++;

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

            grade.Meta.NumberOfTasks--;

            return grade;
        }

        public IChooseWorkerAlgo ChooseMethod()
        {
            return new ChooseLowestGrade();
        }

        
        private void UpdateQueueGrade(ref Grade grade, Worker w)
        {
            
            var currentTime = Simulator.SimulateServer.SimulationClock;

            if (currentTime<window)

                 grade.ResponseGrade = Simulator.SimulateServer.GetWorkerWindowTL(w,0) / currentTime;
            else
                grade.ResponseGrade = Simulator.SimulateServer.GetWorkerWindowTL(w, currentTime-window) / window;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade
           

        }
        
    }
}




