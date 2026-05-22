using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Trạng thái Game")]
    public bool gameStarted = false;
    public static bool isRestart = false;
    public int score = 0;
    private int bestScore = 0;

    [Header("Độ khó động (Dynamic Difficulty)")]
    public float globalSpeed = 1f; // Tốc độ game hiện tại (1 là bình thường)
    public float speedIncreaseRate = 0.02f; // Mỗi giây tốc độ tăng thêm bao nhiêu

    [Header("Giao diện UI")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalBestText;

    [Header("Âm thanh")]
    public AudioSource audioSource;
    public AudioClip flapSound;
    public AudioClip scoreSound;
    public AudioClip hitSound;
    public AudioClip gameOverSound;

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
        globalSpeed = 1f; // Reset độ khó về mức cơ bản khi mới vào ván

        bestScore = PlayerPrefs.GetInt("DiemCaoNhat", 0);
        inGameScoreText.text = score.ToString();

        if (isRestart)
        {
            StartGame();
            isRestart = false;
        }
        else
        {
            gameStarted = false;
            startPanel.SetActive(true);
            inGameScoreText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Nếu game đang chạy, tăng dần tốc độ theo thời gian
        if (gameStarted && Time.timeScale > 0)
        {
            // Giới hạn tốc độ tối đa (ví dụ gấp 3 lần bình thường) để ống không trôi nhanh đến mức xuyên qua chim
            if (globalSpeed < 3f)
            {
                globalSpeed += speedIncreaseRate * Time.deltaTime;
            }
        }
    }

    public void StartGame()
    {
        gameStarted = true;
        startPanel.SetActive(false);
        inGameScoreText.gameObject.SetActive(true);
    }

    public void RestartGame()
    {
        isRestart = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayFlapSound()
    {
        audioSource.PlayOneShot(flapSound);
    }

    public void AddScore()
    {
        score++;
        inGameScoreText.text = score.ToString();
        audioSource.PlayOneShot(scoreSound);
    }

    public void GameOver()
    {
        audioSource.PlayOneShot(hitSound);
        audioSource.PlayOneShot(gameOverSound);

        gameOverPanel.SetActive(true);
        inGameScoreText.gameObject.SetActive(false);
        finalScoreText.text = score.ToString();

        if (score > bestScore)
        {
            bestScore = score;
            PlayerPrefs.SetInt("DiemCaoNhat", bestScore);
        }

        finalBestText.text = bestScore.ToString();
        Time.timeScale = 0f;
    }
}