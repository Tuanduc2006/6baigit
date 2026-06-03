using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Trạng thái Game")]
    public bool gameStarted = false;
    public static bool isRestart = false;
    public int score = 0;
    private int bestScore = 0;

    [Header("Độ khó động")]
    public float globalSpeed = 1f;
    public float speedIncreaseRate = 0.02f;

    [Header("Giao diện UI")]
    public GameObject startPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI inGameScoreText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalBestText;

    [Header("Hệ thống Combo")]
    public GameObject comboContainer; // Cục chứa ảnh COMBO
    public TextMeshProUGUI comboText; // Chữ số hiển thị (x1, x2...)

    [Header("Âm thanh & Nút Bật/Tắt")]
    public AudioSource sfxAudioSource;
    public AudioSource bgmAudioSource;

    public AudioClip flapSound;
    public AudioClip scoreSound;
    public AudioClip hitSound;
    public AudioClip gameOverSound;

    public Image audioButtonImage;
    public Sprite soundOnSprite;
    public Sprite soundOffSprite;
    private bool isMuted = false;

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
        globalSpeed = 1f;

        // Ẩn hệ thống Combo lúc mới vào game
        if (comboContainer != null) comboContainer.SetActive(false);

        bestScore = PlayerPrefs.GetInt("DiemCaoNhat", 0);
        inGameScoreText.text = score.ToString();

        isMuted = PlayerPrefs.GetInt("MuteState", 0) == 1;
        AudioListener.volume = isMuted ? 0f : 1f;

        if (audioButtonImage != null)
        {
            audioButtonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        }

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
            bgmAudioSource.Stop();
        }
    }

    void Update()
    {
        if (gameStarted && Time.timeScale > 0)
        {
            if (globalSpeed < 3f)
            {
                globalSpeed += speedIncreaseRate * Time.deltaTime;
            }
        }
    }

    public void ToggleAudio()
    {
        isMuted = !isMuted;
        AudioListener.volume = isMuted ? 0f : 1f;
        audioButtonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
        PlayerPrefs.SetInt("MuteState", isMuted ? 1 : 0);
    }

    public void StartGame()
    {
        gameStarted = true;
        startPanel.SetActive(false);
        inGameScoreText.gameObject.SetActive(true);
        bgmAudioSource.Play();
    }

    public void RestartGame()
    {
        isRestart = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Home()
    {
        isRestart = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayFlapSound() { sfxAudioSource.PlayOneShot(flapSound); }

    public void AddScore()
    {
        score++;
        inGameScoreText.text = score.ToString();
        sfxAudioSource.PlayOneShot(scoreSound);

        // --- XỬ LÝ HỆ THỐNG COMBO ---
        // Chia lấy phần nguyên (Ví dụ: 5/5 = 1, 9/5 = 1, 10/5 = 2)
        int combo = score / 5;

        if (combo > 0)
        {
            // Bật hình Combo lên nếu nó đang bị ẩn
            if (comboContainer != null && !comboContainer.activeSelf)
            {
                comboContainer.SetActive(true);
            }

            // Cập nhật con số hiển thị
            if (comboText != null)
            {
                comboText.text = "x" + combo.ToString();
            }
        }
    }

    public void GameOver()
    {
        sfxAudioSource.PlayOneShot(hitSound);
        sfxAudioSource.PlayOneShot(gameOverSound);
        bgmAudioSource.Stop();

        // Ẩn bảng Combo và Điểm lúc Game Over cho đỡ rối
        if (comboContainer != null) comboContainer.SetActive(false);
        inGameScoreText.gameObject.SetActive(false);

        gameOverPanel.SetActive(true);
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