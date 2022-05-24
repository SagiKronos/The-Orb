using TheOrb.Inventories;
using UnityEngine;

namespace TheOrb.Control
{
    public class ClickablePickup : MonoBehaviour, IRayCastable
    {
        Pickup pickup;

        private void Awake()
        {
            pickup = GetComponent<Pickup>();
            if (pickup == null)
            {
                pickup = GetComponentInParent<Pickup>();
            }
        }

        public CursorType GetCursorType()
        {
            if (pickup.CanBePickedUp())
            {
                return CursorType.Pickup;
            }
            else
            {
                return CursorType.FullPickup;
            }
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if (Input.GetMouseButtonDown(0))
            {
                pickup.PickupItem();
            }
            return true;
        }
    }
}
