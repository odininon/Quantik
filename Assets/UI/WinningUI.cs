using UnityEngine;

public class WinningUI : MonoBehaviour
{
    public TMPro.TMP_Text text;

    public GameObject panel;

    public void Start()
    {
        HideMenu();
    }

    public void HideMenu() => panel.SetActive(false);

    public void SetWinner(int playerNumber)
    {
        panel.SetActive(true);

        text.text = text.text.Replace("%s", playerNumber.ToString());
    }
}
