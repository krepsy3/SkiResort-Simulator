using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiResort_Simulator
{
    public abstract class ResortBuilding: SimulationObject
    {
        public ELocation Entry { get; protected set; }
        public ELocation Exit { get; protected set; }
        public string Name { get; protected set; }
        public int DequeueFrequency { get; protected set; }
        public Queue<Visitor> EnteredVisitors { get; protected set; }
        public abstract override void CompleteEvent(object eventData);

        protected Simulator parent;
        protected static Random random = new Random();
        protected static object MoveQueueEventData = "Dequeue next visitor!";

        public ResortBuilding(ELocation entry, ELocation exit, int dequeueFrequency, string name, Simulator parent)
        {
            Entry = entry;
            Exit = exit;
            DequeueFrequency = dequeueFrequency;
            Name = name;
            EnteredVisitors = new Queue<Visitor>();
            this.parent = parent;
        }

        public virtual void Use(Visitor who)
        {
            EnteredVisitors.Enqueue(who);
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class SkiLift: ResortBuilding
    {
        public int RideTime { get; private set; }
        public double AccidentProbability { get; private set; }

        public SkiLift(Simulator parent, string name = "Vlek", int rideTime = 300, double accidentProbability = 0, int dequeueFrequency = 15): base(ELocation.Downhill, ELocation.Uphill, dequeueFrequency, name, parent)
        {
            RideTime = rideTime;
            AccidentProbability = accidentProbability;
            parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
        }

        public override void CompleteEvent(object eventData)
        {
            if (eventData == MoveQueueEventData)
            {
                if (EnteredVisitors.Count > 0)
                {
                    Visitor served = EnteredVisitors.Dequeue();
                    MessageFeed.RegisterMessage("Návštěvníka právě odbavil vlek " + Name);
                    parent.RegisterEvent(served, RideTime, this);
                }

                if ((random.Next(1000000001) / 1000000000.0) < AccidentProbability)
                {
                    parent.ModifyEventsOf(this, 60);
                    MessageFeed.RegisterMessage("Nehoda na vleku " + Name + ". Pasažéři vleku jsou zdrženi o 60 sekund");
                    parent.RegisterEvent(this, DequeueFrequency + 60, MoveQueueEventData);
                }

                else
                {
                    parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
                }
            }
        }
    }

    public class SkiHill: ResortBuilding
    {
        public int Length { get; private set; }
        public int MaintenanceFrequency { get; private set; }
        public int MaintenanceTime { get; private set; }
        public double AccidentProbability { get; private set; }
        public double AverageSpeed { get; private set; }
        public ESkill Skill { get; private set; }

        private int ridecount;

        public SkiHill(Simulator parent, int length = 1000, double avgSpeed = 10, ESkill skill = ESkill.Blue, string name = "Sjezdovka", int maintenanceFrequency = 5000, int maintenanceTime = 2000, double accidentProbability = 0, int dequeueFrequency = 15) : base(ELocation.Uphill, ELocation.Downhill, dequeueFrequency, name, parent)
        {
            Length = length;
            AverageSpeed = avgSpeed;
            Skill = skill;
            MaintenanceFrequency = maintenanceFrequency;
            MaintenanceTime = maintenanceTime;
            AccidentProbability = accidentProbability;
            ridecount = 0;
            parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
        }

        public override void CompleteEvent(object eventData)
        {
            if (eventData == MoveQueueEventData)
            {
                if (EnteredVisitors.Count > 0)
                {
                    Visitor served = EnteredVisitors.Dequeue();
                    MessageFeed.RegisterMessage("Návštěvník vjíždí na sjezdovku " + Name);
                    parent.RegisterEvent(served, (int)Math.Ceiling(Length / (AverageSpeed * served.SpeedFactor)), this);
                    ridecount++;
                }

                if (ridecount == MaintenanceFrequency)
                {
                    parent.RegisterEvent(this, MaintenanceTime, MoveQueueEventData);
                    MessageFeed.RegisterMessage("Nastává úprava sjezdovky " + Name + ". Další návštěvníci na ni vjedou až za " + MaintenanceTime.ToString() + " sekund.");
                    ridecount = 0;
                }

                else if ((random.Next(1000000001) / 1000000000.0) < AccidentProbability)
                {
                    parent.ModifyEventsOf(this, 30);
                    MessageFeed.RegisterMessage("Nehoda na sjezdovce " + Name + ". Návštěvníci jsou zdrženi o 30 sekund");
                    parent.RegisterEvent(this, DequeueFrequency + 30, MoveQueueEventData);
                }

                else
                {
                    parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
                }
            }
        }
    }

    public class FastFood: ResortBuilding
    {
        public int BaseServiceTime { get; private set; }

        public FastFood(Simulator parent, string name = "Občerstvení", int dequeueFrequency = 90, int baseServiceTime = 5) : base(ELocation.Downhill, ELocation.Downhill, dequeueFrequency, name, parent)
        {
            BaseServiceTime = baseServiceTime;
            parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
        }

        public override void CompleteEvent(object eventData)
        {
            if (eventData == MoveQueueEventData)
            {
                if (EnteredVisitors.Count > 0)
                {
                    Visitor served = EnteredVisitors.Dequeue();
                    parent.RegisterEvent(served, BaseServiceTime * served.FoodNeed, this);
                }

                parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
            }
        }
    }

    public class EquipmentRental: ResortBuilding
    {
        public int BaseServiceTime { get; private set; }

        public EquipmentRental(Simulator parent, string name = "Půjčovna", int dequeueFrequency = 60, int baseServiceTime = 300) : base(ELocation.Downhill, ELocation.Downhill, dequeueFrequency, name, parent)
        {
            BaseServiceTime = baseServiceTime;
            parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
        }

        public override void CompleteEvent(object eventData)
        {
            if (eventData == MoveQueueEventData)
            {
                if (EnteredVisitors.Count > 0)
                {
                    Visitor served = EnteredVisitors.Dequeue();
                    parent.RegisterEvent(served, BaseServiceTime, this);
                }

                parent.RegisterEvent(this, DequeueFrequency, MoveQueueEventData);
            }
        }
    }
}