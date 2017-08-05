using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorqueEffector {
    public int parentID;
    public int inno;
    public float[] throttle;

    public GameObject parentBody;

	public TorqueEffector(TorqueGenome genome) {
       /* parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];*/
    }

    public void Initialize(TorqueGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("neuron match!!! torqueEffector");
            neuron.currentValue = throttle;
            neuron.neuronType = NeuronGenome.NeuronType.Out;
        }
    }

    public void Tick() {
        parentBody.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0f, throttle[0], 0f) * 0f, ForceMode.Force);
    }
}
