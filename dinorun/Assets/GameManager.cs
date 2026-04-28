using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float speed = 5f;
    public float increaseSpeed = 0.5f;
    public float timeToIncrease = 5f;

    private float timer = 0f;

    public static GameManager instance;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timeToIncrease)
        {
            speed += increaseSpeed;
            timer = 0f;

            Debug.Log("Speed: " + speed);
        }
    }

    // ======================
    // HÀM RESTART GAME
    // ======================
    public void RestartGame()
    {
        Time.timeScale = 1f; // cho game chạy lại
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
