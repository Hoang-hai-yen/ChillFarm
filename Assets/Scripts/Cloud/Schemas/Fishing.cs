using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Fishing
{

    public string UserId {get; set;}
 
    public Pond Pond { get; set; } = new Pond();

    public FishingStats Stats { get; set; } = new FishingStats();

    public Fishing() { }
  
}

