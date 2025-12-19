using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Assets.Scripts.Cloud.Schemas; // Namespace chứa schema của bạn

public class FishingController : MonoBehaviour
{
    public enum FishingState { Idle, Casting, Waiting, Bite, Reeling, Caught }

    //[Header("Settings")]
    //public float minWaitTime = 2f;
    //public float maxWaitTime = 5f;
    //public float biteWindow = 1.5f;

    //// Thay vì ScriptableObject, ta dùng List Class thuần
    //// List này sẽ được "bơm" dữ liệu từ GameConfigManager (JSON Cloud)
    //[Header("Runtime Config")]
    //public List<FishConfigData> currentMapFishList;

    private FishingState currentState = FishingState.Idle;

    //// Reference to global Data Manager
    //private GameDataManager dataManager;

    void Start()
    {
    //    dataManager = FindObjectOfType<GameDataManager>();

    //    // MOCK DATA: Nếu chưa có cloud, tự tạo data giả để test
    //    if (currentMapFishList == null || currentMapFishList.Count == 0)
    //    {
    //        currentMapFishList = new List<FishConfigData>
    //        {
    //            new FishConfigData { id = "fish_carp", name = "Carp", rarityWeight = 50, minSize = 10, maxSize = 30 },
    //            new FishConfigData { id = "fish_shark", name = "Shark", rarityWeight = 5, minSize = 100, maxSize = 200 }
    //        };
    //    }
    }

    //void Update()
    //{
    //    // Spacebar to fish
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        HandleInput();
    //    }
    //}

    //void HandleInput()
    //{
    //    switch (currentState)
    //    {
    //        case FishingState.Idle:
    //            StartCoroutine(CastRodRoutine());
    //            break;

    //        case FishingState.Waiting:
    //            Debug.Log("Pulled too early! Fish scared away.");
    //            CancelFishing();
    //            break;

    //        case FishingState.Bite:
    //            Debug.Log("Hooked! Caught the fish.");
    //            CatchFish();
    //            break;
    //    }
    //}

    //IEnumerator CastRodRoutine()
    //{
    //    currentState = FishingState.Casting;
    //    Debug.Log("Casting rod...");
    //    yield return new WaitForSeconds(1f);

    //    currentState = FishingState.Waiting;
    //    Debug.Log("Waiting for bite...");

    //    float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
    //    yield return new WaitForSeconds(waitTime);

    //    if (currentState == FishingState.Waiting)
    //    {
    //        currentState = FishingState.Bite;
    //        Debug.Log("!!! BITE !!! Press Space!");

    //        yield return new WaitForSeconds(biteWindow);

    //        if (currentState == FishingState.Bite)
    //        {
    //            Debug.Log("Fish escaped...");
    //            CancelFishing();
    //        }
    //    }
    //}

    //void CatchFish()
    //{
    //    currentState = FishingState.Caught;

    //    // 1. Random Fish based on Config (Weight)
    //    FishConfigData config = GetRandomFishConfig();

    //    // 2. Random Size
    //    float sizeVal = UnityEngine.Random.Range(config.minSize, config.maxSize);
    //    string sizeStr = sizeVal.ToString("F2"); // Format: "12.55"

    //    Debug.Log($"You caught a {config.name} (Size: {sizeStr})!");

    //    // 3. Update Local Data Schema
    //    if (dataManager != null && dataManager.FishingData != null)
    //    {
    //        UpdateFishingSchema(dataManager.FishingData, config, sizeStr);
    //    }
    //    else
    //    {
    //        Debug.LogError("GameDataManager or FishingData is null! Cannot save.");
    //    }

    //    ResetFishing();
    //}

    //// --- LOGIC MAPPING SCHEMA ---
    //void UpdateFishingSchema(Fishing fishingData, FishConfigData config, string sizeStr)
    //{
    //    // A. Tạo object Fish mới theo Schema
    //    Assets.Scripts.Cloud.Schemas.Fish newFish = new Assets.Scripts.Cloud.Schemas.Fish
    //    {
    //        FishId = config.id,
    //        Species = config.name,
    //        AddedAt = DateTime.UtcNow,
    //        Size = sizeStr,
    //        CanCatch = true
    //    };

    //    // B. Update FishingStats (TotalCaught)
    //    fishingData.Stats.TotalCaught++;

    //    // C. Update CaughtFishes List (Inventory Logic)
    //    // Tìm xem đã từng bắt loại cá này chưa?
    //    var existingRecord = fishingData.Stats.CaughtFishes
    //        .FirstOrDefault(x => x.Fish.FishId == config.id);

    //    if (existingRecord != null)
    //    {
    //        // Nếu có rồi -> Tăng số lượng
    //        existingRecord.Quantity++;
    //        Debug.Log($"Inventory Updated: {config.name} (Quantity: {existingRecord.Quantity})");
    //    }
    //    else
    //    {
    //        // Nếu chưa -> Tạo record mới
    //        Fishing.FishingStats.CaughtFish newRecord = new Fishing.FishingStats.CaughtFish
    //        {
    //            Fish = newFish, // Lưu thông tin con cá vừa bắt làm đại diện
    //            Quantity = 1
    //        };
    //        fishingData.Stats.CaughtFishes.Add(newRecord);
    //        Debug.Log($"New Species Registered: {config.name}");
    //    }

    //    // D. Update Pond (Optional)
    //    // Nếu hồ đã mở khóa và còn chỗ -> Thả cá vào hồ
    //    if (fishingData.Pond.IsUnlocked)
    //    {
    //        if (fishingData.Pond.Fish.Count < fishingData.Pond.MaxCapacity)
    //        {
    //            fishingData.Pond.Fish.Add(newFish);
    //            Debug.Log("Fish added to Pond.");
    //        }
    //        else
    //        {
    //            Debug.Log("Pond is full.");
    //        }
    //    }
    //}

    //void CancelFishing()
    //{
    //    ResetFishing();
    //}

    //void ResetFishing()
    //{
    //    currentState = FishingState.Idle;
    //}

    //// Thuật toán chọn cá theo tỉ lệ (Weighted Random)
    //FishConfigData GetRandomFishConfig()
    //{
    //    if (currentMapFishList == null || currentMapFishList.Count == 0) return null;

    //    float totalWeight = currentMapFishList.Sum(f => f.rarityWeight);
    //    float randomValue = UnityEngine.Random.Range(0, totalWeight);
    //    float currentWeight = 0;

    //    foreach (var fish in currentMapFishList)
    //    {
    //        currentWeight += fish.rarityWeight;
    //        if (randomValue <= currentWeight) return fish;
    //    }
    //    return currentMapFishList[0];
    //}
}