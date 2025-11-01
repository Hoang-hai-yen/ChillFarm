using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FarmlandManager : MonoBehaviour
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

    public void Interact(Vector3Int plotPosition, ItemData heldItem)
    {
        PlotState currentState = GetPlotState(plotPosition);

        if (heldItem is ToolData tool)
        {
            HandleToolInteraction(plotPosition, currentState, tool.toolType);
        }
        else if (heldItem is SeedData seed)
        {
            HandleSeedInteraction(plotPosition, currentState, seed);
        }
        else if (heldItem == null) 
        {
            HandleHarvest(plotPosition);
        }
    }

    private void HandleToolInteraction(Vector3Int plotPosition, PlotState currentState, ToolType toolType)
    {
        switch (toolType)
        {
            case ToolType.Hoe:
                if (tillableTilemap.HasTile(plotPosition) && !plotStates.ContainsKey(plotPosition))
                {
                    SetPlotState(plotPosition, PlotState.Tilled, tilledTile);
                    Debug.Log("Dùng cuốc cuốc đất!");
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
    }

    private void HandleSeedInteraction(Vector3Int plotPosition, PlotState currentState, SeedData seed)
    {
        if (currentState == PlotState.Tilled || currentState == PlotState.Watered)
        {
            if (!cropsOnPlots.ContainsKey(plotPosition))
            {
                GameObject cropObj = Instantiate(cropPrefab, tillableTilemap.GetCellCenterWorld(plotPosition), Quaternion.identity);
                Crop cropInstance = cropObj.GetComponent<Crop>();
                
                cropInstance.Initialize(seed.cropToPlant);
                cropsOnPlots.Add(plotPosition, cropInstance);
                
                if(currentState == PlotState.Tilled)
                {
                     SetPlotState(plotPosition, PlotState.Watered, wateredTile);
                }

                Debug.Log($"Gieo hạt {seed.cropToPlant.cropName}!");
            }
        }
    }

    private void HandleHarvest(Vector3Int plotPosition)
    {
        if (cropsOnPlots.TryGetValue(plotPosition, out Crop crop))
        {
            if (crop.IsHarvestable())
            {
                crop.Harvest();
                SetPlotState(plotPosition, PlotState.Tilled, tilledTile);
                cropsOnPlots.Remove(plotPosition); 
            }
        }
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
}