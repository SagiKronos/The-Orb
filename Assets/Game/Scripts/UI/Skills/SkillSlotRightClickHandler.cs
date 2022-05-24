using TheOrb.Combat.Skills;

namespace TheOrb.UI.Skills
{
    public class SkillSlotRightClickHandler : UIClickHandler<ActiveSkillConfig>
    {
        private void Update()
        {
            data = GetComponent<SkillSlot>().GetSkill();
        }
    }
}
