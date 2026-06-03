using System.Collections;
using System.Collections.Generic;
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

    // Instance static để GameLogic có thể gọi ShowStatic/HideStatic.
    private static PauseWindow instance;

    private void Awake()
    {
        instance = this;

        // Căn bảng pause phủ đúng vị trí của Canvas/Panel.
        transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        // Tìm nút Resume và gắn sự kiện click.
        Button resumeButton = transform.Find("resumeBtn").GetComponent<Button>();
        resumeButton.onClick.AddListener(ResumeButtonClicked);

        // Tìm nút Main Menu và gắn sự kiện click.
        Button maiMenuButton = transform.Find("mainMenuBtn").GetComponent<Button>();
        maiMenuButton.onClick.AddListener(MainMenuButtonClicked);

        // Ẩn pause panel khi mới vào game.
        Hide();
    }

    // Xử lý nút Resume.
    private void ResumeButtonClicked()
    {
        GameLogic.ResumeGame();
    }

    // Xử lý nút Main Menu.
    private void MainMenuButtonClicked()
    {
        Loader.Load(Loader.Scene.MainMenu);
    }

    // Hiện bảng pause.
    private void Show()
    {
        gameObject.SetActive(true);
    }

    // Ẩn bảng pause.
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // Hàm static để script khác hiển thị pause panel.
    public static void ShowStatic()
    {
        instance.Show();
    }

    // Hàm static để script khác ẩn pause panel.
    public static void HideStatic()
    {
        instance.Hide();
    }
}
