using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ContactSensor : AgentModuleBase {
    //public int parentID;
    //public int inno;
    public float[] contactSensor;

    public ContactSensorComponent component;

    public ContactSensor() {
        //contactSensor = new float[1];
        //parentID = genome.parentID;
        //inno = genome.inno;
    }

    public void Initialize(ContactGenome genome, Agent agent) {
        contactSensor = new float[1];
        parentID = genome.parentID;
        inno = genome.inno;
        isVisible = agent.isVisible;

        component = agent.segmentList[parentID].AddComponent<ContactSensorComponent>();
        //Debug.Log(component.ToString());
        if (component == null) {
            Debug.LogAssertion("No existing ContactSensorComponent on segment " + parentID.ToString());
        }
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
