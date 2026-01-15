using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Assets.Scripts.Cloud.Schemas
{
    public class PlayerData
    {

        public string UserId { get; set; }

        public int Gold { get; set; } = 500;
        public double Stamina { get; set; } = 100f;
        public double MaxStamina { get; set; } = 100f;
        public int CurrentDay { get; set; } = 1;
        public DateTime CurrentTime { get; set; } = DateTime.Today.AddHours(6); // Bắt đầu lúc 6 giờ sáng
        public int CurrentDaysAwake { get; set; } = 0;
        public int SkillLevel { get; set; } = 0;
        public int AnimalLevel { get; set; } = 0;
        public int FishingLevel { get; set; } = 0;
        public int FarmingLevel { get; set; } = 0;
        public PlayerPosition Position { get; set; } = new PlayerPosition();

        // public PlayerXP ExperiencePoint { get; set; } = new PlayerXP();

        public Inventory Inventory { get; set; } = new Inventory();
        public Inventory Storage { get; set; } = new Inventory();



        public PlayerData() { }

        public class PlayerPosition
        {
            public double X { get; set; } = 0;
            public double Y { get; set; } = 0;
            public double Z { get; set; } = 0;
            public string CurrentScene { get; set; } = "Test";

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
}

