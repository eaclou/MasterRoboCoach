using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RaycastSensor {

    public int parentID;
    public int inno;
    public Vector3 sensorPosition;
    public float[] distanceLeft;
    public float[] distanceLeftCenter;
    public float[] distanceCenter;
    public float[] distanceRightCenter;
    public float[] distanceRight;
    public float[] distanceBack;
    public float[] distanceCenterShort;

    public GameObject parentObject;
    public Transform targetPosition;    

    public RaycastSensor(RaycastSensorGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        distanceLeft = new float[1];
        distanceLeftCenter = new float[1];
        distanceCenter = new float[1];
        distanceRightCenter = new float[1];
        distanceRight = new float[1];
        distanceBack = new float[1];
        distanceCenterShort = new float[1];
    }

    public void Initialize(RaycastSensorGenome genome) {
        inno = genome.inno;
        distanceLeft = new float[1];
        distanceLeftCenter = new float[1];
        distanceCenter = new float[1];
        distanceRightCenter = new float[1];
        distanceRight = new float[1];
        distanceBack = new float[1];
        distanceCenterShort = new float[1];
    }
}
