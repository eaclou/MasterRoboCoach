using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OscillatorGenome {

    public int parentID;
    public int inno;
    public float freq = 1f;
    public float amp = 1f;

    public OscillatorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public OscillatorGenome(OscillatorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.freq = template.freq;
        this.amp = template.amp;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuron = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        neuronList.Add(neuron);
    }

    /*public void CopyAttributesFromSourceGenome(OscillatorGenome sourceGenome) {
        this.parentID = sourceGenome.parentID;
        this.inno = sourceGenome.inno;
        this.freq = sourceGenome.freq;
        this.amp = sourceGenome.amp;
    }*/
}
