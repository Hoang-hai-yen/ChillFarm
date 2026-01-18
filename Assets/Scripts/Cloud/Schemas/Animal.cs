using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Cloud.Schemas
{
    public class Animal
    {
        public string Id {get; set;}
        
        public string AnimalDataId {get; set;}
        public bool IsAdult {get; set;} = false; 
        public int CurrentAge {get; set;} = 0;
        public float Affection {get; set;} = 0f;
        public bool IsFedToday {get; set;} = false;
        public bool HasPlayedToday {get; set;} = false;
    
        public int DaysWithoutFood {get; set;} = 0;
        public int MaxStarvationDays {get; set;} = 3;
        public bool IsDead {get; set;} = false; 


        public Animal() { }
    }
}
