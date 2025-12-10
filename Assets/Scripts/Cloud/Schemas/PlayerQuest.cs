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
        public QuestType QuestType { get; set; }


        public List<QuestProgress> progresses { get; set; }

        public bool IsCompleted { get; set; } = false;
        public bool IsClaimed { get; set; } = false;

        public bool IsChanged { get; set; } = false;

        public class QuestProgress
        {
            public string ItemId { get; set; }
            public int CurrentAmount { get; set; } = 0;
            public int TargetAmount { get; set; } = 0;
            public bool IsCompleted { get; set; } = false;
            public QuestProgress() { }
        }
    }
}

