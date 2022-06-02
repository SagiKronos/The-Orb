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
        private AIStates currentState;
        LazyValue<Vector3> position;
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
            position = new LazyValue<Vector3>(GetInitPosition);
        }

        private void Start()
        {
            position.ForceInit();
        }

        private void Update()
        {
            if (health.IsDead) return;
            
            switch (currentState)
            {
                case AIStates.Idle:
                    IdleBehaviour();
                    break;
                case AIStates.Patroling:
                    PatrolBehavior();
                    break;
                case AIStates.PatrolingWait:
                    PartrolingWaitBehaviour();
                    break;
                case AIStates.Suspect:
                    SuspicionBehavior();
                    break;
                case AIStates.Attacking:
                    AttackBehavior();
                    break;
                default:
                    break;
            }

            UpdateTimers();
        }

        private void IdleBehaviour()
        {
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                currentState = AIStates.Attacking;
                return;
            }

            if (path != null)
                currentState = AIStates.Patroling;

        }

        private Vector3 GetInitPosition()
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
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                currentState = AIStates.Attacking;
                return;
            }

            if (AtWaypoint())
            {
                timeAtWayPoint = 0;
                mover.Cancel();
                currentState = AIStates.PatrolingWait;
            }
            else
                mover.StartMoveAction(GetCurrentWaypoint(), patrolSpeedFraction);
        }

        private void PartrolingWaitBehaviour()
        {
            if (timeAtWayPoint > dwellingTimeInWaypoint)
            {
                CycleWaypoint();
                currentState = AIStates.Patroling;
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
            currentState = AIStates.Idle;
        }

        private void AttackBehavior()
        {
            if (IsAggrevated() && fighter.CanAttack(player))
            {
                timeSinceLastSawPlayer = 0f;
                fighter.Attack(player);
                AggrevateNearbyEnemies();
            }
            else if (suspicionTime > timeSinceLastSawPlayer)
            {
                currentState = AIStates.Suspect;
            }
        }

        private void AggrevateNearbyEnemies()
        {
            var hits = Physics.SphereCastAll(transform.position, shoutDistance, Vector3.up, 0).Select(x => x.collider.GetComponent<AIController>()).Where(x => x != null && x != this   );
            foreach (var hit in hits)
            {
                hit.Aggrevate();
            }
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0f;
            currentState = AIStates.Attacking;
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
