using UnityEngine;
using UnityEngine.AI;

namespace VehicleSystem
{
    public class CarAI : MonoBehaviour
    {
        [Header("AI Settings")]
        public Transform destinationPoint;
        public float stoppingDistance = 5f;
        public float obstacleDetectionDistance = 10f;

        private NavMeshAgent navMeshAgent;
        private VehicleController vehicleController;

        void Start()
        {
            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            vehicleController = GetComponent<VehicleController>();
            
            if (vehicleController != null)
            {
                vehicleController.isAIControlled = true;
            }
        }

        void Update()
        {
            if (destinationPoint == null) return;
            
            navMeshAgent.SetDestination(destinationPoint.position);

            if (Vector3.Distance(transform.position, destinationPoint.position) > stoppingDistance)
            {
                Vector3 desiredVelocity = navMeshAgent.desiredVelocity;

                // Set AI input to vehicle
                if (vehicleController != null)
                {
                    float horizontal = Mathf.Clamp(desiredVelocity.x, -1, 1);
                    float vertical = Mathf.Clamp(desiredVelocity.z, -1, 1);
                    bool brake = false;

                    Ray ray = new Ray(transform.position, transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(ray, out hit, obstacleDetectionDistance))
                    {
                        if (hit.collider != null && !hit.collider.isTrigger)
                        {
                            brake = true; // Brake if a close obstacle is detected
                        }
                    }

                    vehicleController.SetInput(horizontal, vertical, brake);
                }
            }
        }

        // Public methods to manage AI
        public void SetDestination(Transform newDestination)
        {
            destinationPoint = newDestination;
        }
    }
}
