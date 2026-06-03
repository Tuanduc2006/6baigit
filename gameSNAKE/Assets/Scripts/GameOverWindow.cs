using System.Collections;
using System.Collections.Generic;
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
    // Instance static để script khác có thể gọi ShowStatic().
    private static GameOverWindow instance;

    // Ô nhập tên người chơi trên màn hình Game Over.
    public TMP_InputField nameInputField;

    private void Start()
    {
        // Lưu instance hiện tại.
        instance = this;

        // Tìm nút retryBtn trong object con và lấy component Button.
        Button retryButton = transform.Find("retryBtn").GetComponent<Button>();

        // Khi bấm Retry thì gọi hàm RetryButtonClicked.
        retryButton.onClick.AddListener(RetryButtonClicked);

        // Ẩn cửa sổ Game Over lúc mới vào game.
        Hide();
    }

    // Xử lý khi bấm nút chơi lại.
    private void RetryButtonClicked()
    {
        // Load lại scene Game.
        Loader.Load(Loader.Scene.Game);
    }

    // Hiển thị bảng Game Over.
    private void Show()
    {
        gameObject.SetActive(true);
    }

    // Ẩn bảng Game Over.
    private void Hide()
    {
        gameObject.SetActive(false);
    }

    // Hàm static để GameLogic có thể gọi hiển thị Game Over.
    public static void ShowStatic()
    {
        instance.Show();
    }

    // Lưu điểm của người chơi vào bảng xếp hạng.
    public void SubmitScore()
    {
        // Đổi màu ô nhập tên sang đỏ sau khi submit.
        nameInputField.image.color = Color.red;

        string playerNameToLeaderboard;

        // Nếu người chơi không nhập tên thì dùng "---".
        if (nameInputField.text == "")
        {
            playerNameToLeaderboard = "---";
        }
        else
        {
            playerNameToLeaderboard = nameInputField.text;
        }

        // Lấy điểm hiện tại từ GameLogic.
        int scoreToLeaderboard = Mathf.RoundToInt(GameLogic.GetScore());

        // Thêm/cập nhật điểm vào bảng xếp hạng.
        HighScoreTable.AddHighscoreEntry(scoreToLeaderboard, playerNameToLeaderboard);
    }

}
