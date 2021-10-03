using Quantik;
using UnityEngine;
using UnityEngine.UI;

public class GamePieceUI : MonoBehaviour
{
    public TMPro.TMP_Text text;
    public PlayerPiecesUI parent;

    public Button button;

    void Start()
    {
        parent = GetComponentInParent<PlayerPiecesUI>();
    }

    public void OnPieceSelected()
    {
        parent.PieceWasSelected(this);
    }

    public void SetPiece(GamePiece gamePiece)
    {
        text.text = gamePiece.name;
    }

    public void EnableButtons()
    {
        button.enabled = true;
    }
    public void DisableButtons()
    {
        button.enabled = false;
    }
}
