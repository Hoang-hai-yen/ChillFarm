using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;





namespace Assets.Scripts.Cloud.Schemas
{
    public class Inventory
    {
        public List<Item> HotbarItems { get; set; } = new List<Item>();
        public List<Item> BackpackItems { get; set; } = new List<Item>();

        public Inventory() { }

        public class Item
        {
            public string ItemId { get; set; }
            public int Quantity { get; set; }
            // public int SlotIndex { get; set; }

            public Item() { }
        }
    }
}

