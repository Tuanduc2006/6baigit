using UnityEngine;

public class PipeSpawner : MonoBehaviour
{
    public GameObject pipePrefab;
    public float spawnInterval = 2f;
    public float minY = -1.5f;
    public float maxY = 1.5f;

    float timer;
    float spawnX;

    void Start()
    {
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        spawnX = camHalfWidth + 2f;
    }

    void OnEnable()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        if (GameManager.instance.state != GameState.Playing) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnPipe();
            timer = spawnInterval;
        }
    }

    void SpawnPipe()
    {
        float y = Random.Range(minY, maxY);
        Instantiate(pipePrefab, new Vector3(spawnX, y, 0f), Quaternion.identity);
    }
}
