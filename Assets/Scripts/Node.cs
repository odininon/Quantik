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

        private Vector2Int gridPosition;

        public void Awake()
        {
            material = visual.material;
            defaultColor = material.GetColor("_Color");
            visual.GetComponent<MeshRenderer>().enabled = false;
        }

        public void Highlight(Color highlightColor)
        {
            material.SetColor("_Color", highlightColor);
            visual.GetComponent<MeshRenderer>().enabled = true;
        }

        public void ClearHighlight()
        {
            material.SetColor("_Color", defaultColor);
            visual.GetComponent<MeshRenderer>().enabled = false;
        }

        public void PlacePiece(GamePiece heldPiece, int currentPlayer, Color playerColor)
        {
            heldPiece.Spawn(transform, playerColor);
            gamePiecePlacement = new GamePiecePlacement { gamePiece = heldPiece, placedByPlayer = currentPlayer };
        }

        internal Vector2Int GetPosition()
        {
            return gridPosition;
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

        internal void SetPosition(Vector2Int position)
        {
            this.gridPosition = position;
        }
    }
}