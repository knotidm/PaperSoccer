using UnityEngine;
using UnityEngine.UI;

public class UIView : MonoBehaviour
{
    private const string PlayerScoreText = "Player score:";
    private const string AIScoreText = "AI score:";
    private const string PlayerTurnText = "Turn: Player";
    private const string AITurnText = "Turn: AI";
    private const string PlayerWinsText = "Player wins!";
    private const string AIWinsText = "AI wins!";
    private const string DrawText = "Draw!";

    [SerializeField] private GameManager gameManager;

    [SerializeField] private Text turnText;
    [SerializeField] private Text winnerText;
    [SerializeField] private Text scoreText;

    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button playerWinButton;
    [SerializeField] private Button playerLoseButton;

    [SerializeField] private Slider fieldSizeSlider;

    private void Start()
    {
        RestartGame();

        gameManager.OnEndTurnEvent += EndTurn;
        gameManager.OnEndGameWithWinnerEvent += EndGameWithWinner;
        gameManager.OnEndGameWithDrawEvent += EndGameWithDraw;

        playAgainButton.onClick.AddListener(RestartGame);
        playerWinButton.onClick.AddListener(WinGame);
        playerLoseButton.onClick.AddListener(LoseGame);

        fieldSizeSlider.onValueChanged.AddListener(ChangeFieldSize);
    }

    private void OnDestroy()
    {
        gameManager.OnEndTurnEvent -= EndTurn;
        gameManager.OnEndGameWithWinnerEvent -= EndGameWithWinner;
        gameManager.OnEndGameWithDrawEvent -= EndGameWithDraw;

        playAgainButton.onClick.RemoveListener(RestartGame);
        playerWinButton.onClick.RemoveListener(WinGame);
        playerLoseButton.onClick.RemoveListener(LoseGame);

        fieldSizeSlider.onValueChanged.RemoveListener(ChangeFieldSize);

    }


    private void Update()
    {
        scoreText.text = $"{PlayerScoreText} {GameStats.playerWinsCount}\n{AIScoreText} {GameStats.aiWinsCount}";
    }

    private void EndTurn(bool isPlayerTurn)
    {
        if (isPlayerTurn)
        {
            turnText.text = PlayerTurnText;
        }
        else
        {
            turnText.text = AITurnText;
        }
    }

    private void EndGameWithWinner(bool isPlayerWins)
    {
        winnerText.gameObject.SetActive(true);

        if (isPlayerWins)
        {
            GameStats.playerWinsCount++;
            winnerText.text = PlayerWinsText;
        }
        else
        {
            GameStats.aiWinsCount++;
            winnerText.text = AIWinsText;
        }
    }

    private void EndGameWithDraw()
    {
        winnerText.gameObject.SetActive(true);
        winnerText.text = DrawText;
    }

    private void RestartGame()
    {
        gameManager.RestartGame();
        winnerText.text = string.Empty;
        winnerText.gameObject.SetActive(false);
    }

    private void WinGame()
    {
        EndGameWithWinner(true);
        RestartGame();
    }

    private void LoseGame()
    {
        EndGameWithWinner(false);
        RestartGame();
    }

    private void ChangeFieldSize(float value)
    {
        Camera.main.orthographicSize = 14 - value * 7;
    }
}
