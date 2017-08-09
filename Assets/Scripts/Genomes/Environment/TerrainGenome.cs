using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainGenome {

    //public float altitude;
    public Vector3 color;
    public Vector3[] terrainWaves;  // x = freq, y = amp, z = offset

    public TerrainGenome() {
        
    }
    public TerrainGenome(TerrainGenome templateGenome) {
        //altitude = templateGenome.altitude;
        color = new Vector3(templateGenome.color.x, templateGenome.color.y, templateGenome.color.z);
        terrainWaves = new Vector3[templateGenome.terrainWaves.Length];
        for(int i = 0; i < terrainWaves.Length; i++) {
            terrainWaves[i] = new Vector3(templateGenome.terrainWaves[i].x, templateGenome.terrainWaves[i].y, templateGenome.terrainWaves[i].z);
        }
    }

    public void InitializeRandomGenome() {
        float r = UnityEngine.Random.Range(0f, 1f);
        float g = UnityEngine.Random.Range(0f, 1f);
        float b = UnityEngine.Random.Range(0f, 1f);
        color = new Vector3(r, g, b);

        for (int i = 0; i < terrainWaves.Length; i++) {
            terrainWaves[i] = new Vector3(0.1f, 0f, 0f);
        }
    }

    public static TerrainGenome BirthNewGenome(TerrainGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        TerrainGenome newGenome = new TerrainGenome();
        newGenome.color = parentGenome.color;
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float r = UnityEngine.Random.Range(0f, 1f);
            newGenome.color = new Vector3(Mathf.Lerp(newGenome.color.x, r, mutationDriftAmount), newGenome.color.y, newGenome.color.z);
        }
        rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float g = UnityEngine.Random.Range(0f, 1f);
            newGenome.color = new Vector3(newGenome.color.x, Mathf.Lerp(newGenome.color.y, g, mutationDriftAmount), newGenome.color.z);
        }
        rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float b = UnityEngine.Random.Range(0f, 1f);
            newGenome.color = new Vector3(newGenome.color.x, newGenome.color.y, Mathf.Lerp(newGenome.color.z, b, mutationDriftAmount));
        }
        // TERRAIN:
        newGenome.terrainWaves = new Vector3[parentGenome.terrainWaves.Length];
        for (int i = 0; i < parentGenome.terrainWaves.Length; i++) {
            newGenome.terrainWaves[i] = new Vector3(parentGenome.terrainWaves[i].x, parentGenome.terrainWaves[i].y, parentGenome.terrainWaves[i].z);
            rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < mutationRate) {
                float newFreq = UnityEngine.Random.Range(0.01f, 2f);
                newGenome.terrainWaves[i].x = Mathf.Lerp(newGenome.terrainWaves[i].x, newFreq, mutationDriftAmount);
            }
            rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < mutationRate) {
                float newAmp = UnityEngine.Random.Range(0f, 4f);
                newGenome.terrainWaves[i].y = Mathf.Lerp(newGenome.terrainWaves[i].y, newAmp, mutationDriftAmount);
            }
            rand = UnityEngine.Random.Range(0f, 1f);
            if (rand < mutationRate) {
                float newOff = UnityEngine.Random.Range(-10f, 10f);
                newGenome.terrainWaves[i].z = Mathf.Lerp(newGenome.terrainWaves[i].z, newOff, mutationDriftAmount);
            }
        }
        return newGenome;
    }
}
