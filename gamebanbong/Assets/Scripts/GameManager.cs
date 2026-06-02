using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Script quản lý toàn bộ game
// Bao gồm: điểm, level, số lượt bắn, thắng, thua, chơi lại, âm thanh và UI
public class GameManager : MonoBehaviour
{
    [Header("Liên kết")]
    // Tham chiếu tới script quản lý lưới bóng
    public BubbleGrid grid;

    // Tham chiếu tới script bắn bóng
    public Shooter shooter;

    [Header("UI Text")]
    // Text hiển thị điểm
    public TMP_Text scoreText;

    // Text hiển thị level hiện tại
    public TMP_Text levelText;

    // Text hiển thị số bóng/lượt bắn còn lại
    public TMP_Text shotsText;

    // Text hiển thị thông báo như GAME OVER hoặc VICTORY
    public TMP_Text messageText;

    [Header("Panel kết thúc")]
    // Panel hiện khi thua game
    public GameObject gameOverPanel;

    // Panel hiện khi thắng game
    public GameObject victoryPanel;

    [Header("Thông số game")]
    // Số lượt bắn ban đầu
    public int startShots = 50;

    // Đạt số điểm này thì thắng
    public int victoryScore = 2000;

    // Mỗi quả bóng biến mất được cộng bao nhiêu điểm
    public int pointPerBubble = 100;

    // Sau bao nhiêu giây thì thêm một hàng bóng mới
    public float dropRowInterval = 25f;

    [Header("Âm thanh")]
    // AudioSource dùng để phát âm thanh
    public AudioSource audioSource;

    // Âm thanh khi thắng
    public AudioClip victoryClip;

    // Âm thanh khi thua
    public AudioClip gameOverClip;

    // Điểm hiện tại
    private int score;

    // Level hiện tại
    private int level;

    // Số lượt bắn còn lại
    private int shotsLeft;

    // Combo hiện tại
    // Hiện tại biến này chưa được dùng nhiều trong code
    private int combo;

    // Kiểm tra game đã kết thúc chưa
    private bool gameEnded;

    // Cho script khác đọc trạng thái game đã kết thúc chưa
    public bool IsGameEnded => gameEnded;

    private void Awake()
    {
        // Đảm bảo thời gian game chạy bình thường khi vào màn
        Time.timeScale = 1f;

        // Tự tìm các object còn thiếu nếu chưa kéo vào Inspector
        FindMissingReferences();
    }

    private void Start()
    {
        // Level bắt đầu từ 1
        level = 1;

        // Điểm bắt đầu bằng 0
        score = 0;

        // Combo ban đầu bằng 0
        combo = 0;

        // Game chưa kết thúc
        gameEnded = false;

        // Xóa điểm và level cũ trong PlayerPrefs nếu có
        // Việc này giúp khi chạy lại game thì điểm về 0
        PlayerPrefs.DeleteKey("Score");
        PlayerPrefs.DeleteKey("CurrentLevel");
        PlayerPrefs.Save();

        // Bắt đầu màn chơi
        StartLevel();
    }

    // Tự động tìm các object cần thiết nếu bạn quên kéo trong Inspector
    private void FindMissingReferences()
    {
        // Nếu chưa gắn BubbleGrid thì tự tìm trong Scene
        if (grid == null)
        {
            grid = FindFirstObjectByType<BubbleGrid>();
        }

        // Nếu chưa gắn Shooter thì tự tìm trong Scene
        if (shooter == null)
        {
            shooter = FindFirstObjectByType<Shooter>();
        }

        // Nếu chưa gắn AudioSource thì lấy AudioSource trên chính GameManager
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Nếu chưa gắn scoreText thì tự tìm theo tên ScoreText hoặc trong Panel_Diem
        if (scoreText == null)
        {
            scoreText = FindTextByNameOrPanel("ScoreText", "Panel_Diem");
        }

        // Nếu chưa gắn levelText thì tự tìm theo tên LevelText hoặc trong Panel_Level
        if (levelText == null)
        {
            levelText = FindTextByNameOrPanel("LevelText", "Panel_Level");
        }

        // Nếu chưa gắn shotsText thì tự tìm theo tên ShotsText hoặc trong Panel_Bong
        if (shotsText == null)
        {
            shotsText = FindTextByNameOrPanel("ShotsText", "Panel_Bong");
        }

        // Nếu chưa gắn messageText thì tự tìm theo tên MessageText
        if (messageText == null)
        {
            messageText = FindTextByNameOrPanel("MessageText", null);
        }

        // Nếu chưa gắn panel thua thì tự tìm object tên GameOverPanel
        if (gameOverPanel == null)
        {
            GameObject obj = GameObject.Find("GameOverPanel");
            if (obj != null)
            {
                gameOverPanel = obj;
            }
        }

        // Nếu chưa gắn panel thắng thì tự tìm object tên VictoryPanel
        if (victoryPanel == null)
        {
            GameObject obj = GameObject.Find("VictoryPanel");
            if (obj != null)
            {
                victoryPanel = obj;
            }
        }
    }

    // Hàm tìm TMP_Text theo tên object hoặc tìm trong panel
    private TMP_Text FindTextByNameOrPanel(string textObjectName, string panelName)
    {
        // Tìm object theo tên truyền vào
        GameObject textObj = GameObject.Find(textObjectName);

        // Nếu tìm thấy object thì lấy component TMP_Text
        if (textObj != null)
        {
            TMP_Text text = textObj.GetComponent<TMP_Text>();

            if (text != null)
            {
                return text;
            }
        }

        // Nếu không tìm thấy text theo tên thì tìm trong panel
        if (!string.IsNullOrEmpty(panelName))
        {
            GameObject panelObj = GameObject.Find(panelName);

            if (panelObj != null)
            {
                // Lấy tất cả TMP_Text nằm trong panel
                TMP_Text[] texts = panelObj.GetComponentsInChildren<TMP_Text>(true);

                if (texts != null && texts.Length > 0)
                {
                    foreach (TMP_Text text in texts)
                    {
                        int temp;

                        // Nếu text hiện tại đang là số thì lấy text này
                        // Ví dụ bảng điểm có số 0, 100, 200...
                        if (int.TryParse(text.text, out temp))
                        {
                            return text;
                        }
                    }

                    // Nếu không tìm thấy text dạng số thì lấy text cuối cùng
                    return texts[texts.Length - 1];
                }
            }
        }

        // Không tìm thấy thì trả về null
        return null;
    }

    // Bắt đầu một level mới
    public void StartLevel()
    {
        // Cho game chạy bình thường
        Time.timeScale = 1f;

        // Tìm lại các object bị thiếu
        FindMissingReferences();

        // Reset trạng thái màn chơi
        gameEnded = false;
        combo = 0;
        shotsLeft = startShots;

        // Ẩn panel game over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Ẩn panel victory
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        // Xóa thông báo
        if (messageText != null)
        {
            messageText.text = "";
        }

        // Tạo lưới bóng cho level hiện tại
        if (grid != null)
        {
            grid.CreateLevel(level);
        }
        else
        {
            Debug.LogError("GameManager chưa gắn BubbleGrid.");
        }

        // Reset súng bắn bóng
        if (shooter != null)
        {
            shooter.ResetShooter();
        }
        else
        {
            Debug.LogError("GameManager chưa gắn Shooter.");
        }

        // Cập nhật lại điểm, level, số lượt bắn trên UI
        UpdateUI();

        // Dừng coroutine cũ nếu có
        StopAllCoroutines();

        // Bắt đầu hệ thống tự tăng độ khó
        StartCoroutine(DynamicDifficultyRoutine());
    }

    // Coroutine tự động thêm hàng bóng mới sau một khoảng thời gian
    private IEnumerator DynamicDifficultyRoutine()
    {
        // Nếu thời gian thêm hàng <= 0 thì không chạy
        if (dropRowInterval <= 0)
        {
            yield break;
        }

        // Level càng cao thì thời gian thêm hàng càng ngắn
        // Nhưng không thấp hơn 8 giây lúc đầu
        float interval = Mathf.Max(8f, dropRowInterval - level * 2f);

        // Lặp khi game chưa kết thúc
        while (!gameEnded)
        {
            // Chờ một khoảng thời gian
            yield return new WaitForSeconds(interval);

            // Nếu không có grid thì thoát
            if (grid == null)
            {
                yield break;
            }

            // Thêm một hàng bóng mới ở trên cùng
            // Nếu không thêm được nghĩa là bóng đã xuống quá thấp
            bool canAddRow = grid.AddNewTopRow(level);

            if (!canAddRow)
            {
                // Không thêm được hàng mới thì thua
                GameOver();
                yield break;
            }

            // Sau mỗi lần thêm hàng, thời gian sẽ giảm dần
            // Làm game càng lúc càng khó
            interval = Mathf.Max(6f, interval * 0.97f);
        }
    }

    // Hàm này được gọi mỗi khi người chơi bắn một quả bóng
    public void UseShot()
    {
        // Nếu game đã kết thúc thì không trừ lượt bắn nữa
        if (gameEnded) return;

        // Trừ một lượt bắn
        shotsLeft--;

        // Không cho số lượt bắn nhỏ hơn 0
        if (shotsLeft < 0)
        {
            shotsLeft = 0;
        }

        // Cập nhật UI
        UpdateUI();
    }

    // Cộng điểm dựa trên số bóng bị xóa và số bóng bị rơi
    public void AddScore(int removedCount, int droppedCount)
    {
        // Nếu game đã kết thúc thì không cộng điểm
        if (gameEnded) return;

        // Tổng số bóng biến mất
        int totalBubble = removedCount + droppedCount;

        // Nếu không có bóng nào biến mất thì không cộng
        if (totalBubble <= 0)
        {
            return;
        }

        // Cộng điểm
        score += totalBubble * pointPerBubble;

        // Cập nhật UI
        UpdateUI();

        // Nếu đạt đủ điểm yêu cầu thì thắng
        if (score >= victoryScore)
        {
            Victory();
        }
    }

    // Cộng điểm chỉ theo số lượng bóng
    // Hàm này dùng khi bạn chỉ cần truyền vào tổng số bóng biến mất
    public void AddScoreByBubble(int bubbleCount)
    {
        // Nếu game đã kết thúc thì không cộng điểm
        if (gameEnded) return;

        // Nếu số bóng <= 0 thì không cộng
        if (bubbleCount <= 0)
        {
            return;
        }

        // Cộng điểm theo số bóng
        score += bubbleCount * pointPerBubble;

        // Cập nhật UI
        UpdateUI();

        // Nếu đủ điểm thì thắng
        if (score >= victoryScore)
        {
            Victory();
        }
    }

    // Reset combo về 0
    public void ResetCombo()
    {
        combo = 0;
    }

    // Hàm được gọi sau khi một phát bắn đã xử lý xong
    // removedSomething = true nếu phát bắn đó làm biến mất bóng
    public void OnShotResolved(bool removedSomething)
    {
        // Nếu game đã kết thúc thì không xử lý nữa
        if (gameEnded) return;

        // Nếu không xóa được bóng nào thì reset combo
        if (!removedSomething)
        {
            combo = 0;
        }

        // Nếu đủ điểm thì thắng
        if (score >= victoryScore)
        {
            Victory();
            return;
        }

        // Nếu lưới đã hết bóng thì thắng
        if (grid != null && grid.IsCleared())
        {
            Victory();
            return;
        }

        // Nếu hết lượt bắn thì thua
        if (shotsLeft <= 0)
        {
            GameOver();
            return;
        }

        // Cập nhật UI
        UpdateUI();
    }

    // Xử lý khi người chơi thắng
    public void Victory()
    {
        // Nếu game đã kết thúc rồi thì không xử lý lại
        if (gameEnded) return;

        // Đánh dấu game đã kết thúc
        gameEnded = true;

        // Dừng các coroutine như tự thêm hàng bóng
        StopAllCoroutines();

        // Hiện panel chiến thắng
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(true);
        }

        // Ẩn panel thua
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Hiện chữ VICTORY nếu có messageText
        if (messageText != null)
        {
            messageText.text = "VICTORY!";
        }

        // Phát âm thanh chiến thắng
        if (audioSource != null && victoryClip != null)
        {
            audioSource.PlayOneShot(victoryClip);
        }

        // Cập nhật UI lần cuối
        UpdateUI();

        // Dừng game lại
        Time.timeScale = 0f;
    }

    // Xử lý khi người chơi thua
    public void GameOver()
    {
        // Nếu game đã kết thúc rồi thì không xử lý lại
        if (gameEnded) return;

        // Đánh dấu game đã kết thúc
        gameEnded = true;

        // Dừng các coroutine
        StopAllCoroutines();

        // Hiện panel game over
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Ẩn panel victory
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }

        // Hiện chữ GAME OVER nếu có messageText
        if (messageText != null)
        {
            messageText.text = "GAME OVER";
        }

        // Phát âm thanh thua
        if (audioSource != null && gameOverClip != null)
        {
            audioSource.PlayOneShot(gameOverClip);
        }

        // Cập nhật UI lần cuối
        UpdateUI();

        // Dừng game lại
        Time.timeScale = 0f;
    }

    // Chơi lại màn hiện tại
    public void Retry()
    {
        // Cho thời gian chạy lại bình thường
        Time.timeScale = 1f;

        // Load lại Scene hiện tại
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Sang level tiếp theo
    public void NextLevel()
    {
        // Cho game chạy lại bình thường
        Time.timeScale = 1f;

        // Tăng level
        level++;

        // Reset combo
        combo = 0;

        // Game chưa kết thúc
        gameEnded = false;

        // Bắt đầu level mới
        StartLevel();
    }

    // Quay về màn hình menu chính
    public void BackToMenu()
    {
        // Cho game chạy lại bình thường
        Time.timeScale = 1f;

        // Load Scene tên MainMenu
        SceneManager.LoadScene("MainMenu");
    }

    // Cập nhật điểm, level và số lượt bắn lên UI
    private void UpdateUI()
    {
        // Tìm lại UI nếu bị thiếu
        FindMissingReferences();

        // Cập nhật điểm
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        // Cập nhật level
        if (levelText != null)
        {
            levelText.text = level.ToString();
        }

        // Cập nhật số lượt bắn còn lại
        if (shotsText != null)
        {
            shotsText.text = shotsLeft.ToString();
        }
    }

    // Thoát game
    // Khi chạy trong Unity Editor thì hàm này gần như không thấy tác dụng
    // Khi build ra file game thật thì sẽ thoát ứng dụng
    public void QuitGame()
    {
        Application.Quit();
    }
}