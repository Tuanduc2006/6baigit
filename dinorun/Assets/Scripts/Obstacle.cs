using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float baseSpeed = 5f; // Tốc độ gốc
    private GameManager gameManager;

    void Start()
    {
        // Tự động tìm GameManager trong màn hình
        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        float currentSpeed = baseSpeed;

        // Nhân tốc độ gốc với tốc độ đang tăng dần của GameManager
        if (gameManager != null)
        {
            currentSpeed = baseSpeed * gameManager.gameSpeed;
        }

        transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);

        if (transform.position.x < -15f)
        {
            Destroy(gameObject);
        }
    }
}
