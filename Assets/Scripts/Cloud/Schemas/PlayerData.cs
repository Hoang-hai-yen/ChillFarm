using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerData
{

    public string UserId {get; set;}

    public int Gold { get; set; } = 0;
    public double Stamina { get; set; } = 0;
    public double MaxStamina { get; set; } = 1;
    public int CurrentDay { get; set; } = 1;
    public int CurrentTime { get; set; } = 0;
    
    public PlayerPosition Position { get; set; } = new PlayerPosition();

    public PlayerXP ExperiencePoint { get; set; } = new PlayerXP();

    public Inventory Inventory { get; set; } = new Inventory();
    public Inventory Storage { get; set; } = new Inventory();



    public PlayerData() { }

    public class PlayerPosition
    {
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        public string CurrentScene { get; set; } = "";

        public PlayerPosition() { }
    }

    public class PlayerXP
    {
        public ExperiencePoint SkillXP { get; set; } = new ExperiencePoint();
        public ExperiencePoint AnimalXP { get; set; } = new ExperiencePoint();
        public ExperiencePoint FishingXP { get; set; } = new ExperiencePoint();
        public ExperiencePoint FarmingXP { get; set; } = new ExperiencePoint();

        public PlayerXP() { }
    }
}

