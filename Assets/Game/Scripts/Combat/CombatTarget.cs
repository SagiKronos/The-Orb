
using TheOrb.Control;
using TheOrb.Attributes;
using UnityEngine;

namespace TheOrb.Combat
{
    [RequireComponent(typeof(Health))]
    public class CombatTarget : MonoBehaviour, IRayCastable
    {
        public CursorType GetCursorType()
        {
            return CursorType.Combat;
        }

        public bool HandleRaycast(PlayerController player)
        {
            var fighter = player.GetComponent<Fighter>();

            if (!fighter.CanAttack(gameObject)) return false;

            if (Input.GetMouseButton(0))
            {
                fighter.Attack(gameObject);
            }

            return true;
        }
    }
}
