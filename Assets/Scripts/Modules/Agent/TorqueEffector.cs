using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorqueEffector : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] throttleX;
    public float[] throttleY;
    public float[] throttleZ;

    public bool useX;
    public bool useY;
    public bool useZ;

    public float strength;

    public GameObject parentBody;
    
	public TorqueEffector() {
       /* parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];*/
    }

    public void Initialize(TorqueGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        throttleX = new float[1];
        throttleY = new float[1];
        throttleZ = new float[1];

        useX = genome.useX;
        useY = genome.useY;
        useZ = genome.useZ;

        strength = genome.strength;

        parentBody = agent.segmentList[parentID];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {            
            if (nid.neuronID == 0) {
                if(useX) {
                    neuron.currentValue = throttleX;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }                
            }
            if (nid.neuronID == 1) {
                if (useY) {
                    neuron.currentValue = throttleY;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }                
            }
            if (nid.neuronID == 2) {
                if (useZ) {
                    neuron.currentValue = throttleZ;
                    neuron.neuronType = NeuronGenome.NeuronType.Out;
                }                
            }
        }
    }

    public void Tick() {
        parentBody.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(throttleX[0], throttleY[0], throttleZ[0]) * strength, ForceMode.Force);
    }
}
