using UnityEngine;


public class SimpleCarController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steeringAngle;
    public WheelCollider backRightWheelCollider, backLeftWheelCollider;
    public WheelCollider frontRightWheelCollider, frontLeftWheelCollider;
    public Transform backRightWheel, backLeftWheel;
    public Transform frontRightWheel, frontLeftWheel;
    // How fast we can turn
    public float maxSteerAngle = 30;
    public float motorForce = 200;

    void Start()
    {

    }

    void Update()
    {

    }

    public void GetInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
    }

    private void Steer()
    {
        // Case where colliders are not defined in the inspector.
        if (frontRightWheelCollider == null || frontLeftWheelCollider == null
        || backRightWheelCollider == null || backLeftWheelCollider == null)
        {
            return;
        }
        // Calculate steering angle from the input.
        steeringAngle = maxSteerAngle * horizontalInput;
        // Steer the wheel
        frontLeftWheelCollider.steerAngle = steeringAngle;
        frontRightWheelCollider.steerAngle = steeringAngle;
        backLeftWheelCollider.steerAngle = steeringAngle;
        backRightWheelCollider.steerAngle = steeringAngle;
    }

    private void Accelerate()
    {
        // Case where colliders are not defined in the inspector.
        if (frontRightWheelCollider == null || frontLeftWheelCollider == null
        || backRightWheelCollider == null || backLeftWheelCollider == null)
        {
            return;
        }
        // Calculate the force from the input.
        float force = verticalInput * motorForce;
        // Accelerate the wheel
        // This will be fixing FPS as 50 because at 50 FPS Time.deltaTime will be 0.2 and multiplying it with 
        // 500 makes it 1.
        frontLeftWheelCollider.motorTorque = verticalInput * motorForce * Time.deltaTime * 500;
        frontRightWheelCollider.motorTorque = verticalInput * motorForce * Time.deltaTime * 500;
        backLeftWheelCollider.motorTorque = verticalInput * motorForce * Time.deltaTime * 500;
        backRightWheelCollider.motorTorque = verticalInput * motorForce * Time.deltaTime * 500;

    }

    private void UpdateWheelPoses()
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

    private void FixedUpdate()
    {
        GetInput();
        Steer();
        Accelerate();
        UpdateWheelPoses();
    }


}
