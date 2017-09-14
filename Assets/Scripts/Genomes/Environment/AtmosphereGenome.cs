using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AtmosphereGenome {

    //public int numObstacles = 4;
    //public float minObstacleSize = 0.5f;
    //public float maxObstacleSize = 6f;
    //public Vector2[] obstaclePositions;
    //public float[] obstacleScales;
    public float maxWindSpeed = 5f;
    public Vector3 windForce = Vector3.zero;

    public AtmosphereGenome() {
        
    }
    public AtmosphereGenome(AtmosphereGenome templateGenome) {
        windForce = templateGenome.windForce;
        maxWindSpeed = templateGenome.maxWindSpeed;
        
    }

    public void InitializeRandomGenome() {

        Vector3 windDir = UnityEngine.Random.onUnitSphere;
        windForce = windDir * UnityEngine.Random.Range(0f, maxWindSpeed);
        
    }

    public static AtmosphereGenome BirthNewGenome(AtmosphereGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        // OBSTACLES:
        // NEED TO COPY ALL ATTRIBUTES HERE unless I switch mutation process to go: full-copy, then re-traverse and mutate on a second sweep...
        AtmosphereGenome newGenome = new AtmosphereGenome();
        newGenome.windForce = parentGenome.windForce;
        newGenome.maxWindSpeed = parentGenome.maxWindSpeed;

        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            Vector3 randDir = UnityEngine.Random.onUnitSphere;
            Vector3 randForce = randDir * UnityEngine.Random.Range(0f, newGenome.maxWindSpeed);

            newGenome.windForce = Vector3.Lerp(newGenome.windForce, randForce, mutationDriftAmount);
        }
            
        return newGenome;
    }
}
