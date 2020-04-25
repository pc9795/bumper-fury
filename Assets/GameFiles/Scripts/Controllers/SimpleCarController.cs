using UnityEngine;

//REF: https://www.youtube.com/watch?v=mnAEeE3FcvA
//I referenced the above video for Wheel physics
public class SimpleCarController : MonoBehaviour
{
    //Public fields
    //Wheel colliders for back wheels
    public WheelCollider backRightWheelCollider, backLeftWheelCollider;
    //Wheel colliders for front wheels
    public WheelCollider frontRightWheelCollider, frontLeftWheelCollider;
    //Transforms of the back wheels
    public Transform backRightWheel, backLeftWheel;
    //Transofrms of the front wheels
    public Transform frontRightWheel, frontLeftWheel;
    //Maximum steering angle in x direction
    public float maxSteerAngle = 30;
    //Maximum torque in z direction.
    public float motorForce = 500;
    //Force to apply when no forward force. Ideally it should not be there if friction curve is already in place.
    public float breakingForce = 400;
    //Turning sensitivity. Not using this right now. Can remove this in future.
    public float turnSensitivity = 1;
    //Accleration multiplier. Supposed to be used for nitro purposes. Greather than 1 in action.
    public float accMultiplier = 1;
    //Deaccleration multiplier. Suppose to be used for environments such as swamp where speed is reduced. Less than 1 in action.
    public float deaccMultiplier = 1;
    //Handbrake force
    public float handbrakeForce = 800;
    //Dimensions of the car.
    //Current caluclation is done using a cube on the modal prefab.
    public Vector3 carDimensions = new Vector3(1.5f, 1.2f, 3.5f);

    //Private fields
    //Status to control whether to apply handbrake or not.
    private bool handbreak;
    //Rigidbody to which this script is attached.
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
        //Remove the effects after some time.
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
        // Accelerate the wheel.
        frontLeftWheelCollider.motorTorque = force;
        frontRightWheelCollider.motorTorque = force;
        backLeftWheelCollider.motorTorque = force;
        backRightWheelCollider.motorTorque = force;
        // If motor there is no motor force applied then apply a breaking force to stop the vechicle.
        force = force == 0 ? breakingForce : 0;
        //Handbreak override default breaking force.
        force = handbreak ? handbrakeForce : force;
        //Break the wheels
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

    //Update the physical wheel according to collider.
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
}
