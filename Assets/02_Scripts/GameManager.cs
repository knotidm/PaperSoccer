using UnityEngine;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    private const string GameStatsJsonFileName = "/GameStatsData.json";
    private const string FirstTimeInAppKey = "firstTime";

    [SerializeField] private GameController gameController;

    public bool isPlayerTurn;
    public bool isEndGame;

    public event Action<bool> OnEndTurnEvent;
    public event Action<bool> OnEndGameWithWinnerEvent;
    public event Action OnEndGameWithDrawEvent;

    private void Start()
    {
        if (!PlayerPrefs.HasKey(FirstTimeInAppKey))
        {
            SaveGameStats();
            PlayerPrefs.SetInt(FirstTimeInAppKey, 0);
        }

        LoadGameStats();
        RestartGame();
    }

    private void OnDestroy()
    {
        SaveGameStats();
    }

    private static void LoadGameStats()
    {
        string gameStatsJson = File.ReadAllText(Application.persistentDataPath + GameStatsJsonFileName);
        GameStatsData gameStatsData = JsonUtility.FromJson<GameStatsData>(gameStatsJson);
        GameStats.playerWinsCount = gameStatsData.playerWinsCount;
        GameStats.aiWinsCount = gameStatsData.aiWinsCount;
    }

    private static void SaveGameStats()
    {
        GameStatsData gameStatsData = new GameStatsData();
        gameStatsData.playerWinsCount = GameStats.playerWinsCount;
        gameStatsData.aiWinsCount = GameStats.aiWinsCount;
        string gameStatsJson = JsonUtility.ToJson(gameStatsData);
        File.WriteAllText(Application.persistentDataPath + GameStatsJsonFileName, gameStatsJson);
    }

    public void RestartGame()
    {
        gameController.ClearLineRenderers();
        gameController.SetDefaultSettings();

        isPlayerTurn = true;
        isEndGame = false;
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;

        OnEndTurnEvent?.Invoke(isPlayerTurn);
    }

    public void EndGame(bool isPlayerWins, bool isDraw = false)
    {
        isEndGame = true;

        if (!isDraw)
        {
            OnEndGameWithWinnerEvent?.Invoke(isPlayerWins);
        }
        else
        {
            OnEndGameWithDrawEvent?.Invoke();
        }
    }
}