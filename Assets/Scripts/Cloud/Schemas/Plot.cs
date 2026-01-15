using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Assets.Scripts.Cloud.Schemas
{
    public class Plot
    {
        // public string PlotId { get; set; }
        // public bool IsUnlocked { get; set; }
        // public bool IsDug { get; set; }

        public int PlotState { get; set; } = 0;
        public Position PlotPosition { get; set; }
        public Crop Crop { get; set; }

        public Plot() { }

        public class Position
        {
            public int X { get; set; } = 0;
            public int Y { get; set; } = 0;
            public int Z { get; set; } = 0;

            public Position() { }
            public Position(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }
        }
    }

    
}
