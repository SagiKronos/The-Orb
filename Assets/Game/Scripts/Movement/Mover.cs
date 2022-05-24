using System.Collections;
using System.Collections.Generic;
using TheOrb.Core;
//using TheOrb.Attributes;
using UnityEngine;
using UnityEngine.AI;
using TheOrb.Saving;

namespace TheOrb.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] float maxSpeed = 3f;
        [SerializeField] float maxNavPathLength = 40f;

        NavMeshAgent navMeshAgent;
        Animator animator;
        ActionScheduler scheduler;
        //private Health health;

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            scheduler = GetComponent<ActionScheduler>();
            animator = GetComponentInChildren<Animator>();
            //health = GetComponent<Health>();
        }


        // Update is called once per frame
        void Update()
        {
            //navMeshAgent.enabled = !health.IsDead;
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 dest, float speedFraction = 1f)
        {
            scheduler.StartAction(this);
            MoveTo(dest, speedFraction);
        }

        public bool CanMoveTo(Vector3 dest)
        {
            var path = new NavMeshPath();
            var hasPath = NavMesh.CalculatePath(transform.position, dest, NavMesh.AllAreas, path);
            if (!hasPath || path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetPathLength(path) > maxNavPathLength) return false;

            return true;
        }


        private float GetPathLength(NavMeshPath path)
        {
            var length = 0f;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return length;
        }

        public void MoveTo(Vector3 dest, float speedFraction)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.SetDestination(dest);
        }

        public void Die()
        {
            navMeshAgent.enabled = false;
        }

        public void Revive()
        {
            navMeshAgent.enabled = true;
        }

        private void UpdateAnimator()
        {
            var velocity = navMeshAgent.velocity;
            var localVelocity = transform.InverseTransformDirection(velocity);
            var speed = localVelocity.z;
            animator.SetFloat("forwardSpeed", speed);
        }

        public void LookAt(Vector3 target)
        {
            var direction = (target - transform.position).normalized;

            var lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = lookRotation;
            //rotate us over time according to speed until we are in the required rotation
            //transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * maxSpeed);
        }

        public void Cancel()
        {
            if(navMeshAgent.enabled)
                navMeshAgent.isStopped = true;

        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false;
            }
            transform.position = ((SerializableVector3)state).ToVector();
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(transform.position);
            }
        }
    }
}
