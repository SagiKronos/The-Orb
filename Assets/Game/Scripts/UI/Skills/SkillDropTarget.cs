using TheOrb.Combat.Skills;
using TheOrb.UI.Dragging;
using UnityEngine;

namespace TheOrb.UI.Skills
{
    public class SkillDropTarget : MonoBehaviour, IDragDestination<ActiveSkillConfig>
    {
        public void AddItems(ActiveSkillConfig item, int amount)
        {
        }

        public int MaxAcceptable(ActiveSkillConfig item)
        {
            return int.MaxValue;
        }
    }
}
