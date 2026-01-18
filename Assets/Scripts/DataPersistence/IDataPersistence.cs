using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData data);

    void SaveData(GameData data);

    // protected void MarkDataDirty(GameDataManager.DataType dataType)
    // {
    //     GameDataManager.instance.MarkDirty(dataType);
    // }
}
