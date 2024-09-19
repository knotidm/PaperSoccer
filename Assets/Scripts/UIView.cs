using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Text turnText;
    [SerializeField] private Text winnerText;
    [SerializeField] private Text scoreText;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button playerWinButton;
    [SerializeField] private Button playerLoseButton;

    private void Start()
    {
        RestartGame();

        gameManager.OnEndTurnEvent += EndTurn;
        gameManager.OnEndGameEvent += EndGame;

        playAgainButton.onClick.AddListener(RestartGame);
        playerWinButton.onClick.AddListener(WinGame);
        playerLoseButton.onClick.AddListener(LoseGame);
    }

    private void OnDestroy()
    {
        gameManager.OnEndTurnEvent -= EndTurn;
        gameManager.OnEndGameEvent -= EndGame;

        playAgainButton.onClick.RemoveListener(RestartGame);
        playerWinButton.onClick.RemoveListener(WinGame);
        playerLoseButton.onClick.RemoveListener(LoseGame);
    }

    private void Update()
    {
        scoreText.text = "Player score: " + GameStats.playerWinsCount + "\nAI score: " + GameStats.aiWinsCount;
    }

    private void EndTurn(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            turnText.text = "Turn: Player";
        }
        else
        {
            turnText.text = "Turn: AI";
        }
    }

    private void EndGame(bool isPlayerWins)
    {
        winnerText.gameObject.SetActive(true);

        if (isPlayerWins)
        {
            GameStats.playerWinsCount++;
            winnerText.text = "Player wins!";
        }
        else
        {
            GameStats.aiWinsCount++;
            winnerText.text = "AI wins!";
        }
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
        winnerText.gameObject.SetActive(false);
    }

    private void WinGame()
    {
        EndGame(true);
        RestartGame();
    }

    private void LoseGame()
    {
        EndGame(false);
        RestartGame();
    }
}
