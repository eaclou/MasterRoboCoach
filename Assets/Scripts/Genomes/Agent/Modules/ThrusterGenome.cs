using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterGenome {
    public int parentID;
    public int inno;
    public Vector3 forcePoint;

    public float horsepowerX;
    public float horsepowerY;
    public float horsepowerZ;

    public bool useX;
    public bool useY;
    public bool useZ;

    public ThrusterGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public ThrusterGenome(ThrusterGenome template) {
        parentID = template.parentID;
        inno = template.inno;
        forcePoint = template.forcePoint;

        horsepowerX = template.horsepowerX;
        horsepowerY = template.horsepowerY;
        horsepowerZ = template.horsepowerZ;

        useX = template.useX;
        useY = template.useY;
        useZ = template.useZ;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        if(useX) {
            NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 0);
            neuronList.Add(neuronX);
        }
        if (useY) {
            NeuronGenome neuronY = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
            neuronList.Add(neuronY);
        }
        if (useZ) {
            NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 2);
            neuronList.Add(neuronZ);
        }        
    }
}
