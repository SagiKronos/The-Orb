using TheOrb.Combat.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.UI.Skills
{
    public class ShowOnFreeSkillPoints : MonoBehaviour
    {
        [SerializeField] GameObject objToShow;
        [SerializeField] TextMeshProUGUI amountText;
        private PlayerSkillsManager skillsManager;


        private void Start()
        {
            skillsManager = PlayerSkillsManager.GetPlayersSkillsManager();
        }

        private void Update()
        {
            var freePoints = skillsManager.GetFreeSkillPoints();
            objToShow.SetActive(freePoints > 0);
            amountText.text = freePoints < 10 ? freePoints.ToString() : "+9";
        }
    }
}
