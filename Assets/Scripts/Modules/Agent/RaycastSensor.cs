using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastSensor {

    public int parentID;
    public int inno;
    public float[] distanceLeft;
    public float[] distanceLeftCenter;
    public float[] distanceCenter;
    public float[] distanceRightCenter;
    public float[] distanceRight;

    public Transform targetPosition;

    public RaycastSensor(RaycastSensorGenome genome) {
        parentID = genome.parentID;
        inno = genome.inno;
        distanceLeft = new float[1];
        distanceLeftCenter = new float[1];
        distanceCenter = new float[1];
        distanceRightCenter = new float[1];
        distanceRight = new float[1];
    }
}
