using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContactSensor {
    public int parentID;
    public int inno;
    public float[] contactSensor;

    public ContactSensorComponent component;

    public ContactSensor(ContactGenome genome) {
        contactSensor = new float[1];
        parentID = genome.parentID;
        inno = genome.inno;
    }

    public void Initialize(ContactGenome genome) {
        contactSensor = new float[1];
        parentID = genome.parentID;
        inno = genome.inno;

        component.sensor = this;
    }

    public void MapNeuron(NID nid, Neuron neuron) {
        if (inno == nid.moduleID) {
            if (nid.neuronID == 0) {
                neuron.currentValue = contactSensor;
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
        }
    }

    public void Tick() {
        float contact = 0f;
        if (component.contact) {
            contact = 1f;
        }
        contactSensor[0] = contact;
    }
}
