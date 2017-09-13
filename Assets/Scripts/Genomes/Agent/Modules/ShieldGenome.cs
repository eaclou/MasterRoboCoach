using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShieldGenome {

    public int parentID;
    public int inno;
    //public Vector3 muzzleLocation;

    public ShieldGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public ShieldGenome(ShieldGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        //this.muzzleLocation = template.muzzleLocation;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuron1 = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        //NeuronGenome neuron2 = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
        neuronList.Add(neuron1);
        //neuronList.Add(neuron2);
    }
}
