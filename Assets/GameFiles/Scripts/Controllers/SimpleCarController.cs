using UnityEngine;

//REF: https://www.youtube.com/watch?v=mnAEeE3FcvA
//I referenced the above video for Wheel physics
public class SimpleCarController : MonoBehaviour
{
    //Public fields
    public WheelCollider backRightWheelCollider, backLeftWheelCollider;
    public WheelCollider frontRightWheelCollider, frontLeftWheelCollider;
    public Transform backRightWheel, backLeftWheel;
    public Transform frontRightWheel, frontLeftWheel;
    public float maxSteerAngle = 30;
    public float motorForce = 500;
    public float breakingForce = 400;
    public float turnSensitivity = 1;
    public float accMultiplier = 1;
    public float deaccMultiplier = 1;
    public float handbrakeForce = 800;
    //Calculated using a cube on the modal prefab.
    public Vector3 carDimensions = new Vector3(1.5f, 1.2f, 3.5f);

    //Private fields
    private bool handbreak;
    new private Rigidbody rigidbody;

    void Start()
    {
        rigidbody = GetComponentInParent<Rigidbody>();
    }


    //Custom methods
    public void Steer(float horizontalInput)
    {
        // Calculate steering angle from the input.
        float steerAngle = maxSteerAngle * horizontalInput * turnSensitivity;
        // Steer the wheel. Only front wheels.
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, steerAngle, 0.5f);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, steerAngle, 0.5f);
    }


    public void NitroBoost(float multiplier, float duration)
    {
        accMultiplier = multiplier;
        Invoke("ResetAccelerationMultipliers", duration);
    }

    private void ResetAccelerationMultipliers()
    {
        accMultiplier = 1;
        deaccMultiplier = 1;
    }

    public void Move(float verticalInput)
    {
        // Calculate the force from the input.
        float force = verticalInput * motorForce * accMultiplier;
        // Accelerate the wheel. Only front wheels.
        frontLeftWheelCollider.motorTorque = force;
        frontRightWheelCollider.motorTorque = force;
        backLeftWheelCollider.motorTorque = force;
        backRightWheelCollider.motorTorque = force;
        // If motor there is no motor force applied then apply a breaking force to stop the vechicle.
        force = force == 0 ? breakingForce * deaccMultiplier : 0;
        //Handbreak override default breaking force.
        force = handbreak ? handbrakeForce : force;
        frontLeftWheelCollider.brakeTorque = force;
        frontRightWheelCollider.brakeTorque = force;
        backLeftWheelCollider.brakeTorque = force;
        backRightWheelCollider.brakeTorque = force;
    }
    public void UpdateWheelPoses()
    {
        UpdateWheelPose(frontRightWheelCollider, frontRightWheel);
        UpdateWheelPose(frontLeftWheelCollider, frontLeftWheel);
        UpdateWheelPose(backRightWheelCollider, backRightWheel);
        UpdateWheelPose(backLeftWheelCollider, backLeftWheel);
    }

    private void UpdateWheelPose(WheelCollider collider, Transform transform)
    {
        Vector3 pos = transform.position;
        Quaternion quaternion = transform.rotation;

        // out is used to pass by reference in C#
        collider.GetWorldPose(out pos, out quaternion);

        transform.position = pos;
        transform.rotation = quaternion;
    }

    public void ApplyHandBrake()
    {
        handbreak = true;
    }

    public void ReleaseHandBrake()
    {
        handbreak = false;
    }

    public void Drift(bool drifting, float horizontalInput)
    {
        
    }

    //Map a value from one range to other.
    private float RemapRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        return (((value - oldMin) / (oldMax - oldMin)) * (newMax - newMin)) + newMin;
    }
}
