using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiResort_Simulator
{
    public abstract class SimulationObject
    {
        public abstract void CompleteEvent(object eventData);
    }

    public struct SimulationEvent
    {
        public SimulationObject Who { get; private set; }
        public int FinishTime { get; private set; }
        public object EventData { get; private set; }

        public SimulationEvent(SimulationObject who, int finishTime, object eventData)
        {
            Who = who;
            FinishTime = finishTime;
            EventData = eventData;
        }

        public void Prolong(int additiveTime)
        {
            FinishTime += additiveTime;
        }
    }
}
