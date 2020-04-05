﻿using UnityEngine;


public class SimpleCarController : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float steerAngle;
    public WheelCollider backRightWheelCollider, backLeftWheelCollider;
    public WheelCollider frontRightWheelCollider, frontLeftWheelCollider;
    public Transform backRightWheel, backLeftWheel;
    public Transform frontRightWheel, frontLeftWheel;
    public float maxSteerAngle = 30;
    public float motorForce = 50;
    public float breakingForce = 30;
    public float turnSensitivity = 1;

    public void ProcessInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        //Reset
        if (Input.GetKeyDown(KeyCode.X))
        {
            Reset();
        }
    }

    private void Reset()
    {
        Vector3 rotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(0, rotation.y, 0));
    }

    private void Steer()
    {
        // Calculate steering angle from the input.
        steerAngle = maxSteerAngle * horizontalInput * turnSensitivity;
        // Steer the wheel. Only front wheels.
        frontLeftWheelCollider.steerAngle = Mathf.Lerp(frontLeftWheelCollider.steerAngle, steerAngle, 0.5f);
        frontRightWheelCollider.steerAngle = Mathf.Lerp(frontRightWheelCollider.steerAngle, steerAngle, 0.5f);
    }

    private void Move()
    {
        // Calculate the force from the input.
        // This will be fixing FPS as 50 because at 50 FPS Time.deltaTime will be 0.2 and multiplying it with 
        // 500 makes it 1.
        float force = verticalInput * motorForce * Time.deltaTime * 500;
        // Accelerate the wheel. Only front wheels.
        frontLeftWheelCollider.motorTorque = force;
        frontRightWheelCollider.motorTorque = force;
        backLeftWheelCollider.motorTorque = force;
        backRightWheelCollider.motorTorque = force;
        // If motor there is no motor force applied then apply a breaking force to stop the vechicle.
        force = force == 0 ? breakingForce : 0;
        frontLeftWheelCollider.brakeTorque = force;
        frontRightWheelCollider.brakeTorque = force;
        backLeftWheelCollider.brakeTorque = force;
        backRightWheelCollider.brakeTorque = force;
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

    private void LateUpdate()
    {
        ProcessInput();
        Steer();
        Move();
        UpdateWheelPoses();
    }
}
