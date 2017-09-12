using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicObstaclesGenome {

    public int numObstacles = 4;
    public float minObstacleSize = 0.5f;
    public float maxObstacleSize = 6f;
    public Vector2[] obstaclePositions;
    public float[] obstacleScales;

    public BasicObstaclesGenome() {
        
    }
    public BasicObstaclesGenome(BasicObstaclesGenome templateGenome) {
        numObstacles = templateGenome.numObstacles;
        minObstacleSize = templateGenome.minObstacleSize;
        maxObstacleSize = templateGenome.maxObstacleSize;

        obstaclePositions = new Vector2[numObstacles];
        obstacleScales = new float[numObstacles];
        for (int i = 0; i < obstaclePositions.Length; i++) {
            if (i < templateGenome.obstaclePositions.Length) {  // when changing numOctaves, doesn't immediately change parentgenome terrainWaves array
                //terrainWaves[i] = new Vector3(templateGenome.terrainWaves[i].x, templateGenome.terrainWaves[i].y, templateGenome.terrainWaves[i].z);
                //Debug.Log("Copy Terrain Genome: " + terrainWaves[i].ToString());
                obstaclePositions[i] = new Vector2(templateGenome.obstaclePositions[i].x, templateGenome.obstaclePositions[i].y);
                obstacleScales[i] = templateGenome.obstacleScales[i];
                // = size;
            }
            else {
                obstaclePositions[i] = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
                // Revisit this for prepping agentStartPositions!!!!
                if ((obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude < 0.15f) {
                    obstaclePositions[i] = new Vector2(0.5f, 0.5f) + (obstaclePositions[i] - new Vector2(0.5f, 0.5f)) * 0.15f / (obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude;
                }
                obstacleScales[i] = UnityEngine.Random.Range(1f, 1f);
            }
            
        }
    }

    public void InitializeRandomGenome() {        
        for (int i = 0; i < obstaclePositions.Length; i++) {
            obstaclePositions[i] = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            // Revisit this for prepping agentStartPositions!!!!
            if ((obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude < 0.15f) {
                obstaclePositions[i] = new Vector2(0.5f, 0.5f) + (obstaclePositions[i] - new Vector2(0.5f, 0.5f)) * 0.15f / (obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude;
            }
            obstacleScales[i] = UnityEngine.Random.Range(1f, 1f);
        }
        Debug.Log("InitializeRandomGenome scale[0]: " + obstacleScales[0].ToString());
    }

    public static BasicObstaclesGenome BirthNewGenome(BasicObstaclesGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        // OBSTACLES:
        // NEED TO COPY ALL ATTRIBUTES HERE unless I switch mutation process to go: full-copy, then re-traverse and mutate on a second sweep...
        BasicObstaclesGenome newGenome = new BasicObstaclesGenome();
        newGenome.numObstacles = parentGenome.numObstacles;
        newGenome.minObstacleSize = parentGenome.minObstacleSize;
        newGenome.maxObstacleSize = parentGenome.maxObstacleSize;

        
        newGenome.obstaclePositions = new Vector2[newGenome.numObstacles];
        newGenome.obstacleScales = new float[newGenome.numObstacles];
        for (int i = 0; i < parentGenome.obstaclePositions.Length; i++) {
            newGenome.obstaclePositions[i] = new Vector2(parentGenome.obstaclePositions[i].x, parentGenome.obstaclePositions[i].y);
            newGenome.obstacleScales[i] = parentGenome.obstacleScales[i];
            float rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < mutationRate) {
                float newPosX = UnityEngine.Random.Range(0f, 1f);
                newGenome.obstaclePositions[i].x = Mathf.Lerp(newGenome.obstaclePositions[i].x, newPosX, mutationDriftAmount);
            }
            if (rand < mutationRate) {
                float newPosZ = UnityEngine.Random.Range(0f, 1f);
                newGenome.obstaclePositions[i].y = Mathf.Lerp(newGenome.obstaclePositions[i].y, newPosZ, mutationDriftAmount);
            }
            if ((newGenome.obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude < 0.15f) {
                newGenome.obstaclePositions[i] = new Vector2(0.5f, 0.5f) + (newGenome.obstaclePositions[i] - new Vector2(0.5f, 0.5f)) * 0.15f / (newGenome.obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude;
            }
            if (rand < mutationRate) {
                float newScale = UnityEngine.Random.Range(newGenome.minObstacleSize, newGenome.maxObstacleSize);
                newGenome.obstacleScales[i] = Mathf.Lerp(newGenome.obstacleScales[i], newScale, mutationDriftAmount);
            }
        }
        return newGenome;
    }
}
