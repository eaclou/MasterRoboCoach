using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorqueEffector {
    public int parentID;
    public int inno;
    public float[] throttle;

	public TorqueEffector(TorqueGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        throttle = new float[1];
    }
}
