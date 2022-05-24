using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.Inventories
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] GameObject pickupText;

        // STATE
        InventoryItem item;
        int amount;

        // CACHED REFERENCE
        Inventory inventory;

        // LIFECYCLE METHODS

        private void Awake()
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
        }

        private void Update()
        {
            pickupText.SetActive(Input.GetKey(KeyCode.LeftAlt));
        }

        public void Setup(InventoryItem item, int amount)
        {
            this.item = item;
            this.amount = amount;
            pickupText.GetComponentInChildren<Text>().text = amount > 1 ? $"{item.GetDisplayName()} x {amount}" :item.GetDisplayName() ;
        }

        public InventoryItem GetItem()
        {
            return item;
        }

        public int GetAmount()
        {
            return amount;
        }

        public void PickupItem()
        {
            bool foundSlot = inventory.AddToFirstEmptySlot(item,amount);
            if (foundSlot)
            {
                Destroy(gameObject);
            }
        }

        public bool CanBePickedUp()
        {
            return inventory.HasSpaceFor(item);
        }
    }
}
