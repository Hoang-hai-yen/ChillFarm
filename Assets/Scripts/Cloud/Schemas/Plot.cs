using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Plot
{
    public string PlotId { get; set; }
    public bool IsUnlocked { get; set; }
    public bool IsDug { get; set; }
    public (double x, double y) Position { get; set; }
    public Crop Crop { get; set; }

    public Plot() { }
}
