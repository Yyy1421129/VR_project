using System;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    const string HighScoreKey = "VRShooter_HighScore";
    const string BackgroundResourcePath = "Audio/background";
    const string DefeatResourcePath = "Audio/success";

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] AudioClip backgroundMusic;
    [SerializeField] AudioClip enemyDefeatSound;
    [SerializeField] float backgroundVolume = 0.6f;
    [SerializeField] float defeatVolume = 1f;

    AudioSource musicSource;
    AudioSource sfxSource;

    int score;
    int highScore;

    public int Score => score;
    public int HighScore => highScore;

    public event Action<int> OnScoreChanged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        instance = null;
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        UpdateHighScoreText();
        SetupAudioSources();
    }

    void Start()
    {
        StopForeignMusic();
        PlayBackgroundMusic();
    }

    void SetupAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        musicSource = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        sfxSource = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        ConfigureSource(musicSource, loop: true, backgroundVolume);
        ConfigureSource(sfxSource, loop: false, defeatVolume);
    }

    static void ConfigureSource(AudioSource source, bool loop, float volume)
    {
        source.playOnAwake = false;
        source.loop = loop;
        source.volume = volume;
        source.spatialBlend = 0f;
        source.Stop();
        source.clip = null;
    }

    void StopForeignMusic()
    {
        AudioSource[] sources = FindObjectsOfType<AudioSource>(true);
        for (int i = 0; i < sources.Length; i++)
        {
            AudioSource source = sources[i];
            if (source == musicSource || source == sfxSource)
            {
                continue;
            }

            // Only stop other looping background music, not weapon SFX.
            if (source.loop && source.isPlaying)
            {
                source.Stop();
                source.clip = null;
            }

            source.playOnAwake = false;
        }
    }

    void PlayBackgroundMusic()
    {
        AudioClip clip = ResolveClip(backgroundMusic, BackgroundResourcePath);
        if (clip == null)
        {
            Debug.LogError($"Background music missing. Assign clip on ScoreManager or add Resources/{BackgroundResourcePath}");
            return;
        }

        musicSource.clip = clip;
        musicSource.volume = backgroundVolume;
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }

        Debug.Log($"[Audio] Background playing: {clip.name} ({clip.length:F1}s)");
    }

    public void PlayEnemyDefeatSound()
    {
        AudioClip clip = ResolveClip(enemyDefeatSound, DefeatResourcePath);
        if (clip == null || sfxSource == null)
        {
            Debug.LogWarning("[Audio] Defeat sound missing.");
            return;
        }

        sfxSource.PlayOneShot(clip, defeatVolume);
        Debug.Log($"[Audio] Defeat sound: {clip.name}");
    }

    static AudioClip ResolveClip(AudioClip assignedClip, string resourcePath)
    {
        if (assignedClip != null)
        {
            return assignedClip;
        }

        return Resources.Load<AudioClip>(resourcePath);
    }

    public void AddScore(int value)
    {
        score += value;
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
            UpdateHighScoreText();
        }

        UpdateScoreText();
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
        OnScoreChanged?.Invoke(score);
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = "Best: " + highScore;
        }
    }
}
