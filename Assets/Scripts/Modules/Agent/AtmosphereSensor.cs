﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtmosphereSensor : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] windDotX;
    public float[] windDotY;
    public float[] windDotZ;    

    public float sensitivity = 1f;
    //public Transform targetPosition;

    //public GameObject parentObject;
    //public Vector3 sensorPosition;

    public AtmosphereSensor() {
        /*parentID = genome.parentID;
        inno = genome.inno;
        dotX = new float[1];
        dotZ = new float[1];
        forward = new float[1];
        horizontal = new float[1];
        inTarget = new float[1];
        velX = new float[1];
        velZ = new float[1];
        targetHealth = new float[1];
        targetAttacking = new float[1];*/
    }

    public void Initialize(AtmosphereSensorGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;
        
        windDotX = new float[1];
        windDotY = new float[1];
        windDotZ = new float[1];        
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                //Debug.Log("neuron match!!! targetSensorX");
                neuron.currentValue = windDotX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                //Debug.Log("neuron match!!! targetSensorZ");
                neuron.currentValue = windDotY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 2) {
                //Debug.Log("neuron match!!! targetSensorZ");
                neuron.currentValue = windDotZ;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void Tick() {
        // NORMALIZED!!!!
        /*Vector3 segmentToTargetVect = new Vector3(targetPosition.position.x - parentObject.transform.position.x + sensorPosition.x, targetPosition.position.y - parentObject.transform.position.y + sensorPosition.x, targetPosition.position.z - parentObject.transform.position.z + sensorPosition.x);
        Vector3 segmentToTargetVectNormalized = segmentToTargetVect.normalized;
        Vector3 rightVector;
        Vector3 upVector;
        Vector3 forwardVector;

        rightVector = parentObject.transform.right;
        upVector = parentObject.transform.up;
        forwardVector = parentObject.transform.forward;

        // Not Normalized!
        float dotRight = Vector3.Dot(segmentToTargetVectNormalized, rightVector);
        float dotUp = Vector3.Dot(segmentToTargetVectNormalized, upVector);
        float dotForward = Vector3.Dot(segmentToTargetVectNormalized, forwardVector);

        windDotX[0] = dotRight;
        windDotY[0] = dotUp;
        windDotZ[0] = dotForward;

        dist[0] = segmentToTargetVect.magnitude;
        invDist[0] = 1f / (dist[0] + 0.000001f); // epsilon to prevent divide by 0
        */

        /*
        forward[0] = Vector3.Dot(segmentToTargetVect, forwardVector) * sensitivity;
        horizontal[0] = Vector3.Dot(segmentToTargetVect, rightVector) * sensitivity;
        float inT = 0f;
        if (segmentToTargetVect.sqrMagnitude <= 12f) { // hardcoded radius
            inT = 1f; // ?? better to just give inverse distance??
        }
        inTarget[0] = inT;

        Vector3 relativeVelocity;
        if (targetPosition.gameObject.GetComponent<Rigidbody>()) {
            relativeVelocity = targetPosition.gameObject.GetComponent<Rigidbody>().velocity - parentObject.GetComponent<Rigidbody>().velocity;
            Agent agentOpponent = targetPosition.gameObject.transform.parent.GetComponent<Agent>();
            float attacking = 0f;
            if (agentOpponent.weaponProjectileList[0].throttle[0] > 0 && agentOpponent.weaponProjectileList[0].energy[0] >= 0.1f) {
                attacking = 1f;
            }
            if (agentOpponent.weaponTazerList[0].throttle[0] > 0 && agentOpponent.weaponTazerList[0].energy[0] >= 0.01f) {
                attacking = 1f;
            }
            targetAttacking[0] = attacking;
        }
        else {
            relativeVelocity = parentObject.GetComponent<Rigidbody>().velocity;
        }
        velX[0] = relativeVelocity.x * sensitivity;
        velZ[0] = relativeVelocity.z * sensitivity;

        if (targetPosition.gameObject.GetComponent<HealthModuleComponent>()) {
            targetHealth[0] = targetPosition.gameObject.GetComponent<HealthModuleComponent>().healthModule.health / targetPosition.gameObject.GetComponent<HealthModuleComponent>().healthModule.maxHealth;
        }
        */
    }
}
