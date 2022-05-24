using TheOrb.Combat;
using TheOrb.Core;
using TheOrb.Movement;
using TheOrb.Attributes;
using System;
using UnityEngine;
using System.Linq;
using TheOrb.Saving;

namespace TheOrb.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] PatrolPath path;
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float aggroCooldownTime = 3f;
        [SerializeField] float waypointTolerence = 0.5f;
        [SerializeField] float dwellingTimeInWaypoint = 2f;
        [SerializeField] float shoutDistance = 5f;
        [Range(0, 1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        private Fighter fighter;
        private GameObject player;
        private Health health;
        private Mover mover;
        private ActionScheduler actionScheduler;

        LazyValue<Vector3> guardPosition;
        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeAtWayPoint = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        private int waypointIndex = 0;

        private void Awake()
        {
            fighter = GetComponent<Fighter>();
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();
            actionScheduler = GetComponent<ActionScheduler>();
            player = GameObject.FindWithTag("Player");
            guardPosition = new LazyValue<Vector3>(GetInitGuardPosition);
        }

        private void Start()
        {
            guardPosition.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead) return;

            if (IsAggrevated() && fighter.CanAttack(player))
            {
                AttackBehavior();
            }
            else if (suspicionTime > timeSinceLastSawPlayer)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimers();
        }

        private Vector3 GetInitGuardPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            timeAtWayPoint += Time.deltaTime;
            timeSinceAggrevated += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = gameObject.transform.position;

            if (path != null)
            {
                if (AtWaypoint())
                {
                    timeAtWayPoint = 0;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            if (timeAtWayPoint >= dwellingTimeInWaypoint)
            {
                mover.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return path.GetWaypoint(waypointIndex);
        }

        private void CycleWaypoint()
        {
            waypointIndex = path.GetNextChild(waypointIndex);
        }

        private bool AtWaypoint()
        {
            return Vector3.Distance(transform.position, GetCurrentWaypoint()) < waypointTolerence;
        }

        private void SuspicionBehavior()
        {
            actionScheduler.CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0f;
            fighter.Attack(player);
            AggrevateNearbyEnemies();
        }

        private void AggrevateNearbyEnemies()
        {
            var hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0).Select(x => x.collider.GetComponent<AIController>()).Where(x => x != null);
            foreach (var hit in hits)
            {
                hit.Aggrevate();
            }
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0f;
        }

        private bool IsAggrevated()
        {
            return Vector3.Distance(player.transform.position, transform.position) < chaseDistance || timeSinceAggrevated < aggroCooldownTime;
        }

        public void Die()
        {
            timeSinceAggrevated = Mathf.Infinity;
            timeSinceLastSawPlayer = Mathf.Infinity;

        }

        // Called by unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
