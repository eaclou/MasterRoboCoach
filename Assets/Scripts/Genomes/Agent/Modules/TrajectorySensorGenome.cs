using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrajectorySensorGenome {
    public int parentID;
    public int inno;
    public Vector3 sensorPosition;

    public TrajectorySensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public TrajectorySensorGenome(TrajectorySensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.sensorPosition = template.sensorPosition;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        NeuronGenome neuronY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
        NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 2);

        neuronList.Add(neuronX);
        neuronList.Add(neuronY);
        neuronList.Add(neuronZ);
    }
}
