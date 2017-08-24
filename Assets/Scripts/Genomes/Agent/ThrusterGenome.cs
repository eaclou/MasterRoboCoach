﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterGenome {
    public int parentID;
    public int inno;
    public Vector3 forcePoint;

    public float horsepowerX;
    public float horsepowerZ;

    /*public ThrusterGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }*/

    public ThrusterGenome(ThrusterGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.forcePoint = template.forcePoint;

        this.horsepowerX = template.horsepowerX;
        this.horsepowerZ = template.horsepowerZ;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 0);
        NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
        neuronList.Add(neuronZ);
        neuronList.Add(neuronX);
    }
}
