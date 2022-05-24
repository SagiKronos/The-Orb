using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOrb.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.UI.Stats
{
    public class PlayerHealthDisplay : MonoBehaviour
    {
        [SerializeField] Health playerHealth;
        [SerializeField] Image fillImage;

        private void Update()
        {
            fillImage.fillAmount = playerHealth.GetFraction();
        }
    }
}
 