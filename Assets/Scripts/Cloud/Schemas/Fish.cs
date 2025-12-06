using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Assets.Scripts.Cloud.Schemas
{
    public class Fish
    {
        public string FishId { get; set; }
        public string Species { get; set; }
        public DateTime AddedAt { get; set; }
        public string Size { get; set; }
        public bool CanCatch { get; set; }

        public Fish() { }
    }
}

