using UnityEngine;
using UnityEngine.Events;

namespace TheOrb.Inventories
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHit;

        public void OnHit()
        {
            onHit.Invoke();
        }
    }
}