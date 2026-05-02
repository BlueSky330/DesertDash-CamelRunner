using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int score = 0;
    public bool isGameOver = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        // Start the game in the main menu scene
        SceneManager.LoadScene("MainMenuScene");
    }

    public void StartGame()
    {
        score = 0;
        isGameOver = false;
        SceneManager.LoadScene("GameplayScene");
    }

    public void EndGame()
    {
        isGameOver = true;
        Debug.Log("Game Over! Score: " + score);
        SceneManager.LoadScene("GameOverScene");
    }

    public void AddScore(int amount)
    {
        if (!isGameOver)
        {
            score += amount;
            Debug.Log("Score: " + score);
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
}
