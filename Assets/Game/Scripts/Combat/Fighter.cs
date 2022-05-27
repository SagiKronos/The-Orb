using TheOrb.Attributes;
using TheOrb.Movement;
using TheOrb.Saving;
using UnityEngine;
using TheOrb.Core;
using TheOrb.Stats;
using System.Collections.Generic;
using TheOrb.Inventories;

namespace TheOrb.Combat
{
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider
    {
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform;
        [SerializeField] Transform leftHandTransform;
        [SerializeField] WeaponConfig defaultWeaponConfig;

        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        Mover mover;
        Animator animator;
        WeaponConfig currentWeaponConfig;
        BaseStats baseStats;
        ActionScheduler actionScheduler;
        LazyValue<Weapon> currentWeapon;

        void Awake()
        {
            mover = GetComponent<Mover>();
            animator = GetComponentInChildren<Animator>();
            currentWeaponConfig = defaultWeaponConfig;
            baseStats = GetComponent<BaseStats>();
            actionScheduler = GetComponent<ActionScheduler>();
            currentWeapon = new LazyValue<Weapon>(SetupDefaultWeapon);
        }

        void Start()
        {
            currentWeapon.ForceInit();
        }

        void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;
            if (target.IsDead) return;


            
            if (!IsInRange(target.transform))
            {
                mover.MoveTo(target.transform.position, 1);
            }
            else
            {
                mover.Cancel();
                AttackBehavior();
            }
        }

        private bool IsInRange(Transform target)
        {
            var distance = Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(target.transform.position.x, 0, target.transform.position.z));
            return distance <= currentWeaponConfig.Range;
        }

        private Weapon SetupDefaultWeapon()
        {
            return AttachWeapon(defaultWeaponConfig);
        }

        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            Destroy(currentWeapon.value);
            currentWeaponConfig = weaponConfig;
            currentWeapon.value = AttachWeapon(weaponConfig);
        }

        public void RemoveWeapon()
        {
            currentWeaponConfig = defaultWeaponConfig;
            currentWeapon.value = SetupDefaultWeapon();
        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            return weapon.SpawnWeapon(rightHandTransform, leftHandTransform, animator);
        }

        public void onAttacked(Health attacker, float dmg)
        {
            if (target == null)
                target = attacker;
        }

        public Health GetTarget()
        {
            return target;
        }

        private void AttackBehavior()
        {
            if (timeSinceLastAttack >= timeBetweenAttacks)
            {
                transform.LookAt(target.transform);
                TriggerAttack();
                timeSinceLastAttack = 0f;
            }
        }

        private void TriggerAttack()
        {
            animator.ResetTrigger("stopAttack");
            animator.SetTrigger("attack");
        }

        // Animation event
        public void Hit()
        {
            if (target == null) return;

            var damage = baseStats.GetStat(Stat.Damage);

            if (currentWeapon != null)
                currentWeapon.value.OnHit();

            if (currentWeaponConfig.HasProjectile())
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, target, gameObject, damage);
            else
                target.TakeDamage(gameObject, damage);
        }

        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            if (!mover.CanMoveTo(combatTarget.transform.position) &&
                !IsInRange(combatTarget.transform)) 
                return false;

            var tar = combatTarget.GetComponent<Health>();
            return tar != null && !tar.IsDead;
        }

        public void Attack(GameObject combatTarget)
        {
            actionScheduler.StartAction(this);
            target = combatTarget.GetComponent<Health>();
        }

        public void Cancel()
        {
            target = null;
            mover.Cancel();
            TriggerStopAttack();

        }

        private void TriggerStopAttack()
        {
            animator.ResetTrigger("attack");
            animator.SetTrigger("stopAttack");
        }

        public IEnumerable<float> GetAdditiveModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.Damage;
            }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.PercentageBonus;
            }
        }

        public object CaptureState()
        {
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            EquipWeapon(Resources.Load<WeaponConfig>(state.ToString()));
            target = null;
        }
    }
}