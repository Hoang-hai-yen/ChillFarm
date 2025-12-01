using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerData
{

    public int Gold {get; set;}
    public double Stamina { get; set; }
    public double MaxStamina { get; set; }
    public int CurrentDay { get; set; }
    public int CurrentTime { get; set; }

    public (double x, double y) Position { get; set; }
    public string CurrentScene { get; set; }

    public ExperiencePoint SkillXP { get; set; }    
    public ExperiencePoint AnimalXP { get; set; }
    public ExperiencePoint FishingXP { get; set; }
    public ExperiencePoint FarmingXP { get; set; }

    public Inventory Inventory { get; set; }
    public Inventory Storage { get; set; }



    public PlayerData() { }
}

