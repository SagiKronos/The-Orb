using UnityEngine;
using TheOrb.Saving;
using TheOrb.Stats;
using TheOrb.Core;
using UnityEngine.Events;
using System.Collections;

namespace TheOrb.Attributes
{
    public class Health : MonoBehaviour, ISaveable
    {
        [SerializeField] float deathFadeTime = 5f;
        [SerializeField] UnityEvent<float> takeDamage;
        [SerializeField] UnityEvent onDie;
        [SerializeField] UnityEvent onRevive;
        
        LazyValue<float> healthPoints;

        public bool IsDead { get; private set; }
        private Animator animator;
        private BaseStats baseStats;
        private ActionScheduler actionScheduler;

        void Awake()
        {
            animator = GetComponentInChildren<Animator>();
            baseStats = GetComponent<BaseStats>();
            actionScheduler = GetComponent<ActionScheduler>();
            healthPoints = new LazyValue<float>(GetInitialHealth);
        }

        void Start()
        {
            healthPoints.ForceInit();
        }

        private float GetInitialHealth()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private IEnumerator DeathCoroutine(float fadeTime)
        {
            yield return new WaitForSeconds(fadeTime);
            if (IsDead)
                GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }

        private void OnEnable()
        {
            GetComponent<BaseStats>().onLevelUp += RegenerateHealth;
        }

        private void OnDisable()
        {
            GetComponent<BaseStats>().onLevelUp -= RegenerateHealth;
        }

        public void Heal(float healDamage)
        {
            healthPoints.value = Mathf.Min(GetMaxHealthPoint(), healthPoints.value + healDamage);
        }

        public void TakeDamage(GameObject instigator, float damage)
        {
            healthPoints.value = Mathf.Max(0f, healthPoints.value - damage);

            if (healthPoints.value == 0f && !IsDead)
            {
                onDie.Invoke();
                Die(deathFadeTime);
                Award(instigator);
            }
            else
            {
                takeDamage.Invoke(damage);
            }
        }

        public float GetPercentage()
        {
            return GetFraction() * 100;
        }

        public float GetFraction()
        {
            return healthPoints.value / GetComponent<BaseStats>().GetStat(Stat.Health);
        }

        public float GetHealth()
        {
            return healthPoints.value;
        }

        public float GetMaxHealthPoint()
        {
            return baseStats.GetStat(Stat.Health);
        }

        private void Die(float fadeTime)
        {
            IsDead = true;
            GetComponent<Collider>().enabled = false;
            GetComponent<Rigidbody>().detectCollisions = false;

            animator.SetTrigger("die");
            actionScheduler.CancelCurrentAction();
            StartCoroutine(DeathCoroutine(fadeTime));
        }

        private void Revive()
        {
            IsDead = false;
            GetComponent<Collider>().enabled = true;
            GetComponent<Rigidbody>().detectCollisions = true;
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

            animator.SetTrigger("revive");
        }


        private void Award(GameObject instigator)
        {
            var expReciever = instigator?.GetComponent<Experience>();

            if (expReciever != null)
            {
                var expReward = baseStats.GetStat(Stat.ExpReward);
                expReciever.GainExp(expReward);
            }
        }

        private void RegenerateHealth(int lvlDiff)
        {
            healthPoints.value = baseStats.GetStat(Stat.Health);
        }

        public object CaptureState()
        {
            return healthPoints.value;
        }

        public void RestoreState(object state)
        {
            healthPoints.value = (float)state;
            var isAlive = healthPoints.value > 0;

            if (!isAlive)
                Die(0);
            else if (IsDead)
                Revive();
            
        }
    }
}
