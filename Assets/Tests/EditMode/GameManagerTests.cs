using NUnit.Framework;
using UnityEngine;

/// <summary>
/// Edit-mode unit tests for GameManager state transitions and score logic.
/// </summary>
public class GameManagerTests
{
    private GameManager gm;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject("GameManager");
        gm = go.AddComponent<GameManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(gm.gameObject);
    }

    [Test]
    public void InitialState_IsReady()
    {
        Assert.AreEqual(GameManager.GameState.Ready, gm.State);
    }

    [Test]
    public void AddScore_WhenRunning_IncreasesScore()
    {
        // Force Running state by reflection (StartGame loads a scene we can't load in EditMode)
        typeof(GameManager)
            .GetMethod("SetState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(gm, new object[] { GameManager.GameState.Running });

        gm.AddScore(100);
        Assert.AreEqual(100, gm.score);
    }

    [Test]
    public void AddScore_WhenNotRunning_DoesNotChangeScore()
    {
        // State is Ready by default — score should not change
        gm.AddScore(100);
        Assert.AreEqual(0, gm.score);
    }

    [Test]
    public void ResetGame_ClearsScore()
    {
        typeof(GameManager)
            .GetMethod("SetState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(gm, new object[] { GameManager.GameState.Running });

        gm.AddScore(500);
        gm.ResetGame();

        Assert.AreEqual(0, gm.score);
    }

    [Test]
    public void ResetGame_SetsStateToReady()
    {
        gm.ResetGame();
        Assert.AreEqual(GameManager.GameState.Ready, gm.State);
    }

    [Test]
    public void IsGameOver_TrueWhenStateIsGameOver()
    {
        typeof(GameManager)
            .GetMethod("SetState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(gm, new object[] { GameManager.GameState.GameOver });

        Assert.IsTrue(gm.isGameOver);
    }

    [Test]
    public void IsGameOver_FalseWhenRunning()
    {
        typeof(GameManager)
            .GetMethod("SetState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            ?.Invoke(gm, new object[] { GameManager.GameState.Running });

        Assert.IsFalse(gm.isGameOver);
    }
}
