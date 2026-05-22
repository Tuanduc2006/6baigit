using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float baseSpawnRate = 2f;  // Thời gian (giây) mặc định giữa 2 lần sinh ống
    private float timer = 0f;
    public float heightOffset = 2.5f;

    void Update()
    {
        if (!GameManager.instance.gameStarted) return;

        // Tính thời gian chờ thực tế (Game càng nhanh, chờ càng ít)
        float currentSpawnRate = baseSpawnRate / GameManager.instance.globalSpeed;

        // Giới hạn để ống đẻ ra không bị dính sát vào nhau (Nhanh nhất là 0.8 giây 1 ống)
        if (currentSpawnRate < 0.8f) currentSpawnRate = 0.8f;

        if (timer < currentSpawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            SpawnPipe();
            timer = 0f;
        }
    }

    void SpawnPipe()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;

        float randomY = Random.Range(lowestPoint, highestPoint);

        Vector3 spawnPosition = new Vector3(transform.position.x, randomY, 0);
        Instantiate(pipePrefab, spawnPosition, Quaternion.identity);
    }
}