using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Quantik
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject panel;
        public GameManager gameManager;

        public void Start()
        {
            gameManager = GameManager.Instance;
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
            SceneManager.LoadScene("MainMenu");
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