using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;

    private float score = 0;
    private bool isGameOver = false;

    void Start()
    {
        ResetScore();
    }

    void Update()
    {
        // Chi tinh diem khi chua Game Over
        if (!isGameOver)
        {
            score += Time.deltaTime * 10; // toc do tang diem
            scoreText.text = "Diem: " + Mathf.FloorToInt(score);
        }
    }

    public void GameOver()
    {
        isGameOver = true;
    }

    public void ResetScore()
    {
        score = 0;
        isGameOver = false;
        scoreText.text = "Diem: 0";
    }
}
