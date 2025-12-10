using System;

public class FarmlandEvents
{
    public event Action<string, int> onCropHarvest;
    public void CropHavest(string id, int quatity) 
    {
        onCropHarvest?.Invoke(id, quatity);
    }

}
