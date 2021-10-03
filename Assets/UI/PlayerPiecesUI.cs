using System;
using System.Collections;
using System.Collections.Generic;
using Quantik;
using UnityEngine;

public class PlayerPiecesUI : MonoBehaviour
{
    public int playerNumber;
    public TMPro.TMP_Text title;
    public GamePieceUI piecePrefab;
    public Canvas piecesCanvas;

    public GamePlayUI parent;

    void Start()
    {
        parent = GetComponentInParent<GamePlayUI>();
        title.text = title.text.Replace("%s", playerNumber.ToString());
    }

    public void PieceWasSelected(GamePieceUI gamePieceUI)
    {
        parent.PieceWasSelected(gamePieceUI.transform.GetSiblingIndex());
    }

    public void UpdatePlayerList(List<GamePiece> list)
    {
        foreach (Transform child in piecesCanvas.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var gamePiece in list)
        {
            var piece = Instantiate(piecePrefab, transform.position, transform.rotation, piecesCanvas.transform);
            piece.SetPiece(gamePiece);
        }
    }

    public void EnableButtons()
    {
        foreach (Transform child in piecesCanvas.transform)
        {
            var ui = child.GetComponent<GamePieceUI>();
            ui.EnableButtons();
        }
    }

    public void DisableButtons()
    {
        foreach (Transform child in piecesCanvas.transform)
        {
            var ui = child.GetComponent<GamePieceUI>();
            ui.DisableButtons();
        }
    }
}
