using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaycastSensorGenome {
    public int parentID;
    public int inno;
    public Vector3 sensorPosition;
    public float maxDistance = 12f;

    public RaycastSensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public RaycastSensorGenome(RaycastSensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.sensorPosition = template.sensorPosition;
        this.maxDistance = template.maxDistance;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuronLeft = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        NeuronGenome neuronLeftCenter = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
        NeuronGenome neuronCenter = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 2);
        NeuronGenome neuronRightCenter = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
        NeuronGenome neuronRight = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 4);
        NeuronGenome neuronBack = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 5);
        NeuronGenome neuronCenterShort = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 6);
        neuronList.Add(neuronLeft);
        neuronList.Add(neuronLeftCenter);
        neuronList.Add(neuronCenter);
        neuronList.Add(neuronRightCenter);
        neuronList.Add(neuronRight);
        neuronList.Add(neuronBack);
        neuronList.Add(neuronCenterShort);
    }
}
