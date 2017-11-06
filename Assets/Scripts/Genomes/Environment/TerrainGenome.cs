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
    public Vector3 primaryHueRock;
    public Vector3 secondaryHueRock;
    public List<GlobalSedimentPass> terrainGlobalSedimentPasses;
    public Vector3 primaryHueSediment;
    public Vector3 secondaryHueSediment;
    public List<GlobalSnowPass> terrainGlobalSnowPasses;
    public Vector3 primaryHueSnow;
    public Vector3 secondaryHueSnow;

    [System.Serializable]
    public struct GlobalRockPass {
        public int heightOperation;  // 0=add, 1=sub, 2=mult, 3=avg

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
    [System.Serializable]
    public struct GlobalSnowPass {
        public float snowLineStart;   // 0-1 for whole heightRange of current map
        public float snowLineEnd;
        public float snowAmount;
        public Vector2 snowDirection;

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
    public struct GlobalSmoothPass {

    }

    public static float epsilon = 0.0000001f;

    public TerrainGenome() {
        
    }
    public TerrainGenome(TerrainGenome templateGenome) {

        primaryHueRock = templateGenome.primaryHueRock;
        secondaryHueRock = templateGenome.secondaryHueRock;    
        primaryHueSediment = templateGenome.primaryHueSediment;
        secondaryHueSediment = templateGenome.secondaryHueSediment;    
        primaryHueSnow = templateGenome.primaryHueSnow;
        secondaryHueSnow = templateGenome.secondaryHueSnow;

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

        if (templateGenome.terrainGlobalSnowPasses != null) {
            terrainGlobalSnowPasses = new List<GlobalSnowPass>();

            for (int i = 0; i < templateGenome.terrainGlobalSnowPasses.Count; i++) {
                GlobalSnowPass globalSnowPass = new GlobalSnowPass();
                globalSnowPass = templateGenome.terrainGlobalSnowPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!
                terrainGlobalSnowPasses.Add(globalSnowPass);
            }
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

    private static void MutateAmplitude(ref GenomeNoiseOctaveData passData, float maxAmpToFreqRatio, float minAmplitude, float maxAmplitude, float mutationRate, float mutationDriftAmount) {
        
        float mutationCheck = UnityEngine.Random.Range(0f, 1f);
        if (mutationCheck < mutationRate) {
            Vector3 newHeightAmplitudeAdd = UnityEngine.Random.insideUnitSphere + passData.amplitude;
            Vector3 newHeightAmplitudeMult = new Vector3(UnityEngine.Random.Range(0.5f, 2f), UnityEngine.Random.Range(0.5f, 2f), UnityEngine.Random.Range(0.5f, 2f)) * passData.amplitude.x;
            Vector3 newHeightAmplitudeLerp = Vector3.Lerp(newHeightAmplitudeAdd, newHeightAmplitudeMult, 0.2f);
            passData.amplitude = Vector3.Lerp(passData.amplitude, newHeightAmplitudeLerp, mutationDriftAmount);

            //float ampToFreqRatio = passData.amplitude.magnitude / passData.frequency.magnitude;
            //if (ampToFreqRatio > maxAmpToFreqRatio) {
            //    newHeightAmplitudeLerp = passData.frequency * maxAmpToFreqRatio;
            //}


            // CAPS:
            if (passData.amplitude.magnitude > maxAmplitude) {
                passData.amplitude *= maxAmplitude / passData.amplitude.magnitude;
            }
            if (passData.amplitude.magnitude < minAmplitude) {
                passData.amplitude *= 0f;
            }
        }
    }

    private static void MutateFrequency(ref GenomeNoiseOctaveData passData, float minFrequency, float maxFrequency, float mutationRate, float mutationDriftAmount) {
        float mutationCheck = UnityEngine.Random.Range(0f, 1f);
        if (mutationCheck < mutationRate) {
            Vector3 newHeightFrequencyAdd = UnityEngine.Random.insideUnitSphere + passData.frequency;
            Vector3 newHeightFrequencyMult = new Vector3(UnityEngine.Random.Range(0.5f, 2f), UnityEngine.Random.Range(0.5f, 2f), UnityEngine.Random.Range(0.5f, 2f)) * passData.frequency.x;
            Vector3 newHeightFrequencyLerp = Vector3.Lerp(newHeightFrequencyAdd, newHeightFrequencyMult, 0.2f);
            
            passData.frequency = Vector3.Lerp(passData.frequency, newHeightFrequencyLerp, mutationDriftAmount);

            if (passData.frequency.magnitude > maxFrequency) {
                passData.frequency *= maxFrequency / passData.frequency.magnitude;
            }
            if (passData.frequency.magnitude < minFrequency) {
                passData.frequency /= passData.frequency.magnitude / minFrequency;
            }
        }
    }

    private static void MutateFloatBasic(ref float data, float minValue, float maxValue, float mutationRate, float mutationDriftAmount) {
        float mutationCheck = UnityEngine.Random.Range(0f, 1f);
        if (mutationCheck < mutationRate) {
            float newData = UnityEngine.Random.Range(minValue, maxValue);
            data = Mathf.Lerp(data, newData, mutationDriftAmount);

            if(data < minValue) {
                data = minValue + epsilon;
            }
            if (data > maxValue) {
                data = maxValue - epsilon;
            }
        }
    }

    private static void MutateIntBasic(ref int data, int minValue, int maxValue, float mutationRate, float mutationDriftAmount) {
        float mutationCheck = UnityEngine.Random.Range(0f, 1f);
        if (mutationCheck < mutationRate) {
            int newData = UnityEngine.Random.Range(-1, 1) + data;
            newData = Mathf.Min(Mathf.Max(newData, minValue), maxValue);
            data = newData;
        }
    }
    private static void MutateVector3Basic(ref Vector3 data, float minMagnitude, float maxMagnitude, float addStrength, float addMultLerp, float mutationRate, float mutationDriftAmount) {
        float mutationCheck = UnityEngine.Random.Range(0f, 1f);
        if (mutationCheck < mutationRate) {
            //Vector3 newHeightOffset = UnityEngine.Random.insideUnitSphere * addStrength + data;
            //data = Vector3.Lerp(data, newHeightOffset, mutationDriftAmount);


            Vector3 newDataAdd = UnityEngine.Random.insideUnitSphere * addStrength + data;
            Vector3 newDataMult = new Vector3(UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f), UnityEngine.Random.Range(0.25f, 2f)) * data.magnitude;
            Vector3 newDataLerp = Vector3.Lerp(newDataAdd, newDataMult, addMultLerp);

            data = Vector3.Lerp(data, newDataLerp, mutationDriftAmount);

            if (data.magnitude > maxMagnitude) {
                data *= maxMagnitude / data.magnitude;
            }
            if (data.magnitude < minMagnitude) {
                data /= data.magnitude / minMagnitude;
            }
        }
    }
    private static void MutateVector3Color(ref Vector3 data, float mutationRate, float mutationDriftAmount) {
        float mutationCheck = UnityEngine.Random.Range(0f, 1f);
        if (mutationCheck < mutationRate) {            
            Vector3 newData = UnityEngine.Random.insideUnitSphere;
            data = Vector3.Lerp(data, newData, mutationDriftAmount);
        }
    }

    public static TerrainGenome BirthNewGenome(TerrainGenome parentGenome, float mutationRate, float mutationDriftAmount) {
        TerrainGenome newGenome = new TerrainGenome();

        newGenome.primaryHueRock = parentGenome.primaryHueRock;
        MutateVector3Color(ref newGenome.primaryHueRock, mutationRate, mutationDriftAmount);
        newGenome.secondaryHueRock = parentGenome.secondaryHueRock;
        MutateVector3Color(ref newGenome.secondaryHueRock, mutationRate, mutationDriftAmount);
        newGenome.primaryHueSediment = parentGenome.primaryHueSediment;
        MutateVector3Color(ref newGenome.primaryHueSediment, mutationRate, mutationDriftAmount);
        newGenome.secondaryHueSediment = parentGenome.secondaryHueSediment;
        MutateVector3Color(ref newGenome.secondaryHueSediment, mutationRate, mutationDriftAmount);
        newGenome.primaryHueSnow = parentGenome.primaryHueSnow;
        MutateVector3Color(ref newGenome.primaryHueSnow, mutationRate, mutationDriftAmount);
        newGenome.secondaryHueSnow = parentGenome.secondaryHueSnow;
        MutateVector3Color(ref newGenome.secondaryHueSnow, mutationRate, mutationDriftAmount);

        float baseMaxAmplitude = 100f;
        float baseMinAmplitude = 0f;
        float baseMaxFrequency = 8f;
        float baseMinFrequency = 0.01f;

        // ROCK!!! &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        #region GLOBAL ROCK PASSES:
        // GLOBAL ROCK PASSES:
        if (parentGenome.terrainGlobalRockPasses != null) {
            newGenome.terrainGlobalRockPasses = new List<GlobalRockPass>();

            for (int i = 0; i < parentGenome.terrainGlobalRockPasses.Count; i++) {

                float maxAmplitude = baseMaxAmplitude / Mathf.Pow(2, i);
                float minAmplitude = baseMinAmplitude / Mathf.Pow(2, i);
                float maxFrequency = baseMaxFrequency * Mathf.Pow(2, i);
                float minFrequency = baseMinFrequency * Mathf.Pow(2, i);
                float maxAmpToFreqRatio = 1f;

                // Start with copy of Parent:
                GlobalRockPass globalRockPass = new GlobalRockPass();
                globalRockPass = parentGenome.terrainGlobalRockPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!

                MutateIntBasic(ref globalRockPass.heightOperation, 0, 3, mutationRate, mutationDriftAmount);

                // MUTATION:
                // Height NOISE:
                MutateIntBasic(ref globalRockPass.numNoiseOctavesHeight, 4, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.heightFlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
               // AmpFreqOff
                MutateAmplitude(ref globalRockPass.heightSampleData, maxAmpToFreqRatio, minAmplitude, maxAmplitude, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalRockPass.heightSampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalRockPass.heightSampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalRockPass.heightSampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.heightSampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalRockPass.heightLevelsAdjust.x, 0f, globalRockPass.heightLevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.heightLevelsAdjust.y, globalRockPass.heightLevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.heightLevelsAdjust.z, 0f, globalRockPass.heightLevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.heightLevelsAdjust.w, globalRockPass.heightLevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);
                
                // MASK1 !!!!!
                MutateIntBasic(ref globalRockPass.numNoiseOctavesHeight, 4, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask1FlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalRockPass.mask1SampleData, maxAmpToFreqRatio, 0f, 5f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalRockPass.mask1SampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalRockPass.mask1SampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalRockPass.mask1SampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask1SampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalRockPass.mask1LevelsAdjust.x, 0f, globalRockPass.mask1LevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask1LevelsAdjust.y, globalRockPass.mask1LevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask1LevelsAdjust.z, 0f, globalRockPass.mask1LevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask1LevelsAdjust.w, globalRockPass.mask1LevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);
                
                // MASK2 !!!!!
                MutateIntBasic(ref globalRockPass.numNoiseOctavesHeight, 4, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask2FlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalRockPass.mask2SampleData, maxAmpToFreqRatio, 0f, 5f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalRockPass.mask2SampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalRockPass.mask2SampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalRockPass.mask2SampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask2SampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalRockPass.mask2LevelsAdjust.x, 0f, globalRockPass.mask2LevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask2LevelsAdjust.y, globalRockPass.mask2LevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask2LevelsAdjust.z, 0f, globalRockPass.mask2LevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.mask2LevelsAdjust.w, globalRockPass.mask2LevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);
                
                // FLOW !!!!!
                MutateIntBasic(ref globalRockPass.numNoiseOctavesHeight, 4, 8, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalRockPass.flowSampleData, maxAmpToFreqRatio, -1f, 1f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalRockPass.flowSampleData, minFrequency, maxFrequency * 0.33f, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalRockPass.flowSampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalRockPass.flowSampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalRockPass.flowSampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                                
                newGenome.terrainGlobalRockPasses.Add(globalRockPass);
            }
        }
        #endregion
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
                float maxAmpToFreqRatio = 10f;

                // Start with copy of Parent:
                GlobalSedimentPass globalSedimentPass = new GlobalSedimentPass();
                globalSedimentPass = parentGenome.terrainGlobalSedimentPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!

                //globalSedimentPass.maxAltitudeSedimentDrape = parentGenome.terrainGlobalSedimentPasses[i].maxAltitudeSedimentDrape; // is this necessary?
                //globalSedimentPass.sedimentDrapeMagnitude = parentGenome.terrainGlobalSedimentPasses[i].sedimentDrapeMagnitude;
                //globalSedimentPass.talusAngle = parentGenome.terrainGlobalSedimentPasses[i].talusAngle;
                //globalSedimentPass.uniformSedimentHeight = parentGenome.terrainGlobalSedimentPasses[i].uniformSedimentHeight;

                MutateFloatBasic(ref globalSedimentPass.maxAltitudeSedimentDrape, 0f, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.sedimentDrapeMagnitude, 0f, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.talusAngle, 0f, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.uniformSedimentHeight, 0f, 5f, mutationRate, mutationDriftAmount);
                
                // MUTATION:
                // SEDIMENT HEIGHT NOISE:
                MutateIntBasic(ref globalSedimentPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.heightFlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSedimentPass.heightSampleData, maxAmpToFreqRatio, minAmplitude, maxAmplitude, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSedimentPass.heightSampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSedimentPass.heightSampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSedimentPass.heightSampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.heightSampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalSedimentPass.heightLevelsAdjust.x, 0f, globalSedimentPass.heightLevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.heightLevelsAdjust.y, globalSedimentPass.heightLevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.heightLevelsAdjust.z, 0f, globalSedimentPass.heightLevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.heightLevelsAdjust.w, globalSedimentPass.heightLevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);

                // MASK1 !!!!!
                MutateIntBasic(ref globalSedimentPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask1FlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSedimentPass.mask1SampleData, maxAmpToFreqRatio, 0f, 5f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSedimentPass.mask1SampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSedimentPass.mask1SampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSedimentPass.mask1SampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask1SampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalSedimentPass.mask1LevelsAdjust.x, 0f, globalSedimentPass.mask1LevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask1LevelsAdjust.y, globalSedimentPass.mask1LevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask1LevelsAdjust.z, 0f, globalSedimentPass.mask1LevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask1LevelsAdjust.w, globalSedimentPass.mask1LevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);

                // MASK2 !!!!!
                MutateIntBasic(ref globalSedimentPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask2FlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSedimentPass.mask2SampleData, maxAmpToFreqRatio, 0f, 5f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSedimentPass.mask2SampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSedimentPass.mask2SampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSedimentPass.mask2SampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask2SampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalSedimentPass.mask2LevelsAdjust.x, 0f, globalSedimentPass.mask2LevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask2LevelsAdjust.y, globalSedimentPass.mask2LevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask2LevelsAdjust.z, 0f, globalSedimentPass.mask2LevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.mask2LevelsAdjust.w, globalSedimentPass.mask2LevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);

                // FLOW !!!!!
                MutateIntBasic(ref globalSedimentPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSedimentPass.flowSampleData, maxAmpToFreqRatio, -1f, 1f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSedimentPass.flowSampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSedimentPass.flowSampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSedimentPass.flowSampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSedimentPass.flowSampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                

                newGenome.terrainGlobalSedimentPasses.Add(globalSedimentPass);
            }
        }
        #endregion
        // SNOW!!! &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        #region SNOW

        // GLOBAL SNOW PASSES:
        if (parentGenome.terrainGlobalSnowPasses != null) {
            newGenome.terrainGlobalSnowPasses = new List<GlobalSnowPass>();

            for (int i = 0; i < parentGenome.terrainGlobalSnowPasses.Count; i++) {

                float maxAmplitude = baseMaxAmplitude / Mathf.Pow(2, i);
                float minAmplitude = baseMinAmplitude / Mathf.Pow(2, i);
                float maxFrequency = baseMaxFrequency * Mathf.Pow(2, i);
                float minFrequency = baseMinFrequency * Mathf.Pow(2, i);
                float maxAmpToFreqRatio = 10f;

                // Start with copy of Parent:
                GlobalSnowPass globalSnowPass = new GlobalSnowPass();
                globalSnowPass = parentGenome.terrainGlobalSnowPasses[i];  // MAKE SURE THIS IS VALUE-TYPED !!!!!!

                //globalSnowPass.snowLineStart = parentGenome.terrainGlobalSnowPasses[i].snowLineStart;
                //globalSnowPass.snowLineEnd = parentGenome.terrainGlobalSnowPasses[i].snowLineEnd;
                //globalSnowPass.snowAmount = parentGenome.terrainGlobalSnowPasses[i].snowAmount;
                //globalSnowPass.snowDirection = parentGenome.terrainGlobalSnowPasses[i].snowDirection;

                MutateFloatBasic(ref globalSnowPass.snowLineStart, 0f, globalSnowPass.snowLineEnd, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.snowLineEnd, globalSnowPass.snowLineStart, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.snowAmount, 0f, 4f, mutationRate, mutationDriftAmount);
                float mutationCheck = UnityEngine.Random.Range(0f, 1f);
                if (mutationCheck < mutationRate) {
                    Vector2 newSnowDirection = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
                    globalSnowPass.snowDirection = Vector2.Lerp(globalSnowPass.snowDirection, newSnowDirection, mutationDriftAmount).normalized;
                }

                // MUTATION:
                // Height NOISE:
                MutateIntBasic(ref globalSnowPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.heightFlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSnowPass.heightSampleData, maxAmpToFreqRatio, minAmplitude, maxAmplitude, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSnowPass.heightSampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSnowPass.heightSampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSnowPass.heightSampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.heightSampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalSnowPass.heightLevelsAdjust.x, 0f, globalSnowPass.heightLevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.heightLevelsAdjust.y, globalSnowPass.heightLevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.heightLevelsAdjust.z, 0f, globalSnowPass.heightLevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.heightLevelsAdjust.w, globalSnowPass.heightLevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);

                // MASK1 !!!!!
                MutateIntBasic(ref globalSnowPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask1FlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSnowPass.mask1SampleData, maxAmpToFreqRatio, 0f, 5f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSnowPass.mask1SampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSnowPass.mask1SampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSnowPass.mask1SampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask1SampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalSnowPass.mask1LevelsAdjust.x, 0f, globalSnowPass.mask1LevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask1LevelsAdjust.y, globalSnowPass.mask1LevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask1LevelsAdjust.z, 0f, globalSnowPass.mask1LevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask1LevelsAdjust.w, globalSnowPass.mask1LevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);

                // MASK2 !!!!!
                MutateIntBasic(ref globalSnowPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask2FlowAmount, -1f, 1f, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSnowPass.mask2SampleData, maxAmpToFreqRatio, 0f, 5f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSnowPass.mask2SampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSnowPass.mask2SampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSnowPass.mask2SampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask2SampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);
                // LEVELS!!!
                MutateFloatBasic(ref globalSnowPass.mask2LevelsAdjust.x, 0f, globalSnowPass.mask2LevelsAdjust.y, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask2LevelsAdjust.y, globalSnowPass.mask2LevelsAdjust.x, 1f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask2LevelsAdjust.z, 0f, globalSnowPass.mask2LevelsAdjust.w, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.mask2LevelsAdjust.w, globalSnowPass.mask2LevelsAdjust.z, 1f, mutationRate, mutationDriftAmount);

                // FLOW !!!!!
                MutateIntBasic(ref globalSnowPass.numNoiseOctavesHeight, 1, 8, mutationRate, mutationDriftAmount);
                // AmpFreqOff
                MutateAmplitude(ref globalSnowPass.flowSampleData, maxAmpToFreqRatio, -1f, 1f, mutationRate, mutationDriftAmount);
                MutateFrequency(ref globalSnowPass.flowSampleData, minFrequency, maxFrequency, mutationRate, mutationDriftAmount);
                MutateVector3Basic(ref globalSnowPass.flowSampleData.offset, -1000f, 1000f, 1f, 0.25f, mutationRate, mutationDriftAmount);
                // Rotation & ridgeNoise
                MutateFloatBasic(ref globalSnowPass.flowSampleData.rotation, -6.28f, 6.28f, mutationRate, mutationDriftAmount);
                MutateFloatBasic(ref globalSnowPass.flowSampleData.ridgeNoise, 0f, 1f, mutationRate, mutationDriftAmount);


                newGenome.terrainGlobalSnowPasses.Add(globalSnowPass);
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
