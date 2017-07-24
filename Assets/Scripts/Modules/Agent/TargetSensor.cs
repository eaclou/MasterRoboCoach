using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSensor {
    public int parentID;
    public int inno;
    public float[] dotX;
    public float[] dotY;
    public float[] dotZ;
    public float[] vel;
    public float[] angVel;
    public float[] inTarget;

    public Transform targetPosition;

	public TargetSensor(TargetSensorGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        dotX = new float[1];
        dotY = new float[1];
        dotZ = new float[1];
        vel = new float[1];
        angVel = new float[1];
        inTarget = new float[1];
    }
}
