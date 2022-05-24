using TheOrb.UI.ToolTips;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TheOrb.UI.Inventories
{
    [RequireComponent(typeof(IItemHolder))]
    public class ItemToolTipSpawner : TooltipSpawner
    {
        public override bool CanCreateTooltip()
        {
            return GetComponent<IItemHolder>().GetItem() != null;
        }

        public override void UpdateTooltip(GameObject tooltip)
        {
            var itemTooltip = tooltip.GetComponent<ItemToolTip>();
            if (!itemTooltip) return;

            var item = GetComponent<IItemHolder>().GetItem();
            itemTooltip.Setup(item);
        }
    }
}
