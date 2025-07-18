using UnityEngine;

namespace VehicleSystem
{
    public class VehicleController : MonoBehaviour
    {
        [Header("Movement Settings")]
        public float motorForce = 1500f;
        public float brakeForce = 3000f;
        public float maxSteerAngle = 30f;
        public float maxSpeed = 80f;
        
        [Header("Wheel Settings")]
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;
        
        [Header("Wheel Transforms")]
        public Transform frontLeftWheelTransform;
        public Transform frontRightWheelTransform;
        public Transform rearLeftWheelTransform;
        public Transform rearRightWheelTransform;
        
        [Header("Physics")]
        public Transform centerOfMass;
        public float downForce = 100f;
        
        private float horizontalInput;
        private float verticalInput;
        private float steerAngle;
        private bool isBreaking;
        private Rigidbody vehicleRigidbody;
        
        // For AI control
        [HideInInspector]
        public bool isAIControlled = false;
        
        void Start()
        {
            vehicleRigidbody = GetComponent<Rigidbody>();
            
            // Set center of mass
            if (centerOfMass != null)
            {
                vehicleRigidbody.centerOfMass = centerOfMass.localPosition;
            }
        }
        
        void Update()
        {
            if (!isAIControlled)
            {
                GetInput();
            }
            HandleMotor();
            HandleSteering();
            UpdateWheels();
        }
        
        void FixedUpdate()
        {
            // Add downforce for better grip
            vehicleRigidbody.AddForce(-transform.up * downForce * vehicleRigidbody.linearVelocity.magnitude);
        }
        
        void GetInput()
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            isBreaking = Input.GetKey(KeyCode.Space);
        }
        
        void HandleMotor()
        {
            float currentSpeed = vehicleRigidbody.linearVelocity.magnitude;
            
            // Apply motor force only if below max speed
            if (currentSpeed < maxSpeed)
            {
                frontLeftWheelCollider.motorTorque = verticalInput * motorForce;
                frontRightWheelCollider.motorTorque = verticalInput * motorForce;
            }
            else
            {
                frontLeftWheelCollider.motorTorque = 0;
                frontRightWheelCollider.motorTorque = 0;
            }
            
            // Apply brake force
            float brakeInput = isBreaking ? brakeForce : 0f;
            frontLeftWheelCollider.brakeTorque = brakeInput;
            frontRightWheelCollider.brakeTorque = brakeInput;
            rearLeftWheelCollider.brakeTorque = brakeInput;
            rearRightWheelCollider.brakeTorque = brakeInput;
        }
        
        void HandleSteering()
        {
            steerAngle = maxSteerAngle * horizontalInput;
            frontLeftWheelCollider.steerAngle = steerAngle;
            frontRightWheelCollider.steerAngle = steerAngle;
        }
        
        void UpdateWheels()
        {
            UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
            UpdateSingleWheel(frontRightWheelCollider, frontRightWheelTransform);
            UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
            UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        }
        
        void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
        {
            if (wheelTransform == null) return;
            
            Vector3 pos;
            Quaternion rot;
            wheelCollider.GetWorldPose(out pos, out rot);
            wheelTransform.rotation = rot;
            wheelTransform.position = pos;
        }
        
        // Public methods for AI control
        public void SetInput(float horizontal, float vertical, bool brake)
        {
            horizontalInput = horizontal;
            verticalInput = vertical;
            isBreaking = brake;
        }
        
        public float GetCurrentSpeed()
        {
            return vehicleRigidbody.linearVelocity.magnitude * 3.6f; // Convert to km/h
        }
        
        public Vector3 GetVelocity()
        {
            return vehicleRigidbody.linearVelocity;
        }
        
        public float GetSteerAngle()
        {
            return steerAngle;
        }
    }
}
