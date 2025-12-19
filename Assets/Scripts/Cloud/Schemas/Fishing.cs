using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Assets.Scripts.Cloud.Schemas
{
    public class Fishing
    {

        public string UserId { get; set; }

        //public Pond Pond { get; set; } = new Pond();

        public FishingStats Stats { get; set; } = new FishingStats();

        public Fishing() { }

        public class FishingStats
        {

            public int TotalCaught { get; set; } = 0;
            public List<CaughtFish> CaughtFishes { get; set; } = new List<CaughtFish>();

            public FishingStats() { }

            public class CaughtFish
            {
                //public string Species { get; set; }
                //public string Size { get; set; }
                //public DateTime CaughtAt { get; set; }
                public Fish Fish { get; set; }
                public int Quantity { get; set; } = 0;

                public CaughtFish() { }
            }

        }
    }
}

