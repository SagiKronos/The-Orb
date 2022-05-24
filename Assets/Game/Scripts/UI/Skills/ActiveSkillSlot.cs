using System.Linq;
using TheOrb.Combat.Skills;
using TheOrb.Saving;
using UnityEngine;

namespace TheOrb.UI.Skills
{
    public class ActiveSkillSlot : MonoBehaviour, ISaveable
    {
        [SerializeField] EquippedSkillSlot activeSkillSlot;


        public void ActiveSkillChanged(ActiveSkillConfig activeSkillConfig)
        {
            if (activeSkillConfig == null)
                activeSkillSlot.SetSkill(SkillIds.None, null);
            else
                activeSkillSlot.SetSkill(activeSkillConfig.skill.GetId(), activeSkillConfig.skillSprite);  
        }

        public object CaptureState()
        {
            return activeSkillSlot.GetSkillId();
        }

        public void RestoreState(object state)
        {
            var activeSkill = Resources.LoadAll<ActiveSkillConfig>("").FirstOrDefault(x => x.skill.GetId() == (SkillIds)state);
            ActiveSkillChanged(activeSkill);
        }
    }
}
