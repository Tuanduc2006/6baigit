using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * MainMenuWindow.cs
 * Tác dụng: Quản lý màn hình menu chính.
 * - Nút Play để vào game.
 * - Nút Quit để thoát game.
 */
public class MainMenuWindow : MonoBehaviour
{
    private void Awake()
    {
        // Tìm nút Play trong object mainSub/playBtn.
        Button playButton = transform.Find("mainSub/playBtn").GetComponent<Button>();
        playButton.onClick.AddListener(PlayButtonClicked);

        // Tìm nút Quit trong object mainSub/quitBtn.
        Button quitButton = transform.Find("mainSub/quitBtn").GetComponent<Button>();
        quitButton.onClick.AddListener(QuitButtonClicked);
    }

    // Khi bấm Play thì load scene Game.
    private void PlayButtonClicked()
    {
        Loader.Load(Loader.Scene.Game);
        Time.timeScale = 1f; // Đảm bảo game không bị pause khi vào lại.
    }

    // Khi bấm Quit thì thoát ứng dụng.
    private void QuitButtonClicked()
    {
        Application.Quit();
    }
}
