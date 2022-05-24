using UnityEngine;
using TheOrb.Movement;
using System.Linq;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using System.Collections.Generic;
using TheOrb.Core;
using TheOrb.Attributes;
using TheOrb.Combat.Skills;

namespace TheOrb.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Health health;
        private PlayerSkillsManager skillsManager;
        private Mover mover;

        [SerializeField] CursorMapping[] cursorMappings = null;
        [SerializeField] float maxNavMeshProjectionDistance = 1f;
        [SerializeField] float raycastRadius = 0.3f;

        private void Awake()
        {
            mover = GetComponent<Mover>();
            health = GetComponent<Health>();
            skillsManager = GetComponent<PlayerSkillsManager>();
        }


        void Update()
        {
            if (InteractWithUI()) return;
            if (health.IsDead)
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithSpecialSkill()) return;
            if (InteractWithComponent()) return;
            if (InteractWithMovement()) return;

            SetCursor(CursorType.None);
        }

        private bool InteractWithSpecialSkill()
        {
            if (Input.GetMouseButton(1))
            {
                var lookAtMouse = TryToLookAtMouse();
                if (!lookAtMouse.isAble) return false;

                return skillsManager.LaunchActiveSkill(lookAtMouse.position);
            }

            return false;
        }

        private (bool isAble, Vector3 position) TryToLookAtMouse()
        {
            var raycastNavMesh = RaycastAny();

            if (!raycastNavMesh.hasHit)
                return (false, Vector3.zero);

            mover.LookAt(new Vector3(raycastNavMesh.target.x, transform.position.y, raycastNavMesh.target.z));
            return (true, raycastNavMesh.target);
        }

        private (bool hasHit, Vector3 target) RaycastAny()
        {
            (bool hasHit, Vector3 target) falseResponse = (false, Vector3.zero);
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);

            if (!hasHit)
                return falseResponse;

            var raycastable = hit.transform.GetComponent<IRayCastable>();

            if (raycastable != null)
                return (true, hit.transform.position);

            var canCast = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!canCast)
                return falseResponse;

            return (true, navMeshHit.position);
        }

        private bool InteractWithComponent()
        {
            var rayCastable = Physics.SphereCastAll(GetMouseRay(), raycastRadius).OrderBy(x => x.distance)
                .SelectMany(x => x.transform.GetComponents<IRayCastable>()).FirstOrDefault(x => x.HandleRaycast(this));

            if (rayCastable != null)
            {
                SetCursor(rayCastable.GetCursorType());
                return true;
            }

            return false;
        }

        private bool InteractWithUI()
        {
            var currentSelectedGameObject = EventSystem.current.currentSelectedGameObject;
            var uiElements = GetAllUIElements();
            if ((EventSystem.current.IsPointerOverGameObject() && !uiElements.All(IsClickablePickup)) || UIInteractionBlockers.IsBlocked())
            {
                SetCursor(CursorType.UI);
                return true;
            }

            return false;
        }

        private static bool IsClickablePickup(RaycastResult x)
        {
            return x.gameObject.GetComponent<ClickablePickupText>() || x.gameObject.GetComponentInParent<ClickablePickupText>();
        }

        private List<RaycastResult> GetAllUIElements()
        {
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                pointerId = -1,
            };

            pointerData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            return results;
        }

        private bool InteractWithMovement()
        {
            var raycastNavMesh = RaycastNavMesh();

            if (raycastNavMesh.hasHit)
            {
                if (!mover.CanMoveTo(raycastNavMesh.target)) return false;

                if (Input.GetMouseButton(0))
                {
                    mover.StartMoveAction(raycastNavMesh.target, 1f);
                }

                SetCursor(CursorType.Movement);
                return true;
            }

            return false;
        }

        private (bool hasHit, Vector3 target) RaycastNavMesh()
        {
            (bool hasHit, Vector3 target) falseResponse = (false, Vector3.zero);
            bool hasHit = Physics.Raycast(GetMouseRay(), out RaycastHit hit);

            if (!hasHit) return falseResponse;

            var canCast = NavMesh.SamplePosition(hit.point, out NavMeshHit navMeshHit, maxNavMeshProjectionDistance, NavMesh.AllAreas);

            if (!canCast) return falseResponse;

            return (true, navMeshHit.position);
        }

        private void SetCursor(CursorType type)
        {
            var mapping = GetCursorMapping(type);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            return cursorMappings.First(x => x.type == type);
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
