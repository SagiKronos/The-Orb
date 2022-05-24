using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.Inventories
{
    [CreateAssetMenu(menuName = "The Orb/Inventory/Item")]
    public class InventoryItem : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] string itemID = null;
        [SerializeField] string displayName = null;
        [SerializeField] [TextArea] string description = null;
        [SerializeField] Sprite icon = null;
        [SerializeField] Pickup pickup = null;
        [SerializeField] bool stackable = false;
        [SerializeField] ItemType type;
        [SerializeField] WeaponConfig wepConfig;

        static Dictionary<string, InventoryItem> itemLookupCache;

        private static void LoadCache()
        {
            itemLookupCache = new Dictionary<string, InventoryItem>();
            var items = Resources.LoadAll<InventoryItem>("");
            foreach (var item in items)
            {
                itemLookupCache.Add(item.itemID, item);
            }
        }

        public static InventoryItem GetFromID(string itemID)
        {
            if (itemLookupCache == null)
                LoadCache();

            return itemID != null && itemLookupCache.ContainsKey(itemID) ? itemLookupCache[itemID]: null;
        }

        public Pickup SpawnPickup(Vector3 position, int amount)
        {
            var pickup = Instantiate(this.pickup);
            pickup.transform.position = position;
            pickup.Setup(this,amount);
            return pickup;
        }

        public Sprite GetIcon()
        {
            return icon;
        }

        public string GetItemID()
        {
            return itemID;
        }

        public bool IsStackable()
        {
            return stackable;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public string GetDescription()
        {
            return description;
        }

        public ItemType GetItemType()
        {
            return type;
        }

        public WeaponConfig GetWeaponConfig()
        {
            return type == ItemType.Weapon ? wepConfig : null;
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize() { }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (itemID == null)
            {
                itemID = Guid.NewGuid().ToString();
            }
        }
    }
}