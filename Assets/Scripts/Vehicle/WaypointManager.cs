using UnityEngine;
using System.Collections.Generic;

namespace VehicleSystem
{
    public class WaypointManager : MonoBehaviour
    {
        [Header("Waypoints")]
        public List<Transform> waypoints = new List<Transform>();
        public bool cycleWaypoints = true;
        public float waitTimeAtWaypoint = 2f;
        
        [Header("Visualization")]
        public bool drawWaypointGizmos = true;
        public Color waypointColor = Color.cyan;
        public float waypointSize = 1f;
        
        private int currentWaypointIndex = 0;
        private float waitTimer = 0f;
        private bool isWaiting = false;
        
        void Start()
        {
            // Auto-find waypoints if none are assigned
            if (waypoints.Count == 0)
            {
                FindWaypoints();
            }
        }
        
        void FindWaypoints()
        {
            GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
            foreach (GameObject waypoint in waypointObjects)
            {
                waypoints.Add(waypoint.transform);
            }
            
            // Sort waypoints by name for consistent ordering
            waypoints.Sort((a, b) => a.name.CompareTo(b.name));
        }
        
        void Update()
        {
            if (isWaiting)
            {
                waitTimer += Time.deltaTime;
                if (waitTimer >= waitTimeAtWaypoint)
                {
                    isWaiting = false;
                    waitTimer = 0f;
                }
            }
        }
        
        public Transform GetCurrentWaypoint()
        {
            if (waypoints.Count == 0) return null;
            return waypoints[currentWaypointIndex];
        }
        
        public Transform GetNextWaypoint()
        {
            if (waypoints.Count == 0) return null;
            
            if (isWaiting) return null;
            
            if (cycleWaypoints)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
            }
            else
            {
                currentWaypointIndex = Mathf.Min(currentWaypointIndex + 1, waypoints.Count - 1);
            }
            
            isWaiting = true;
            return waypoints[currentWaypointIndex];
        }
        
        public void SetWaypointIndex(int index)
        {
            if (index >= 0 && index < waypoints.Count)
            {
                currentWaypointIndex = index;
                isWaiting = false;
                waitTimer = 0f;
            }
        }
        
        public int GetWaypointCount()
        {
            return waypoints.Count;
        }
        
        public bool IsWaiting()
        {
            return isWaiting;
        }
        
        void OnDrawGizmos()
        {
            if (!drawWaypointGizmos || waypoints.Count == 0) return;
            
            Gizmos.color = waypointColor;
            
            for (int i = 0; i < waypoints.Count; i++)
            {
                if (waypoints[i] == null) continue;
                
                // Draw waypoint sphere
                Gizmos.DrawWireSphere(waypoints[i].position, waypointSize);
                
                // Draw waypoint number
                #if UNITY_EDITOR
                UnityEditor.Handles.Label(waypoints[i].position + Vector3.up * 2f, i.ToString());
                #endif
                
                // Draw connection lines
                if (i < waypoints.Count - 1)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
                else if (cycleWaypoints && waypoints.Count > 1)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
            
            // Highlight current waypoint
            if (currentWaypointIndex < waypoints.Count && waypoints[currentWaypointIndex] != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(waypoints[currentWaypointIndex].position, waypointSize * 0.5f);
            }
        }
    }
}
