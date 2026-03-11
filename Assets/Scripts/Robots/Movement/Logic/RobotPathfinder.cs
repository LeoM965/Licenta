using UnityEngine;
using System.Collections.Generic;
using AI.Navigation;

namespace Robots.Components.Movement
{
    public class RobotPathfinder : MonoBehaviour
    {
        [Header("Settings")]
        public float waypointThreshold = 0.8f;
        public float arrivalThreshold = 0.5f;

        private List<Vector3> path;
        private int pathIndex;
        private Vector3? finalTarget;
        private float stuckTimer;
        private Vector3 stuckPushDir;
        private Vector3 lastFixedPos;

        public bool HasTarget => path != null && pathIndex < path.Count;
        public bool HasArrived => finalTarget.HasValue && Vector3.Distance(transform.position, finalTarget.Value) < arrivalThreshold;
        public Vector3? FinalTarget => finalTarget;
        public List<Vector3> CurrentPath => path;
        public Vector3 StuckPushDir => stuckPushDir;

        public void SetTarget(Vector3 target)
        {
            finalTarget = target;
            stuckTimer = 0f;
            stuckPushDir = Vector3.zero;
            RequestPath(target);
        }

        public void ClearTarget()
        {
            path = null;
            finalTarget = null;
            pathIndex = 0;
        }

        public Vector3 GetMoveDirection(Vector3 currentPos, float deltaTime, out float targetAngle)
        {
            targetAngle = transform.eulerAngles.y;

            stuckPushDir = RobotHelper.UpdateStuckDetection(
                currentPos, lastFixedPos, path, pathIndex,
                ref stuckTimer, ref stuckPushDir,
                finalTarget, RequestPath, transform, deltaTime);

            lastFixedPos = currentPos;

            if (path == null || pathIndex >= path.Count)
                return Vector3.zero;

            Vector3 target = path[pathIndex];
            Vector3 dir = target - currentPos;
            dir.y = 0;
            float dist = dir.magnitude;

            if (dist < waypointThreshold)
            {
                pathIndex++;
                if (pathIndex >= path.Count && finalTarget.HasValue)
                {
                    Vector3 toFinal = finalTarget.Value - currentPos;
                    toFinal.y = 0;
                    if (toFinal.magnitude > arrivalThreshold)
                        RequestPath(finalTarget.Value);
                }
            }

            if (pathIndex < path.Count)
            {
                target = path[pathIndex];
                dir = target - currentPos;
                dir.y = 0;
                dist = dir.magnitude;
            }

            if (dist > arrivalThreshold)
            {
                Vector3 moveDirection = dir.normalized;
                targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
                return moveDirection;
            }

            return Vector3.zero;
        }

        private void RequestPath(Vector3 target)
        {
            if (Pathfinder.Instance != null)
            {
                List<Vector3> newPath = Pathfinder.Instance.FindPath(transform.position, target);
                if (newPath != null && newPath.Count > 0)
                {
                    path = newPath;
                    pathIndex = 0;
                    return;
                }
            }
            path = new List<Vector3> { target };
            pathIndex = 0;
        }
    }
}
