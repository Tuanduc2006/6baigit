using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Giao dien UI")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;

    [Header("He thong Diem & Toc Do")]
    public float score;
    public float scoreMultiplier = 10f;
    public float gameSpeed = 1f;
    public float speedIncreaseRate = 0.02f;
    public float maxGameSpeed = 3f;

    [Header("Trang thai Game")]
    public bool isGameStarted = false;
    private bool isGameOver = false;
    public static bool skipStartMenu = false;

    [Header("Âm thanh")]
    public AudioSource bgMusic;
    public AudioSource uiAudioSource;

    [Header("Hệ thống Đếm ngược")]
    public Image countdownImage;
    public Sprite sprite3;
    public Sprite sprite2;
    public Sprite sprite1;
    public AudioClip sound3;
    public AudioClip sound2;
    public AudioClip sound1;
    public AudioClip soundGo;

    void Start()
    {
        isGameOver = false;
        score = 0;
        gameSpeed = 1f;

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (countdownImage != null) countdownImage.gameObject.SetActive(false);

        if (skipStartMenu)
        {
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
            StartCoroutine(CountdownRoutine());
        }
        else
        {
            isGameStarted = false;
            if (startMenuPanel != null) startMenuPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        UpdateHighScoreDisplay();
    }

    void Update()
    {
        if (isGameStarted && !isGameOver)
        {
            score += Time.deltaTime * scoreMultiplier;
            if (scoreText != null) scoreText.text = "DIEM: " + Mathf.FloorToInt(score).ToString();

            if (gameSpeed < maxGameSpeed)
            {
                gameSpeed += speedIncreaseRate * Time.deltaTime;
            }
        }
    }

    public void StartGame()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        Time.timeScale = 0f;
        if (countdownImage != null) countdownImage.gameObject.SetActive(true);

        if (countdownImage != null) countdownImage.sprite = sprite3;
        if (uiAudioSource != null && sound3 != null) uiAudioSource.PlayOneShot(sound3);
        yield return new WaitForSecondsRealtime(1f);

        if (countdownImage != null) countdownImage.sprite = sprite2;
        if (uiAudioSource != null && sound2 != null) uiAudioSource.PlayOneShot(sound2);
        yield return new WaitForSecondsRealtime(1f);

        if (countdownImage != null) countdownImage.sprite = sprite1;
        if (uiAudioSource != null && sound1 != null) uiAudioSource.PlayOneShot(sound1);
        yield return new WaitForSecondsRealtime(1f);

        if (uiAudioSource != null && soundGo != null) uiAudioSource.PlayOneShot(soundGo);
        if (countdownImage != null) countdownImage.gameObject.SetActive(false);

        isGameStarted = true;
        Time.timeScale = 1f;
        if (bgMusic != null) bgMusic.Play();
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (bgMusic != null) bgMusic.Stop();
        CheckAndSaveHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    // Nút RETRY gọi hàm này
    public void RestartGame()
    {
        skipStartMenu = true; // Bỏ qua menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // --- MỚI: HÀM GẮN VÀO NÚT HOME ---
    public void ReturnToHome()
    {
        skipStartMenu = false; // BẮT BUỘC hiện lại Menu
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Nút EXIT gọi hàm này
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void CheckAndSaveHighScore()
    {
        int currentScoreInt = Mathf.FloorToInt(score);
        int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScoreInt > savedHighScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScoreInt);
            PlayerPrefs.Save();
            UpdateHighScoreDisplay();
        }
    }

    private void UpdateHighScoreDisplay()
    {
        if (highScoreText != null)
        {
            int savedHighScore = PlayerPrefs.GetInt("HighScore", 0);
            highScoreText.text = "LS: " + savedHighScore.ToString();
        }
    }
}