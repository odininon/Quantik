using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quantik
{
    public class GameGrid : MonoBehaviour
    {
        public Vector2Int gridSize;
        public int boardWidth;
        public float gizmoSphereWidth = 0.5f;

        [Header("Prefabs")]
        public Node node;
        private Node[] positions;

        void Start()
        {
            positions = new Node[gridSize.x * gridSize.y];

            for (var k = 0; k < gridSize.y; k++)
            {
                for (var i = 0; i < gridSize.x; i++)
                {
                    var location = transform.position;
                    location += new Vector3(k * boardWidth / (gridSize.y - 1), 0, i * boardWidth / (gridSize.x - 1));
                    location -= new Vector3(boardWidth / 2, 0, boardWidth / 2);
                    var _node = Instantiate(node, location, Quaternion.identity, transform);

                    positions[k * gridSize.x + i] = _node;
                }
            }
        }

        public void ClearHighlights()
        {
            foreach (var position in positions)
            {
                position.ClearHighlight();
            }
        }

        public Vector2Int? GetNodePosition(Node node)
        {
            Vector2Int? selectedNode = null;
            for (var i = 0; i < positions.Length; i++)
            {
                if (positions[i] == node)
                {
                    var x = i % gridSize.x;
                    var y = i / gridSize.x;

                    selectedNode = new Vector2Int(x, y);
                    break;
                }
            }

            return selectedNode;
        }

        public void HighlightSector(int index)
        {
            var i = gridSize.x * index / 2 + gridSize.x * (index / 2);

            positions[i].Highlight(Color.magenta);
            positions[i + 1].Highlight(Color.magenta);
            positions[i + gridSize.x].Highlight(Color.magenta);
            positions[i + 1 + gridSize.x].Highlight(Color.magenta);
        }

        public bool IsPlacementAllowed(GamePiece heldPiece, Vector2Int selectedNode, int currentPlayer)
        {
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

            return true;
        }

        public void HighlightForPosition(Vector2Int position, Color highlight)
        {
            HighlightColum(position.x, highlight);
            HighlightRow(position.y, highlight);
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


        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, new Vector3(boardWidth, 0, boardWidth));

            for (var k = 0; k < gridSize.y; k++)
            {
                for (var i = 0; i < gridSize.x; i++)
                {
                    var location = transform.position;
                    location += new Vector3(k * boardWidth / (gridSize.y - 1), 0, i * boardWidth / (gridSize.x - 1));
                    location -= new Vector3(boardWidth / 2, 0, boardWidth / 2);
                    Gizmos.DrawWireSphere(location, gizmoSphereWidth);
                }
            }
        }

        public bool DoesRowContainAllPieces(int y, List<GamePiece> gamePieces)
        {
            for (var i = 0; i < gridSize.x; i++)
            {
                var position = y * gridSize.x + i;
                var piece = positions[position].gamePiecePlacement.gamePiece;

                gamePieces.Remove(piece);
            }

            return gamePieces.Count == 0;
        }

        public bool DoesColumnContainAllPieces(int x, List<GamePiece> gamePieces)
        {
            for (var i = 0; i < gridSize.y; i++)
            {
                var position = i * gridSize.x + x;
                var piece = positions[position].gamePiecePlacement.gamePiece;

                gamePieces.Remove(piece);
            }

            return gamePieces.Count == 0;
        }

        public bool DoesNodeSectorContainAllPieces(Vector2Int selectedNode, List<GamePiece> gamePieces)
        {
            var p = selectedNode.y * gridSize.x + selectedNode.x;

            var sector = GetSectorForPosition(p);
            var intPositions = GetPositionsForSector(sector);

            foreach (var position in intPositions)
            {
                var piece = positions[position].gamePiecePlacement.gamePiece;
                gamePieces.Remove(piece);
            }

            return gamePieces.Count == 0;
        }
    }
}