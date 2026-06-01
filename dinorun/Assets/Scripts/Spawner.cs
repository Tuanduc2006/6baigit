using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] obstacles; // Mảng chứa các chướng ngại vật
    public float minSpawnTime = 1.5f;
    public float maxSpawnTime = 3f;
    private float timer;

    [Header("Cấu hình độ cao cho Chim")]
    public float birdLowY = 0.8f;
    public float birdHighY = 1.8f;

    [Header("Cấu hình Độ khó")]
    public float scoreToSpawnBird = 400f;

    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        timer = Random.Range(minSpawnTime, maxSpawnTime);
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            SpawnObstacle();
            timer = Random.Range(minSpawnTime, maxSpawnTime);
        }
    }

    void SpawnObstacle()
    {
        int randomIndex;

        if (gameManager != null && gameManager.score >= scoreToSpawnBird)
        {
            // Nếu đủ điểm: Random thả ga tất cả mọi thứ (Xương rồng 1, 2, Chim...)
            randomIndex = Random.Range(0, obstacles.Length);
        }
        else
        {
            // Nếu CHƯA đủ 400 điểm: Phải loại con chim ra
            // Vòng lặp này sẽ random liên tục cho đến khi trúng 1 vật KHÔNG CÓ CHỮ "bird" trong tên
            do
            {
                randomIndex = Random.Range(0, obstacles.Length);
            }
            while (obstacles[randomIndex].name.ToLower().Contains("bird"));
        }

        GameObject obstacleToSpawn = obstacles[randomIndex];
        Vector3 spawnPosition = transform.position;

        // Nếu vật được chọn là Chim thì mới tính toán độ cao
        if (obstacleToSpawn.name.ToLower().Contains("bird"))
        {
            float chosenY = (Random.value > 0.5f) ? birdHighY : birdLowY;
            spawnPosition = new Vector3(transform.position.x, transform.position.y + chosenY, transform.position.z);
        }

        Instantiate(obstacleToSpawn, spawnPosition, Quaternion.identity);
    }
}
