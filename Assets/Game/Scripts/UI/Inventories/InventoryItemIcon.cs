using TheOrb.Inventories;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.UI.Inventories
{
    [RequireComponent(typeof(Image))]
    public class InventoryItemIcon : MonoBehaviour
    {
        [SerializeField] GameObject textContainer = null;
        [SerializeField] TextMeshProUGUI itemNumber = null;

        public void SetItem(InventoryItem item, int amount)
        {
            var image = GetComponent<Image>();

            if (item == null)
            {
                image.sprite = null;
                image.enabled = false;
            }
            else
            {
                image.sprite = item.GetIcon();
                image.enabled = true;
            }

            if (itemNumber)
            {
                if (amount <= 1)
                {
                    textContainer.SetActive(false);
                }
                else
                {
                    textContainer.SetActive(true);
                    itemNumber.text = amount < 100 ? amount.ToString() : "+99";
                }
            }
        }

        public Sprite GetItem()
        {
            var image = GetComponent<Image>();

            if (!image.enabled)
            {
                return null;
            }

            return image.sprite;
        }
    }
}