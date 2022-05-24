using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheOrb.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] Text damageText;

        public void SetDamageText(float damage)
        {
            damageText.text = damage.ToString();
        }

        public void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}