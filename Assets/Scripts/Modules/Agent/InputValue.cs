using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputValue : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] value;

	public InputValue() {
        /*Debug.Log("InputValue(ValueInputGenome genome)");
        parentID = genome.parentID; 
        inno = genome.inno;
        value = new float[1];
        value[0] = genome.val;*/
    }

    public void Initialize(ValueInputGenome genome, Agent agent) {
        //Debug.Log("InputValue(ValueInputGenome genome)");
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        value = new float[1];
        value[0] = genome.val;
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("neuron match!!! inputValue");
            neuron.currentValue = value;
            neuron.neuronType = NeuronGenome.NeuronType.In;
        }
    }

    public void Tick() {

    }
}
