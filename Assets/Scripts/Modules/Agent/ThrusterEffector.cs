using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterEffector {
    public int parentID;
    public int inno;
    public float[] throttle;
    public float[] strafe;

    public float horsepowerX;
    public float horsepowerZ;

    public GameObject parentBody;
    public Vector3 forcePoint;

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

        horsepowerX = genome.horsepowerX;
        horsepowerZ = genome.horsepowerZ;
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
        parentBody.GetComponent<Rigidbody>().AddForceAtPosition(parentBody.transform.forward * Mathf.Clamp01(throttle[0]) * horsepowerZ, parentBody.GetComponent<Rigidbody>().worldCenterOfMass + new Vector3(0f, 0.005f, 0f)); //.AddRelativeForce(new Vector3(0f, 0f, Mathf.Clamp01(throttle[0])) * horsepowerZ, ForceMode.Force);
        parentBody.GetComponent<Rigidbody>().AddForceAtPosition(parentBody.transform.right * Mathf.Clamp01(strafe[0]) * horsepowerX, parentBody.GetComponent<Rigidbody>().worldCenterOfMass + new Vector3(0f, 0.005f, 0f));
    }
}
