using TheOrb.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheOrb.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefarb;

        public void Spawn(float damage)
        {
            var damageText = Instantiate<DamageText>(damageTextPrefarb, transform);
            damageText.SetDamageText(damage);
        }
    }
}