using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentGenome {

    public Challenge.Type challengeType;
    public Vector3 arenaBounds;
    public float altitude;
    public Vector3 color;
    public Vector3[] terrainWaves;  // x = freq, y = amp, z = offset
    public Vector2[] obstaclePositions;
    public float[] obstacleScales;

    public TargetColumnGenome targetColumnGenome;

	public EnvironmentGenome(Challenge.Type challengeType) {
        this.challengeType = challengeType;
        arenaBounds = Challenge.GetChallengeArenaBounds(challengeType);
        terrainWaves = new Vector3[24];
        obstaclePositions = new Vector2[16];
        obstacleScales = new float[16];
    }

    public void TempInitializeGenome() {
        
        // Terrain:
        altitude = 0f;        
        for(int i = 0; i < terrainWaves.Length; i++) {
            terrainWaves[i] = new Vector3(0.1f, 0f, 0f);
        }
        for (int i = 0; i < obstaclePositions.Length; i++) {
            obstaclePositions[i] = new Vector2(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            if ((obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude < 0.15f) {
                obstaclePositions[i] = new Vector2(0.5f, 0.5f) + (obstaclePositions[i] - new Vector2(0.5f, 0.5f)) * 0.15f / (obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude;
            }
            obstacleScales[i] = 4f;
        }

        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);

        color = new Vector3(r, g, b);

        // Target:
        if(challengeType == Challenge.Type.Test) {
            targetColumnGenome = new TargetColumnGenome(this);

            targetColumnGenome.InitializeRandomGenome();
        }
    }
}
