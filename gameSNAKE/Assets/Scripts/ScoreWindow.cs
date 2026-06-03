using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    // Text UI hiển thị điểm.
    private Text scoreText;

    private void Awake()
    {
        // Tìm object con tên scoreText và lấy component Text.
        scoreText = transform.Find("scoreText").GetComponent<Text>();
    }

    private void Update()
    {
        // Cập nhật điểm mỗi frame.
        scoreText.text = GameLogic.GetScore().ToString();
    }

}
