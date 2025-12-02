using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AnimalFarm
{
    public string UserId {get; set;}
    public List<Animal> Animals { get; set; } = new List<Animal>();
    public int FarmLevel { get; set; } = 1;
    public int MaxCapacity { get; set; } = 5;
    //public bool HasAutoMilker { get; set; }
    //public bool HasAutoIncubator { get; set; }


    public AnimalFarm() { }
}
