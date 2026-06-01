using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // THÊM THƯ VIỆN NÀY ĐỂ ĐỔI ẢNH NÚT BẤM

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

    [Header("Âm thanh & Nút Bật/Tắt")]
    public AudioSource audioSource;
    public AudioClip flapSound;
    public AudioClip scoreSound;
    public AudioClip hitSound;
    public AudioClip gameOverSound;

    // Các biến mới cho nút Âm thanh
    public Image audioButtonImage; // Nơi chứa Component Image của nút Audio
    public Sprite soundOnSprite;   // Ảnh cái loa bật
    public Sprite soundOffSprite;  // Ảnh cái loa tắt (có dấu gạch)
    private bool isMuted = false;  // Biến lưu trí nhớ xem đang bật hay tắt

    void Awake()
    {
        if (instance == null) { instance = this; }
    }

    void Start()
    {
        Time.timeScale = 1f;
        gameOverPanel.SetActive(false);
        globalSpeed = 1f;

        bestScore = PlayerPrefs.GetInt("DiemCaoNhat", 0);
        inGameScoreText.text = score.ToString();

        // --- KHỞI TẠO TRẠNG THÁI ÂM THANH ---
        // Lấy trí nhớ từ máy (0 là bật, 1 là tắt)
        isMuted = PlayerPrefs.GetInt("MuteState", 0) == 1;

        // Điều khiển âm lượng tổng của cả game
        AudioListener.volume = isMuted ? 0f : 1f;

        // Cập nhật đúng ảnh cho cái nút
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

    // --- HÀM MỚI: BẤM NÚT ĐỂ ĐẢO TRẠNG THÁI ÂM THANH ---
    public void ToggleAudio()
    {
        isMuted = !isMuted; // Đảo ngược trạng thái (đang tắt thì thành bật, đang bật thì thành tắt)

        AudioListener.volume = isMuted ? 0f : 1f; // Tắt/bật âm thanh tổng

        // Đổi ảnh tương ứng
        audioButtonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;

        // Lưu lại trí nhớ vào máy
        PlayerPrefs.SetInt("MuteState", isMuted ? 1 : 0);
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

    public void Home()
    {
        isRestart = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayFlapSound() { audioSource.PlayOneShot(flapSound); }

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