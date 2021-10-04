using System.Collections.Generic;
using UnityEngine;

namespace Quantik
{
    [CreateAssetMenu]
    public class GamePieceLoadout : ScriptableObject
    {
        public List<GamePiece> pieces;
    }
}