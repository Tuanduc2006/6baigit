using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * SoundManager.cs
 * Tác dụng: Quản lý và phát âm thanh trong game.
 * - Phát âm thanh khi rắn di chuyển.
 * - Phát âm thanh khi rắn ăn.
 * - Phát âm thanh khi rắn chết.
 */
public static class SoundManager
{

    // Các loại âm thanh dùng trong game.
    public enum Sound
    {
        SnakeMove,
        SnakeEat,
        SnakeDie
    }

    // Phát một âm thanh theo loại truyền vào.
    public static void PlaySound(Sound sound)
    {
        // Tạo GameObject mới để chứa AudioSource.
        GameObject soundGameObject = new GameObject("Sound");

        // Thêm AudioSource vào object vừa tạo.
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        // Giảm âm lượng còn 20%.
        audioSource.volume = 0.2f;

        // Phát AudioClip tương ứng.
        audioSource.PlayOneShot(GetAudioClip(sound));
    }

    // Tìm AudioClip tương ứng với loại âm thanh.
    private static AudioClip GetAudioClip(Sound sound)
    {
        foreach (GameResources.SoundAudioClip soundAudioClip in GameResources.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }

        // Nếu không tìm thấy âm thanh thì báo lỗi.
        Debug.LogError("Unknown sound " + sound.ToString());
        return null;
    }

}
