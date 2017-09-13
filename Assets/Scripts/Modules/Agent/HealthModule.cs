using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HealthModule : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public bool destroyed = false;
    public float maxHealth;
    public float prevHealth;
    public float health;
    public float[] healthSensor;
    public float[] takingDamage;

    public HealthModuleComponent component;

    public HealthModule() {
        /*healthSensor = new float[1];
        takingDamage = new float[1];
        health = maxHealth;
        prevHealth = health;
        parentID = genome.parentID;
        inno = genome.inno;*/
    }

    public void Initialize(HealthGenome genome, Agent agent) {
        destroyed = false;
        healthSensor = new float[1];
        takingDamage = new float[1];
        maxHealth = genome.maxHealth;
        health = maxHealth;
        prevHealth = health;

        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        component = agent.segmentList[parentID].AddComponent<HealthModuleComponent>();
        if (component == null) {
            Debug.LogAssertion("No existing HealthModuleComponent on segment " + parentID.ToString());
        }
        component.healthModule = this;
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = healthSensor;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            if (nid.neuronID == 1) {
                neuron.currentValue = takingDamage;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void InflictDamage(float amount) {
        health -= amount;
        if(health <= 0f) {
            health = 0f;
            destroyed = true;
        }
    }

    public void Tick() {
        healthSensor[0] = health / maxHealth;
        if (health != prevHealth) {
            takingDamage[0] = 1f;
        }
        else {
            takingDamage[0] = 0f;
        }
        prevHealth = health;
        takingDamage[0] = (maxHealth - health) / maxHealth;
    }
}
