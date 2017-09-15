using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TargetColumnGenome {

    //public EnvironmentGenome parentGenome;

    public float targetRadius;
    public float minX;  // 0 - 1
    public float maxX;  // 0 - 1
    public float minZ;  // 0 - 1
    public float maxZ;  // 0 - 1
    public float minDistanceFromAgent;

    // constructor
	public TargetColumnGenome() {
        //this.parentGenome = parentGenome;
    }

    public TargetColumnGenome(TargetColumnGenome templateGenome) {
        targetRadius = templateGenome.targetRadius;
        minX = templateGenome.minX;
        maxX = templateGenome.maxX;
        minZ = templateGenome.minZ;
        maxZ = templateGenome.maxZ;
        minDistanceFromAgent = templateGenome.minDistanceFromAgent;
    }

    public void InitializeRandomGenome() {
        targetRadius = 0.33f;
        minX = UnityEngine.Random.Range(0f, 1f);
        maxX = UnityEngine.Random.Range(1f, 1f);
        minZ = UnityEngine.Random.Range(0f, 1f);
        maxZ = UnityEngine.Random.Range(1f, 1f);
        minDistanceFromAgent = 0.2f;
    }

    public static TargetColumnGenome BirthNewGenome(TargetColumnGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        // TARGET: 
        TargetColumnGenome newGenome = new TargetColumnGenome();
               
        newGenome.targetRadius = parentGenome.targetRadius;
        newGenome.minX = parentGenome.minX;
        newGenome.maxX = parentGenome.maxX;
        newGenome.minZ = parentGenome.minZ;
        newGenome.maxZ = parentGenome.maxZ;
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float newX = Mathf.Lerp(newGenome.minX, UnityEngine.Random.Range(0f, 1f), mutationDriftAmount);
            newGenome.minX = Mathf.Min(newX, parentGenome.maxX);  // prevent min being bigger than max
        }
        rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float newX = Mathf.Lerp(newGenome.maxX, UnityEngine.Random.Range(0f, 1f), mutationDriftAmount);
            newGenome.maxX = Mathf.Max(newX, parentGenome.minX);
        }
        rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float newZ = Mathf.Lerp(newGenome.minZ, UnityEngine.Random.Range(0f, 1f), mutationDriftAmount);
            newGenome.minZ = Mathf.Min(newZ, parentGenome.maxZ);  // prevent min being bigger than max
        }
        rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float newZ = Mathf.Lerp(newGenome.maxZ, UnityEngine.Random.Range(0f, 1f), mutationDriftAmount);
            newGenome.maxZ = Mathf.Max(newZ, parentGenome.minZ);
        }
        return newGenome;
    }
}
