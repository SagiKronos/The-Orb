using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TheOrb.Control
{
    public class PatrolPath : MonoBehaviour
    {
        const float wayPointGizmoRadius = 0.2f;

        private void OnDrawGizmos()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var waypoint = GetWaypoint(i);
                Gizmos.DrawSphere(waypoint, wayPointGizmoRadius);

                Gizmos.DrawLine(waypoint, GetWaypoint(GetNextChild(i)));
            }
        }

        public int GetNextChild(int i)
        {
            return (i + 1) % transform.childCount;
        }

        public Vector3 GetWaypoint(int i)
        {
            return transform.GetChild(i).position;
        }
    }
}
