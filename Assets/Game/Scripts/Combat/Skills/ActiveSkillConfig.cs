using System;
using UnityEngine;

namespace TheOrb.Combat.Skills
{
    [CreateAssetMenu(menuName = "The Orb/Skills/Make new skill")]
    public class ActiveSkillConfig : ScriptableObject
    {
        [SerializeField] public ActiveSkill skill;
        [SerializeField] public Sprite skillSprite;
        [SerializeField] public int requiredLvl;
        [SerializeField] public AnimatorOverrideController skillOverride = null;
        [SerializeField] public SkillIds[] SkillRequirements;
        [SerializeField] string displayName;
        [SerializeField] string descirptionFormat;

        public float timeSinceLastActivated;


        public string GetSkillName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return string.Format(descirptionFormat, skill.GetParams());
        }
    }
}
