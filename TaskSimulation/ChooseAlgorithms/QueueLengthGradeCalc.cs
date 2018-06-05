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
                
            };

            return grade;
        }

        public Grade UpdateOnTaskAdd(Grade grade)
        {
            

            UpdateQueueGrade(ref grade);
            grade.Meta.NumberOfTasks++;

            return grade;
        }
        public Grade UpdateOnTaskArrival(Grade grade)
        {
            
            UpdateQueueGrade(ref grade);
            return grade;

        }
        public Grade UpdateOnTaskRemoved(Worker worker, Task task)
        {
            var grade = worker.Grade;


            UpdateQueueGrade(ref grade);

            grade.Meta.NumberOfTasks--; 

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
           
            var currentTime = Simulator.SimulateServer.SimulationClock;
            var sum = grade.Meta.tl;
            var currentQeueuValue = grade.Meta.NumberOfTasks ;
           
            var newDeltaTime = currentTime - grade.Meta.LastModifiedAt;
            if (newDeltaTime <= 0)
                return;
            sum += currentQeueuValue * newDeltaTime;

            grade.ResponseGrade = sum / currentTime;

            grade.TotalGrade = grade.ResponseGrade; // TODO add FeedbackGrade

            grade.Meta.tl = sum;

            grade.Meta.LastModifiedAt = Simulator.SimulateServer.SimulationClock;

            
            grade.Meta.WorkingTime += newDeltaTime;

        }
    }
}
