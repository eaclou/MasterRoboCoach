using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicAxle {
    public int parentID;
    public int inno;
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public float horsepower;
    public float maxSteering;
    public float brakePower;

    public float[] throttle;
    public float[] steerAngle;
    public float[] brake;

    public BasicAxle() {
        //parentID = genome.parentID;
        //inno = genome.inno;
        //throttle = new float[1];
        //strafe = new float[1];
        //throttle[0] = 0f;
    }

    public void Initialize(BasicAxleGenome genome) {
        inno = genome.inno;
        throttle = new float[1];
        steerAngle = new float[1];
        brake = new float[1];

        horsepower = genome.horsepower;
        maxSteering = genome.maxSteering;
        brakePower = genome.brakePower;

        //Debug.Log("BasicAxle horsepower= " + genome.horsepower.ToString());
    }

    public void Tick() {
        leftWheel.motorTorque = throttle[0] * horsepower;
        rightWheel.motorTorque = throttle[0] * horsepower;

        leftWheel.steerAngle = steerAngle[0] * maxSteering;
        rightWheel.steerAngle = steerAngle[0] * maxSteering;

        leftWheel.brakeTorque = Mathf.Clamp01(brake[0]) * brakePower;
        rightWheel.brakeTorque = Mathf.Clamp01(brake[0]) * brakePower;
    }
}
