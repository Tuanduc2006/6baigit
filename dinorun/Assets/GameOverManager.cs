using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Panel chua nut Restart

    void Start()
    {
        gameOverPanel.SetActive(false); // An luc dau
    }

    // Goi khi chet
    public void GameOver()
    {
        Time.timeScale = 0f; // Dung game
        gameOverPanel.SetActive(true); // Hien nut
    }
}
