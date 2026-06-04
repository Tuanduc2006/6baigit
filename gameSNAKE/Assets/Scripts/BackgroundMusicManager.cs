using UnityEngine;

/*
 * BackgroundMusicManager.cs
 * Tác dụng:
 * - Quản lý nhạc nền của game.
 * - Nhạc nền chỉ phát khi vào màn chơi Game.
 * - Khi rắn chết thì nhạc nền dừng.
 * - Khi bấm chơi lại thì nhạc nền phát lại từ đầu.
 */
[RequireComponent(typeof(AudioSource))]
public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    [Header("Nhạc nền")]
    public AudioClip backgroundMusic;

    [Header("Âm lượng nhạc nền")]
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;

    private AudioSource audioSource;

    private void Awake()
    {
        Instance = this;

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = backgroundMusic;
        audioSource.volume = musicVolume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.Stop();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void PlayMusicFromBeginning()
    {
        if (audioSource == null) return;

        if (backgroundMusic == null)
        {
            Debug.LogWarning("Bạn chưa kéo file nhạc nền vào BackgroundMusicManager!");
            return;
        }

        audioSource.clip = backgroundMusic;
        audioSource.volume = musicVolume;
        audioSource.time = 0f;
        audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource == null) return;

        audioSource.Stop();
        audioSource.time = 0f;
    }

    public void PauseMusic()
    {
        if (audioSource == null) return;
        audioSource.Pause();
    }

    public void ResumeMusic()
    {
        if (audioSource == null) return;
        audioSource.UnPause();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);

        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
}
