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
    public bool useMotors;
    public bool angleSensors;
    public bool velocitySensors;
    public bool positionSensors;
    public bool quaternionSensors;
    public bool usePistonY;
    // Settings

    public BasicJointGenome(BasicJointGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.motorStrength = template.motorStrength;
        this.angleSensitivity = template.angleSensitivity;
        this.useX = template.useX;
        this.useY = template.useY;
        this.useZ = template.useZ;
        this.useMotors = template.useMotors;
        this.angleSensors = template.angleSensors;
        this.velocitySensors = template.velocitySensors;
        this.positionSensors = template.positionSensors;
        this.quaternionSensors = template.quaternionSensors;
        this.usePistonY = template.usePistonY;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        if (useX) {
            if(useMotors) {
                NeuronGenome throttleX = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 0);
                neuronList.Add(throttleX);
            }            
            if (angleSensors) {
                NeuronGenome angleX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);
                neuronList.Add(angleX);
            }
            if (velocitySensors) {
                NeuronGenome velX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 6);
                neuronList.Add(velX);
            }
        }
        if (useY) {
            if(useMotors) {
                NeuronGenome throttleY = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 1);
                neuronList.Add(throttleY);
            }            
            if(angleSensors) {
                NeuronGenome angleY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 4);
                neuronList.Add(angleY);
            }
            if (velocitySensors) {
                NeuronGenome velY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 7);
                neuronList.Add(velY);
            }
        }
        if (useZ) {
            if (useMotors) {
                NeuronGenome throttleZ = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 2);
                neuronList.Add(throttleZ);
            }            
            if(angleSensors) {
                NeuronGenome angleZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 5);
                neuronList.Add(angleZ);
            }
            if (velocitySensors) {
                NeuronGenome velZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 8);
                neuronList.Add(velZ);
            }
        }        
        if(positionSensors) {
            NeuronGenome posX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 9);
            neuronList.Add(posX);
            NeuronGenome posY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 10);
            neuronList.Add(posY);
            NeuronGenome posZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 11);
            neuronList.Add(posZ);
        }
        if(quaternionSensors) {
            NeuronGenome quatX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 12);
            neuronList.Add(quatX);
            NeuronGenome quatY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 13);
            neuronList.Add(quatY);
            NeuronGenome quatZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 14);
            neuronList.Add(quatZ);
            NeuronGenome quatW = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 15);
            neuronList.Add(quatW);
        }
        if (usePistonY) {
            NeuronGenome pistonThrottleY = new NeuronGenome(NeuronGenome.NeuronType.Out, inno, 16);
            neuronList.Add(pistonThrottleY);
            NeuronGenome pistonPosY = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 17);
            neuronList.Add(pistonPosY);
        }
    }
}
