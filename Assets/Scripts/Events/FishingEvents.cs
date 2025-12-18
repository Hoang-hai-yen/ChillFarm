using System;

public class FishingEvents
{
    public event Action<string> onFishCaught;
    public void FishCaught(string id) 
    {
       onFishCaught?.Invoke(id);
    }

   
}
