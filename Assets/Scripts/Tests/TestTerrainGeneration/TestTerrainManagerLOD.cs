using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTerrainManagerLOD : MonoBehaviour {

    public GameObject TextureDisplayQuadGO1;
    public GameObject TextureDisplayQuadGO2;
    public GameObject TextureDisplayQuadGO3;

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

    //private RenderTexture[] generatedPaletteNoiseTextures;  // created procedurally based on genome parameters
    private Texture2D[] presetNoiseTextures;  // set in inspector, existing images

    private RenderTexture[] heightMapCascadeTextures;
    private RenderTexture temporaryRT;

    private ComputeBuffer terrainGenomeCBuffer;

    public ComputeBuffer rockStrataDataCBuffer;
    public int numRockStrataLayers = 8;
    public RenderTexture rockStrataRemapRT;
    public int strataRemapResolution = 64;

    public int xResolution = 1024;
    public int yResolution = 1024;

    public struct GenomeNoiseOctaveData {
        public Vector3 amplitude;
        public Vector3 frequency;
        public Vector3 offset;
        public float rotation;
        public float ridgeNoise;
    }

    public struct RockStrataData {
        public Vector3 color;
        public float hardness;
    }

    // Use this for initialization
    void Start () {

        envGenome = (Resources.Load("Templates/Environments/TemplateTestDefault") as EnvironmentGenomeTemplate).templateGenome;
        
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
        
        //generatedPaletteNoiseTextures = new RenderTexture[4];
        // Create palette noise textures:
        //GeneratePaletteNoise();

        //TerrainConstructorGPU.presetNoiseTextures = this.presetNoiseTextures;


        // &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        // &&&&&&&&&&&&&&&&&&&&&&&&&&&&    CONSTRUCT HEIGHT MAP CASCADE    &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        

        heightMapCascadeTextures = new RenderTexture[4];    // Initialize Cascade Textures
        for(int i = 0; i < heightMapCascadeTextures.Length; i++) {
            RenderTexture renderTexture = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
            renderTexture.wrapMode = TextureWrapMode.Clamp;
            renderTexture.filterMode = FilterMode.Bilinear;
            renderTexture.enableRandomWrite = true;
            renderTexture.useMipMap = true;
            renderTexture.Create();

            heightMapCascadeTextures[i] = renderTexture;
        }
        temporaryRT = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        temporaryRT.wrapMode = TextureWrapMode.Clamp;
        temporaryRT.filterMode = FilterMode.Bilinear;
        temporaryRT.enableRandomWrite = true;
        temporaryRT.useMipMap = true;
        temporaryRT.Create();

        // STRATA INIT:::::
        rockStrataRemapRT = new RenderTexture(strataRemapResolution, 1, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        rockStrataRemapRT.wrapMode = TextureWrapMode.Clamp;
        rockStrataRemapRT.filterMode = FilterMode.Bilinear;
        rockStrataRemapRT.enableRandomWrite = true;
        //rockStrataRemapRT.useMipMap = true;
        rockStrataRemapRT.Create();
        
        //
        rockStrataDataCBuffer = new ComputeBuffer(numRockStrataLayers, sizeof(float) * 4);
        RockStrataData[] rockStrataDataArray = new RockStrataData[rockStrataDataCBuffer.count];
        for(int i = 0; i < rockStrataDataArray.Length; i++) {
            RockStrataData data = new RockStrataData();
            data.color = UnityEngine.Random.insideUnitSphere;  // set random color
            data.hardness = UnityEngine.Random.Range(0f, 1f);
            rockStrataDataArray[i] = data;
        }
        rockStrataDataCBuffer.SetData(rockStrataDataArray);
        

        // PASSES:        
        FirstPass();  // populated height texture with solid rocks height
        //SecondPass();

        //ThirdPass();  // Debris 1

        //FourthPass(); // SNOW

        for(int i = 0; i < 2; i++) {
            SmoothHeights();
        }

        // STRATA ADJUST:
        for(int i = numRockStrataLayers - 1; i >= 0; i--) {
            StrataAdjustments(i);
        }
        //StrataAdjustments(2);

        //for (int i = 0; i < 2; i++) {
        //    SmoothHeights();
        //}


        // Arena Adjustments:
        ArenaAdjustments();


        // &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
        // &&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

        TextureDisplayQuadGO1.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", heightMapCascadeTextures[0]);
        if(heightMapCascadeTextures.Length > 1) {
            TextureDisplayQuadGO2.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", heightMapCascadeTextures[1]);
        }
        if (heightMapCascadeTextures.Length > 2) {
            TextureDisplayQuadGO3.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", heightMapCascadeTextures[2]);
        }


        // Set Data on Constructor:
        TerrainConstructorGPU.terrainConstructorGPUCompute = this.terrainConstructorGPUCompute;
        // Set Cascade Height Textures:
        TerrainConstructorGPU.heightMapCascadeTextures = heightMapCascadeTextures;

        // BUILD MESH FROM TEXTURE STACK!!!!
        terrainManagerRef.Initialize(this.gameObject, envGenome, groundMat, new Vector2(0f, 0f), new Vector2(680f, 680f), 7);        
    }

    private void FirstPass() {

        //Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyRockHeightGlobal"));
        modifyHeightMat.SetPass(0);
        //modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        //modifyHeightMat.SetInt("_PixelsHeight", yResolution);

        modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_NOISE");
        modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");
        
        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 6;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f,1f,1f) * 30f, new Vector3(1f, 1f, 1f) * 2f, Vector3.zero, 0.0f, 0f));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", new Vector4(0f, 0.25f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", 0f);
        
        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 1f, 1f) * 0.05f, Vector3.zero, 0.8f, 0f));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", new Vector4(0f, 0.5f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", 1f);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 2f, 1f) * 0.25f, Vector3.zero, 0f, 0f));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", new Vector4(0.0f, 0.1f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", 1f);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 5;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 0.05f, new Vector3(1f, 1f, 1f) * 0.5f, Vector3.one * 80, -1.1f, 0f));
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);
                

        for(int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }
            
        

        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
    }
    private void SecondPass() {

        //Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyRockHeightGlobal"));
        modifyHeightMat.SetPass(0);
        //modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        //modifyHeightMat.SetInt("_PixelsHeight", yResolution);

        modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 6;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 2f, new Vector3(1f, 0.8f, 0.7f) * 16f, Vector3.zero, -0.15f, 0f));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", new Vector4(0f, 0.25f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", 0.5f);
        

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 1f, 1f) * 0.05f, Vector3.zero, 0.8f, 0f));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", new Vector4(0f, 0.5f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", 1f);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 2f, 1f) * 0.25f, Vector3.zero, 0f, 0f));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", new Vector4(0.0f, 0.1f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", 1f);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 5;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 0.05f, new Vector3(1f, 1f, 1f) * 1f, Vector3.one * 20, -0.1f, 0f));
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);


        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }



        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
    }
    private void ThirdPass() {

        //Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyDebrisHeightGlobal"));
        modifyHeightMat.SetPass(0);
        //modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        //modifyHeightMat.SetInt("_PixelsHeight", yResolution);

        modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 6;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        
        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1, new Vector3(1f, 1.8f, 0.7f) * 16f, Vector3.zero, -2.56f, 0f));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", new Vector4(0f, 0.25f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", 1f);
        
        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 1f, 1f) * 0.05f, Vector3.zero, 0.8f, 0f));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", new Vector4(0f, 0.5f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", 1f);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 2f, 1f) * 0.25f, Vector3.zero, 0f, 0f));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", new Vector4(0.0f, 0.1f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", 1f);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 5;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 0.05f, new Vector3(1f, 1f, 1f) * 1f, Vector3.one * 20, -0.1f, 0f));
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);


        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }



        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
    }
    private void FourthPass() {

        //Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifySnowHeightGlobal"));
        modifyHeightMat.SetPass(0);
        //modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        //modifyHeightMat.SetInt("_PixelsHeight", yResolution);

        modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 3;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);

        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 2, new Vector3(1f, 1.8f, 0.7f) * 4f, Vector3.zero, 0.1f, 0f));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", new Vector4(0f, 0.25f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", 1f);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 1f, 1f) * 1f, Vector3.zero, 0.8f, 0f));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", new Vector4(0f, 1f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", 1f);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 1f, new Vector3(1f, 2f, 1f) * 0.25f, Vector3.zero, 0f, 0f));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", new Vector4(0.7f, 1f, 0f, 1f));   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", 0f);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 5;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, new Vector3(1f, 1f, 1f) * 0.005f, new Vector3(1f, 1f, 1f) * 2f, Vector3.one * 20, -0.2f, 0f));
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);


        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }

        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
    }

    private void SmoothHeights() {
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitSmooth"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        modifyHeightMat.SetInt("_PixelsHeight", yResolution);

        //modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        //modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        //modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }

    }
    private void FakeErosion() {
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitFakeErosion"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        modifyHeightMat.SetInt("_PixelsHeight", yResolution);

        //modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        //modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        //modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }

    }
    private void ArenaAdjustments() {
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitArenaAdjustments"));
        modifyHeightMat.SetPass(0);        

        //modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
        Graphics.Blit(heightMapCascadeTextures[3], temporaryRT, modifyHeightMat);  // perform calculations on texture
        Graphics.Blit(temporaryRT, heightMapCascadeTextures[3]); // copy results back into main texture

        //modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        //modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        //modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        /*for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }*/

    }
    private void StrataAdjustments(int strataIndex) {
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitStrataAdjustments"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetFloat("_MinAltitude", -30f);
        modifyHeightMat.SetFloat("_MaxAltitude", 30f);
        modifyHeightMat.SetFloat("_RemapOffset", 5f);
        modifyHeightMat.SetFloat("_NumStrata", numRockStrataLayers);
        modifyHeightMat.SetFloat("_StrataIndex", strataIndex);
        modifyHeightMat.SetBuffer("rockStrataDataCBuffer", rockStrataDataCBuffer);

        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }

        //modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
        
        //modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        //modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        //modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[5]);
        //modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[2]);
        //modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[3]);

        /*for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }*/

    }

    private GenomeNoiseOctaveData[] SetNoiseSamplerSettings(int numOctaves, Vector3 baseAmplitude, Vector3 baseFrequency, Vector3 baseOffset, float baseRotation, float ridgeNoise) {
        
        GenomeNoiseOctaveData[] sampleParamsArray = new GenomeNoiseOctaveData[numOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation * i;
            genomeNoiseOctaveData.ridgeNoise = ridgeNoise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        return sampleParamsArray;
    }

    /*
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
        float ridgeNoise = 1f;

        terrainGenomeCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        GenomeNoiseOctaveData[] genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.ridgeNoise = ridgeNoise;
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
        ridgeNoise = 1f;
        genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.ridgeNoise = ridgeNoise;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        GeneratePaletteNoiseTex(1, tempTex, genomeNoiseOctaveDataArray);

        numNoiseOctaves = 4;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 100f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        ridgeNoise = 1f;
        genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.ridgeNoise = ridgeNoise;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        GeneratePaletteNoiseTex(2, tempTex, genomeNoiseOctaveDataArray);

        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 2f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        ridgeNoise = 1f;
        genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.ridgeNoise = ridgeNoise;
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
	*/
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy() {
        if(rockStrataDataCBuffer != null) {
            rockStrataDataCBuffer.Release();
        }
        if (rockStrataRemapRT != null) {
            rockStrataRemapRT.Release();
        }
    }

    private void LateUpdate() {
        // Set textures on display material:

        groundMat.SetPass(0);
        groundMat.SetFloat("_MinAltitude", -30f);
        groundMat.SetFloat("_MaxAltitude", 30f);
        groundMat.SetFloat("_RemapOffset", 5f); 
        groundMat.SetFloat("_NumStrata", numRockStrataLayers);
        groundMat.SetBuffer("rockStrataDataCBuffer", rockStrataDataCBuffer);       
        groundMat.SetTexture("_StrataRemapTex", rockStrataRemapRT);
        //groundMat.SetTexture("_MainTex", rockStrataRemapRT);

        //Debug.Log(" " + groundMat.GetTexture("_MainTex").ToString());
    }
}
