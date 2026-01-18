using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Assets.Scripts.Cloud.Schemas;

public class FarmlandManager : MonoBehaviour, IDataPersistence
{
    public enum PlotState
    {
        None,
        Tilled,   
        Watered  
    }

    [Header("Tilemaps & Tiles")]
    public Tilemap tillableTilemap; 
    public Tile tilledTile;
    public Tile wateredTile;

    [Header("Prefabs")]
    public GameObject cropPrefab; 
    private Dictionary<Vector3Int, PlotState> plotStates = new Dictionary<Vector3Int, PlotState>();
    private Dictionary<Vector3Int, Crop> cropsOnPlots = new Dictionary<Vector3Int, Crop>();
    
    private TimeController timeController;

    void Start()
    {
        timeController = FindFirstObjectByType<TimeController>();
        if (timeController != null)
        {
            timeController.OnNewDayStart += OnNewDay;
        }
        else
        {
            Debug.LogError("FarmlandManager: Không tìm thấy TimeController!");
        }
    }

    void OnDisable()
    {
        if (timeController != null)
        {
            timeController.OnNewDayStart -= OnNewDay;
        }
    }

    public void LoadData(GameData data)
    {
        foreach(var plot in data.FarmlandData.Plots)
        {
            Vector3Int plotPos = new Vector3Int(plot.PlotPosition.X, plot.PlotPosition.Y, plot.PlotPosition.Z);
            PlotState state = (PlotState)plot.PlotState;
            SetPlotState(plotPos, state, state == PlotState.Tilled ? tilledTile : wateredTile);

            if (plot.Crop != null)
            {
                GameObject cropObj = Instantiate(cropPrefab, tillableTilemap.GetCellCenterWorld(plotPos), Quaternion.identity);
                Crop cropInstance = cropObj.GetComponent<Crop>();
                cropInstance.LoadFromSchema(plot.Crop);
                cropsOnPlots.Add(plotPos, cropInstance);
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.FarmlandData.Plots.Clear();
        foreach (var plotEntry in plotStates)
        {
            Plot plotSchema = new Plot
            {
                PlotPosition = new Plot.Position
                (
                    plotEntry.Key.x,
                    plotEntry.Key.y,
                    plotEntry.Key.z
                ),
                PlotState = (int)plotEntry.Value
            };

            if (cropsOnPlots.TryGetValue(plotEntry.Key, out Crop crop))
            {
                plotSchema.Crop = crop.ToSchema();
            }

            data.FarmlandData.Plots.Add(plotSchema);
        }
    }

    public void MarkDataDirty()
    {
        GameDataManager.instance.MarkDirty(GameDataManager.DataType.farmland);
    }


    public bool Interact(Vector3Int plotPosition, ItemData heldItem)
    {
        PlotState currentState = GetPlotState(plotPosition);
        bool actionSuccessful = false; 
        if (heldItem == null)
        {
            actionSuccessful = HandleHarvest(plotPosition); 
            if (actionSuccessful) return true;
        }
        else if (heldItem is ToolData tool)
        {
            actionSuccessful = HandleToolInteraction(plotPosition, currentState, tool.toolType);
        }
        else if (heldItem is SeedData seed)
        {
            actionSuccessful = HandleSeedInteraction(plotPosition, currentState, seed);
        }
        else if (heldItem is FertilizerData fertilizer)
        {
            actionSuccessful = HandleFertilizerInteraction(plotPosition, fertilizer);
        }
        else if (heldItem == null) 
        {
            actionSuccessful = HandleHarvest(plotPosition);
        }
        MarkDataDirty();
        return actionSuccessful; 
    }

    private bool HandleToolInteraction(Vector3Int plotPosition, PlotState currentState, ToolType toolType)
    {   
        MarkDataDirty();
        switch (toolType)
        {
            case ToolType.Hoe:
            if (tillableTilemap.HasTile(plotPosition) && !plotStates.ContainsKey(plotPosition))
                {
                    SetPlotState(plotPosition, PlotState.Tilled, tilledTile);
                    Debug.Log("Dùng cuốc cuốc đất!");
                    return true; 
                }
            break;

            case ToolType.WateringCan:
                if (currentState == PlotState.Tilled && !cropsOnPlots.ContainsKey(plotPosition))
                {
                    SetPlotState(plotPosition, PlotState.Watered, wateredTile);
                }
                else if (cropsOnPlots.TryGetValue(plotPosition, out Crop crop))
                {
                    crop.Water();
                    SetPlotState(plotPosition, PlotState.Watered, wateredTile);
                    Debug.Log("Đã tưới cây!");
                }
                break;
        }
        return false;
    }

    private bool HandleSeedInteraction(Vector3Int plotPosition, PlotState currentState, SeedData seed)
    {
        if (currentState == PlotState.Tilled || currentState == PlotState.Watered)
        {
            if (!cropsOnPlots.ContainsKey(plotPosition))
            {
                GameObject cropObj = Instantiate(cropPrefab, tillableTilemap.GetCellCenterWorld(plotPosition), Quaternion.identity);
                Crop cropInstance = cropObj.GetComponent<Crop>();
                
                bool isSoilWet = (currentState == PlotState.Watered);

                cropInstance.Initialize(seed.cropToPlant, isSoilWet);
                
                cropsOnPlots.Add(plotPosition, cropInstance);
            
                Debug.Log($"Gieo hạt {seed.cropToPlant.cropName}!");
                MarkDataDirty();
                return true;
            }
        }
        return false;
    }

    private bool HandleHarvest(Vector3Int plotPosition)
    {
        if (cropsOnPlots.TryGetValue(plotPosition, out Crop crop)) 
        {
            if (crop.IsHarvestable()) 
            {
                crop.Harvest();
                SetPlotState(plotPosition, PlotState.Tilled, tilledTile); 
                
                cropsOnPlots.Remove(plotPosition);
                MarkDataDirty(); 
                return true;
            }
        }
        return false;
    }

    private void OnNewDay()
    {
        List<Vector3Int> plotsToReset = new List<Vector3Int>();

        foreach (var plotEntry in plotStates)
        {
            if (plotEntry.Value == PlotState.Watered)
            {
                plotsToReset.Add(plotEntry.Key);
            }
        }
        
        foreach(var pos in plotsToReset)
        {
            if (!cropsOnPlots.ContainsKey(pos))
            {
                SetPlotState(pos, PlotState.Tilled, tilledTile);
            }
        }

        foreach (var crop in cropsOnPlots.Values)
        {
            crop.Grow();
        }
        MarkDataDirty();
    }

    public PlotState GetPlotState(Vector3Int position)
    {
        if (plotStates.TryGetValue(position, out PlotState state))
        {
            return state;
        }
            return PlotState.None; 
    }

    public void SetPlotState(Vector3Int position, PlotState state, Tile tile)
    {
        tillableTilemap.SetTile(position, tile);
        plotStates[position] = state;
    }

    public bool IsTillableArea(Vector3Int position)
    {
        return tillableTilemap.HasTile(position); 
    }
    private bool HandleFertilizerInteraction(Vector3Int plotPosition, FertilizerData fertilizer)
    {
        if (cropsOnPlots.TryGetValue(plotPosition, out Crop crop))
        {
            bool success = crop.ApplyFertilizer(fertilizer.minMultiplier, fertilizer.maxMultiplier);
            
            if (success)
            {
                MarkDataDirty();
                return true;
            }
        }
        return false;
    }
}