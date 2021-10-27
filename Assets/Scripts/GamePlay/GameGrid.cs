using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quantik
{
    public class GameGrid : MonoBehaviour
    {
        public Vector2Int gridSize;
        public int boardWidth;

        public float paddingTop;
        public float paddingLeft;

        public float nodeOffset;
        public float gizmoSphereWidth = 0.5f;

        [Header("Prefabs")]
        public Node node;


        private Node[] positions;

        void Start()
        {
            Clear();
        }

        public void ClearHighlights()
        {
            foreach (var position in positions)
            {
                position.ClearHighlight();
            }
        }

        public bool IsWinningMove(GameMove move, Player player, List<GamePiece> gamePieces)
        {
            var rowPieces = NeededGamePiecesInRow(move.Position.y, gamePieces);

            if (rowPieces.Count == 1 && rowPieces.Contains(move.Piece))
            {
                return true;
            }

            var columnPieces = NeededGamePiecesInColumn(move.Position.x, gamePieces);

            if (columnPieces.Count == 1 && columnPieces.Contains(move.Piece))
            {
                return true;
            }

            var sectorPieces = NeededGamePiecesInSector(move.Position, gamePieces);

            if (sectorPieces.Count == 1 && sectorPieces.Contains(move.Piece))
            {
                return true;
            }

            return false;
        }

        public Vector2Int GetNodePosition(Node node)
        {
            return node.GetPosition();
        }

        public void HighlightSector(int index)
        {
            var i = gridSize.x * index / 2 + gridSize.x * (index / 2);

            positions[i].Highlight(Color.magenta);
            positions[i + 1].Highlight(Color.magenta);
            positions[i + gridSize.x].Highlight(Color.magenta);
            positions[i + 1 + gridSize.x].Highlight(Color.magenta);
        }

        public bool IsPlacementAllowed(GamePiece heldPiece, Vector2Int selectedNode, Player currentPlayer)
        {
            if (selectedNode.x < 0 || selectedNode.x >= gridSize.x || selectedNode.y < 0 || selectedNode.y >= gridSize.y)
            {
                return false;
            }

            var chosenPosition = selectedNode.y * gridSize.x + selectedNode.x;

            if (!positions[chosenPosition].IsEmpty())
            {
                return false;
            }


            for (var i = 0; i < gridSize.y; i++)
            {
                var position = i * gridSize.x + selectedNode.x;
                var node = positions[position];
                if (node.IsPlacedByOtherPlayer(currentPlayer, heldPiece))
                {
                    return false;
                }
            }

            for (var i = 0; i < gridSize.x; i++)
            {
                var position = selectedNode.y * gridSize.x + i;
                var node = positions[position];
                if (node.IsPlacedByOtherPlayer(currentPlayer, heldPiece))
                {
                    return false;
                }
            }

            var _positions = GetPositionsForSector(GetSectorForPosition(selectedNode));

            foreach (var position in _positions)
            {
                var node = positions[position];
                if (node.IsPlacedByOtherPlayer(currentPlayer, heldPiece))
                {
                    return false;
                }
            }

            return true;
        }

        public void Clear()
        {
            foreach (Transform child in transform)
            {
                if (child.gameObject.GetComponent<Node>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            positions = new Node[gridSize.x * gridSize.y];

            var location = transform.position + new Vector3(-boardWidth / 2 + paddingTop, 0, -boardWidth / 2 + paddingLeft);

            for (var k = 0; k < gridSize.y; k++)
            {
                for (var i = 0; i < gridSize.x; i++)
                {
                    var newLocation = location + new Vector3(2 * paddingTop * k, nodeOffset, 2 * paddingLeft * i);
                    var _node = Instantiate(node, newLocation, Quaternion.identity, transform);
                    _node.SetPosition(new Vector2Int(i, k));
                    positions[k * gridSize.x + i] = _node;
                }
            }

        }

        public Node GetNode(Vector2Int position)
        {
            return positions[position.y * gridSize.x + position.x];
        }

        public void HighlightForPosition(Vector2Int position, Color highlight)
        {
            HighlightColum(position.x, highlight);
            HighlightRow(position.y, highlight);
            HighlightSector(position, highlight);
        }

        public List<GameMove> GetValidMoves(Player player)
        {
            var moves = new List<GameMove>();
            var pieces = player.GetUniquePieces();

            foreach (var piece in pieces)
            {
                for (var k = 0; k < gridSize.y; k++)
                {
                    for (var i = 0; i < gridSize.x; i++)
                    {
                        var selectedPosition = new Vector2Int(i, k);
                        if (IsPlacementAllowed(piece, selectedPosition, player))
                        {
                            var move = new GameMove
                            {
                                Piece = piece,
                                Position = selectedPosition
                            };
                            moves.Add(move);
                        }
                    }
                }
            }

            return moves;
        }


        private void HighlightColum(int column, Color highlight)
        {
            for (var i = 0; i < gridSize.y; i++)
            {
                var position = i * gridSize.x + column;
                positions[position].Highlight(highlight);
            }
        }

        private void HighlightRow(int row, Color highlight)
        {
            for (var i = 0; i < gridSize.x; i++)
            {
                positions[row * gridSize.x + i].Highlight(highlight);
            }
        }

        private void HighlightSector(Vector2Int position, Color highlight)
        {
            var _positions = GetPositionsForSector(GetSectorForPosition(position));

            foreach (var pos in _positions)
            {
                positions[pos].Highlight(highlight);
            }
        }

        private int GetSectorForPosition(Vector2Int position)
        {
            return GetSectorForPosition(position.y * gridSize.x + position.x);
        }

        private List<int> GetPositionsForSector(int sector)
        {
            List<int> positions = new List<int>();
            var i = gridSize.x * sector / 2 + gridSize.x * (sector / 2);

            positions.Add(i);
            positions.Add(i + 1);
            positions.Add(i + gridSize.x);
            positions.Add(i + gridSize.x + 1);

            return positions;
        }

        private int GetSectorForPosition(int position)
        {

            var row = position / gridSize.x;

            if (row % 2 != 0)
            {
                position -= gridSize.x;
            }

            var column = position % gridSize.x;

            if (column % 2 != 0)
            {
                position -= 1;
            }

            return (int)Math.Ceiling(position / 4.0);
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(boardWidth, 0, boardWidth));

            var location = transform.position + new Vector3(-boardWidth / 2 + paddingTop, 0, -boardWidth / 2 + paddingLeft);

            for (var k = 0; k < gridSize.y; k++)
            {
                for (var i = 0; i < gridSize.x; i++)
                {
                    var newLocation = location + new Vector3(2 * paddingTop * k, 0, 2 * paddingLeft * i);
                    Gizmos.DrawWireSphere(newLocation, gizmoSphereWidth);
                }
            }
        }

        private List<GamePiece> NeededGamePiecesInRow(int y, List<GamePiece> gamePieces)
        {
            for (var i = 0; i < gridSize.x; i++)
            {
                var position = y * gridSize.x + i;
                var piece = positions[position].gamePiecePlacement.gamePiece;

                gamePieces.Remove(piece);
            }

            return gamePieces;
        }

        public bool DoesRowContainAllPieces(int y, List<GamePiece> gamePieces)
        {
            return NeededGamePiecesInRow(y, gamePieces).Count == 0;
        }

        private List<GamePiece> NeededGamePiecesInColumn(int x, List<GamePiece> gamePieces)
        {
            for (var i = 0; i < gridSize.y; i++)
            {
                var position = i * gridSize.x + x;
                var piece = positions[position].gamePiecePlacement.gamePiece;

                gamePieces.Remove(piece);
            }

            return gamePieces;
        }

        public bool DoesColumnContainAllPieces(int x, List<GamePiece> gamePieces)
        {
            return NeededGamePiecesInColumn(x, gamePieces).Count == 0;
        }

        private List<GamePiece> NeededGamePiecesInSector(Vector2Int selectedNode, List<GamePiece> gamePieces)
        {
            var _positions = GetPositionsForSector(GetSectorForPosition(selectedNode));

            foreach (var position in _positions)
            {
                var piece = positions[position].gamePiecePlacement.gamePiece;
                gamePieces.Remove(piece);
            }

            return gamePieces;
        }

        public bool DoesNodeSectorContainAllPieces(Vector2Int selectedNode, List<GamePiece> gamePieces)
        {
            return NeededGamePiecesInSector(selectedNode, gamePieces).Count == 0;
        }
    }
}
