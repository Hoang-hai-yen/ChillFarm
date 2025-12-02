using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Pond
{
    public bool IsUnlocked { get; set; } = false;
    public int MaxCapacity { get; set; } = 5;
    public List<Fish> Fish { get; set; } = new List<Fish>();
    public Pond() { }
}

