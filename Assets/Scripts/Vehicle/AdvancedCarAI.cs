using UnityEngine;
using System.Collections.Generic;

namespace VehicleSystem
{
    public class AdvancedCarAI : MonoBehaviour
    {
        [Header("AI Navigation")]
        public Transform destinationPoint;
        public float stoppingDistance = 5f;
        public float maxSpeed = 50f;
        public float slowDownDistance = 20f;
        
        [Header("Pathfinding")]
        public float waypointDistance = 10f;
        public LayerMask obstacleLayer = -1;
        public float obstacleDetectionDistance = 15f;
        public float sideRayOffset = 1.5f;
        
        [Header("Steering")]
        public float maxSteerAngle = 30f;
        public float steerSensitivity = 2f;
        public float lookAheadDistance = 10f;
        
        [Header("Debug")]
        public bool drawDebugLines = true;
        
        private VehicleController vehicleController;
        private List<Vector3> currentPath = new List<Vector3>();
        private int currentWaypointIndex = 0;
        private Vector3 currentTarget;
        private bool hasReachedDestination = false;
        
        void Start()
        {
            vehicleController = GetComponent<VehicleController>();
            
            if (vehicleController != null)
            {
                vehicleController.isAIControlled = true;
            }
            
            // Create initial path
            if (destinationPoint != null)
            {
                CreatePath();
            }
        }
        
        void Update()
        {
            if (destinationPoint == null || hasReachedDestination) return;
            
            // Check if we need to recalculate path
            if (currentPath.Count == 0 || Vector3.Distance(transform.position, destinationPoint.position) > 100f)
            {
                CreatePath();
            }
            
            // Navigate along the path
            NavigateToWaypoint();
            
            // Calculate steering and throttle
            CalculateVehicleInput();
            
            // Debug drawing
            if (drawDebugLines)
            {
                DrawDebugInfo();
            }
        }
        
        void CreatePath()
        {
            currentPath.Clear();
            currentWaypointIndex = 0;
            hasReachedDestination = false;
            
            // Simple pathfinding - for more complex scenarios, use A* or NavMesh
            Vector3 startPos = transform.position;
            Vector3 endPos = destinationPoint.position;
            
            // Create waypoints along the path
            float distance = Vector3.Distance(startPos, endPos);
            int numWaypoints = Mathf.CeilToInt(distance / waypointDistance);
            
            for (int i = 1; i <= numWaypoints; i++)
            {
                float t = (float)i / numWaypoints;
                Vector3 waypoint = Vector3.Lerp(startPos, endPos, t);
                
                // Adjust waypoint height to ground level
                RaycastHit hit;
                if (Physics.Raycast(waypoint + Vector3.up * 10f, Vector3.down, out hit, 20f))
                {
                    waypoint.y = hit.point.y + 1f;
                }
                
                currentPath.Add(waypoint);
            }
            
            if (currentPath.Count > 0)
            {
                currentTarget = currentPath[0];
            }
        }
        
        void NavigateToWaypoint()
        {
            if (currentPath.Count == 0) return;
            
            float distanceToTarget = Vector3.Distance(transform.position, currentTarget);
            
            // Check if we reached the current waypoint
            if (distanceToTarget < waypointDistance * 0.5f)
            {
                currentWaypointIndex++;
                
                if (currentWaypointIndex >= currentPath.Count)
                {
                    // Check if we reached the final destination
                    if (Vector3.Distance(transform.position, destinationPoint.position) < stoppingDistance)
                    {
                        hasReachedDestination = true;
                        vehicleController.SetInput(0, 0, true);
                        return;
                    }
                    
                    // Need to recalculate path
                    CreatePath();
                    return;
                }
                
                currentTarget = currentPath[currentWaypointIndex];
            }
        }
        
        void CalculateVehicleInput()
        {
            if (hasReachedDestination) return;
            
            // Calculate steering
            Vector3 targetDirection = (currentTarget - transform.position).normalized;
            float steerInput = CalculateSteerInput(targetDirection);
            
            // Calculate throttle
            float throttleInput = CalculateThrottleInput();
            
            // Check for obstacles
            bool shouldBrake = DetectObstacles();
            
            vehicleController.SetInput(steerInput, throttleInput, shouldBrake);
        }
        
        float CalculateSteerInput(Vector3 targetDirection)
        {
            Vector3 localTarget = transform.InverseTransformDirection(targetDirection);
            float steerInput = Mathf.Clamp(localTarget.x * steerSensitivity, -1f, 1f);
            
            return steerInput;
        }
        
        float CalculateThrottleInput()
        {
            float distanceToDestination = Vector3.Distance(transform.position, destinationPoint.position);
            float currentSpeed = vehicleController.GetCurrentSpeed();
            
            float throttleInput = 1f;
            
            // Slow down when approaching destination
            if (distanceToDestination < slowDownDistance)
            {
                throttleInput = Mathf.Lerp(0.1f, 1f, distanceToDestination / slowDownDistance);
            }
            
            // Limit speed
            if (currentSpeed > maxSpeed)
            {
                throttleInput = 0f;
            }
            
            return throttleInput;
        }
        
        bool DetectObstacles()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            
            // Forward ray
            if (Physics.Raycast(origin, forward, obstacleDetectionDistance, obstacleLayer))
            {
                return true;
            }
            
            // Side rays
            if (Physics.Raycast(origin + right * sideRayOffset, forward, obstacleDetectionDistance * 0.8f, obstacleLayer))
            {
                return true;
            }
            
            if (Physics.Raycast(origin - right * sideRayOffset, forward, obstacleDetectionDistance * 0.8f, obstacleLayer))
            {
                return true;
            }
            
            return false;
        }
        
        void DrawDebugInfo()
        {
            // Draw path
            if (currentPath.Count > 1)
            {
                for (int i = 0; i < currentPath.Count - 1; i++)
                {
                    Debug.DrawLine(currentPath[i], currentPath[i + 1], Color.blue);
                }
            }
            
            // Draw current target
            if (currentTarget != Vector3.zero)
            {
                Debug.DrawLine(transform.position, currentTarget, Color.green);
                Gizmos.DrawSphere(currentTarget, 2f);
            }
            
            // Draw obstacle detection rays
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;
            
            Debug.DrawRay(origin, forward * obstacleDetectionDistance, Color.red);
            Debug.DrawRay(origin + right * sideRayOffset, forward * obstacleDetectionDistance * 0.8f, Color.yellow);
            Debug.DrawRay(origin - right * sideRayOffset, forward * obstacleDetectionDistance * 0.8f, Color.yellow);
        }
        
        // Public methods
        public void SetDestination(Transform newDestination)
        {
            destinationPoint = newDestination;
            hasReachedDestination = false;
            CreatePath();
        }
        
        public void SetDestination(Vector3 newDestination)
        {
            // Create a temporary transform for the destination
            GameObject tempDestination = new GameObject("AI Destination");
            tempDestination.transform.position = newDestination;
            destinationPoint = tempDestination.transform;
            hasReachedDestination = false;
            CreatePath();
        }
        
        public bool HasReachedDestination()
        {
            return hasReachedDestination;
        }
        
        public float GetDistanceToDestination()
        {
            if (destinationPoint == null) return float.MaxValue;
            return Vector3.Distance(transform.position, destinationPoint.position);
        }
    }
}
