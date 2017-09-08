using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContactGenome {
    public int parentID;
    public int inno;

    public ContactGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public ContactGenome(ContactGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuron1 = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        neuronList.Add(neuron1);
    }
}
