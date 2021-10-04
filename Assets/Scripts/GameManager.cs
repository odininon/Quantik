using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Quantik
{
    [RequireComponent(typeof(GameGrid))]
    public class GameManager : MonoBehaviour
    {
        public GamePlayUI gamePlayUI;
        public PauseMenu pauseMenu;

        [Header("Highlights")]
        public Color validHighlight;
        public Color invalidHighlight;

        [Header("Player Colors")]
        public Color player1Color;
        public Color player2Color;

        [Header("Player Pieces")]
        public HashSet<GamePiece> uniquePieces = new HashSet<GamePiece>();
        public GamePieceLoadout gamePieceLoadout;
        private List<GamePiece> player1Pieces = new List<GamePiece>();
        private List<GamePiece> player2Pieces = new List<GamePiece>();

        [Header("Game State")]
        public int currentPlayer = 0;
        public int selectedPiece = -1;

        private GameGrid gameBoard;
        private Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
            gameBoard = GetComponent<GameGrid>();

            RestartGame();
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                pauseMenu.Toggle();
            }

            gameBoard.ClearHighlights();

            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit))
            {
                var node = hit.collider.GetComponentInParent<Node>();

                if (!node.IsEmpty() || selectedPiece == -1)
                {
                    return;
                }

                Vector2Int? selectedNode = gameBoard.GetNodePosition(node);

                var heldPiece = currentPlayer == 0 ? player1Pieces[selectedPiece] : player2Pieces[selectedPiece];

                if (selectedNode.HasValue && heldPiece != null)
                {
                    var isValidPlacement = gameBoard.IsPlacementAllowed(heldPiece, selectedNode.Value, currentPlayer);
                    gameBoard.HighlightForPosition(selectedNode.Value, isValidPlacement ? validHighlight : invalidHighlight);

                    if (Input.GetMouseButtonDown(0) && isValidPlacement)
                    {
                        node.PlacePiece(heldPiece, currentPlayer, currentPlayer == 0 ? player1Color : player2Color);
                        removePieceForPlayer(currentPlayer);
                        hasPlayerWon(selectedNode.Value);

                        selectedPiece = -1;
                        if (++currentPlayer > 1)
                        {
                            currentPlayer = 0;
                        }
                        gamePlayUI.SetCurrentPlayer(currentPlayer + 1);
                    }
                }
            }
        }

        public void RestartGame()
        {
            gamePlayUI.HideMenu();
            pauseMenu.HideMenu();
            gameBoard.Clear();
            var playerPieces = gamePieceLoadout.pieces;

            uniquePieces.Clear();

            foreach (var piece in playerPieces)
            {
                uniquePieces.Add(piece);
            }

            player1Pieces.Clear();
            player2Pieces.Clear();

            player1Pieces.AddRange(playerPieces);
            player2Pieces.AddRange(playerPieces);

            gamePlayUI.SetListForPlayer(player1Pieces, 0);
            gamePlayUI.SetListForPlayer(player2Pieces, 1);

            gamePlayUI.pieceSelected += (index) =>
            {
                selectedPiece = index;
            };

            gamePlayUI.SetCurrentPlayer(currentPlayer + 1);
        }

        private void hasPlayerWon(Vector2Int selectedNode)
        {
            //Check row
            var row = gameBoard.DoesRowContainAllPieces(selectedNode.y, uniquePieces.ToList());
            var column = gameBoard.DoesColumnContainAllPieces(selectedNode.x, uniquePieces.ToList());
            var sector = gameBoard.DoesNodeSectorContainAllPieces(selectedNode, uniquePieces.ToList());

            if (row || column || sector)
            {
                gamePlayUI.SetWinner(currentPlayer + 1);
            }
        }

        private void removePieceForPlayer(int currentPlayer)
        {
            var list = currentPlayer == 0 ? player1Pieces : player2Pieces;
            list.RemoveAt(selectedPiece);

            gamePlayUI.SetListForPlayer(list, currentPlayer);
        }
    }
}