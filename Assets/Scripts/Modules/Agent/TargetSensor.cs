using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetSensor {
    public int parentID;
    public int inno;
    public float[] dotX;
    public float[] dotZ;
    public float[] forward;
    public float[] horizontal;
    public float[] inTarget;
    public float[] velX;
    public float[] velZ;
    public float[] targetHealth;
    public float[] targetAttacking;

    public float sensitivity = 0.1f;
    public Transform targetPosition;

	public TargetSensor(TargetSensorGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        dotX = new float[1];
        dotZ = new float[1];
        forward = new float[1];
        horizontal = new float[1];
        inTarget = new float[1];
        velX = new float[1];
        velZ = new float[1];
        targetHealth = new float[1];
        targetAttacking = new float[1];
    }
}
