using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthGenome {
    public int parentID;
    public int inno;
    public int maxHealth;


    public HealthGenome(int parentID, int inno) {
        this.parentID = parentID;
        this.inno = inno;
    }

    public HealthGenome(HealthGenome template) {
        this.parentID = template.parentID;
        this.inno = template.inno;
        this.maxHealth = template.maxHealth;
    }

    public void InitializeBrainGenome(List<NeuronGenome> neuronList) {
        NeuronGenome health = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 0);
        NeuronGenome takingDamage = new NeuronGenome(NeuronGenome.NeuronType.In, inno, 1);
        neuronList.Add(health);
        neuronList.Add(takingDamage);
    }
}
