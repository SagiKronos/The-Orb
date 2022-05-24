using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOrb.Stats;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.UI.Stats
{
    public class PlayerExpDisplay : MonoBehaviour
    {
        [SerializeField] BaseStats playerStats;
        [SerializeField] Image expFill;
        [SerializeField] Text expPercentageText;
        [SerializeField] Text levelText;

        private void Update()
        {
            var fraction = playerStats.GetExperienceFraction();
            expFill.fillAmount = fraction;
            expPercentageText.text = $"{fraction * 100:n2}%";
            levelText.text = $"Lvl {playerStats.GetLevel()}";
        }
    }
}
