using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Quantik
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager _instance;

        public static GameManager Instance
        {
            get
            {

                return _instance;
            }
        }

        public GamePlayUI gamePlayUI;
        public PauseMenu pauseMenu;

        [Header("Highlights")]
        public Color validHighlight;
        public Color invalidHighlight;

        [Header("Player Pieces")]
        public HashSet<GamePiece> uniquePieces = new HashSet<GamePiece>();

        public GamePieceLoadout gamePieceLoadout;

        [Header("Game State")]
        public int selectedPiece = -1;

        public Player[] players = new Player[2];
        public int currentPlayerIndex = 0;

        public GameGrid gameBoard;
        private Camera mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            mainCamera = Camera.main;
            gameBoard = FindObjectOfType<GameGrid>();

            RestartGame();
        }

        void Awake()
        {
            _instance = this;
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

                Vector2Int selectedNode = gameBoard.GetNodePosition(node);

                var currentPlayer = players[currentPlayerIndex];
                var heldPiece = currentPlayer.GetPiece(selectedPiece);

                if (heldPiece != null)
                {
                    var isValidPlacement = gameBoard.IsPlacementAllowed(heldPiece, selectedNode, currentPlayer);
                    gameBoard.HighlightForPosition(selectedNode, isValidPlacement ? validHighlight : invalidHighlight);

                    if (Input.GetMouseButtonDown(0) && isValidPlacement)
                    {
                        var move = new GameMove
                        {
                            Piece = heldPiece,
                            Position = selectedNode
                        };
                        ExecuteMove(move, currentPlayer);
                    }
                }
            }
        }

        public bool IsWinningMove(GameMove move, ComputerPlayer computerPlayer)
        {
            return this.gameBoard.IsWinningMove(move, computerPlayer, uniquePieces.ToList());
        }

        public void ExecuteMove(GameMove move, Player player)
        {
            var node = gameBoard.GetNode(move.Position);
            node.PlacePiece(move.Piece, player);
            removePieceForPlayer(move.Piece, player);
            hasPlayerWon(move.Position);

            player.EndTurn();
            selectedPiece = -1;
            if (++currentPlayerIndex > players.Length - 1)
            {
                currentPlayerIndex = 0;
            }
            player = players[currentPlayerIndex];
            gamePlayUI.SetCurrentPlayer(player);
            player.StartTurn();
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

            players[0].Pieces.Clear();
            players[1].Pieces.Clear();

            players[0].Pieces.AddRange(playerPieces);
            players[1].Pieces.AddRange(playerPieces);

            gamePlayUI.SetListForPlayer(players[0].Pieces, 0);
            gamePlayUI.SetListForPlayer(players[1].Pieces, 1);

            gamePlayUI.pieceSelected += (index) =>
            {
                selectedPiece = index;
            };

            var currentPlayer = players[currentPlayerIndex];

            gamePlayUI.SetCurrentPlayer(currentPlayer);
        }

        private void hasPlayerWon(Vector2Int selectedNode)
        {
            var row = gameBoard.DoesRowContainAllPieces(selectedNode.y, uniquePieces.ToList());
            var column = gameBoard.DoesColumnContainAllPieces(selectedNode.x, uniquePieces.ToList());
            var sector = gameBoard.DoesNodeSectorContainAllPieces(selectedNode, uniquePieces.ToList());

            if (row || column || sector)
            {
                var currentPlayer = players[currentPlayerIndex];
                gamePlayUI.SetWinner(currentPlayer);
            }
        }

        private void removePieceForPlayer(GamePiece gamePiece, Player currentPlayer)
        {
            var list = currentPlayer.Pieces;
            list.Remove(gamePiece);

            gamePlayUI.SetListForPlayer(list, currentPlayerIndex);
        }
    }
}