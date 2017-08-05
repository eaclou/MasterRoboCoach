using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSensorGenome {
    public int parentID;
    public int inno;
	
    public TargetSensorGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public TargetSensorGenome(TargetSensorGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome neuronX = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        NeuronGenome neuronZ = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
        //NeuronGenome forward = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 2);
        //NeuronGenome horizontal = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 3);
        //NeuronGenome neuronInTarget = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 4);
        //NeuronGenome velX = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 5);
        //NeuronGenome velZ = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 6);
        //NeuronGenome health = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 7);
        //NeuronGenome attacking = new NeuronGenome(NeuronGenome.NeuronType.In, targetSensorList[i].inno, 8);
        NeuronGenome neuronDist = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 2);
        NeuronGenome neuronInvDist = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 3);

        neuronList.Add(neuronX);
        neuronList.Add(neuronZ);
        //brainGenome.neuronList.Add(forward);
        //brainGenome.neuronList.Add(horizontal);
        //brainGenome.neuronList.Add(neuronInTarget);
        //brainGenome.neuronList.Add(velX);
        //brainGenome.neuronList.Add(velZ);
        //brainGenome.neuronList.Add(health);
        //brainGenome.neuronList.Add(attacking);
        neuronList.Add(neuronDist);
        neuronList.Add(neuronInvDist);
    }
}
