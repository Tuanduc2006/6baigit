using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject[] prefabs;
    public GameObject gameOverPanel;

    // Thêm một "cái khóa" để kiểm soát trạng thái game
    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1;
        SpawnNext();
    }

    public void SpawnNext()
    {
        // KẾT THÚC VÒNG LẶP: Nếu đã thua thì quay xe luôn, KHÔNG sinh thêm gạch!
        if (isGameOver) return;

        int randomIndex = Random.Range(0, prefabs.Length);

        GameObject nextTetromino = Instantiate(prefabs[randomIndex], new Vector3(4, 18, 0), Quaternion.identity);

        // Nếu khối gạch vừa sinh ra đã chạm vào gạch cũ
        if (!nextTetromino.GetComponent<Tetromino>().IsValidPosition())
        {
            // TẮT NGAY TỨC KHẮC quyền điều khiển của viên gạch này để nó không gọi Update() nữa
            nextTetromino.GetComponent<Tetromino>().enabled = false;

            // Kích hoạt hàm thua cuộc
            GameOver();
        }
    }

    void GameOver()
    {
        isGameOver = true; // Sập cầu dao! Khóa không cho sinh gạch nữa

        Debug.Log("GAME OVER! Gạch đã chạm đỉnh!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Time.timeScale = 0;
    }
}