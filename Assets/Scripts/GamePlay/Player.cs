using System;
using System.Collections.Generic;
using UnityEngine;

namespace Quantik
{
    [CreateAssetMenu(fileName = "Player", menuName = "Quantik/Player/Human")]
    public class Player : ScriptableObject
    {
        public Color Color;
        public string Name;

        public List<GamePiece> Pieces = new List<GamePiece>();

        public GamePiece GetPiece(int selectedPiece)
        {
            return Pieces[selectedPiece];
        }

        public HashSet<GamePiece> GetUniquePieces()
        {
            return new HashSet<GamePiece>(Pieces);
        }

        virtual public void EndTurn()
        {
        }

        virtual public void StartTurn()
        {
        }
    }
}