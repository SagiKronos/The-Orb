using TheOrb.Attributes;
using TheOrb.Control;
using UnityEngine;

namespace TheOrb.Combat.Skills.SkillTypes
{
    public class MagicBolt : MonoBehaviour
    {
        [SerializeField] GameObject hitEffect;
        private float damage;

        public void SetDamage(float dmg)
        {
            damage = dmg;
        }


        private void OnTriggerEnter(Collider other)
        {
            var player = GameObject.FindWithTag("Player");

            var target = other.GetComponent<AIController>()?.GetComponent<Health>();
            if (other.gameObject == player.gameObject || other.gameObject.GetComponent<MagicBolt>() || other.GetComponent<Collider>().isTrigger || (target != null && target.IsDead))
                return;

            if (target != null)
            {
                target.TakeDamage(player, damage);
            }

            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, transform.rotation);
            }

            gameObject.SetActive(false);
        }
    }
}
