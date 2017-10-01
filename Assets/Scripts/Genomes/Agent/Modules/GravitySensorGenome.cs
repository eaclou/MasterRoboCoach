using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GravitySensorGenome {
    public int parentID;
    public int inno;
    //public Vector3 sensorPosition;
    public bool useGravityDir = true;
    public bool useVel;
    public bool useAltitude;

    public GravitySensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public GravitySensorGenome(GravitySensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.useGravityDir = template.useGravityDir;
        this.useVel = template.useVel;
        this.useAltitude = template.useAltitude;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        if(useGravityDir) {
            NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
            NeuronGenome neuronY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
            NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 2);
            neuronList.Add(neuronX);
            neuronList.Add(neuronY);
            neuronList.Add(neuronZ);
        }
        if(useVel) {
            NeuronGenome neuronVelX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
            NeuronGenome neuronVelY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 4);
            NeuronGenome neuronVelZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 5);
            neuronList.Add(neuronVelX);
            neuronList.Add(neuronVelY);
            neuronList.Add(neuronVelZ);
        }
        if(useAltitude) {
            NeuronGenome neuronAltitude = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 6);
            neuronList.Add(neuronAltitude);
        }
    }
}
