using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Central game loop controller. Owns state transitions and exposes an event bus
/// that other systems subscribe to instead of polling IsGameOver every frame.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // ── State ──────────────────────────────────────────────────────────────
    public enum GameState { Ready, Running, Paused, GameOver }

    public GameState State { get; private set; } = GameState.Ready;

    // Convenience shorthand kept for backward compatibility with older scripts
    public bool isGameOver => State == GameState.GameOver;

    // ── Events (event bus) ─────────────────────────────────────────────────
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int>       OnScoreChanged;
    public static event Action            OnGameStarted;
    public static event Action            OnGameOver;
    public static event Action            OnGameReset;

    // ── Score ──────────────────────────────────────────────────────────────
    public int score { get; private set; }

    // ── Run-time tracking ──────────────────────────────────────────────────
    public float runTime { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // Subscribe to HealthSystem death event
        HealthSystem.onGameOver += HandleHealthGameOver;
        SetState(GameState.Ready);
    }

    void OnDestroy()
    {
        HealthSystem.onGameOver -= HandleHealthGameOver;
    }

    void Update()
    {
        if (State == GameState.Running)
            runTime += Time.deltaTime;
    }

    // ── Public API ─────────────────────────────────────────────────────────

    public void StartGame()
    {
        score = 0;
        runTime = 0f;
        OnScoreChanged?.Invoke(score);
        SetState(GameState.Running);
        OnGameStarted?.Invoke();
        GameAudioEvents.OnChangeBGM?.Invoke("Egypt");
        SceneManager.LoadScene("Gameplay");
    }

    public void PauseGame()
    {
        if (State != GameState.Running) return;
        Time.timeScale = 0f;
        SetState(GameState.Paused);
    }

    public void ResumeGame()
    {
        if (State != GameState.Paused) return;
        Time.timeScale = 1f;
        SetState(GameState.Running);
    }

    /// <summary>Continues game after an ad reward (e.g. revive). Resumes from GameOver state.</summary>
    public void ContinueGame()
    {
        Time.timeScale = 1f;
        SetState(GameState.Running);
    }

    public void EndGame()
    {
        if (State == GameState.GameOver) return;
        SetState(GameState.GameOver);
        OnGameOver?.Invoke();
        GameAudioEvents.OnPlayGameOver?.Invoke();
        Debug.Log($"[GameManager] Game Over — score: {score}, time: {runTime:F1}s");
        SceneManager.LoadScene("GameOver");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ResetGame()
    {
        Time.timeScale = 1f;
        score = 0;
        runTime = 0f;
        OnScoreChanged?.Invoke(score);
        SetState(GameState.Ready);
        OnGameReset?.Invoke();
    }

    /// <summary>Adds to the run-time score. Ignored when game is not Running.</summary>
    public void AddScore(int amount)
    {
        if (State != GameState.Running) return;
        score += amount;
        OnScoreChanged?.Invoke(score);
    }

    // ── Internals ──────────────────────────────────────────────────────────

    private void SetState(GameState newState)
    {
        State = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleHealthGameOver()
    {
        EndGame();
    }
}
