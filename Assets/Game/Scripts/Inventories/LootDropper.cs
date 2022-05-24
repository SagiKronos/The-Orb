using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.Inventories
{
    [RequireComponent(typeof(ItemDropper))]
    public class LootDropper : MonoBehaviour
    {
        [SerializeField] int maxAmountOfLoot;
        [SerializeField] ItemDropRate[] drops;

        private ItemDropper itemDropper;

        private void Awake()
        {
            itemDropper = GetComponent<ItemDropper>();
        }

        public void DropLoot()
        {
            var lootAmount = 0;
            foreach (var drop in drops)
            {
                if (maxAmountOfLoot <= lootAmount)
                    break;

                var rnd = UnityEngine.Random.Range(0, 100);

                if (rnd < drop.dropRate)
                {
                    itemDropper.DropItem(drop.item, UnityEngine.Random.Range(drop.minDropAmount, drop.maxDropAmount + 1));
                }
            }
        }
    }

    [Serializable]
    public class ItemDropRate
    {
        [SerializeField] public InventoryItem item;
        [SerializeField] public int minDropAmount;
        [SerializeField] public int maxDropAmount;
        [SerializeField] public float dropRate;
    }
}
