using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkiResort_Simulator
{
    public enum ELocation { Uphill, Downhill }
    public enum ESkill { Blue = 1, Red = 2, Black = 4 }

    public class Visitor: SimulationObject
    {
        private static Random random = new Random();
        
        public bool Equipped { get; private set; }
        public bool Rents { get; private set; }
        public ESkill Skill { get; private set; }
        public int LiftRidesLeft { get; private set; }
        public int FoodNeed { get; private set; }
        public double SpeedFactor { get; private set; }
        public ELocation CurrentLocation { get; private set; }

        private Simulator parent;

        public Visitor(Simulator parent, bool equipped = false, int arrival = 0, int food = 0, bool rents = false, ESkill skill = ESkill.Blue, int liftrides = 0, int speed = 1)
        {
            Equipped = equipped;
            Rents = rents;
            Skill = skill;
            LiftRidesLeft = (Equipped ? liftrides : 0);
            SpeedFactor = speed;
            FoodNeed = food;
            CurrentLocation = ELocation.Downhill;
            this.parent = parent;
            parent.RegisterEvent(this, arrival, null);
        }

        public override void CompleteEvent(object eventData)
        {
            #region Finish Building interaction
            if (eventData == null)
            {
                MessageFeed.RegisterMessage("Nový návštěvník dorazil.");
            }

            if (eventData != null && eventData is ResortBuilding)
            {
                CurrentLocation = (eventData as ResortBuilding).Exit;

                if (eventData is SkiHill)
                {
                    SkiHill ridden = eventData as SkiHill;
                    FoodNeed += (int)Math.Ceiling(3 * ridden.Length * ((int)ridden.Skill / 2.0));
                }

                else if (eventData is SkiLift)
                {
                    LiftRidesLeft--;
                }

                else if (eventData is FastFood)
                {
                    FoodNeed = 0;
                }

                else if (eventData is EquipmentRental)
                {
                    Rents = false;
                }
            }
            #endregion

            IEnumerable<ResortBuilding> nextTargets = parent.GetAvailableBuildings(CurrentLocation);

            #region Decide if Downhill
            if (CurrentLocation == ELocation.Downhill)
            {
                if (Rents && LiftRidesLeft > 0)
                {
                    IEnumerable<EquipmentRental> rentals = nextTargets.Where(rb => (rb is EquipmentRental)).Select(rb => (rb as EquipmentRental));
                    if (rentals.Count() > 0)
                    {
                        EquipmentRental best = GetLeastCrowdedBuilding(rentals) as EquipmentRental;
                        MessageFeed.RegisterMessage("Návštěvník jde do půjčovny " + best.ToString());
                        best.Use(this);
                        return;
                    }
                }

                if (Rents && FoodNeed == 0)
                {
                    parent.VisitorExit(this);
                    return;
                }

                if (LiftRidesLeft == 0)
                {
                    IEnumerable<FastFood> foods = nextTargets.Where(rb => (rb is FastFood)).Select(rb => (rb as FastFood));

                    if (FoodNeed == 0 && foods.Count() == 0)
                    {
                        parent.VisitorExit(this);
                        return;
                    }
                    
                    else if (foods.Count() > 0)
                    {
                        FastFood best = GetLeastCrowdedBuilding(foods) as FastFood;
                        MessageFeed.RegisterMessage("Návštěvník jde na jídlo do " + best.ToString());
                        best.Use(this);
                        return;
                    }
                }

                if (FoodNeed > (random.Next(100) + 1))
                {
                    IEnumerable<FastFood> foods = nextTargets.Where(rb => (rb is FastFood)).Select(rb => (rb as FastFood));
                    if (foods.Count() > 0)
                    {
                        FastFood best = GetLeastCrowdedBuilding(foods) as FastFood;
                        MessageFeed.RegisterMessage("Návštěvník jde na jídlo do " + best.ToString());
                        best.Use(this);
                        return;
                    }
                }

                IEnumerable<SkiLift> lifts = nextTargets.Where(rb => (rb is SkiLift)).Select(rb => (rb as SkiLift));
                IEnumerable<SkiHill> hills = parent.GetAvailableBuildings(ELocation.Uphill).Where(rb => (rb is SkiHill)).Select(rb => (rb as SkiHill));
                if (Skill <= ESkill.Red)
                {
                    hills = hills.Where(sh => (sh.Skill != ESkill.Black));
                }

                if (Skill == ESkill.Blue)
                {
                    hills = hills.Where(sh => (sh.Skill != ESkill.Red));
                }

                if (lifts.Count() > 0 && hills.Count() > 0)
                {
                    SkiLift best = GetLeastCrowdedBuilding(lifts) as SkiLift;
                    MessageFeed.RegisterMessage("Návštěvník jde na vlek " + best.ToString());
                    best.Use(this);
                    return;
                }

                parent.VisitorExit(this);
                return;
            }
            #endregion

            #region Decide if Uphill
            else
            {
                if (FoodNeed > (random.Next(100) + 1))
                {
                    IEnumerable<FastFood> foods = nextTargets.Where(rb => (rb is FastFood)).Select(rb => (rb as FastFood));
                    if (foods.Count() > 0)
                    {
                        FastFood bestf = GetLeastCrowdedBuilding(foods) as FastFood;
                        MessageFeed.RegisterMessage("Návštěvník jde na jídlo do " + bestf.ToString());
                        return;
                    }
                }

                IEnumerable<SkiHill> hills = nextTargets.Where(rb => (rb is SkiHill)).Select(rb => (rb as SkiHill));
                IEnumerable<SkiHill> bluehills = hills.Where(sh => (sh.Skill == ESkill.Blue));
                IEnumerable<SkiHill> redhills = hills.Where(sh => (sh.Skill == ESkill.Red));
                IEnumerable<SkiHill> blackhills = hills.Where(sh => (sh.Skill == ESkill.Black));

                SkiHill bestblue = GetLeastCrowdedBuilding(bluehills) as SkiHill;
                SkiHill bestred = GetLeastCrowdedBuilding(redhills) as SkiHill;
                SkiHill bestblack = GetLeastCrowdedBuilding(blackhills) as SkiHill;

                SkiHill best = null;
                
                if (Skill == ESkill.Black)
                {
                    if (best == null && bestblack != null)
                    {
                        best = bestblack;
                    }
                }

                if (Skill >= ESkill.Red)
                {
                    if (bestred != null)
                    {
                        if (best == null)
                        {
                            best = bestred;
                        }

                        else if (bestred.EnteredVisitors.Count + 30 < best.EnteredVisitors.Count)
                        {
                            best = bestred;
                        }
                    }
                }

                if (bestblue != null)
                {
                    if (best == null)
                    {
                        best = bestblue;
                    }

                    else if (bestblue.EnteredVisitors.Count + 30 < best.EnteredVisitors.Count)
                    {
                        best = bestblue;
                    }
                }

                if (best != null)
                {
                    MessageFeed.RegisterMessage("Návštěvník jde na sjezdovku " + best.ToString());
                    best.Use(this);
                    return;
                }
                
                else //FAILPROOF - Should never happen
                {
                    MessageFeed.RegisterMessage("Warning!");
                    parent.VisitorExit(this);
                    return;
                }
            }
            #endregion
        }

        private ResortBuilding GetLeastCrowdedBuilding(IEnumerable<ResortBuilding> buildings)
        {
            if (buildings.Count() > 0)
            {
                ResortBuilding best = buildings.First();
                foreach (ResortBuilding rb in buildings)
                {
                    if (rb.EnteredVisitors.Count < best.EnteredVisitors.Count)
                    {
                        best = rb;
                    }
                }

                return best;
            }

            else return null;
        }
    }
}