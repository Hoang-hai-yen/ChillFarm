using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Inventory
{
    public List<Item> Items { get; set; } = new List<Item>();
    public int MaxSlots { get; set; } = 20;

    public Inventory() { }

    public class Item
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public int SlotIndex { get; set; }

        public Item() { }
    }
}

