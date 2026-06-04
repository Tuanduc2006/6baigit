using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * SoundManager.cs
 * Tác dụng: Quản lý và phát âm thanh trong game.
 * - Phát âm thanh khi rắn di chuyển.
 * - Phát âm thanh khi rắn ăn.
 * - Phát âm thanh khi rắn chết.
 * - Tăng âm lượng âm thanh trong game.
 */
public static class SoundManager
{
    // Các loại âm thanh dùng trong game
    public enum Sound
    {
        SnakeMove,
        SnakeEat,
        SnakeDie
    }

    // Âm lượng âm thanh
    // 1f = 100%
    // 0.5f = 50%
    // 0.2f = 20%
    private static float volume = 1f;

    // Phát một âm thanh theo loại truyền vào
    public static void PlaySound(Sound sound)
    {
        // Lấy file âm thanh tương ứng
        AudioClip audioClip = GetAudioClip(sound);

        // Nếu không có âm thanh thì dừng lại, tránh lỗi
        if (audioClip == null)
        {
            Debug.LogWarning("Không tìm thấy AudioClip cho âm thanh: " + sound);
            return;
        }

        // Tạo GameObject mới để chứa AudioSource
        GameObject soundGameObject = new GameObject("Sound_" + sound.ToString());

        // Thêm AudioSource vào object vừa tạo
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();

        // Tăng âm lượng lên 100%
        audioSource.volume = volume;

        // Để âm thanh dạng 2D, nghe rõ hơn trong game 2D
        audioSource.spatialBlend = 0f;

        // Không lặp lại âm thanh
        audioSource.loop = false;

        // Phát âm thanh
        audioSource.PlayOneShot(audioClip);

        // Xóa object âm thanh sau khi phát xong để tránh sinh quá nhiều object
        GameObject.Destroy(soundGameObject, audioClip.length);
    }

    // Hàm đổi âm lượng nếu sau này bạn muốn làm nút tăng/giảm âm thanh
    public static void SetVolume(float newVolume)
    {
        volume = Mathf.Clamp01(newVolume);
    }

    // Hàm lấy âm lượng hiện tại
    public static float GetVolume()
    {
        return volume;
    }

    // Tìm AudioClip tương ứng với loại âm thanh
    private static AudioClip GetAudioClip(Sound sound)
    {
        if (GameResources.instance == null)
        {
            Debug.LogError("Chưa có GameResources trong Scene!");
            return null;
        }

        foreach (GameResources.SoundAudioClip soundAudioClip in GameResources.instance.soundAudioClipArray)
        {
            if (soundAudioClip.sound == sound)
            {
                return soundAudioClip.audioClip;
            }
        }

        Debug.LogError("Không tìm thấy âm thanh: " + sound.ToString());
        return null;
    }
}