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
    }
}
