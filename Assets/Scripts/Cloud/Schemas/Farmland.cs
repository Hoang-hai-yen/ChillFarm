using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Farmland
{
    public string UserId {get; set;}
    public List<Plot> Plots { get; set; } = new List<Plot>();
    public int TotalPlotsUnlocked { get; set; } = 0;
    public Farmland() { }
   
}

