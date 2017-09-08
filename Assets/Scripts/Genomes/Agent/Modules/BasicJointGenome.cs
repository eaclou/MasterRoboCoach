using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicJointGenome {

    public int parentID;
    public int inno;
    public float motorStrength;
    public float angleSensitivity;
    public bool useX;
    public bool useY;
    public bool useZ;
    // Settings

    public BasicJointGenome(BasicJointGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.motorStrength = template.motorStrength;
        this.angleSensitivity = template.angleSensitivity;
        this.useX = template.useX;
        this.useY = template.useY;
        this.useZ = template.useZ;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        if (useX) {
            NeuronGenome throttleX = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 0);
            NeuronGenome angleX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
            neuronList.Add(throttleX);
            neuronList.Add(angleX);
        }
        if (useY) {
            NeuronGenome throttleY = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
            NeuronGenome angleY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 4);
            neuronList.Add(throttleY);
            neuronList.Add(angleY);
        }
        if (useZ) {
            NeuronGenome throttleZ = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 2);
            NeuronGenome angleZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 5);
            neuronList.Add(throttleZ);
            neuronList.Add(angleZ);
        }
    }
}
