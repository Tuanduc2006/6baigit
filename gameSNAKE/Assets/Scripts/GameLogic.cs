using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/*
 * GameLogic.cs
 * Tác dụng: Quản lý logic tổng của game Snake.
 * - Tạo bàn chơi 20x20.
 * - Kết nối Snake với LevelGrid.
 * - Quản lý điểm số.
 * - Quản lý tạm dừng / tiếp tục game bằng phím ESC.
 * - Gọi màn hình Game Over khi rắn chết.
 */
public class GameLogic : MonoBehaviour
{
    // Lưu instance hiện tại của GameLogic để các hàm static có thể dùng chung.
    private static GameLogic instance;

    // Điểm hiện tại của người chơi. Đây là biến static nên có thể lấy từ script khác.
    private static int score;

    // Kéo object Snake từ Inspector vào biến này.
    [SerializeField] private Snake snake;

    // Bàn chơi dạng lưới, dùng để quản lý vị trí thức ăn và giới hạn màn chơi.
    private LevelGrid levelGrid;

    private void Awake()
    {
        // Gán instance khi object được tạo.
        instance = this;

        // Reset điểm về 0 mỗi khi bắt đầu scene game.
        InitializeStatic();
    }

    void Start()
    {
        // Tạo bàn chơi có kích thước 20 ô ngang và 20 ô dọc.
        levelGrid = new LevelGrid(20, 20);

        // Truyền bàn chơi cho rắn để rắn biết giới hạn và vị trí thức ăn.
        snake.Setup(levelGrid);

        // Truyền rắn cho bàn chơi để không sinh thức ăn trùng lên thân rắn.
        levelGrid.Setup(snake);
    }

    private void Update()
    {
        // Nếu người chơi bấm ESC thì bật/tắt tạm dừng game.
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

    // Khởi tạo lại các biến static của game.
    private static void InitializeStatic()
    {
        score = 0;
    }

    // Trả về điểm hiện tại để UI hoặc bảng xếp hạng sử dụng.
    public static int GetScore()
    {
        return score;
    }

    // Cộng điểm khi rắn ăn được thức ăn.
    public static void AddScore()
    {
        score += 100;
    }

    // Hàm được gọi khi rắn chết.
    public static void SnakeDied()
    {
        // Hiển thị cửa sổ Game Over.
        GameOverWindow.ShowStatic();
    }

    // Tiếp tục game sau khi pause.
    public static void ResumeGame()
    {
        PauseWindow.HideStatic();
        Time.timeScale = 1f; // Cho thời gian chạy lại bình thường.
    }

    // Tạm dừng game.
    public static void PauseGame()
    {
        PauseWindow.ShowStatic();
        Time.timeScale = 0f; // Đóng băng thời gian trong game.
    }

    // Kiểm tra game có đang tạm dừng hay không.
    public static bool IsGamePaused()
    {
        return Time.timeScale == 0f;
    }
}
