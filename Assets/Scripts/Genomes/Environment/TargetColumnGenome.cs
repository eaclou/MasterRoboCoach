using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetColumnGenome {

    public EnvironmentGenome parentGenome;

    public float targetRadius;
    public float minX;  // 0 - 1
    public float maxX;  // 0 - 1
    public float minZ;  // 0 - 1
    public float maxZ;  // 0 - 1
    public float minDistanceFromAgent;
    public float maxDistanceFromAgent;

    // constructor
	public TargetColumnGenome(EnvironmentGenome parentGenome) {
        this.parentGenome = parentGenome;
    }

    public void InitializeRandomGenome() {
        targetRadius = 0.5f;
        minX = UnityEngine.Random.Range(0f, 1f);
        maxX = UnityEngine.Random.Range(0f, 1f);
        minZ = UnityEngine.Random.Range(0f, 1f);
        maxZ = UnityEngine.Random.Range(0f, 1f);
        //minDistanceFromAgent = 1f;
        //maxDistanceFromAgent = 40f;
    }
}
