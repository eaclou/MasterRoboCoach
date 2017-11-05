using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct GenomeNoiseOctaveData {  // used both as a single octave noisePass, and also the base settings for a series of octaves
    public Vector3 amplitude;
    public Vector3 frequency;
    public Vector3 offset;
    public float rotation;
    public float ridgeNoise;
}

[System.Serializable]
public class TerrainGenome {

    //public float altitude;
    public bool useAltitude;
    public Vector3 color;
    public int numOctaves = 4;
    public Vector3[] terrainWaves;  // x = freq, y = amp, z = offset

    public int numRockSmoothPasses = 6;
    public List<GlobalRockPass> terrainGlobalRockPasses;
    public List<GlobalSedimentPass> terrainGlobalSedimentPasses;

    [System.Serializable]
    public struct GlobalRockPass {
        public int numNoiseOctavesHeight;
        public GenomeNoiseOctaveData heightSampleData;
        public Vector4 heightLevelsAdjust;
        public float heightFlowAmount;

        public int numNoiseOctavesMask1;
        public GenomeNoiseOctaveData mask1SampleData;
        public Vector4 mask1LevelsAdjust;
        public float mask1FlowAmount;
        //public bool useMask1Noise;
        //public int mask1TexIndex;

        public int numNoiseOctavesMask2;
        public GenomeNoiseOctaveData mask2SampleData;
        public Vector4 mask2LevelsAdjust;
        public float mask2FlowAmount;
        //public bool useMask2Noise;       
        //public int mask2TexIndex;

        public int numNoiseOctavesFlow;
        public GenomeNoiseOctaveData flowSampleData;
    }
    [System.Serializable]
    public struct GlobalSedimentPass {
        public float maxAltitudeSedimentDrape;
        public float sedimentDrapeMagnitude;
        public float uniformSedimentHeight;
        public float talusAngle;

        public int numNoiseOctavesHeight;
        public GenomeNoiseOctaveData heightSampleData;
        public Vector4 heightLevelsAdjust;
        public float heightFlowAmount;

        public int numNoiseOctavesMask1;
        public GenomeNoiseOctaveData mask1SampleData;
        public Vector4 mask1LevelsAdjust;
        public float mask1FlowAmount;

        public int numNoiseOctavesMask2;
        public GenomeNoiseOctaveData mask2SampleData;
        public Vector4 mask2LevelsAdjust;
        public float mask2FlowAmount;

        public int numNoiseOctavesFlow;
        public GenomeNoiseOctaveData flowSampleData;
    }
    public struct GlobalSnowPass {

    }
    public struct GlobalSmoothPass {

    }

    public TerrainGenome() {
        
    }
    public TerrainGenome(TerrainGenome templateGenome) {

        if(templateGenome.terrainGlobalRockPasses != null) {
            terrainGlobalRockPasses = new List<GlobalRockPass>();

            for(int i = 0; i < templateGenome.terrainGlobalRockPasses.Count; i++) {
                GlobalRockPass globalRockPass = new GlobalRockPass();
                globalRockPass = templateGenome.terrainGlobalRockPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!
                terrainGlobalRockPasses.Add(globalRockPass);
            }

            //Debug.Log(terrainGlobalRockPasses[0].heightLevelsAdjust.y.ToString());
        }

        if (templateGenome.terrainGlobalSedimentPasses != null) {
            terrainGlobalSedimentPasses = new List<GlobalSedimentPass>();

            for (int i = 0; i < templateGenome.terrainGlobalSedimentPasses.Count; i++) {
                GlobalSedimentPass globalSedimentPass = new GlobalSedimentPass();
                globalSedimentPass = templateGenome.terrainGlobalSedimentPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!
                terrainGlobalSedimentPasses.Add(globalSedimentPass);
            }

            //Debug.Log(terrainGlobalRockPasses[0].heightLevelsAdjust.y.ToString());
        }



        // OLD:::::

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

        float baseMaxAmplitude = 60f;
        float baseMinAmplitude = 0f;
        float baseMaxFrequency = 4f;
        float baseMinFrequency = 0.01f;

        // GLOBAL ROCK PASSES:
        if (parentGenome.terrainGlobalRockPasses != null) {
            newGenome.terrainGlobalRockPasses = new List<GlobalRockPass>();

            for (int i = 0; i < parentGenome.terrainGlobalRockPasses.Count; i++) {

                float maxAmplitude = baseMaxAmplitude / Mathf.Pow(2, i);
                float minAmplitude = baseMinAmplitude / Mathf.Pow(2, i);
                float maxFrequency = baseMaxFrequency * Mathf.Pow(2, i);
                float minFrequency = baseMinFrequency * Mathf.Pow(2, i);
                // Start with copy of Parent:
                GlobalRockPass globalRockPass = new GlobalRockPass();
                globalRockPass = parentGenome.terrainGlobalRockPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!

                // MUTATION:
                // Height NOISE:
                float mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    int newNumHeightOctaves = UnityEngine.Random.Range(-1, 2) + globalRockPass.numNoiseOctavesHeight;
                    newNumHeightOctaves = Mathf.Min(Mathf.Max(newNumHeightOctaves, 1), 8);
                    globalRockPass.numNoiseOctavesHeight = newNumHeightOctaves;
                    //Debug.Log(globalRockPass.numNoiseOctavesHeight.ToString());
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newHeightFlowAmount = UnityEngine.Random.Range(-1f, 1f);
                    globalRockPass.heightFlowAmount = Mathf.Lerp(globalRockPass.heightFlowAmount, newHeightFlowAmount, mutationDriftAmount);
                }
                float maxAmpToFreqRatio = 10f;
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newHeightAmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.heightSampleData.amplitude;
                    Vector3 newHeightAmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.heightSampleData.amplitude.x;
                    Vector3 newHeightAmplitudeLerp = Vector3.Lerp(newHeightAmplitudeAdd, newHeightAmplitudeMult, 0.2f);
                    globalRockPass.heightSampleData.amplitude = Vector3.Lerp(globalRockPass.heightSampleData.amplitude, newHeightAmplitudeLerp, mutationDriftAmount);

                    float ampToFreqRatio = globalRockPass.heightSampleData.amplitude.magnitude / globalRockPass.heightSampleData.frequency.magnitude;
                    if (ampToFreqRatio > maxAmpToFreqRatio) {
                        newHeightAmplitudeLerp = globalRockPass.heightSampleData.frequency * maxAmpToFreqRatio;
                    }


                    // CAPS:
                    if (globalRockPass.heightSampleData.amplitude.magnitude > maxAmplitude) {
                        globalRockPass.heightSampleData.amplitude *= maxAmplitude / globalRockPass.heightSampleData.amplitude.magnitude;
                    }
                    if (globalRockPass.heightSampleData.amplitude.magnitude < minAmplitude) {
                        globalRockPass.heightSampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newHeightFrequencyAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.heightSampleData.frequency;
                    Vector3 newHeightFrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.heightSampleData.frequency.x;
                    Vector3 newHeightFrequencyLerp = Vector3.Lerp(newHeightFrequencyAdd, newHeightFrequencyMult, 0.2f);
                    //float ampToFreqRatio = globalRockPass.heightSampleData.amplitude.magnitude / globalRockPass.heightSampleData.frequency.magnitude;
                    //if (ampToFreqRatio > maxAmpToFreqRatio) {
                    //    newHeightFrequencyLerp = newHeightFrequencyLerp * (1.0f / globalRockPass.heightSampleData.amplitude.magnitude);
                    //}
                    globalRockPass.heightSampleData.frequency = Vector3.Lerp(globalRockPass.heightSampleData.frequency, newHeightFrequencyLerp, mutationDriftAmount);

                    if(globalRockPass.heightSampleData.frequency.magnitude > maxFrequency) {
                        globalRockPass.heightSampleData.frequency *= maxFrequency / globalRockPass.heightSampleData.frequency.magnitude;
                    }
                    if (globalRockPass.heightSampleData.frequency.magnitude < minFrequency) {
                        globalRockPass.heightSampleData.frequency /= globalRockPass.heightSampleData.frequency.magnitude / minFrequency;
                    }
                    //globalRockPass.heightSampleData.frequency = new Vector3((1.0f / globalRockPass.heightSampleData.amplitude.x), (1.0f / globalRockPass.heightSampleData.amplitude.y), (1.0f / globalRockPass.heightSampleData.amplitude.z));
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newHeightOffset = UnityEngine.Random.insideUnitSphere + globalRockPass.heightSampleData.offset;
                    globalRockPass.heightSampleData.offset = Vector3.Lerp(globalRockPass.heightSampleData.offset, newHeightOffset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newHeightRotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalRockPass.heightSampleData.rotation = Mathf.Lerp(globalRockPass.heightSampleData.rotation, newHeightRotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newHeightRidge = UnityEngine.Random.Range(0f, 1f);
                    globalRockPass.heightSampleData.ridgeNoise = Mathf.Lerp(globalRockPass.heightSampleData.ridgeNoise, newHeightRidge, mutationDriftAmount);
                }

                // MASK1 !!!!!
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    int newNumMask1Octaves = UnityEngine.Random.Range(-1, 2) + globalRockPass.numNoiseOctavesMask1;
                    newNumMask1Octaves = Mathf.Min(Mathf.Max(newNumMask1Octaves, 1), 8);
                    globalRockPass.numNoiseOctavesMask1 = newNumMask1Octaves;
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newMask1FlowAmount = UnityEngine.Random.Range(-1f, 1f);
                    globalRockPass.mask1FlowAmount = Mathf.Lerp(globalRockPass.mask1FlowAmount, newMask1FlowAmount, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask1AmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.mask1SampleData.amplitude;
                    Vector3 newMask1AmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.mask1SampleData.amplitude.x;
                    Vector3 newMask1AmplitudeLerp = Vector3.Lerp(newMask1AmplitudeAdd, newMask1AmplitudeMult, 0.2f);
                    globalRockPass.mask1SampleData.amplitude = Vector3.Lerp(globalRockPass.mask1SampleData.amplitude, newMask1AmplitudeLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalRockPass.mask1SampleData.amplitude.magnitude > maxAmplitude) {
                        globalRockPass.mask1SampleData.amplitude *= maxAmplitude / globalRockPass.mask1SampleData.amplitude.magnitude;
                    }
                    if (globalRockPass.mask1SampleData.amplitude.magnitude < minAmplitude) {
                        globalRockPass.mask1SampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask1FrequencyAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.mask1SampleData.frequency;
                    Vector3 newMask1FrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.mask1SampleData.frequency.x;
                    Vector3 newMask1FrequencyLerp = Vector3.Lerp(newMask1FrequencyAdd, newMask1FrequencyMult, 0.2f);
                    globalRockPass.mask1SampleData.frequency = Vector3.Lerp(globalRockPass.mask1SampleData.frequency, newMask1FrequencyLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalRockPass.mask1SampleData.frequency.magnitude > maxFrequency) {
                        globalRockPass.mask1SampleData.frequency *= maxFrequency / globalRockPass.mask1SampleData.frequency.magnitude;
                    }
                    if (globalRockPass.mask1SampleData.frequency.magnitude < minFrequency) {
                        globalRockPass.mask1SampleData.frequency /= globalRockPass.mask1SampleData.frequency.magnitude / minFrequency;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask1Offset = UnityEngine.Random.insideUnitSphere + globalRockPass.mask1SampleData.offset;
                    globalRockPass.mask1SampleData.offset = Vector3.Lerp(globalRockPass.mask1SampleData.offset, newMask1Offset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask1Rotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalRockPass.mask1SampleData.rotation = Mathf.Lerp(globalRockPass.mask1SampleData.rotation, newMask1Rotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask1Ridge = UnityEngine.Random.Range(0f, 1f);
                    globalRockPass.mask1SampleData.ridgeNoise = Mathf.Lerp(globalRockPass.mask1SampleData.ridgeNoise, newMask1Ridge, mutationDriftAmount);
                }

                // MASK2 !!!!!
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    int newNumMask2Octaves = UnityEngine.Random.Range(-1, 2) + globalRockPass.numNoiseOctavesMask2;
                    newNumMask2Octaves = Mathf.Min(Mathf.Max(newNumMask2Octaves, 1), 8);
                    globalRockPass.numNoiseOctavesMask2 = newNumMask2Octaves;
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newMask2FlowAmount = UnityEngine.Random.Range(-1f, 1f);
                    globalRockPass.mask2FlowAmount = Mathf.Lerp(globalRockPass.mask2FlowAmount, newMask2FlowAmount, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask2AmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.mask2SampleData.amplitude;
                    Vector3 newMask2AmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.mask2SampleData.amplitude.x;
                    Vector3 newMask2AmplitudeLerp = Vector3.Lerp(newMask2AmplitudeAdd, newMask2AmplitudeMult, 0.2f);
                    globalRockPass.mask2SampleData.amplitude = Vector3.Lerp(globalRockPass.mask2SampleData.amplitude, newMask2AmplitudeLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalRockPass.mask2SampleData.amplitude.magnitude > maxAmplitude) {
                        globalRockPass.mask2SampleData.amplitude *= maxAmplitude / globalRockPass.mask2SampleData.amplitude.magnitude;
                    }
                    if (globalRockPass.mask2SampleData.amplitude.magnitude < minAmplitude) {
                        globalRockPass.mask2SampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask2FrequencyAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.mask2SampleData.frequency;
                    Vector3 newMask2FrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.mask2SampleData.frequency.x;
                    Vector3 newMask2FrequencyLerp = Vector3.Lerp(newMask2FrequencyAdd, newMask2FrequencyMult, 0.2f);
                    globalRockPass.mask2SampleData.frequency = Vector3.Lerp(globalRockPass.mask2SampleData.frequency, newMask2FrequencyLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalRockPass.mask2SampleData.frequency.magnitude > maxFrequency) {
                        globalRockPass.mask2SampleData.frequency *= maxFrequency / globalRockPass.mask2SampleData.frequency.magnitude;
                    }
                    if (globalRockPass.mask2SampleData.frequency.magnitude < minFrequency) {
                        globalRockPass.mask2SampleData.frequency /= globalRockPass.mask2SampleData.frequency.magnitude / minFrequency;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask2Offset = UnityEngine.Random.insideUnitSphere + globalRockPass.mask2SampleData.offset;
                    globalRockPass.mask2SampleData.offset = Vector3.Lerp(globalRockPass.mask2SampleData.offset, newMask2Offset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask2Rotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalRockPass.mask2SampleData.rotation = Mathf.Lerp(globalRockPass.mask2SampleData.rotation, newMask2Rotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask2Ridge = UnityEngine.Random.Range(0f, 1f);
                    globalRockPass.mask2SampleData.ridgeNoise = Mathf.Lerp(globalRockPass.mask2SampleData.ridgeNoise, newMask2Ridge, mutationDriftAmount);
                }

                // FLOW !!!!!
                mutationCheck = UnityEngine.Random.Range(0f, 2f);
                if (mutationCheck < mutationRate) {
                    int newNumFlowOctaves = UnityEngine.Random.Range(-1, 1) + globalRockPass.numNoiseOctavesFlow;
                    newNumFlowOctaves = Mathf.Min(Mathf.Max(newNumFlowOctaves, 1), 8);
                    globalRockPass.numNoiseOctavesFlow = newNumFlowOctaves;
                }                
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newFlowAmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.flowSampleData.amplitude;
                    Vector3 newFlowAmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.flowSampleData.amplitude.x;
                    Vector3 newFlowAmplitudeLerp = Vector3.Lerp(newFlowAmplitudeAdd, newFlowAmplitudeMult, 0.2f);
                    globalRockPass.flowSampleData.amplitude = Vector3.Lerp(globalRockPass.flowSampleData.amplitude, newFlowAmplitudeLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalRockPass.flowSampleData.amplitude.magnitude > maxAmplitude) {
                        globalRockPass.flowSampleData.amplitude *= maxAmplitude / globalRockPass.flowSampleData.amplitude.magnitude;
                    }
                    if (globalRockPass.flowSampleData.amplitude.magnitude < minAmplitude) {
                        globalRockPass.flowSampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newFlowFrequencyAdd = UnityEngine.Random.insideUnitSphere + globalRockPass.flowSampleData.frequency;
                    Vector3 newFlowFrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalRockPass.flowSampleData.frequency.x;
                    Vector3 newFlowFrequencyLerp = Vector3.Lerp(newFlowFrequencyAdd, newFlowFrequencyMult, 0.2f);
                    globalRockPass.flowSampleData.frequency = Vector3.Lerp(globalRockPass.flowSampleData.frequency, newFlowFrequencyLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalRockPass.flowSampleData.frequency.magnitude > maxFrequency) {
                        globalRockPass.flowSampleData.frequency *= maxFrequency / globalRockPass.flowSampleData.frequency.magnitude;
                    }
                    if (globalRockPass.flowSampleData.frequency.magnitude < minFrequency) {
                        globalRockPass.flowSampleData.frequency /= globalRockPass.flowSampleData.frequency.magnitude / minFrequency;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newFlowOffset = UnityEngine.Random.insideUnitSphere + globalRockPass.flowSampleData.offset;
                    globalRockPass.flowSampleData.offset = Vector3.Lerp(globalRockPass.flowSampleData.offset, newFlowOffset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newFlowRotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalRockPass.flowSampleData.rotation = Mathf.Lerp(globalRockPass.flowSampleData.rotation, newFlowRotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newFlowRidge = UnityEngine.Random.Range(0f, 1f);
                    globalRockPass.flowSampleData.ridgeNoise = Mathf.Lerp(globalRockPass.flowSampleData.ridgeNoise, newFlowRidge, mutationDriftAmount);
                }

                
                newGenome.terrainGlobalRockPasses.Add(globalRockPass);
                //Debug.Log("inside loop: " + newGenome.terrainGlobalRockPasses[0].numNoiseOctavesHeight.ToString());
            }

            //Debug.Log(newGenome.terrainGlobalRockPasses[0].numNoiseOctavesHeight.ToString());
        }



        // SEDIMENT!!! &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        #region Sediment

        // GLOBAL SEDIMENT PASSES:
        if (parentGenome.terrainGlobalSedimentPasses != null) {
            newGenome.terrainGlobalSedimentPasses = new List<GlobalSedimentPass>();
            

            for (int i = 0; i < parentGenome.terrainGlobalSedimentPasses.Count; i++) {

                

                float maxAmplitude = baseMaxAmplitude / Mathf.Pow(2, i);
                float minAmplitude = baseMinAmplitude / Mathf.Pow(2, i);
                float maxFrequency = baseMaxFrequency * Mathf.Pow(2, i);
                float minFrequency = baseMinFrequency * Mathf.Pow(2, i);
                // Start with copy of Parent:
                GlobalSedimentPass globalSedimentPass = new GlobalSedimentPass();
                globalSedimentPass = parentGenome.terrainGlobalSedimentPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!

                globalSedimentPass.maxAltitudeSedimentDrape = parentGenome.terrainGlobalSedimentPasses[i].maxAltitudeSedimentDrape;
                globalSedimentPass.sedimentDrapeMagnitude = parentGenome.terrainGlobalSedimentPasses[i].sedimentDrapeMagnitude;
                globalSedimentPass.talusAngle = parentGenome.terrainGlobalSedimentPasses[i].talusAngle;
                globalSedimentPass.uniformSedimentHeight = parentGenome.terrainGlobalSedimentPasses[i].uniformSedimentHeight;

                float mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newSedimentMaxAltitude = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.maxAltitudeSedimentDrape = Mathf.Lerp(globalSedimentPass.maxAltitudeSedimentDrape, newSedimentMaxAltitude, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newSedimentDrapeMagnitude = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.sedimentDrapeMagnitude = Mathf.Lerp(globalSedimentPass.sedimentDrapeMagnitude, newSedimentDrapeMagnitude, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newSedimentTalusAngle = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.talusAngle = Mathf.Lerp(globalSedimentPass.talusAngle, newSedimentTalusAngle, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newSedimentUniformHeight = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.uniformSedimentHeight = Mathf.Lerp(globalSedimentPass.uniformSedimentHeight, newSedimentUniformHeight, mutationDriftAmount);
                }

                // MUTATION:
                // Height NOISE:
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    int newNumHeightOctaves = UnityEngine.Random.Range(-1, 2) + globalSedimentPass.numNoiseOctavesHeight;
                    newNumHeightOctaves = Mathf.Min(Mathf.Max(newNumHeightOctaves, 1), 8);
                    globalSedimentPass.numNoiseOctavesHeight = newNumHeightOctaves;
                    //Debug.Log(globalSedimentPass.numNoiseOctavesHeight.ToString());
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newHeightFlowAmount = UnityEngine.Random.Range(-1f, 1f);
                    globalSedimentPass.heightFlowAmount = Mathf.Lerp(globalSedimentPass.heightFlowAmount, newHeightFlowAmount, mutationDriftAmount);
                }
                float maxAmpToFreqRatio = 10f;
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newHeightAmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.heightSampleData.amplitude;
                    Vector3 newHeightAmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.heightSampleData.amplitude.x;
                    Vector3 newHeightAmplitudeLerp = Vector3.Lerp(newHeightAmplitudeAdd, newHeightAmplitudeMult, 0.2f);
                    globalSedimentPass.heightSampleData.amplitude = Vector3.Lerp(globalSedimentPass.heightSampleData.amplitude, newHeightAmplitudeLerp, mutationDriftAmount);

                    float ampToFreqRatio = globalSedimentPass.heightSampleData.amplitude.magnitude / globalSedimentPass.heightSampleData.frequency.magnitude;
                    if (ampToFreqRatio > maxAmpToFreqRatio) {
                        newHeightAmplitudeLerp = globalSedimentPass.heightSampleData.frequency * maxAmpToFreqRatio;
                    }


                    // CAPS:
                    if (globalSedimentPass.heightSampleData.amplitude.magnitude > maxAmplitude) {
                        globalSedimentPass.heightSampleData.amplitude *= maxAmplitude / globalSedimentPass.heightSampleData.amplitude.magnitude;
                    }
                    if (globalSedimentPass.heightSampleData.amplitude.magnitude < minAmplitude) {
                        globalSedimentPass.heightSampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newHeightFrequencyAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.heightSampleData.frequency;
                    Vector3 newHeightFrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.heightSampleData.frequency.x;
                    Vector3 newHeightFrequencyLerp = Vector3.Lerp(newHeightFrequencyAdd, newHeightFrequencyMult, 0.2f);
                    //float ampToFreqRatio = globalSedimentPass.heightSampleData.amplitude.magnitude / globalSedimentPass.heightSampleData.frequency.magnitude;
                    //if (ampToFreqRatio > maxAmpToFreqRatio) {
                    //    newHeightFrequencyLerp = newHeightFrequencyLerp * (1.0f / globalSedimentPass.heightSampleData.amplitude.magnitude);
                    //}
                    globalSedimentPass.heightSampleData.frequency = Vector3.Lerp(globalSedimentPass.heightSampleData.frequency, newHeightFrequencyLerp, mutationDriftAmount);

                    if (globalSedimentPass.heightSampleData.frequency.magnitude > maxFrequency) {
                        globalSedimentPass.heightSampleData.frequency *= maxFrequency / globalSedimentPass.heightSampleData.frequency.magnitude;
                    }
                    if (globalSedimentPass.heightSampleData.frequency.magnitude < minFrequency) {
                        globalSedimentPass.heightSampleData.frequency /= globalSedimentPass.heightSampleData.frequency.magnitude / minFrequency;
                    }
                    //globalSedimentPass.heightSampleData.frequency = new Vector3((1.0f / globalSedimentPass.heightSampleData.amplitude.x), (1.0f / globalSedimentPass.heightSampleData.amplitude.y), (1.0f / globalSedimentPass.heightSampleData.amplitude.z));
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newHeightOffset = UnityEngine.Random.insideUnitSphere + globalSedimentPass.heightSampleData.offset;
                    globalSedimentPass.heightSampleData.offset = Vector3.Lerp(globalSedimentPass.heightSampleData.offset, newHeightOffset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newHeightRotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalSedimentPass.heightSampleData.rotation = Mathf.Lerp(globalSedimentPass.heightSampleData.rotation, newHeightRotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newHeightRidge = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.heightSampleData.ridgeNoise = Mathf.Lerp(globalSedimentPass.heightSampleData.ridgeNoise, newHeightRidge, mutationDriftAmount);
                }

                // MASK1 !!!!!
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    int newNumMask1Octaves = UnityEngine.Random.Range(-1, 2) + globalSedimentPass.numNoiseOctavesMask1;
                    newNumMask1Octaves = Mathf.Min(Mathf.Max(newNumMask1Octaves, 1), 8);
                    globalSedimentPass.numNoiseOctavesMask1 = newNumMask1Octaves;
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newMask1FlowAmount = UnityEngine.Random.Range(-1f, 1f);
                    globalSedimentPass.mask1FlowAmount = Mathf.Lerp(globalSedimentPass.mask1FlowAmount, newMask1FlowAmount, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask1AmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.mask1SampleData.amplitude;
                    Vector3 newMask1AmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.mask1SampleData.amplitude.x;
                    Vector3 newMask1AmplitudeLerp = Vector3.Lerp(newMask1AmplitudeAdd, newMask1AmplitudeMult, 0.2f);
                    globalSedimentPass.mask1SampleData.amplitude = Vector3.Lerp(globalSedimentPass.mask1SampleData.amplitude, newMask1AmplitudeLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalSedimentPass.mask1SampleData.amplitude.magnitude > maxAmplitude) {
                        globalSedimentPass.mask1SampleData.amplitude *= maxAmplitude / globalSedimentPass.mask1SampleData.amplitude.magnitude;
                    }
                    if (globalSedimentPass.mask1SampleData.amplitude.magnitude < minAmplitude) {
                        globalSedimentPass.mask1SampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask1FrequencyAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.mask1SampleData.frequency;
                    Vector3 newMask1FrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.mask1SampleData.frequency.x;
                    Vector3 newMask1FrequencyLerp = Vector3.Lerp(newMask1FrequencyAdd, newMask1FrequencyMult, 0.2f);
                    globalSedimentPass.mask1SampleData.frequency = Vector3.Lerp(globalSedimentPass.mask1SampleData.frequency, newMask1FrequencyLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalSedimentPass.mask1SampleData.frequency.magnitude > maxFrequency) {
                        globalSedimentPass.mask1SampleData.frequency *= maxFrequency / globalSedimentPass.mask1SampleData.frequency.magnitude;
                    }
                    if (globalSedimentPass.mask1SampleData.frequency.magnitude < minFrequency) {
                        globalSedimentPass.mask1SampleData.frequency /= globalSedimentPass.mask1SampleData.frequency.magnitude / minFrequency;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask1Offset = UnityEngine.Random.insideUnitSphere + globalSedimentPass.mask1SampleData.offset;
                    globalSedimentPass.mask1SampleData.offset = Vector3.Lerp(globalSedimentPass.mask1SampleData.offset, newMask1Offset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask1Rotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalSedimentPass.mask1SampleData.rotation = Mathf.Lerp(globalSedimentPass.mask1SampleData.rotation, newMask1Rotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask1Ridge = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.mask1SampleData.ridgeNoise = Mathf.Lerp(globalSedimentPass.mask1SampleData.ridgeNoise, newMask1Ridge, mutationDriftAmount);
                }

                // MASK2 !!!!!
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    int newNumMask2Octaves = UnityEngine.Random.Range(-1, 2) + globalSedimentPass.numNoiseOctavesMask2;
                    newNumMask2Octaves = Mathf.Min(Mathf.Max(newNumMask2Octaves, 1), 8);
                    globalSedimentPass.numNoiseOctavesMask2 = newNumMask2Octaves;
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    float newMask2FlowAmount = UnityEngine.Random.Range(-1f, 1f);
                    globalSedimentPass.mask2FlowAmount = Mathf.Lerp(globalSedimentPass.mask2FlowAmount, newMask2FlowAmount, mutationDriftAmount);
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask2AmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.mask2SampleData.amplitude;
                    Vector3 newMask2AmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.mask2SampleData.amplitude.x;
                    Vector3 newMask2AmplitudeLerp = Vector3.Lerp(newMask2AmplitudeAdd, newMask2AmplitudeMult, 0.2f);
                    globalSedimentPass.mask2SampleData.amplitude = Vector3.Lerp(globalSedimentPass.mask2SampleData.amplitude, newMask2AmplitudeLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalSedimentPass.mask2SampleData.amplitude.magnitude > maxAmplitude) {
                        globalSedimentPass.mask2SampleData.amplitude *= maxAmplitude / globalSedimentPass.mask2SampleData.amplitude.magnitude;
                    }
                    if (globalSedimentPass.mask2SampleData.amplitude.magnitude < minAmplitude) {
                        globalSedimentPass.mask2SampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask2FrequencyAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.mask2SampleData.frequency;
                    Vector3 newMask2FrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.mask2SampleData.frequency.x;
                    Vector3 newMask2FrequencyLerp = Vector3.Lerp(newMask2FrequencyAdd, newMask2FrequencyMult, 0.2f);
                    globalSedimentPass.mask2SampleData.frequency = Vector3.Lerp(globalSedimentPass.mask2SampleData.frequency, newMask2FrequencyLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalSedimentPass.mask2SampleData.frequency.magnitude > maxFrequency) {
                        globalSedimentPass.mask2SampleData.frequency *= maxFrequency / globalSedimentPass.mask2SampleData.frequency.magnitude;
                    }
                    if (globalSedimentPass.mask2SampleData.frequency.magnitude < minFrequency) {
                        globalSedimentPass.mask2SampleData.frequency /= globalSedimentPass.mask2SampleData.frequency.magnitude / minFrequency;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newMask2Offset = UnityEngine.Random.insideUnitSphere + globalSedimentPass.mask2SampleData.offset;
                    globalSedimentPass.mask2SampleData.offset = Vector3.Lerp(globalSedimentPass.mask2SampleData.offset, newMask2Offset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask2Rotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalSedimentPass.mask2SampleData.rotation = Mathf.Lerp(globalSedimentPass.mask2SampleData.rotation, newMask2Rotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newMask2Ridge = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.mask2SampleData.ridgeNoise = Mathf.Lerp(globalSedimentPass.mask2SampleData.ridgeNoise, newMask2Ridge, mutationDriftAmount);
                }

                // FLOW !!!!!
                mutationCheck = UnityEngine.Random.Range(0f, 2f);
                if (mutationCheck < mutationRate) {
                    int newNumFlowOctaves = UnityEngine.Random.Range(-1, 1) + globalSedimentPass.numNoiseOctavesFlow;
                    newNumFlowOctaves = Mathf.Min(Mathf.Max(newNumFlowOctaves, 1), 8);
                    globalSedimentPass.numNoiseOctavesFlow = newNumFlowOctaves;
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newFlowAmplitudeAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.flowSampleData.amplitude;
                    Vector3 newFlowAmplitudeMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.flowSampleData.amplitude.x;
                    Vector3 newFlowAmplitudeLerp = Vector3.Lerp(newFlowAmplitudeAdd, newFlowAmplitudeMult, 0.2f);
                    globalSedimentPass.flowSampleData.amplitude = Vector3.Lerp(globalSedimentPass.flowSampleData.amplitude, newFlowAmplitudeLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalSedimentPass.flowSampleData.amplitude.magnitude > maxAmplitude) {
                        globalSedimentPass.flowSampleData.amplitude *= maxAmplitude / globalSedimentPass.flowSampleData.amplitude.magnitude;
                    }
                    if (globalSedimentPass.flowSampleData.amplitude.magnitude < minAmplitude) {
                        globalSedimentPass.flowSampleData.amplitude *= 0f;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newFlowFrequencyAdd = UnityEngine.Random.insideUnitSphere + globalSedimentPass.flowSampleData.frequency;
                    Vector3 newFlowFrequencyMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * globalSedimentPass.flowSampleData.frequency.x;
                    Vector3 newFlowFrequencyLerp = Vector3.Lerp(newFlowFrequencyAdd, newFlowFrequencyMult, 0.2f);
                    globalSedimentPass.flowSampleData.frequency = Vector3.Lerp(globalSedimentPass.flowSampleData.frequency, newFlowFrequencyLerp, mutationDriftAmount);

                    // CAPS:
                    if (globalSedimentPass.flowSampleData.frequency.magnitude > maxFrequency) {
                        globalSedimentPass.flowSampleData.frequency *= maxFrequency / globalSedimentPass.flowSampleData.frequency.magnitude;
                    }
                    if (globalSedimentPass.flowSampleData.frequency.magnitude < minFrequency) {
                        globalSedimentPass.flowSampleData.frequency /= globalSedimentPass.flowSampleData.frequency.magnitude / minFrequency;
                    }
                }
                mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector3 newFlowOffset = UnityEngine.Random.insideUnitSphere + globalSedimentPass.flowSampleData.offset;
                    globalSedimentPass.flowSampleData.offset = Vector3.Lerp(globalSedimentPass.flowSampleData.offset, newFlowOffset, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newFlowRotation = UnityEngine.Random.Range(-3.14f, 3.14f);
                    globalSedimentPass.flowSampleData.rotation = Mathf.Lerp(globalSedimentPass.flowSampleData.rotation, newFlowRotation, mutationDriftAmount);
                }
                if (mutationCheck < mutationRate) {
                    float newFlowRidge = UnityEngine.Random.Range(0f, 1f);
                    globalSedimentPass.flowSampleData.ridgeNoise = Mathf.Lerp(globalSedimentPass.flowSampleData.ridgeNoise, newFlowRidge, mutationDriftAmount);
                }


                newGenome.terrainGlobalSedimentPasses.Add(globalSedimentPass);
                //Debug.Log("inside loop: " + newGenome.terrainGlobalSedimentPasses[0].numNoiseOctavesHeight.ToString());
            }

            //Debug.Log(newGenome.terrainGlobalSedimentPasses[0].numNoiseOctavesHeight.ToString());
        }


        #endregion




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
