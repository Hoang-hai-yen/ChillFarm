using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class FishingStats
{
    
    public int TotalCaught { get; set; } = 0;
    public CaughtFish BiggestFish {get; set; } 

    public FishingStats() { }

    public class CaughtFish
    {
        public string Species { get; set; }
        public string Size { get; set; }
        public DateTime CaughtAt { get; set; }

        public CaughtFish() { }
    }

}

