using TheOrb.Combat.Skills;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

namespace TheOrb.UI.Skills
{
    public class SkillToolTip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI titleText = null;
        [SerializeField] TextMeshProUGUI bodyText = null;
        [SerializeField] TextMeshProUGUI requirementText = null;

        private PlayerSkillsManager skillsManager;

        public void Setup(ActiveSkillConfig skill)
        {
            titleText.text = skill.GetSkillName();
            bodyText.text = string.Format(skill.GetDescription());
            requirementText.text = GetRequirementsText(skill.skill.GetId());
        }

        // Use this for initialization
        void Awake()
        {
            skillsManager = PlayerSkillsManager.GetPlayersSkillsManager();
        }

        private string GetRequirementsText(SkillIds id)
        {
            StringBuilder message = new StringBuilder();

            var skill = skillsManager.GetActiveSkillConfig(id);
            if (!skillsManager.IsLevelRequirementMet(skill))
                message.AppendLine($"Lvl required: {skill.requiredLvl + skillsManager.GetSkillLevel(id)}");

            var skillReqs = skillsManager.GetMissingSkillRequirements(skill);
            foreach (var skillReq in skillReqs)
            {
                message.AppendLine($"Skill required: {skillsManager.GetActiveSkillConfig(skillReq).GetSkillName()}");

            }

            if (message.ToString() == string.Empty)
                return string.Empty;

            message.Insert(0, $"Requirements:{Environment.NewLine}");
            return message.ToString();
        }
    }
}