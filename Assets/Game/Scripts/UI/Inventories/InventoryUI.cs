using TheOrb.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TheOrb.UI.Inventories
{
    public class InventoryUI : MonoBehaviour
    {
        // CONFIG DATA
        [SerializeField] InventorySlotUi InventoryItemPrefab = null;

        // CACHE
        Inventory playerInventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            playerInventory = Inventory.GetPlayerInventory();
            playerInventory.inventoryUpdated += Redraw;
        }

        private void Start()
        {
            Redraw();
        }

        private void Redraw()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < playerInventory.GetSize(); i++)
            {
                var itemUI = Instantiate(InventoryItemPrefab, transform);
                itemUI.Setup(playerInventory, i);
            }
        }
    }
}