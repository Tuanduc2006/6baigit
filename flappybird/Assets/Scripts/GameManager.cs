using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Waiting,
    Playing,
    GameOver
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameState state;

    public GameObject readyUI;
    public GameObject gameOverUI;
    public PipeSpawner pipeSpawner;
    public BirdController bird;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        EnterWaiting();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
    }

    void HandleEscape()
    {
        Application.Quit();
    }

    void EnterWaiting()
    {
        state = GameState.Waiting;

        readyUI.SetActive(true);
        gameOverUI.SetActive(false);

        pipeSpawner.enabled = false;

        CleanupPipes();     // 🔥 dọn pipe nếu còn sót
        bird.ResetBird();
    }

    public void StartGame()
    {
        if (state != GameState.Waiting) return;

        state = GameState.Playing;

        readyUI.SetActive(false);
        gameOverUI.SetActive(false);

        pipeSpawner.enabled = true;
        bird.EnableControl();
    }

    public void GameOver()
    {
        if (state != GameState.Playing) return;

        state = GameState.GameOver;

        gameOverUI.SetActive(true);
        pipeSpawner.enabled = false;
        bird.DisableControl();

        CleanupPipes();     // 🔥 dọn sạch pipe khi thua
    }

    void CleanupPipes()
    {
        Pipe[] pipes = FindObjectsOfType<Pipe>();
        foreach (Pipe pipe in pipes)
        {
            Destroy(pipe.gameObject);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
