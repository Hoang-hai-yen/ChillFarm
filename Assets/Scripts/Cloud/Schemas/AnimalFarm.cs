using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Cloud.Schemas
{
    public class AnimalFarm
    {
        public string UserId { get; set; }
        
        // public int FarmLevel { get; set; } = 1;
        // public int MaxCapacity { get; set; } = 5;
        //public bool HasAutoMilker { get; set; }
        //public bool HasAutoIncubator { get; set; }

        public List<Pen> Pens {get; set;} = new List<Pen>();

        public AnimalFarm() { }
    }

    public class Pen
    {
        public string Id {get; set;}
        public bool IsUnlocked { get; set; } = false;
        public int CurrentLevel { get; set; } = 0;
        public List<Animal> Animals {get; set;} = new List<Animal>();
    }
}