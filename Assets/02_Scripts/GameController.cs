using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] private Transform ballTransform;

    [SerializeField] private LineRenderer playerLineRendererPrefab;
    [SerializeField] private LineRenderer aiLineRendererPrefab;

    private Vector3[] positions;

    private List<LineRenderer> playerLineRenderers;
    private List<LineRenderer> aiLineRenderers;

    private int positionsIndex;

    private int ballX;
    private int ballY;

    private int selectionX;
    private int selectionY;

    private void Start()
    {
        playerLineRenderers = new List<LineRenderer>();
        aiLineRenderers = new List<LineRenderer>();
    }

    private void Update()
    {
        UpdateSelection();

        if (!gameManager.isEndGame)
        {
            if (gameManager.isPlayerTurn)
            {
                if (Input.GetMouseButtonDown(0) && CanMove(selectionX, selectionY))
                {
                    MakeMove(selectionX, selectionY);
                }
            }
            else
            {
                AITurn();
            }
        }
    }

    public void ClearLineRenderers()
    {
        ClearLineRenderers(playerLineRenderers);
        ClearLineRenderers(aiLineRenderers);
    }


    private void ClearLineRenderers(List<LineRenderer> lineRenderers)
    {
        if (lineRenderers.Count > 0)
        {
            foreach (LineRenderer lineRenderer in lineRenderers)
            {
                Destroy(lineRenderer.gameObject);
            }

            lineRenderers.Clear();
        }
    }

    public void SetDefaultSettings()
    {
        ballX = 0;
        ballY = 0;

        ballTransform.position = new Vector3(0, 0, 1.0f);

        positionsIndex = 1;

        positions = new Vector3[300];
        positions[0] = new Vector3(ballX, ballY, 1.0f);

        for (int i = 1; i < positions.Length; i++)
        {
            positions[i] = new Vector3(100f, 100f, 1.0f);
        }
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }

        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("Field")))
        {
            selectionX = (int)decimal.Round((decimal)hit.point.x);
            selectionY = (int)decimal.Round((decimal)hit.point.y);
        }
    }

    private void DrawBallLine(LineRenderer lineRenderer)
    {
        Vector3[] slicedPositions = positions[(positionsIndex - 2)..positionsIndex];

        lineRenderer.positionCount = slicedPositions.Length;
        lineRenderer.SetPositions(slicedPositions);
    }

    private bool CanMove(int selectionX, int selectionY)
    {
        // drawed ball line restrictions
        for (int i = 0; i < positionsIndex; i++)
        {
            if (positions[i].x == ballX && positions[i].y == ballY) // if there was selection
            {
                // selection is on already drawed move
                if (positions[i + 1].x == selectionX && positions[i + 1].y == selectionY)
                {
                    return false;
                }
                if (i > 0)
                {
                    // selection is on previous move
                    if (positions[i - 1].x == selectionX && positions[i - 1].y == selectionY)
                    {
                        return false;
                    }
                }
            }
        }

        // same point restriction
        if (selectionX == ballX && selectionY == ballY)
        {
            return false;
        }

        // 2 points far horizontal restriction
        else if ((selectionX < ballX - 1) || (selectionX > ballX + 1))
        {
            return false;
        }

        // 2 point far vertical restriction
        else if ((selectionY < ballY - 1) || (selectionY > ballY + 1))
        {
            return false;
        }

        // field left band restriction
        else if (ballX == -4 && selectionX == -4 && (ballY != selectionY))
        {
            return false;
        }

        // field right band restriction
        else if (ballX == 4 && selectionX == 4 && (ballY != selectionY))
        {
            return false;
        }

        // field horizontal band restriction
        else if (selectionX < -4 || selectionX > 4)
        {
            return false;
        }

        // field bottom band restriction
        else if (ballX != 0 && ballY == -5 && selectionY <= -5 && (selectionX >= 1 || selectionX <= -1))
        {
            return false;
        }

        // field top band restriction
        else if (ballX != 0 && ballY == 5 && selectionY >= 5 && (selectionX >= 1 || selectionX <= -1))
        {
            return false;
        }

        else
        {
            return true;
        }

    }

    private bool CanBounce()
    {
        for (int i = 0; i < positionsIndex - 2; i++) // check previous ball spots
        {
            if (positions[i].x == ballX && positions[i].y == ballY)
            {
                return true;
            }
        }

        if (ballX == -4 || ballX == 4) // check left / right band
        {
            return true;
        }

        if (ballY == -5 || ballY == 5) // check up / down band
        {
            if (ballX != 0)
            {
                return true;
            }

        }

        return false;
    }

    private void MakeMove(int selectionX, int selectionY)
    {
        ballX = selectionX;
        ballY = selectionY;
        positions[positionsIndex] = new Vector3(ballX, ballY, 1.0f);

        LineRenderer lineRenderer;

        if (gameManager.isPlayerTurn)
        {
            lineRenderer = Instantiate(playerLineRendererPrefab);
            playerLineRenderers.Add(lineRenderer);
        }
        else
        {
            lineRenderer = Instantiate(aiLineRendererPrefab);
            playerLineRenderers.Add(lineRenderer);
        }

        lineRenderer.positionCount++;

        positionsIndex++;

        DrawBallLine(lineRenderer);

        if (ballY > 5)
        {
            gameManager.EndGame(false);
        }
        else if (ballY < -5)
        {
            gameManager.EndGame(true);
        }

        if (!CanBounce())
        {
            gameManager.EndTurn();
        }
        else
        {
            CheckIfGameCanEndAsDraw();
        }


        ballTransform.position = new Vector3(ballX, ballY, 1.0f);
    }

    private void CheckIfGameCanEndAsDraw()
    {
        var tempSelectionX = ballX + 1;
        var tempSelectionY = ballY;

        bool tempCanMove = false;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX - 1;
        tempSelectionY = ballY;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX;
        tempSelectionY = ballY + 1;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX;
        tempSelectionY = ballY - 1;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX + 1;
        tempSelectionY = ballY + 1;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX + 1;
        tempSelectionY = ballY - 1;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX - 1;
        tempSelectionY = ballY - 1;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        tempSelectionX = ballX - 1;
        tempSelectionY = ballY + 1;

        if (CanMove(tempSelectionX, tempSelectionY))
        {
            tempCanMove = true;
        }

        if (!tempCanMove)
        {
            // end game as draw
            gameManager.EndGame(false, true);
        }
    }

    private void AITurn()
    {
        if (ballX < 0)
        {
            selectionX = ballX + 1;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }
        }
        else if (ballX > 0)
        {
            selectionX = ballX - 1;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }
        }
        else
        {
            selectionX = ballX;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY + 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX + 1;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }

            selectionX = ballX - 1;
            selectionY = ballY - 1;

            if (CanMove(selectionX, selectionY))
            {
                MakeMove(selectionX, selectionY);
                return;
            }
        }
    }
}
