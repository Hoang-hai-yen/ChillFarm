using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class PlayerQuest
{
  public string QuestId { get; set; }
  
  public List<QuestProgress> progresses{ get; set; }

  public bool IsCompleted { get; set; }
  public bool IsClaimed { get; set; }   


    public class QuestProgress
    {
        public string ItemId { get; set; }
        public int CurrentAmount { get; set; }
        public QuestProgress() { }
    }
}

