using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputOscillator : AgentModuleBase {

    //public int parentID;
    //public int inno;
    public float freq;
    public float amp;

    public float[] value;

    public InputOscillator() {
        //parentID = genome.parentID;
        //inno = genome.inno;
        //throttle = new float[1];
        //strafe = new float[1];
        //throttle[0] = 0f;
    }

    public void Initialize(OscillatorGenome genome, Agent agent) {
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;
        value = new float[1];

        freq = genome.freq;
        amp = genome.amp;        
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            //Debug.Log("neuron match!!! inputValue");
            neuron.currentValue = value;
            neuron.neuronType = NeuronGenome.NeuronType.In;
        }
    }

    public void Tick(int timeStep) {
        value[0] = Mathf.Sin((float)timeStep * 0.05f * freq) * amp;
    }
}
