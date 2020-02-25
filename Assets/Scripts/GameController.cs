using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum spaceState
{
    BLANK = 0,
    X = 1,
    O = -1
}

[System.Serializable]
public class Player
{
    public Image panel;
    public Text text;
    public Button button;
}

[System.Serializable]
public class PlayerColor
{
    public Color panelColor;
    public Color textColor;
}

public class GameController : MonoBehaviour
{
    private int moveCount;
    public Text[] buttonList;
    public spaceState[] gameState;
    private string playerSide;

    public GameObject gameOverPanel;
    public Text gameOverText;
    public GameObject restartButton;
    public GameObject startInfo;

    public Player playerX;
    public Player playerO;
    public PlayerColor activePlayerColor;
    public PlayerColor inactivePlayerColor;

    public AI ai;

    private void Awake()
    {
        SetGameControllerOnButtons();
        gameOverPanel.SetActive(false);
        moveCount = 0;
        restartButton.SetActive(false);
        ai = new AI();
    }

    void SetGameControllerOnButtons()
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<GridSpace>().SetGameController(this);
        }
    }

    public void SetSide(string side)
    {
        // Need to modify for AI selection
        ai.SetSideAI(side);
        playerSide = "X";
        SetPlayerColors(playerX, playerO);
        //playerSide = side;
        //if (playerSide == "X")
        //{
        //    SetPlayerColors(playerX, playerO);
        //}
        //else
        //{
        //    SetPlayerColors(playerO, playerX);
        //}
        StartGame();
    }

    void StartGame()
    {
        SetPlayerButtons(false);
        SetBoardInteractable(true);
        startInfo.SetActive(false);
        for (int i = 0; i < gameState.Length; i++)
        {
            gameState[i] = spaceState.BLANK;
        }
    }

    public string GetPlayerSide()
    {
        return playerSide;
    }

    public void Update()
    {
        if (playerSide == ai.GetSideAI())
        {
            int move = -1;
            int score = -10000;
            for (int i = 0; i < buttonList.Length; i++)
            {
                if(buttonList[i].text == "")
                {
                    gameState[i] = spaceState.BLANK;
                }
                else if(buttonList[i].text == "X")
                {
                    gameState[i] = spaceState.X;
                }
                else if(buttonList[i].text == "O")
                {
                    gameState[i] = spaceState.O;
                }
            }
            for (int i = 0; i < gameState.Length; i++)
            {
                if (gameState[i] == spaceState.BLANK)
                {
                    gameState[i] = (ai.GetSideAI() == "X") ? spaceState.X : spaceState.O;
                    int tempScore = ai.GetNextMove(gameState, playerSide, moveCount);
                    gameState[i] = spaceState.BLANK;

                    if (tempScore > score)
                    {
                        move = i;
                        score = tempScore;
                    }
                }
            }
            Debug.Log("Current Move: " + move + ", Current Score: " + score);
            if (move >= 0)
            {
                buttonList[move].text = ai.GetSideAI();
                buttonList[move].GetComponentInParent<Button>().interactable = false;
                Debug.Log("AI Picks Move: " + move);
            }
            EndTurn();
        }
    }

    public void EndTurn()
    {
        moveCount++;
        if (buttonList[0].text == playerSide && buttonList[1].text == playerSide && buttonList[2].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[3].text == playerSide && buttonList[4].text == playerSide && buttonList[5].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[6].text == playerSide && buttonList[7].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[0].text == playerSide && buttonList[3].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[1].text == playerSide && buttonList[4].text == playerSide && buttonList[7].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[2].text == playerSide && buttonList[5].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[0].text == playerSide && buttonList[4].text == playerSide && buttonList[8].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (buttonList[2].text == playerSide && buttonList[4].text == playerSide && buttonList[6].text == playerSide)
        {
            GameOver(playerSide);
        }
        else if (moveCount >= 9)
        {
            GameOver("draw");
        }
        else
        {
            ChangeSides();
        }
    }

    void GameOver(string winningPlayer)
    {
        SetBoardInteractable(false);
        if (winningPlayer == "draw")
        {
            SetGameOverText("It's a Draw!");
        }
        else
        {
            SetGameOverText(winningPlayer + " Wins!");
        }
        restartButton.SetActive(true);
        SetPlayerColorsInactive();
        ai.SetSideAI("");
    }

    void ChangeSides()
    {
        playerSide = (playerSide == "X") ? "O" : "X";
        if (playerSide == "X")
        {
            SetPlayerColors(playerX, playerO);
        }
        else
        {
            SetPlayerColors(playerO, playerX);
        }
    }

    void SetGameOverText(string value)
    {
        gameOverPanel.SetActive(true);
        gameOverText.text = value;
    }

    public void RestartGame()
    {
        moveCount = 0;

        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].text = "";
        }

        gameOverPanel.SetActive(false);
        restartButton.SetActive(false);
        SetPlayerButtons(true);
        SetPlayerColorsInactive();
        startInfo.SetActive(true);
    }

    void SetBoardInteractable(bool toggle)
    {
        for (int i = 0; i < buttonList.Length; i++)
        {
            buttonList[i].GetComponentInParent<Button>().interactable = toggle;
        }
    }

    void SetPlayerColors(Player newPlayer, Player oldPlayer)
    {
        newPlayer.panel.color = activePlayerColor.panelColor;
        newPlayer.text.color = activePlayerColor.textColor;
        oldPlayer.panel.color = inactivePlayerColor.panelColor;
        oldPlayer.text.color = inactivePlayerColor.textColor;
    }

    void SetPlayerColorsInactive()
    {
        playerX.panel.color = inactivePlayerColor.panelColor;
        playerX.text.color = inactivePlayerColor.textColor;
        playerO.panel.color = inactivePlayerColor.panelColor;
        playerO.text.color = inactivePlayerColor.textColor;
    }

    void SetPlayerButtons(bool toggle)
    {
        playerX.button.interactable = toggle;
        playerO.button.interactable = toggle;
    }
}

public class AI
{
    private string side = "";

    public void SetSideAI(string s)
    {
        side = s;
    }

    public string GetSideAI()
    {
        return side;
    }

    public int GetNextMove(spaceState[] gameState, string playerTurn, int moves)
    {
        spaceState[] currentState = gameState;
        spaceState currentPlayer = (playerTurn == "X") ? spaceState.X : spaceState.O;

        int currentMoves = ++moves;
        if (currentState[0] == currentPlayer && currentState[1] == currentPlayer && currentState[2] == currentPlayer)
        {
            if(playerTurn == side) {
                return 1;
            }
            else {
                return -1;
            }
        }
        else if (currentState[3] == currentPlayer && currentState[4] == currentPlayer && currentState[5] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentState[6] == currentPlayer && currentState[7] == currentPlayer && currentState[8] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentState[0] == currentPlayer && currentState[3] == currentPlayer && currentState[6] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentState[1] == currentPlayer && currentState[4] == currentPlayer && currentState[7] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentState[2] == currentPlayer && currentState[5] == currentPlayer && currentState[8] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentState[0] == currentPlayer && currentState[4] == currentPlayer && currentState[8] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentState[2] == currentPlayer && currentState[4] == currentPlayer && currentState[6] == currentPlayer)
        {
            if (playerTurn == side)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        else if (currentMoves >= 9)
        {
            return 0;
        }
        else
        {
            string nextPlayer = (playerTurn == "X") ? "O" : "X";
            int score = 0;

            for (int i = 0; i < currentState.Length; i++)
            {
                if (currentState[i] == spaceState.BLANK)
                {
                    currentState[i] = (nextPlayer == "X") ? spaceState.X : spaceState.O;
                    score += GetNextMove(currentState, nextPlayer, currentMoves);
                    currentState[i] = spaceState.BLANK;
                }
            }
            return score;
        }
    }
}