using System;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.UI
{
    public class TabManager : MonoBehaviour
    {
        [SerializeField] TabInfo[] tabs;
        Dictionary<KeyCode, GameObject> tabLookUp;

        public void ActivateTab(string keyCode)
        {
            var code = (KeyCode) Enum.Parse(typeof(KeyCode), keyCode); 
            ActivateTab(code);
        }

        public void ActivateTab(KeyCode code)
        {
            foreach (var tab in tabs)
            {
                tab.tab.SetActive(tab.code == code);
            }
        }



        [Serializable]
        public struct TabInfo
        {
            [SerializeField] public KeyCode code;
            [SerializeField] public GameObject tab;
        }
    }
}
