using System;

public class FishingEvents
{
    public event Action<string, int> onFishCaught;
    public void FishCaught(string id, int amount) 
    {
       onFishCaught?.Invoke(id, amount);
    }

   
}
