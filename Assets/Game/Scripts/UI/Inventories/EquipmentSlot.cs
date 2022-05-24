using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOrb.Inventories;
using TheOrb.Saving;
using TheOrb.UI.Dragging;
using UnityEngine;

namespace TheOrb.UI.Inventories
{
    public class EquipmentSlot : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>, ISaveable
    {
        [SerializeField] ItemType equipmentType;
        [SerializeField] InventoryItemIcon icon = null;
        private string itemId;

        Equipment equipment;

        private void Awake()
        {
            equipment = GameObject.FindGameObjectWithTag("Player").GetComponent<Equipment>();
        }

        public void AddItems(InventoryItem item, int amount)
        {
            if (equipmentType == item.GetItemType())
            {
                itemId = item.GetItemID();
                equipment.EquipItem(item);
                icon.SetItem(item, 1);
            }
        }

        public int GetAmount()
        {
            return 1;
        }

        public InventoryItem GetItem()
        {
            return equipment.GetItem(equipmentType);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            return item.GetItemType() == equipmentType ? 1 : 0;

        }

        public void RemoveItems(int number)
        {
            equipment.RemoveItem(equipmentType);
            icon.SetItem(null, 0);
        }

        public object CaptureState()
        {
            return itemId;
        }

        public void RestoreState(object state)
        {
            RemoveItems(0);
            if (state != null)
                AddItems(Resources.LoadAll<InventoryItem>("").FirstOrDefault(x => x.GetItemID() == state.ToString()), 1);
        }
    }
}
