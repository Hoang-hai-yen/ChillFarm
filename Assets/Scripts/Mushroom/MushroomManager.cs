using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomRegion : MonoBehaviour
{
    public GameObject mushroomPrefab;
    public List<MushroomData> mushroomList;
    public BoxCollider2D spawnRegion;
    public int maxMushroomCount = 5;

    private void OnEnable()
    {
        // Đăng ký nhận tin khi trời mưa
        WeatherManager.OnRainStarted += HandleRainStarted;
    }

    private void OnDisable()
    {
        // Hủy đăng ký để tránh lỗi bộ nhớ
        WeatherManager.OnRainStarted -= HandleRainStarted;
    }

    private void Start()
    {
        // Lần đầu vào game, nấm mọc sẵn
        SpawnMissingMushrooms();
    }

    void HandleRainStarted()
    {
        // Khi nghe thấy tín hiệu mưa, bắt đầu mọc nấm sau một khoảng trễ ngắn cho tự nhiên
        StartCoroutine(DelayedRegrow());
    }

    IEnumerator DelayedRegrow()
    {
        // Chờ 1-2 giây sau khi mưa rơi thì nấm mới bắt đầu nhú lên
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        SpawnMissingMushrooms();
    }

    void SpawnMissingMushrooms()
    {
        int currentCount = transform.childCount;
        int needToSpawn = maxMushroomCount - currentCount;
        Bounds bounds = spawnRegion.bounds;

        for (int i = 0; i < needToSpawn; i++)
        {
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y)
            );

            // Tạo nấm lần lượt (đã thêm delay mọc từng cây cho đẹp)
            StartCoroutine(GrowOneMushroom(randomPos));
        }
    }

    IEnumerator GrowOneMushroom(Vector2 pos)
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 1.5f));
        GameObject newMush = Instantiate(mushroomPrefab, pos, Quaternion.identity, transform);
        
        MushroomData randomData = mushroomList[UnityEngine.Random.Range(0, mushroomList.Count)];
        newMush.GetComponent<Mushroom>().Init(randomData);
    }
}