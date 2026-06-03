using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * GameResources.cs
 * Tác dụng: Lưu tài nguyên dùng chung cho game.
 * - Sprite đầu rắn, thân rắn, thức ăn.
 * - Danh sách âm thanh của game.
 * Script này thường được gắn vào một GameObject trong scene và kéo asset vào Inspector.
 */
public class GameResources : MonoBehaviour
{
    // Instance dùng chung để các script khác truy cập tài nguyên.
    public static GameResources instance;

    private void Awake()
    {
        // Gán instance khi scene chạy.
        instance = this;
    }

    // Sprite đầu rắn.
    public Sprite snakeHeadSprite;

    // Sprite thân rắn.
    public Sprite snakeBodySprite;

    // Sprite thức ăn.
    public Sprite foodSprite;

    // Mảng chứa các cặp: loại âm thanh + file AudioClip tương ứng.
    public SoundAudioClip[] soundAudioClipArray;

    // Serializable để Unity Inspector hiển thị class này.
    [Serializable]
    public class SoundAudioClip
    {
        // Loại âm thanh, ví dụ SnakeMove, SnakeEat, SnakeDie.
        public SoundManager.Sound sound;

        // File âm thanh tương ứng.
        public AudioClip audioClip;
    }

}
