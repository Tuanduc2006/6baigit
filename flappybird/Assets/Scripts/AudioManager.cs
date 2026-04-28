using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource audioSource;

    public AudioClip pressClip;
    public AudioClip releaseClip;
    public AudioClip scoreClip;
    public AudioClip diePipeClip;
    public AudioClip dieFallClip;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void PlayPress() => audioSource.PlayOneShot(pressClip);
    public void PlayRelease() => audioSource.PlayOneShot(releaseClip);
    public void PlayScore() => audioSource.PlayOneShot(scoreClip);
    public void PlayDiePipe() => audioSource.PlayOneShot(diePipeClip);
    public void PlayDieFall() => audioSource.PlayOneShot(dieFallClip);
}
