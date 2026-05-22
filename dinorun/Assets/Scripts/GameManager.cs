using System.Collections; // BẮT BUỘC PHẢI CÓ DÒNG NÀY ĐỂ CHẠY ĐẾM NGƯỢC
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // BẮT BUỘC PHẢI CÓ DÒNG NÀY ĐỂ XÀI IMAGE
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
    public AudioSource uiAudioSource; // Loa phát tiếng đếm ngược

    [Header("Hệ thống Đếm ngược (MỚI)")]
    public Image countdownImage; // Nơi hiển thị hình ảnh số
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
        if (countdownImage != null) countdownImage.gameObject.SetActive(false); // Ẩn số lúc đầu

        if (skipStartMenu)
        {
            // NẾU BẤM RETRY: Bỏ qua Menu nhưng VẪN PHẢI ĐẾM NGƯỢC
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
            StartCoroutine(CountdownRoutine());
        }
        else
        {
            // LẦN ĐẦU VÀO GAME: Hiện Menu chờ bấm Start
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

    // GẮN VÀO NÚT START
    public void StartGame()
    {
        if (startMenuPanel != null) startMenuPanel.SetActive(false);

        // GỌI HÀM ĐẾM NGƯỢC THAY VÌ CHẠY GAME LUÔN
        StartCoroutine(CountdownRoutine());
    }

    // --- HÀM XỬ LÝ ĐẾM NGƯỢC 3-2-1-GO ---
    IEnumerator CountdownRoutine()
    {
        Time.timeScale = 0f; // Đóng băng mọi thứ trong lúc đếm
        if (countdownImage != null) countdownImage.gameObject.SetActive(true); // Hiện bảng số

        // 1. Số 3
        if (countdownImage != null) countdownImage.sprite = sprite3;
        if (uiAudioSource != null && sound3 != null) uiAudioSource.PlayOneShot(sound3);
        yield return new WaitForSecondsRealtime(1f); // Chờ 1 giây thời gian thực

        // 2. Số 2
        if (countdownImage != null) countdownImage.sprite = sprite2;
        if (uiAudioSource != null && sound2 != null) uiAudioSource.PlayOneShot(sound2);
        yield return new WaitForSecondsRealtime(1f);

        // 3. Số 1
        if (countdownImage != null) countdownImage.sprite = sprite1;
        if (uiAudioSource != null && sound1 != null) uiAudioSource.PlayOneShot(sound1);
        yield return new WaitForSecondsRealtime(1f);

        // 4. GO
        if (uiAudioSource != null && soundGo != null) uiAudioSource.PlayOneShot(soundGo);
        if (countdownImage != null) countdownImage.gameObject.SetActive(false); // Ẩn số đi

        // 5. BẮT ĐẦU GAME CHÍNH THỨC
        isGameStarted = true;
        Time.timeScale = 1f; // Mở khóa thời gian
        if (bgMusic != null) bgMusic.Play(); // Bật nhạc nền
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        if (bgMusic != null) bgMusic.Stop();
        CheckAndSaveHighScore();
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        skipStartMenu = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

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
