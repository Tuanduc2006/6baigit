using UnityEngine;

/*
 * GameLogic.cs
 * Tác dụng:
 * - Quản lý logic tổng của game Snake.
 * - Tạo bàn chơi 20x20.
 * - Kết nối Snake với LevelGrid.
 * - Quản lý điểm số.
 * - Quản lý tạm dừng / tiếp tục game bằng phím ESC.
 * - Gọi màn hình Game Over khi rắn chết.
 * - Điều khiển nhạc nền khi bắt đầu game và khi game over.
 */
public class GameLogic : MonoBehaviour
{
    private static GameLogic instance;
    private static int score;

    [SerializeField] private Snake snake;

    private LevelGrid levelGrid;

    private void Awake()
    {
        instance = this;
        InitializeStatic();
    }

    private void Start()
    {
        Time.timeScale = 1f;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PlayMusicFromBeginning();
        }

        if (snake == null)
        {
            snake = FindFirstObjectByType<Snake>();
        }

        if (snake == null)
        {
            Debug.LogError("Chưa gán Snake cho GameLogic. Hãy kéo GameObject Snake vào ô Snake trong Inspector.");
            enabled = false;
            return;
        }

        levelGrid = new LevelGrid(20, 20);
        snake.Setup(levelGrid);
        levelGrid.Setup(snake);
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused())
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private static void InitializeStatic()
    {
        score = 0;
    }

    public static int GetScore()
    {
        return score;
    }

    public static void AddScore()
    {
        score += 10;
    }

    public static void SnakeDied()
    {
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.StopMusic();
        }

        GameOverWindow.ShowStatic();
    }

    public static void ResumeGame()
    {
        PauseWindow.HideStatic();
        Time.timeScale = 1f;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.ResumeMusic();
        }
    }

    public static void PauseGame()
    {
        PauseWindow.ShowStatic();
        Time.timeScale = 0f;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.PauseMusic();
        }
    }

    public static bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }
}
