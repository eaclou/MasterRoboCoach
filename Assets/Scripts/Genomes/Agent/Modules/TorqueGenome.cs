using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TorqueGenome {
    public int parentID;
    public int inno;

    public float strength;

    public bool useX;
    public bool useY;
    public bool useZ;

    public TorqueGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public TorqueGenome(TorqueGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;

        this.strength = template.strength;
        useX = template.useX;
        useY = template.useY;
        useZ = template.useZ;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        if(useX) {
            NeuronGenome neuron1 = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 0);
            neuronList.Add(neuron1);
        }
        if (useY) {
            NeuronGenome neuron2 = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
            neuronList.Add(neuron2);
        }
        if (useZ) {
            NeuronGenome neuron3 = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 2);
            neuronList.Add(neuron3);
        }
    }
}
