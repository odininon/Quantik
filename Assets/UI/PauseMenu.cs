using System;
using UnityEngine;

namespace Quantik
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject panel;
        public GameManager gameManager;

        public void Start()
        {
            HideMenu();
        }

        public void ShowMenu() => panel.SetActive(true);
        public void HideMenu() => panel.SetActive(false);

        public void RestartGame()
        {
            HideMenu();
            gameManager.RestartGame();
        }

        public void QuitGame()
        {
            HideMenu();
            Application.Quit();
        }

        public void Toggle()
        {
            if (panel.activeInHierarchy)
            {
                HideMenu();
            }
            else
            {
                ShowMenu();
            }
        }
    }
}