using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] float gameDuration = 60f;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI statusText;
    [SerializeField] EnemySpawner enemySpawner;
    [SerializeField] Player player;
    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] bool autoStart = true;

    float timeRemaining;
    bool isPlaying;

    public bool IsPlaying => isPlaying;
    public float TimeRemaining => timeRemaining;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (enemySpawner == null)
        {
            enemySpawner = FindObjectOfType<EnemySpawner>();
        }

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }

        if (autoStart)
        {
            StartGame();
        }
        else
        {
            ShowMainMenu();
        }
    }

    void Update()
    {
        if (!isPlaying)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;
        UpdateTimerUI();

        if (timeRemaining <= 0f)
        {
            EndGame("Time's up!");
        }
    }

    public void StartGame()
    {
        isPlaying = true;
        timeRemaining = gameDuration;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }

        if (ScoreManager.instance != null)
        {
            ScoreManager.instance.ResetScore();
        }

        if (player != null)
        {
            player.ResetHealth();
        }

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(true);
        }

        UpdateStatus("Fight!");
        UpdateTimerUI();
    }

    public void EndGame(string message)
    {
        if (!isPlaying)
        {
            return;
        }

        isPlaying = false;

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(false);
        }

        UpdateStatus(message);
    }

    public void OnPlayerDied()
    {
        EndGame("Game Over");
    }

    public void ShowMainMenu()
    {
        isPlaying = false;

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }

        if (enemySpawner != null)
        {
            enemySpawner.SetSpawningEnabled(false);
        }

        UpdateStatus("Press Start");
    }

    void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int seconds = Mathf.CeilToInt(Mathf.Max(0f, timeRemaining));
            timerText.text = $"Time: {seconds}s";
        }
    }

    void UpdateStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }
}
