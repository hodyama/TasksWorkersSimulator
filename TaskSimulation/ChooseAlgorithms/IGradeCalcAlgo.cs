using TaskSimulation.Simulator.Tasks;
using TaskSimulation.Simulator.Workers;

namespace TaskSimulation.ChooseAlgorithms
{
    public interface IGradeCalcAlgo
    {
        Grade InitialGrade();

        Grade UpdateOnTaskAdd(Grade grade, Worker worker);

        Grade UpdateOnTaskArrival(Grade grade, Worker worker);

        Grade UpdateOnTaskRemoved(Worker worker, Task task);

        IChooseWorkerAlgo ChooseMethod();
    }
}
