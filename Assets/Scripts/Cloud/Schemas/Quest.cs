using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class Quest
{
   
    public string Id { get; set; }
  
  // Quest info
   public string NpcId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    // Requirements
    public List<QuestRequirement> Requirements { get; set; }

    // Rewards
    public QuestRewards Rewards { get; set; }

    static public bool IsTargetItem(Quest quest, string targetItemId)
    {
        QuestRequirement questRequirement = quest.Requirements.Find(r => r.ItemId == targetItemId);
        
        return questRequirement != null;
    }

    public class QuestRequirement
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }

        public QuestRequirement() { }
    }

     public class QuestRewards
     {
         public int Gold { get; set; }
         public List<RewardXP> Xp { get; set; }
         //public List<Inventory.Item> Items { get; set; }
         public QuestRewards() { }

        public class RewardXP
        {
            public string type { get; set; }
            public int amount {  get; set; }
        }
     }
}

