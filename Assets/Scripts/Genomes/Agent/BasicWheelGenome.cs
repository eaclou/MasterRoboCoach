using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicWheelGenome {
    public int parentID;
    public int inno;
    public float horsepower;
    public float maxSteering;
    public float brakePower;
    // Settings

    public BasicWheelGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public BasicWheelGenome(BasicWheelGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.horsepower = template.horsepower;
        this.maxSteering = template.maxSteering;
        this.brakePower = template.brakePower;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome throttle = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 0);
        NeuronGenome steerAngle = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
        NeuronGenome brake = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 2);
        //NeuronGenome speed = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
        neuronList.Add(throttle);
        neuronList.Add(steerAngle);
        neuronList.Add(brake);
        //neuronList.Add(speed);
    }
}
