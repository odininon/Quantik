using System;
using UnityEngine;

namespace Quantik
{
    public struct GamePiecePlacement
    {
        public GamePiece gamePiece;
        public int placedByPlayer;
    }

    public class Node : MonoBehaviour
    {
        public Renderer visual;
        public GamePiecePlacement gamePiecePlacement;

        private Color defaultColor;
        private Material material;

        void Awake()
        {
            material = visual.material;
            defaultColor = material.GetColor("_Color");
        }

        public void Highlight(Color highlightColor)
        {
            material.SetColor("_Color", highlightColor);
        }

        public void ClearHighlight()
        {
            material.SetColor("_Color", defaultColor);
        }

        public void PlacePiece(GamePiece heldPiece, int currentPlayer, Color playerColor)
        {
            heldPiece.Spawn(transform, playerColor);
            gamePiecePlacement = new GamePiecePlacement { gamePiece = heldPiece, placedByPlayer = currentPlayer };
        }

        public bool IsPlacedByOtherPlayer(int currentPlayer, GamePiece gamePiece)
        {
            if (IsEmpty())
            {
                return false;
            }

            if (IsPlacedByPlayer(currentPlayer))
            {
                return false;
            }

            if (gamePiecePlacement.gamePiece != gamePiece)
            {
                return false;
            }

            return true;
        }

        public bool IsEmpty()
        {
            return gamePiecePlacement.gamePiece == null;
        }

        private bool IsPlacedByPlayer(int currentPlayer)
        {
            return gamePiecePlacement.placedByPlayer == currentPlayer;
        }
    }
}