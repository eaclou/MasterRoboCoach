using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GravitySensor : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] dotX;
    public float[] dotY;
    public float[] dotZ;

    public GameObject parentObject;

    public GravitySensor() {

    }

    public void Initialize(GravitySensorGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;
        
        dotX = new float[1];
        dotY = new float[1];
        dotZ = new float[1];

        parentObject = agent.segmentList[parentID];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = dotX;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = dotY;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 2) {
                neuron.currentValue = dotZ;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void Tick() {
        // NORMALIZED!!!!
        
        //Vector3 segmentToTargetVect = new Vector3(targetPosition.position.x - parentObject.transform.position.x + sensorPosition.x, targetPosition.position.y - parentObject.transform.position.y + sensorPosition.x, targetPosition.position.z - parentObject.transform.position.z + sensorPosition.x);
        //Vector3 segmentToTargetVectNormalized = segmentToTargetVect.normalized;

        Vector3 gravityDirection = Physics.gravity.normalized;
        Vector3 rightVector = parentObject.transform.right;
        Vector3 upVector = parentObject.transform.up;
        Vector3 forwardVector = parentObject.transform.forward;
        

        // Not Normalized!
        float dotRight = Vector3.Dot(gravityDirection, rightVector);
        float dotUp = Vector3.Dot(gravityDirection, upVector);
        float dotForward = Vector3.Dot(gravityDirection, forwardVector);

        dotX[0] = dotRight;
        dotY[0] = dotUp;
        dotZ[0] = dotForward;
    }
}
