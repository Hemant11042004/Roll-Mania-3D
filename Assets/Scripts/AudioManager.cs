using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;  // Background music
    public AudioSource sfxSource;    // Sound effects

    [Header("Audio Clips")]
    public AudioClip backgroundMusic;
    public AudioClip playerEnemyCollisionSFX;
    public AudioClip enemyEnemyCollisionSFX;

    [Header("Powerup Pickup Sounds")]
    public AudioClip pushbackPickupSFX;
    public AudioClip rocketsPickupSFX;
    public AudioClip smashPickupSFX;
    public AudioClip genericPickupSFX;

    [Header("Powerup Use Sounds")]
    public AudioClip pushbackUseSFX;   // when Pushback activates
    public AudioClip rocketsUseSFX;    //  when pressing F with Rockets
    public AudioClip smashUseSFX;      //  when player lands Smash

    [Header("Settings")]
    public float fadeDuration = 1.5f;
    public float sfxVolume = 1.0f;

    [Range(0.8f, 1.2f)]
    public float minPitch = 0.9f;
    [Range(0.8f, 1.2f)]
    public float maxPitch = 1.1f;

    private Coroutine currentFade;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }

        if (sfxSource != null)
            sfxSource.playOnAwake = false;
    }

    // --- Background Music ---
    public void PlayBackgroundMusic()
    {
        if (musicSource == null || backgroundMusic == null)
            return;

        musicSource.clip = backgroundMusic;

        if (currentFade != null)
            StopCoroutine(currentFade);

        musicSource.volume = 0f;
        musicSource.Play();
        currentFade = StartCoroutine(FadeAudio(musicSource, fadeDuration, 1f));
    }

    public void StopBackgroundMusic()
    {
        if (musicSource == null || !musicSource.isPlaying)
            return;

        if (currentFade != null)
            StopCoroutine(currentFade);

        currentFade = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeAudio(AudioSource source, float duration, float targetVolume)
    {
        float startVolume = source.volume;
        float time = 0f;

        while (time < duration)
        {
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        source.volume = targetVolume;
    }

    private IEnumerator FadeOutAndStop()
    {
        float startVolume = musicSource.volume;
        float time = 0f;

        while (time < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0f, time / fadeDuration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        musicSource.volume = 0f;
        musicSource.Stop();
    }

    // --- General SFX ---
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.pitch = Random.Range(minPitch, maxPitch);
            sfxSource.PlayOneShot(clip, sfxVolume);
        }
    }

    // --- Collisions ---
    public void PlayPlayerEnemyCollisionSFX() => PlaySFX(playerEnemyCollisionSFX);
    public void PlayEnemyEnemyCollisionSFX() => PlaySFX(enemyEnemyCollisionSFX);

    // --- Powerup Pickup Sounds ---
    public void PlayPowerupPickupSFX(PowerUpType type)
    {
        AudioClip clipToPlay = genericPickupSFX;

        switch (type)
        {
            case PowerUpType.Pushback:
                clipToPlay = pushbackPickupSFX;
                break;
            case PowerUpType.Rockets:
                clipToPlay = rocketsPickupSFX;
                break;
            case PowerUpType.Smash:
                clipToPlay = smashPickupSFX;
                break;
        }

        PlaySFX(clipToPlay);
    }

    // --- Powerup Use Sounds ---
    public void PlayPowerupUseSFX(PowerUpType type)
    {
        AudioClip clipToPlay = null;

        switch (type)
        {
            case PowerUpType.Pushback:
                clipToPlay = pushbackUseSFX;
                break;
            case PowerUpType.Rockets:
                clipToPlay = rocketsUseSFX;
                break;
            case PowerUpType.Smash:
                clipToPlay = smashUseSFX;
                break;
        }

        PlaySFX(clipToPlay);
    }
}
