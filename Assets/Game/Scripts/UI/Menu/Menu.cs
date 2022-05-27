using System.Collections;
using System.Collections.Generic;
using TheOrb.Saving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheOrb.UI.Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] GameObject buttonsPanel;

        public void NewGame()
        {
            buttonsPanel.SetActive(false);
            SceneManager.LoadScene(1);
        }

        public void SaveGame()
        {
            SavingSystem.Instance.Save();
        }

        public void LoadGame()
        {
            buttonsPanel.SetActive(false);
            SavingSystem.Instance.Load();
        }

        public void LoadMainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            Application.Quit();
        }
    }

}