using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterEffector {
    public int parentID;
    public int inno;
    public float[] throttle;
    public float[] strafe;

    public GameObject parentBody;

	public ThrusterEffector(ThrusterGenome genome) {
        /*parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        strafe = new float[1];
        //throttle[0] = 0f;*/
    }

    public void Initialize(ThrusterGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        strafe = new float[1];
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("neuron match!!! thrusterEffector");
            if (nid.neuronID == 0) {
                neuron.currentValue = throttle;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = strafe;
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
        }
    }

    public void Tick() {
        parentBody.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0f, 0.01f, throttle[0]) * 9000f, ForceMode.Force);
        parentBody.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(strafe[0], 0f, 0f) * 0f, ForceMode.Force);
    }
}
