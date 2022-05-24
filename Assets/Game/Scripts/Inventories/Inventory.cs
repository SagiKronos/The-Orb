using TheOrb.Saving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TheOrb.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable
    {
        // CONFIG DATA
        [Tooltip("Allowed size")]
        [SerializeField] int inventorySize = 16;

        // STATE
        InventorySlot[] slots;

        public event Action inventoryUpdated;

        private void Awake()
        {
            slots = new InventorySlot[inventorySize];
        }

        public static Inventory GetPlayerInventory()
        {
            return GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        }

        public bool HasSpaceFor(InventoryItem item)
        {
            return FindSlot(item) >= 0;
        }

        public int GetSize()
        {
            return slots.Length;
        }

        public bool AddToFirstEmptySlot(InventoryItem item, int amount)
        {
            var slotId = FindSlot(item);
            
            if (slotId < 0)
                return false;

            if (slots[slotId].item == null)
            {
                slots[slotId] = new InventorySlot { item = item, amount = amount };
            }
            else
            {
                slots[slotId].amount += amount;
            }

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }

        public bool HasItem(InventoryItem item)
        {
            return slots.Any(x => ReferenceEquals(x, item));
        }

        public InventoryItem GetItemInSlot(int slot)
        {
            return slots[slot].item;
        }

        public int GetAmountInSlot(int slot)
        {
            return slots[slot].amount;
        }

        public void RemoveFromSlot(int slot, int amount)
        {
            slots[slot].amount -= amount;
            if (slots[slot].amount <= 0)
            {
                slots[slot].amount = 0;
                slots[slot].item = null;
            }

            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        }

        public bool AddItemToSlot(int slot, InventoryItem item, int amount)
        {
            if (slots[slot].item != null)
            {
                return AddToFirstEmptySlot(item, amount);
            }

            slots[slot] = new InventorySlot { item = item, amount = amount };
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
            return true;
        }
        

        private int FindSlot(InventoryItem item)
        {
            if (item.IsStackable()) 
            {
                for (int i = 0; i < slots.Length; i++)
                {
                    if (slots[i].item?.GetItemID() == item.GetItemID())
                        return i;
                }

            }

            return FindEmptySlot();
        }

        private int FindEmptySlot()
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                    return i;
            }

            return -1;
        }

        object ISaveable.CaptureState()
        {
            return slots.Select(x => new InventorySlotRecord { itemID = x.item?.GetItemID(),amount = x.amount}).ToArray();
        }

        void ISaveable.RestoreState(object state)
        {
            var item = (InventorySlotRecord[])state;
            slots = item.Select(x => new InventorySlot{item = InventoryItem.GetFromID(x.itemID), amount = x.amount}).ToArray();
            if (inventoryUpdated != null)
            {
                inventoryUpdated();
            }
        } 

        [Serializable]
        public struct InventorySlot
        {
            public InventoryItem item { get; set; }
            public int amount { get; set; }
        }

        [System.Serializable]
        private struct InventorySlotRecord
        {
            public string itemID;
            public int amount;
        }
    }
}