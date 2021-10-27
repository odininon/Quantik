using UnityEngine;

namespace Quantik
{
    [CreateAssetMenu(fileName = "ComputerPlayer", menuName = "Quantik/Player/Computer")]
    public class ComputerPlayer : Player
    {
        override public void StartTurn()
        {
            base.StartTurn();

            // Get the list of valid moves for the current player
            var validMoves = GameManager.Instance.gameBoard.GetValidMoves(this);

            // Check to see if any of the valid moves are a win
            foreach (var move in validMoves)
            {
                if (GameManager.Instance.IsWinningMove(move, this))
                {
                    GameManager.Instance.ExecuteMove(move, this);
                    return;
                }
            }

            // Choose a move at random from the valid move list 
            var randomMove = validMoves[Random.Range(0, validMoves.Count)];

            // Execute the move
            GameManager.Instance.ExecuteMove(randomMove, this);
        }

        override public void EndTurn()
        {
            base.EndTurn();
        }
    }
}