using TheOrb.Inventories;
using TheOrb.UI.Dragging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.UI.Inventories
{
    public class InventorySlotUi : MonoBehaviour, IItemHolder, IDragContainer<InventoryItem>
    {
        [SerializeField] InventoryItemIcon icon = null;


        int index;
        Inventory inventory;

        public void Setup(Inventory inventory, int index)
        {
            this.inventory = inventory;
            this.index = index;
            icon.SetItem(inventory.GetItemInSlot(index), inventory.GetAmountInSlot(index));
        }
        public void AddItems(InventoryItem item, int amount)
        {
            inventory.AddItemToSlot(index, item, amount);
        }

        public int GetAmount()
        {
            return inventory.GetAmountInSlot(index);
        }

        public InventoryItem GetItem()
        {
            return inventory.GetItemInSlot(index);
        }

        public int MaxAcceptable(InventoryItem item)
        {
            if (GetItem() == null)
            {
                return int.MaxValue;
            }

            return 0;
        }

        public void RemoveItems(int number)
        {
            inventory.RemoveFromSlot(index,int.MaxValue);
        }
    }
}