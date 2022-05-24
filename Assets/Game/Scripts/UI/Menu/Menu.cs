using System.Collections;
using System.Collections.Generic;
using TheOrb.Saving;
using UnityEngine;

namespace TheOrb.UI.Menu
{
    public class Menu : MonoBehaviour
    {
        [SerializeField] GameObject buttonsPanel;

        public void StartLoading()
        {
            buttonsPanel.SetActive(false);
        }


        public void Exit()
        {
            Application.Quit();
        }
    }

}