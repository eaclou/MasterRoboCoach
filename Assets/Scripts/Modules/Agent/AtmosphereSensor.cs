using System.Collections;
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

    public GameObject parentObject;
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

        parentObject = agent.segmentList[parentID];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = windDotX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = windDotY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 2) {
                neuron.currentValue = windDotZ;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void Tick(Environment currentEnvironment) {
        if(currentEnvironment.environmentGameplay.atmosphere != null) {
            Vector3 rightVector = parentObject.transform.right;
            Vector3 upVector = parentObject.transform.up;
            Vector3 forwardVector = parentObject.transform.forward;

            // Not Normalized!
            float dotRight = Vector3.Dot(currentEnvironment.environmentGameplay.atmosphere.genome.windForce, rightVector);
            float dotUp = Vector3.Dot(currentEnvironment.environmentGameplay.atmosphere.genome.windForce, upVector);
            float dotForward = Vector3.Dot(currentEnvironment.environmentGameplay.atmosphere.genome.windForce, forwardVector);

            windDotX[0] = dotRight;
            windDotY[0] = dotUp;
            windDotZ[0] = dotForward;
        }
    }
}
