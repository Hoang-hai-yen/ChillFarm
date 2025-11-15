using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Crop
{
    public string SeedId { get; set; }
    public DateTime PlantedAt { get; set; }
    public int GrowthStage { get; set; }
    public double GrowthProgress { get; set; }
    public bool IsWatered { get; set; }
    public bool IsFertilized { get; set; }
    public bool ReadyToHarvest { get; set; }

    public Crop() { }
}

