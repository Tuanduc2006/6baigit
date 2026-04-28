using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public TextMeshProUGUI scoreText;

    int score = 0;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ResetScore();
    }

    public void AddScore()
    {
        //Nếu muốn tìm lỗi thì bật nó lên  = cách xóa comment
        //Debug.Log("AddScore called - state = " + GameManager.instance.state);
        score++;
        scoreText.text = score.ToString();
        AudioManager.instance.PlayScore();
    }


    public void ResetScore()
    {
        score = 0;
        scoreText.text = "0";
        scoreText.gameObject.SetActive(true);
    }
}
