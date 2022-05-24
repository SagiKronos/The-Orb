using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOrb.UI
{
    public class ShowHideUI : MonoBehaviour
    {
        [SerializeField] KeyCode[] toggleKeys = new KeyCode[0];
        [SerializeField] GameObject uiContainer = null;

        private KeyCode lastKeyCode;

        private void Start()
        {
            uiContainer.SetActive(false);
        }

        private void Update()
        {
            foreach (var toggleKey in toggleKeys)
            {
                if (Input.GetKeyDown(toggleKey))
                {
                    if (toggleKey == lastKeyCode || !uiContainer.activeInHierarchy)
                    {
                        ChangeContainerActivity(!uiContainer.activeSelf);
                    }

                    ActivateRelevantTab(toggleKey);

                    break;
                }
            }
        }

        private void ActivateRelevantTab(KeyCode toggleKey)
        {
            uiContainer.GetComponentInChildren<TabManager>()?.ActivateTab(toggleKey);
            lastKeyCode = toggleKey;
        }

        public void Show()
        {
            ChangeContainerActivity(true);
        }


        public void Show(string keyCode)
        {
            Show();
            var code = (KeyCode)Enum.Parse(typeof(KeyCode), keyCode);
            uiContainer.GetComponentInChildren<TabManager>()?.ActivateTab(code);
            lastKeyCode = code;
        }

        public void Hide()
        {
            ChangeContainerActivity(false);
        }

        private void ChangeContainerActivity(bool isActive)
        {
            uiContainer.SetActive(isActive);
        }
    }
}