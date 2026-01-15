using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Cloud.Schemas
{
    public class Crop
    {
        public string CropId { get; set; }
        public float CurrentGrowth { get; set; } = 0;
        public int CurrentStage { get; set; } = 0;
        public bool IsWatered { get; set; } = false;
        public bool IsHarvestable { get; set; } = false;
        public float YieldMultiplier { get; set; } = 1f;
        public bool HasBeenFertilized { get; set; } = false;

        public Crop() { }
    }
}
