using UnityEngine;

namespace Quantik
{
    [CreateAssetMenu]
    public class GamePiece : ScriptableObject
    {
        public Renderer visualPrefab;

        public void Spawn(Transform transform, Color playerColor)
        {
            var piece = Instantiate(visualPrefab, transform.position, transform.rotation, transform);
            piece.material.SetColor("_Color", playerColor);
        }
    }
}