using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace Assets.Scripts.Cloud.Schemas
{
    public class PlayerQuest
    {
        public string QuestId { get; set; }
        public List<QuestObjective> progresses { get; set; } = new List<QuestObjective>();
        public bool IsCompleted { get; set; } = false;
        // public bool IsClaimed { get; set; } = false;
        // public bool IsChanged { get; set; } = false;

        

        // public void AdvanceQuest(List<Quest.QuestRequirement> questRequirements)
        // {
        //     CurrentLevel++;
        //     IsCompleted = false;
        //     IsClaimed = false;
        //     IsChanged = true;

        //     questRequirements.ForEach(p => { progresses.Add(new PlayerQuest.QuestProgress() { ItemId = p.ItemId, CurrentAmount = 0, TargetAmount = p.GetTargetAmount(CurrentLevel), }); });

        // }
    }

    public class QuestObjective
    {
        public string ObjectiveId { get; set; }
        public int CurrentAmount { get; set; }
        public int TargetAmount { get; set; }
        public bool isCompleted {get; set;} = false;

    }

}

