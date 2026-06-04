using UnityEngine;
using UnityEngine.UI;

/*
 * PauseWindow.cs
 * Tác dụng: Quản lý bảng tạm dừng game.
 * - Hiển thị khi bấm ESC.
 * - Nút Resume để chơi tiếp.
 * - Nút Main Menu để quay về menu chính.
 */
public class PauseWindow : MonoBehaviour
{
    private static PauseWindow instance;

    private void Awake()
    {
        instance = this;

        RectTransform rectTransform = transform.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
        }

        Button resumeButton = GetChildButton("resumeBtn");
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeButtonClicked);
        }

        Button mainMenuButton = GetChildButton("mainMenuBtn");
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(MainMenuButtonClicked);
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

    private Button GetChildButton(string childName)
    {
        Transform child = transform.Find(childName);
        if (child == null)
        {
            Debug.LogWarning("Không tìm thấy nút " + childName + " trong PauseWindow.");
            return null;
        }

        Button button = child.GetComponent<Button>();
        if (button == null)
        {
            Debug.LogWarning(childName + " chưa có component Button.");
        }

        return button;
    }

    private void ResumeButtonClicked()
    {
        GameLogic.ResumeGame();
    }

    private void MainMenuButtonClicked()
    {
        Time.timeScale = 1f;

        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.StopMusic();
        }

        Loader.Load(Loader.Scene.MainMenu);
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
    }

    public static void HideStatic()
    {
        if (instance != null)
        {
            instance.Hide();
        }
    }
}
