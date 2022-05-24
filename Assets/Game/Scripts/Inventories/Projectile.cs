using TheOrb.Attributes;
using UnityEngine;
using UnityEngine.Events;

namespace TheOrb.Inventories
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] Health target;
        [SerializeField] float speed = 1;
        [SerializeField] bool isHoming = false;
        [SerializeField] GameObject hitEffect;
        [SerializeField] float maxLifeTime = 10;
        [SerializeField] GameObject[] destoryOnHit = null;
        [SerializeField] float lifeAfterImpact = 2;
        [SerializeField] UnityEvent onHit;

        float damage = 0;
        GameObject instigator = null;
        Vector3 targetPos;

        void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        // Update is called once per frame
        void Update()
        {
            if (target == null) return;

            if (isHoming && !target.IsDead)
                transform.LookAt(GetAimLocation());

            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        public void SetTarget(GameObject instigator, Health target, float dmg)
        {
            this.instigator = instigator;
            this.target = target;
            damage = dmg;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            var collider = target.GetComponent<CapsuleCollider>();
            return collider == null ? target.transform.position : target.transform.position + Vector3.up * collider.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (target != other.GetComponent<Health>() || target.IsDead) return;
            target.TakeDamage(instigator, damage);

            speed = 0;
            onHit.Invoke();

            if (hitEffect != null)
            {
                var effect = Instantiate(hitEffect, GetAimLocation(), transform.rotation);
            }

            foreach (var toDestroy in destoryOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject,lifeAfterImpact);
        }

    }

}