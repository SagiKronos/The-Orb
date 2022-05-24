using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheOrb.Saving;
using UnityEngine;
using UnityEngine.Events;

namespace TheOrb.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        IDictionary<ItemType, InventoryItem> equipment;
        [SerializeField] UnityEvent<WeaponConfig> weaponEquiped;
        [SerializeField] UnityEvent weaponRemoved;

        private void Awake()
        {
            equipment = new Dictionary<ItemType, InventoryItem>();
            foreach (var val in Enum.GetValues(typeof(ItemType)))
            {
                equipment.Add((ItemType)val, null);
            }
        }

        public void EquipItem(InventoryItem item)
        {
            equipment[item.GetItemType()] = item;
            if (item.GetItemType() == ItemType.Weapon)
                weaponEquiped.Invoke(item.GetWeaponConfig());
        }

        public InventoryItem GetItem(ItemType type)
        {
            return equipment[type];
        }

        public void RemoveItem(ItemType type)
        {
            equipment[type] = null;

            if (type == ItemType.Weapon)
                weaponRemoved.Invoke();
        }

        public object CaptureState()
        {
            return equipment[ItemType.Weapon]?.GetItemID();
        }

        public void RestoreState(object state)
        {
            if (state == null) return;

            var item = Resources.LoadAll<InventoryItem>("").First(x => x.GetItemID() == state.ToString());
            EquipItem(item);
        }
    }
}
