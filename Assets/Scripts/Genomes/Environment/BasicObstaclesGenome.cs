using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BasicObstaclesGenome {

    public Vector2[] obstaclePositions;
    public float[] obstacleScales;

    public BasicObstaclesGenome() {
        
    }
    public BasicObstaclesGenome(BasicObstaclesGenome templateGenome) {
        obstaclePositions = new Vector2[templateGenome.obstaclePositions.Length];
        obstacleScales = new float[templateGenome.obstacleScales.Length];
        for (int i = 0; i < obstaclePositions.Length; i++) {
            obstaclePositions[i] = new Vector2(templateGenome.obstaclePositions[i].x, templateGenome.obstaclePositions[i].y);
            float size = templateGenome.obstacleScales[i];
            obstacleScales[i] = size;
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
    }

    public static BasicObstaclesGenome BirthNewGenome(BasicObstaclesGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        // OBSTACLES:
        BasicObstaclesGenome newGenome = new BasicObstaclesGenome();
        newGenome.obstaclePositions = new Vector2[parentGenome.obstaclePositions.Length];
        newGenome.obstacleScales = new float[parentGenome.obstacleScales.Length];
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
                float newScale = UnityEngine.Random.Range(1f, 5f);
                newGenome.obstacleScales[i] = Mathf.Lerp(newGenome.obstacleScales[i], newScale, mutationDriftAmount);
            }
        }
        return newGenome;
    }
}
