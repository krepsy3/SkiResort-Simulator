using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SkiResort_Simulator
{
    public static class Loader
    {
        public static void Load(string location, Simulator simulator)
        {
            using (StreamReader sr = new StreamReader(location))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();
                    line = line.Trim();
                    string[] items = line.Split(',');
                    switch (items[0].ToLower())
                    {
                        case "v":
                            bool equipped = true;
                            int arrival = 0;
                            int food = 0;
                            bool rents = false;
                            ESkill skill = ESkill.Blue;
                            int liftrides = 20;
                            int speed = 1;

                            foreach(string s in items)
                            {
                                string[] ss = s.Trim().Split('=');
                                switch (ss[0].Trim().ToLower())
                                {
                                    case "equipped": equipped = ((ss[1].Trim().ToLower() == "true" || ss[1].Trim().ToLower() == "1") ? true : false); break;
                                    case "arrival": arrival = int.Parse(ss[1].Trim()); break;
                                    case "food": food = int.Parse(ss[1].Trim()); break;
                                    case "rents": rents = ((ss[1].Trim().ToLower() == "true" || ss[1].Trim().ToLower() == "1") ? true : false); break;
                                    case "skill": skill = (ss[1].Trim().ToLower() == "Black" ? ESkill.Black : (ss[1].Trim().ToLower() == "Red" ? ESkill.Red : ESkill.Blue)); break;
                                    case "liftrides": liftrides = int.Parse(ss[1].Trim()); break;
                                    case "speed": speed = int.Parse(ss[1].Trim()); break;
                                }
                            }

                            simulator.Visitors.Add(new Visitor(simulator, equipped, arrival, food, rents, skill, liftrides, speed));
                            break;

                        case "l":
                            string name = "Vlek";
                            int ridetime = 300;
                            double accident = 0;
                            int dequeue = 15;

                            foreach (string s in items)
                            {
                                string[] ss = s.Trim().Split('=');
                                switch (ss[0].Trim().ToLower())
                                {
                                    case "name": name = ss[1].Trim(); break;
                                    case "ridetime": ridetime = int.Parse(ss[1].Trim()); break;
                                    case "accident": accident = double.Parse(ss[1].Trim()); break;
                                    case "dequeue": dequeue = int.Parse(ss[1].Trim()); break;
                                }
                            }

                            simulator.Buildings.Add(new SkiLift(simulator, name, ridetime, accident, dequeue));
                            break;

                        case "h":
                            int length = 1000;
                            int avgspeed = 10;
                            skill = ESkill.Blue;
                            name = "Sjezdovka";
                            int maintfreq = 5000;
                            int mainttime = 2000;
                            accident = 0;
                            dequeue = 15;

                            foreach (string s in items)
                            {
                                string[] ss = s.Trim().Split('=');
                                switch (ss[0].Trim().ToLower())
                                {
                                    case "length": length = int.Parse(ss[1].Trim()); break;
                                    case "avgspeed": avgspeed = int.Parse(ss[1].Trim()); break;
                                    case "name": name = ss[1].Trim(); break;
                                    case "maintfreq": maintfreq = int.Parse(ss[1].Trim()); break;
                                    case "mainttime": mainttime = int.Parse(ss[1].Trim()); break;
                                    case "accident": accident = double.Parse(ss[1].Trim()); break;
                                    case "dequeue": dequeue = int.Parse(ss[1].Trim()); break;
                                }
                            }

                            simulator.Buildings.Add(new SkiHill(simulator, length, avgspeed, skill, name, maintfreq, mainttime, accident, dequeue));
                            break;

                        case "f":
                            name = "Občerstvení";
                            dequeue = 15;
                            int service = 5;

                            foreach (string s in items)
                            {
                                string[] ss = s.Trim().Split('=');
                                switch (ss[0].Trim().ToLower())
                                {
                                    case "name": name = ss[1].Trim(); break;
                                    case "dequeue": dequeue = int.Parse(ss[1].Trim()); break;
                                    case "service": service = int.Parse(ss[1].Trim()); break;
                                }
                            }

                            simulator.Buildings.Add(new FastFood(simulator, name, dequeue, service));
                            break;

                        case "r":
                            name = "Půjčovna";
                            dequeue = 15;
                            service = 300;

                            foreach (string s in items)
                            {
                                string[] ss = s.Trim().Split('=');
                                switch (ss[0].Trim().ToLower())
                                {
                                    case "name": name = ss[1].Trim(); break;
                                    case "dequeue": dequeue = int.Parse(ss[1].Trim()); break;
                                    case "service": service = int.Parse(ss[1].Trim()); break;
                                }
                            }

                            simulator.Buildings.Add(new EquipmentRental(simulator, name, dequeue, service));
                            break;
                    }
                }
            }
        }
    }
}
