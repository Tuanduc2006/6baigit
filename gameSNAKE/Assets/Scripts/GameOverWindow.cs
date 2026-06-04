using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * GameOverWindow.cs
 * Tác dụng: Quản lý màn hình Game Over.
 * - Hiện/ẩn bảng Game Over.
 * - Nút Retry để chơi lại.
 * - Nhập tên người chơi và lưu điểm vào bảng xếp hạng.
 */
public class GameOverWindow : MonoBehaviour
{
    private static GameOverWindow instance;

    public TMP_InputField nameInputField;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Button retryButton = null;
        Transform retryTransform = transform.Find("retryBtn");
        if (retryTransform != null)
        {
            retryButton = retryTransform.GetComponent<Button>();
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(RetryButtonClicked);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy nút retryBtn trong GameOverWindow.");
        }

        Hide();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void RetryButtonClicked()
    {
        Time.timeScale = 1f;
        Loader.Load(Loader.Scene.Game);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public static void ShowStatic()
    {
        if (instance != null)
        {
            instance.Show();
        }
        else
        {
            Debug.LogWarning("Không tìm thấy GameOverWindow trong scene.");
        }
    }

    public void SubmitScore()
    {
        string playerNameToLeaderboard = "---";

        if (nameInputField != null)
        {
            nameInputField.image.color = Color.red;

            if (!string.IsNullOrWhiteSpace(nameInputField.text))
            {
                playerNameToLeaderboard = nameInputField.text;
            }
        }

        int scoreToLeaderboard = Mathf.RoundToInt(GameLogic.GetScore());
        HighScoreTable.AddHighscoreEntry(scoreToLeaderboard, playerNameToLeaderboard);
    }
}
