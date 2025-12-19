using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Cloud.Schemas
{
    public class Animal
    {
        public string AnimalId { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int Affection { get; set; }
        public bool IsFed { get; set; }
        public bool CanProduce { get; set; }
        public string ProductId { get; set; }
        public bool IsDead { get; set; }


        public Animal() { }
    }
}
