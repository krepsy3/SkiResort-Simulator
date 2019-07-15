using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiResort_Simulator
{
    public class Simulator
    {
        public List<ResortBuilding> Buildings { get; private set; }
        public List<Visitor> Visitors { get; private set; }
        private List<SimulationEvent> ActiveEvents { get; set; }
        private List<SimulationEvent> CurrentTimeEvents { get; set; }
        public int CurrentTime { get; private set; }

        public Simulator()
        {
            Buildings = new List<ResortBuilding>();
            Visitors = new List<Visitor>();
            ActiveEvents = new List<SimulationEvent>();
            CurrentTimeEvents = new List<SimulationEvent>();
            CurrentTime = 0;
        }

        public void RegisterEvent(SimulationObject who, int eventTime, object eventData)
        {
            ActiveEvents.Add(new SimulationEvent(who, eventTime + CurrentTime, eventData));
        }

        public void ModifyEvent(SimulationObject who, int additiveTime)
        {
            foreach(SimulationEvent se in ActiveEvents.Where(se => (se.Who == who)))
            {
                se.Prolong(additiveTime);
            }
        }

        public void ModifyEventsOf(ResortBuilding ofwho, int additivetime)
        {
            foreach(SimulationEvent se in ActiveEvents)
            {
                if (se.Who is Visitor)
                {
                    Visitor v = se.Who as Visitor;
                    ResortBuilding rb = se.EventData as ResortBuilding;
                    if (rb == ofwho)
                    {
                        ModifyEvent(v, additivetime);
                    }
                }
            }
        }

        public bool PerformSimulationStep()
        {
            if (CurrentTimeEvents.Count == 0)
            {
                if (ActiveEvents.Count > 0)
                {
                    int mintime = int.MaxValue;
                    foreach (SimulationEvent se in ActiveEvents)
                    {
                        if (se.FinishTime < mintime)
                        {
                            mintime = se.FinishTime;
                        }
                    }

                    CurrentTime = mintime;
                    MessageFeed.RegisterMessage("Změna aktuálního času simulace - " + CurrentTime);
                    
                    foreach (SimulationEvent se in ActiveEvents)
                    {
                        if (se.FinishTime == mintime)
                        {
                            CurrentTimeEvents.Add(se);
                        }
                    }

                    foreach (SimulationEvent se in CurrentTimeEvents)
                    {
                        ActiveEvents.Remove(se);
                    }

                    return true;
                }

                else return false;
            }

            else
            {
                SimulationEvent se = CurrentTimeEvents[CurrentTimeEvents.Count - 1];
                CurrentTimeEvents.RemoveAt(CurrentTimeEvents.Count - 1);
                se.Who.CompleteEvent(se.EventData);
                return true;
            }
        }

        public void VisitorExit(Visitor who)
        {
            Visitors.Remove(who);
            MessageFeed.RegisterMessage("Návštěvník odchází.");
        }

        public IEnumerable<ResortBuilding> GetAvailableBuildings(ELocation where)
        {
            return Buildings.Where(rb => (rb.Entry == where));
        }
    }
}
