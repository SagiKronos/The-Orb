using TheOrb.Combat.Skills;
using TheOrb.UI.Dragging;
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
    public class SkillSlot : MonoBehaviour, IDragSource<ActiveSkillConfig>, ISkillHolder
    {
        [SerializeField] Image imageSlot;
        [SerializeField] Image disabledSkillImage;
        [SerializeField] GameObject learnSkillButton;
        [SerializeField] GameObject skillLevelGameObj;

        private ActiveSkillConfig activeSkillConfig;
        private PlayerSkillsManager SkillsManager
        {
            get
            {
                if (skillsManager == null)
                {
                    skillsManager = PlayerSkillsManager.GetPlayersSkillsManager();
                }

                return skillsManager;
            }
        }
        private PlayerSkillsManager skillsManager;

        void Update()
        {
            var skillId = activeSkillConfig.skill.GetId();
            disabledSkillImage.gameObject.SetActive(!SkillsManager.CanBeUsed(skillId));
            learnSkillButton.gameObject.SetActive(SkillsManager.CanBeLearned(skillId));

            var skillLevel = skillsManager.GetSkillLevel(skillId);
            skillLevelGameObj.gameObject.SetActive(skillLevel > 0);
            skillLevelGameObj.GetComponentInChildren<TextMeshProUGUI>().text = skillLevel.ToString();
        }

        public void LearnSkill()
        {
            skillsManager.LearnSkill(activeSkillConfig.skill.GetId());
        }

        public void SetSkill(SkillIds skillId)
        {
            activeSkillConfig = SkillsManager.GetActiveSkillConfig(skillId);
            imageSlot.sprite = activeSkillConfig.skillSprite;
        }

        public ActiveSkillConfig GetSkill()
        {
            return activeSkillConfig;
        }

        int IDragSource<ActiveSkillConfig>.GetAmount()
        {
            return 1;
        }

        ActiveSkillConfig IDragSource<ActiveSkillConfig>.GetItem()
        {
            var skill = GetSkill();
            if (skillsManager.CanBeUsed(skill.skill.GetId()))
                return skill;
            return null;
        }

        void IDragSource<ActiveSkillConfig>.RemoveItems(int number)
        {
        }

    }
}
