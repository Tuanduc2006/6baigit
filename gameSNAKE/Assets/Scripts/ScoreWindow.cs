using UnityEngine;
using UnityEngine.UI;

/*
 * ScoreWindow.cs
 * Tác dụng: Hiển thị điểm hiện tại lên UI.
 * - Lấy điểm từ GameLogic.GetScore().
 * - Cập nhật liên tục vào Text scoreText.
 */
public class ScoreWindow : MonoBehaviour
{
    private Text scoreText;

    private void Awake()
    {
        Transform scoreTransform = transform.Find("scoreText");
        if (scoreTransform != null)
        {
            scoreText = scoreTransform.GetComponent<Text>();
        }

        if (scoreText == null)
        {
            Debug.LogWarning("Không tìm thấy Text scoreText trong ScoreWindow.");
        }
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.text = GameLogic.GetScore().ToString();
        }
    }
}
