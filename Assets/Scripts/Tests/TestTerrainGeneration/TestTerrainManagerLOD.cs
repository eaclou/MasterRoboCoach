using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTerrainManagerLOD : MonoBehaviour {

    public GameObject TextureDisplayQuadGO;

    public TerrainManager terrainManagerRef;
    public ComputeShader terrainConstructorGPUCompute;
    public Material groundMat;
    private EnvironmentGenome envGenome;
    
    public Texture2D presetNoiseTex0;
    public Texture2D presetNoiseTex1;
    public Texture2D presetNoiseTex2;
    public Texture2D presetNoiseTex3;
    public Texture2D presetNoiseTex4;
    public Texture2D presetNoiseTex5;
    public Texture2D presetNoiseTex6;
    public Texture2D presetNoiseTex7;

    private RenderTexture[] generatedPaletteNoiseTextures;  // created procedurally based on genome parameters
    private Texture2D[] presetNoiseTextures;  // set in inspector, existing images

    private ComputeBuffer terrainGenomeCBuffer;

    public struct GenomeNoiseOctaveData {
        public Vector3 amplitude;
        public Vector3 frequency;
        public Vector3 offset;
        public float rotation;
        public float use_ridged_noise;
    }

    // Use this for initialization
    void Start () {

        envGenome = (Resources.Load("Templates/Environments/TemplateTestDefault") as EnvironmentGenomeTemplate).templateGenome;

        // Compute Shader:
        TerrainConstructorGPU.terrainConstructorGPUCompute = this.terrainConstructorGPUCompute;

        // Set Noise Textures:
        presetNoiseTextures = new Texture2D[8];

        presetNoiseTextures[0] = this.presetNoiseTex0;
        presetNoiseTextures[1] = this.presetNoiseTex1;
        presetNoiseTextures[2] = this.presetNoiseTex2;
        presetNoiseTextures[3] = this.presetNoiseTex3;
        presetNoiseTextures[4] = this.presetNoiseTex4;
        presetNoiseTextures[5] = this.presetNoiseTex5;
        presetNoiseTextures[6] = this.presetNoiseTex6;
        presetNoiseTextures[7] = this.presetNoiseTex7;


        // PROCESS GENOME DATA FOR COMPUTE SHADER!!!!!!
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();        
        
        generatedPaletteNoiseTextures = new RenderTexture[4];
        // Create palette noise textures:
        GeneratePaletteNoise();

        TerrainConstructorGPU.presetNoiseTextures = this.presetNoiseTextures;

        TextureDisplayQuadGO.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", generatedPaletteNoiseTextures[1]);

        terrainManagerRef.Initialize(this.gameObject, envGenome, groundMat, new Vector2(0f, 0f), new Vector2(680f, 680f), 6);
        
    }

    private void GeneratePaletteNoise() {
        // PROCESS GENOME DATA FOR COMPUTE SHADER!!!!!!
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();        

        RenderTexture tempTex = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
        tempTex.wrapMode = TextureWrapMode.Repeat;
        tempTex.filterMode = FilterMode.Point;
        tempTex.enableRandomWrite = true;
        tempTex.useMipMap = true;
        tempTex.Create();

        generatedPaletteNoiseTextures = new RenderTexture[4];

        // Create palette noise textures:
        int numNoiseOctaves = 4;
        Vector3 baseAmplitude = Vector3.one * 1f;
        Vector3 baseFrequency = Vector3.one * 10f;
        Vector3 baseOffset = Vector3.zero;
        float baseRotation = 0;
        float use_ridged_noise = 1f;

        terrainGenomeCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        GenomeNoiseOctaveData[] genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        GeneratePaletteNoiseTex(0, tempTex, genomeNoiseOctaveDataArray);

        numNoiseOctaves = 4;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 33f;
        baseFrequency.x = 7f;
        baseOffset = Vector3.zero;
        baseRotation = 4;
        use_ridged_noise = 1f;
        genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        GeneratePaletteNoiseTex(1, tempTex, genomeNoiseOctaveDataArray);

        numNoiseOctaves = 4;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 100f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 1f;
        genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        GeneratePaletteNoiseTex(2, tempTex, genomeNoiseOctaveDataArray);

        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 2f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 1f;
        genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        GeneratePaletteNoiseTex(3, tempTex, genomeNoiseOctaveDataArray);

        tempTex.Release();
        terrainGenomeCBuffer.Release();

        TerrainConstructorGPU.generatedPaletteNoiseTextures = this.generatedPaletteNoiseTextures;
    }

    private void GeneratePaletteNoiseTex(int index, RenderTexture tempTex, GenomeNoiseOctaveData[] genomeNoiseOctaveDataArray) {
        RenderTexture renderTex = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
        renderTex.wrapMode = TextureWrapMode.Repeat;
        renderTex.filterMode = FilterMode.Point;
        renderTex.enableRandomWrite = true;
        renderTex.useMipMap = true;
        renderTex.Create();

        generatedPaletteNoiseTextures[index] = renderTex;

        if (genomeNoiseOctaveDataArray.Length >= index) {
            Material noiseMat = new Material(Shader.Find("TerrainBlit/TerrainBlitGeneratePaletteNoise"));
            noiseMat.SetPass(0);
            //noiseMat.SetInt("_PixelsWidth", mainRenderTexture.width);
            //noiseMat.SetInt("_PixelsHeight", mainRenderTexture.height);
            noiseMat.SetBuffer("terrainGenomeCBuffer", terrainGenomeCBuffer);
            noiseMat.SetInt("_GenomeIndex", index);
            Graphics.Blit(generatedPaletteNoiseTextures[index], tempTex, noiseMat);  // perform calculations on texture
            Graphics.Blit(tempTex, generatedPaletteNoiseTextures[index]); // copy results back into main texture
                                                                      //Blit(generate noise texture and save it to generatedPaletteNoiseTextures[i])
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
