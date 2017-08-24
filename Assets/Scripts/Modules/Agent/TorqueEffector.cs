using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorqueEffector : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] throttle;

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

        throttle = new float[1];

        strength = genome.strength;

        parentBody = agent.segmentList[parentID];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("neuron match!!! torqueEffector");
            neuron.currentValue = throttle;
            neuron.neuronType = NeuronGenome.NeuronType.Out;
        }
    }

    public void Tick() {
        parentBody.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0f, throttle[0], 0f) * strength, ForceMode.Force);
    }
}
