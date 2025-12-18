using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Assets.Scripts.Cloud.Schemas
{
    public class Quest
    {

        public string Id { get; set; }

        // Quest info
        public string NpcId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public QuestType QuestType { get; set; }

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
            public int baseAmount { get; set; } = 5;     
            public float difficultyMultiplier = 1.5f;

            public QuestRequirement() { }

             public int GetTargetAmount( int currentLevel)
            {

                return Mathf.RoundToInt(baseAmount * Mathf.Pow(difficultyMultiplier, currentLevel - 1));
            }
        }

        public class QuestRewards
        {
            public int baseGold { get; set; }
            public List<RewardXP> Xp { get; set; }
            //public List<Inventory.Item> Items { get; set; }
            public float rewardMultiplier = 1.2f;
            public QuestRewards() { }

            public class RewardXP
            {
                public string type { get; set; }
                public int baseAmount { get; set; }

                public int GetTotalXP(float rewardMultiplier , int currentLevel)
                {
                    return Mathf.RoundToInt(baseAmount * Mathf.Pow(rewardMultiplier, currentLevel - 1));

                }
            }

            public int GetTotalGold(int currentLevel)
            {
                return Mathf.RoundToInt(baseGold * Mathf.Pow(rewardMultiplier, currentLevel - 1));
                
            }
        }
    }

    public enum QuestType
    {
        HARVEST,
        FISHING
    }

}

