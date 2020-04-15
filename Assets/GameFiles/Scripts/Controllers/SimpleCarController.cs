using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    //Public fields
    public WheelCollider backRightWheelCollider, backLeftWheelCollider;
    public WheelCollider frontRightWheelCollider, frontLeftWheelCollider;
    public Transform backRightWheel, backLeftWheel;
    public Transform frontRightWheel, frontLeftWheel;
    public float maxSteerAngle = 45;
    public float motorForce = 50;
    public float breakingForce = 30;
    public float turnSensitivity = 1;   
    public float accMultiplier = 1;
    public float deaccMultiplier = 1;


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
}
