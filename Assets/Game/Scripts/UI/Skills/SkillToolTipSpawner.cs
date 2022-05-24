using TheOrb.UI.ToolTips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOrb.UI.Skills
{
    [RequireComponent(typeof(ISkillHolder))]
    public class SkillToolTipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return GetComponent<ISkillHolder>().GetSkill() != null;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<SkillToolTip>();
            if (!itemTooltip) return;

            var item = GetComponent<ISkillHolder>().GetSkill();
            itemTooltip.Setup(item);
        }
    }
}
