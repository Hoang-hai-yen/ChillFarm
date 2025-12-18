using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Assets.Scripts.Cloud.Schemas
{

    public class ExperiencePoint
    {
        public int Level { get; set; } = 1;
        public int CurrentXP { get; set; } = 0;
        public int TotalXP { get; set; } = 0;

        public ExperiencePoint() { }
    }
}
