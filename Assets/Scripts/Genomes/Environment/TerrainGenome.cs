using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TerrainGenome {

    //public float altitude;
    public bool useAltitude;
    public Vector3 color;
    public int numOctaves = 4;
    public Vector3[] terrainWaves;  // x = freq, y = amp, z = offset

    public TerrainGenome() {
        
    }
    public TerrainGenome(TerrainGenome templateGenome) {
        //altitude = templateGenome.altitude;
        useAltitude = templateGenome.useAltitude;
        numOctaves = templateGenome.numOctaves;
        color = new Vector3(templateGenome.color.x, templateGenome.color.y, templateGenome.color.z);
        terrainWaves = new Vector3[templateGenome.numOctaves];
        if(useAltitude) {
            for (int i = 0; i < terrainWaves.Length; i++) {
                if(i < templateGenome.terrainWaves.Length) {  // when changing numOctaves, doesn't immediately change parentgenome terrainWaves array
                    terrainWaves[i] = new Vector3(templateGenome.terrainWaves[i].x, templateGenome.terrainWaves[i].y, templateGenome.terrainWaves[i].z);
                    //Debug.Log("Copy Terrain Genome: " + terrainWaves[i].ToString());
                }
                else {
                    terrainWaves[i] = new Vector3(0.001f, 0f, 0f); // new entry
                }
                
            }
        }        
    }

    public void InitializeRandomGenome() {
        float r = UnityEngine.Random.Range(0.4f, 1f);
        //float g = UnityEngine.Random.Range(0f, 1f);
        //float b = UnityEngine.Random.Range(0f, 1f);
        color = new Vector3(r, r, r);

        if (useAltitude) {
            for (int i = 0; i < terrainWaves.Length; i++) {
                terrainWaves[i] = new Vector3(0.001f, 0f, 0f);
            }
        }        
    }

    public static TerrainGenome BirthNewGenome(TerrainGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        TerrainGenome newGenome = new TerrainGenome();
        // NEED TO COPY ALL ATTRIBUTES HERE unless I switch mutation process to go: full-copy, then re-traverse and mutate on a second sweep...
        newGenome.useAltitude = parentGenome.useAltitude;
        newGenome.numOctaves = parentGenome.numOctaves;
        newGenome.color = parentGenome.color;

        float rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float r = UnityEngine.Random.Range(0.4f, 1f);
            newGenome.color = new Vector3(Mathf.Lerp(newGenome.color.x, r, mutationDriftAmount), Mathf.Lerp(newGenome.color.y, r, mutationDriftAmount), Mathf.Lerp(newGenome.color.z, r, mutationDriftAmount));
        }
        /*rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float g = UnityEngine.Random.Range(0f, 1f);
            newGenome.color = new Vector3(newGenome.color.x, Mathf.Lerp(newGenome.color.y, g, mutationDriftAmount), newGenome.color.z);
        }
        rand = UnityEngine.Random.Range(0f, 1f);
        if (rand < mutationRate) {
            float b = UnityEngine.Random.Range(0f, 1f);
            newGenome.color = new Vector3(newGenome.color.x, newGenome.color.y, Mathf.Lerp(newGenome.color.z, b, mutationDriftAmount));
        }*/
        // TERRAIN:        
        newGenome.terrainWaves = new Vector3[newGenome.numOctaves];
        if(parentGenome.useAltitude) {
            //Debug.Log("Mutate EnvTerrain Altitude ");
            for (int i = 0; i < newGenome.terrainWaves.Length; i++) {
                newGenome.terrainWaves[i] = new Vector3(parentGenome.terrainWaves[i].x, parentGenome.terrainWaves[i].y, parentGenome.terrainWaves[i].z);
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < mutationRate) {
                    float newFreq = UnityEngine.Random.Range(0.001f, 0.005f);
                    newGenome.terrainWaves[i].x = Mathf.Lerp(newGenome.terrainWaves[i].x, newFreq, mutationDriftAmount);
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < mutationRate) {
                    float newAmp = UnityEngine.Random.Range(0f, 1f);
                    newGenome.terrainWaves[i].y = Mathf.Lerp(newGenome.terrainWaves[i].y, newAmp, mutationDriftAmount);
                    //Debug.Log("Mutate EnvTerrain Altitude " + newGenome.terrainWaves[i].y.ToString());
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < mutationRate) {
                    float newOff = UnityEngine.Random.Range(-100f, 100f);
                    newGenome.terrainWaves[i].z = Mathf.Lerp(newGenome.terrainWaves[i].z, newOff, mutationDriftAmount);
                }
            }
        }
        
        return newGenome;
    }
}
