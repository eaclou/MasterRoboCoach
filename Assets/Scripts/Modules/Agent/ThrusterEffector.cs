using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ThrusterEffector {
    public int parentID;
    public int inno;
    public float[] throttle;
    public float[] strafe;

	public ThrusterEffector(ThrusterGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
        strafe = new float[1];
        //throttle[0] = 0f;
    }
}
