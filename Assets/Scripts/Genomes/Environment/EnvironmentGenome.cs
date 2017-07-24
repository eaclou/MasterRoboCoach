using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentGenome {

    public int index = -1;

    public Challenge.Type challengeType;
    public Vector3 arenaBounds;

    // I don't really like this!!! Try to revisit in the future!
    [System.NonSerialized]
    public Environment environmentPrefab; // reference to disabled starting conditions of Environment that is then cloned for actual use
    // This prefab would be the invisible, purely-collision, functional skeleton of the environment, for use in hidden EvaluationInstances
    // If it is to be used in the exhibition instance, would need to call a function on Environment script to create all the renderable stuff

    // Environment should be split between static/dynamic and visible/collision
    // !$#$!#$!#
    // Think about how to split terrain/objects into chunks of a grid so they can be loaded only when necessary
    // %$#%@$#%@$#%@34

    // For now, due to Serialization concerns, all environmentGenomes will have fields for all possible environmental Modules
    // If they are unused they will simply be null. The challengeType / preset Templates will determine which ones are necessary initially
    public float altitude;
    public Vector3 color;
    public Vector3[] terrainWaves;  // x = freq, y = amp, z = offset
    public Vector2[] obstaclePositions;
    public float[] obstacleScales;

    // MODULES:
    public TargetColumnGenome targetColumnGenome;
    public List<Transform> agentStartPositionsList; 

	public EnvironmentGenome(int index, Challenge.Type challengeType) {
        this.index = index;
        this.challengeType = challengeType;
        arenaBounds = Challenge.GetChallengeArenaBounds(challengeType); // Look into support for expandable bounds
        terrainWaves = new Vector3[6];
        obstaclePositions = new Vector2[6];
        obstacleScales = new float[6];
    }

    public void TempInitializeGenome() {
        // Creates a basic but functioning Genome
        // This might be replaceable in the future with the use of prefab Templates, similar to Agent Bodies
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
            obstacleScales[i] = UnityEngine.Random.Range(1f, 1f);
        }

        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);

        color = new Vector3(r, g, b);

        // Target:
        if(challengeType == Challenge.Type.Test) {
            targetColumnGenome = new TargetColumnGenome();

            targetColumnGenome.InitializeRandomGenome();
        }
    }

    public void ClearEnvironmentPrefab() {
        environmentPrefab = null;
    }
}
