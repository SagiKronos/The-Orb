using TheOrb.Combat.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOrb.UI.Skills
{
    public class SkillsBar : MonoBehaviour
    {
        private EquippedSkillSlot[] skillSlots;
        private PlayerSkillsManager skillsManager;

        private void Start()
        {
            LoadSkillSlots();
            skillsManager = PlayerSkillsManager.GetPlayersSkillsManager();
        }

        private void LoadSkillSlots()
        {
            skillSlots = GetComponentsInChildren<EquippedSkillSlot>();
        }

        public void FillFirstEmptySlot(ActiveSkillConfig skillConfig)
        {
            if (!skillsManager.CanBeUsed(skillConfig.skill.GetId())) return;

            var emptySlot = skillSlots.FirstOrDefault(x => x.GetSkillId() == SkillIds.None);

            if (emptySlot == null)
                return ;

            emptySlot.SetSkill(skillConfig.skill.GetId(), skillConfig.skillSprite);
        }
    }
}
