using System;
using TaskSimulation.Utiles;

namespace TaskSimulation.Simulator.Events
{
    public abstract class AEvent : IComparable<AEvent>
    {
        public double ArriveTime { get; }
        //protected bool Periodic;

        protected AEvent(double arriveAt)
        {
            ArriveTime = arriveAt;
        }


        public int CompareTo(AEvent other)
        {
            return ArriveTime.CompareTo(other.ArriveTime);
        }

        public override string ToString()
        {
            return $"{this.GetType().Name.SpaceCapitals()}";
        }

        public abstract void Accept(ISimulatable visitor);
    }
}