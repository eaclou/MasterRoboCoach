using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSensorGenome {
    public int parentID;
    public int inno;
    public Vector3 sensorPosition;
    public float sensitivity = 1f;
    public bool useX;
    public bool useY;
    public bool useZ;
    public bool useVel;
    public bool useDist;
    public bool useInvDist;    

    public TargetSensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;        
    }

    public TargetSensorGenome(TargetSensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.sensorPosition = template.sensorPosition;
        sensitivity = template.sensitivity;
        useX = template.useX;
        useY = template.useY;
        useZ = template.useZ;
        useVel = template.useVel;
        useDist = template.useDist;
        useInvDist = template.useInvDist;        
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        if(useX) {
            if (!useVel) {
                NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
                neuronList.Add(neuronX);
            }
            else {
                NeuronGenome neuronVelX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
                neuronList.Add(neuronVelX);
            }
        }
        if (useY) {
            if (!useVel) {
                NeuronGenome neuronY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 2);
                neuronList.Add(neuronY);
            }
            else {
                NeuronGenome neuronVelY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
                neuronList.Add(neuronVelY);
            }
        }
        if (useZ) {
            if (!useVel) {
                NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 4);
                neuronList.Add(neuronZ);
            }
            else {
                NeuronGenome neuronVelZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 5);
                neuronList.Add(neuronVelZ);
            }
        }
        if(useDist) {
            NeuronGenome neuronDist = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 6);
            neuronList.Add(neuronDist);
        }
        if (useInvDist) {
            NeuronGenome neuronInvDist = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 7);
            neuronList.Add(neuronInvDist);
        }
    }
}
