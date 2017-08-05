﻿using System.Collections;
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

    public float[] speed;

    public GameObject parentBody;
    
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

        //speedX = new float[1];
        speed = new float[1];

        horsepower = genome.horsepower;
        maxSteering = genome.maxSteering;
        brakePower = genome.brakePower;

        //Debug.Log("BasicAxle horsepower= " + genome.horsepower.ToString());
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = throttle;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = steerAngle;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 2) {
                neuron.currentValue = brake;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 3) {
                neuron.currentValue = speed;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void Tick() {
        speed[0] = parentBody.GetComponent<Rigidbody>().velocity.magnitude;
        //speed[0] = Vector3.Dot(parentBody.transform.forward, parentBody.GetComponent<Rigidbody>().velocity);
        float inverseSteering = 1f;
        if (speed[0] < 0f) {
            //inverseSteering = -1f;
        }

        leftWheel.motorTorque = throttle[0] * horsepower;
        rightWheel.motorTorque = throttle[0] * horsepower;

        leftWheel.steerAngle = steerAngle[0] * maxSteering * inverseSteering;
        rightWheel.steerAngle = steerAngle[0] * maxSteering * inverseSteering;

        leftWheel.brakeTorque = Mathf.Clamp01(brake[0]) * brakePower;
        rightWheel.brakeTorque = Mathf.Clamp01(brake[0]) * brakePower;        
    }
}
