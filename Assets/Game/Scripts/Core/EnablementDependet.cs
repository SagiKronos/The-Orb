using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.Core
{
    public class EnablementDependet : MonoBehaviour
    {
        [SerializeField] GameObject dependetGameObject;

        private void OnEnable()
        {
            dependetGameObject.SetActive(true);
        }

        private void OnDisable()
        {
            dependetGameObject.SetActive(false);
        }
    }
}
