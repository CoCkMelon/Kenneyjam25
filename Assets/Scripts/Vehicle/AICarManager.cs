using UnityEngine;

namespace VehicleSystem
{
    public class AICarManager : MonoBehaviour
    {
        [Header("AI Components")]
        public AdvancedCarAI carAI;
        public WaypointManager waypointManager;
        
        [Header("Behavior Settings")]
        public bool useWaypoints = true;
        public bool autoStartNavigation = true;
        public float destinationReachedThreshold = 3f;
        
        [Header("Manual Control")]
        public Transform manualDestination;
        [Space]
        public KeyCode nextWaypointKey = KeyCode.N;
        public KeyCode toggleAIKey = KeyCode.T;
        
        private bool isNavigating = false;
        private bool aiEnabled = true;
        
        void Start()
        {
            // Get components if not assigned
            if (carAI == null)
                carAI = GetComponent<AdvancedCarAI>();
            
            if (waypointManager == null)
                waypointManager = GetComponent<WaypointManager>();
            
            // Start navigation if enabled
            if (autoStartNavigation && useWaypoints)
            {
                StartWaypointNavigation();
            }
            else if (autoStartNavigation && manualDestination != null)
            {
                SetDestination(manualDestination);
            }
        }
        
        void Update()
        {
            HandleInput();
            
            if (!aiEnabled) return;
            
            // Check if we reached the destination
            if (isNavigating && carAI != null)
            {
                if (carAI.HasReachedDestination() || carAI.GetDistanceToDestination() < destinationReachedThreshold)
                {
                    OnDestinationReached();
                }
            }
        }
        
        void HandleInput()
        {
            // Toggle AI on/off
            if (Input.GetKeyDown(toggleAIKey))
            {
                ToggleAI();
            }
            
            // Manual waypoint navigation
            if (Input.GetKeyDown(nextWaypointKey) && useWaypoints)
            {
                GoToNextWaypoint();
            }
        }
        
        void OnDestinationReached()
        {
            Debug.Log("AI Car reached destination!");
            
            if (useWaypoints && waypointManager != null)
            {
                // Wait a bit, then go to next waypoint
                Invoke("GoToNextWaypoint", waypointManager.waitTimeAtWaypoint);
            }
            else
            {
                isNavigating = false;
            }
        }
        
        public void StartWaypointNavigation()
        {
            if (waypointManager == null || waypointManager.GetWaypointCount() == 0)
            {
                Debug.LogWarning("No waypoints available for navigation!");
                return;
            }
            
            Transform currentWaypoint = waypointManager.GetCurrentWaypoint();
            if (currentWaypoint != null)
            {
                SetDestination(currentWaypoint);
            }
        }
        
        public void GoToNextWaypoint()
        {
            if (waypointManager == null) return;
            
            Transform nextWaypoint = waypointManager.GetNextWaypoint();
            if (nextWaypoint != null)
            {
                SetDestination(nextWaypoint);
            }
        }
        
        public void SetDestination(Transform destination)
        {
            if (carAI == null || destination == null) return;
            
            carAI.SetDestination(destination);
            isNavigating = true;
            
            Debug.Log($"AI Car navigating to: {destination.name}");
        }
        
        public void SetDestination(Vector3 destination)
        {
            if (carAI == null) return;
            
            carAI.SetDestination(destination);
            isNavigating = true;
            
            Debug.Log($"AI Car navigating to position: {destination}");
        }
        
        public void ToggleAI()
        {
            aiEnabled = !aiEnabled;
            
            if (carAI != null)
            {
                carAI.enabled = aiEnabled;
            }
            
            VehicleController vehicleController = GetComponent<VehicleController>();
            if (vehicleController != null)
            {
                vehicleController.isAIControlled = aiEnabled;
            }
            
            Debug.Log($"AI {(aiEnabled ? "Enabled" : "Disabled")}");
        }
        
        public void StopNavigation()
        {
            isNavigating = false;
            
            if (carAI != null)
            {
                // Stop the car
                VehicleController vehicleController = GetComponent<VehicleController>();
                if (vehicleController != null)
                {
                    vehicleController.SetInput(0, 0, true);
                }
            }
        }
        
        // Public getters
        public bool IsNavigating() { return isNavigating; }
        public bool IsAIEnabled() { return aiEnabled; }
        public float GetDistanceToDestination() 
        { 
            return carAI != null ? carAI.GetDistanceToDestination() : float.MaxValue; 
        }
    }
}
