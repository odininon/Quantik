using System;
using System.Collections.Generic;
using Quantik;
using UnityEngine;

public class GamePlayUI : MonoBehaviour
{
    public PlayerPiecesUI player1;
    public PlayerPiecesUI player2;

    public WinningUI winningUI;

    public TMPro.TMP_Text currentPlayerText;

    private string originalPlayerText;

    public delegate void PieceSelected(int index);
    public PieceSelected pieceSelected;

    void Start()
    {
        originalPlayerText = currentPlayerText.text;
    }

    public void SetCurrentPlayer(Player player)
    {
        currentPlayerText.text = originalPlayerText.Replace("%s", player.Name);

        // var ui = playerNumber == 1 ? player2 : player1;

        // ui.DisableButtons();

        // ui = playerNumber == 1 ? player1 : player2;
        // ui.EnableButtons();
    }

    public void PieceWasSelected(int index)
    {
        pieceSelected(index);
    }

    public void SetListForPlayer(List<GamePiece> list, int currentPlayer)
    {
        var ui = currentPlayer == 0 ? player1 : player2;

        ui.UpdatePlayerList(list);
    }

    public void SetWinner(Player player)
    {
        winningUI.SetWinner(player);
    }

    public void HideMenu() => winningUI.HideMenu();
}
