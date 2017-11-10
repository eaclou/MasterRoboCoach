using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TerrainConstructorGPU {

    public static ComputeShader terrainConstructorGPUCompute;
    public static  ComputeShader terrainInstanceCompute;

    public static Mesh rockAMesh0;
    public static Mesh rockAMesh1;
    public static Mesh rockAMesh2;
    //public static RenderTexture mainRenderTexture;
    //private static RenderTexture secondaryRenderTexture;
    //private static ComputeBuffer terrainGenomeCBuffer;

    private static ComputeBuffer terrainVertexCBuffer;
    private static ComputeBuffer terrainUVCBuffer;
    private static ComputeBuffer terrainNormalCBuffer;
    private static ComputeBuffer terrainColorCBuffer;
    private static ComputeBuffer terrainTriangleCBuffer;

    public static RenderTexture[] heightMapCascadeTextures;
    public static RenderTexture[] heightMapCascadeTexturesRender;
    public static RenderTexture temporaryRT;

    public static RenderTexture detailTexRock;
    public static RenderTexture detailTexSedi;
    public static RenderTexture detailTexSnow;
    //public static Material terrainDisplayMaterial;

    public static int xResolution = 512;
    public static int yResolution = 512;

    public struct TriangleIndexData {
        public int v1;
        public int v2;
        public int v3;
    }

    public static void GenerateTerrainTexturesFromGenome(EnvironmentGenome genome, bool updateDetailTextures) {
        

        if(heightMapCascadeTextures == null) {
            heightMapCascadeTextures = new RenderTexture[4];
            // Initialize Cascade Textures
            for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
                RenderTexture renderTexture = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
                renderTexture.wrapMode = TextureWrapMode.Clamp;
                renderTexture.filterMode = FilterMode.Bilinear;
                renderTexture.enableRandomWrite = true;
                renderTexture.useMipMap = true;
                renderTexture.Create();

                heightMapCascadeTextures[i] = renderTexture;
            }
        }
        else {
            ClearCascadeHeightTextures();
        }
        
        if(temporaryRT == null) {
            temporaryRT = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            temporaryRT.wrapMode = TextureWrapMode.Clamp;
            temporaryRT.filterMode = FilterMode.Bilinear;
            temporaryRT.enableRandomWrite = true;
            temporaryRT.useMipMap = true;
            temporaryRT.Create();
        }

        if (detailTexRock == null) {
            detailTexRock = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            detailTexRock.wrapMode = TextureWrapMode.Repeat;
            detailTexRock.filterMode = FilterMode.Bilinear;
            detailTexRock.enableRandomWrite = true;
            detailTexRock.useMipMap = true;
            detailTexRock.Create();
        }
        if (detailTexSedi == null) {
            detailTexSedi = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            detailTexSedi.wrapMode = TextureWrapMode.Repeat;
            detailTexSedi.filterMode = FilterMode.Bilinear;
            detailTexSedi.enableRandomWrite = true;
            detailTexSedi.useMipMap = true;
            detailTexSedi.Create();
        }
        if (detailTexSnow == null) {
            detailTexSnow = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            detailTexSnow.wrapMode = TextureWrapMode.Repeat;
            detailTexSnow.filterMode = FilterMode.Bilinear;
            detailTexSnow.enableRandomWrite = true;
            detailTexSnow.useMipMap = true;
            detailTexSnow.Create();
        }


        //Debug.Log("CascadeRenderTexturesCreated!");
        // BLIT PASSES HERE::::::
        //terrainDisplayMaterial = mat;
        //Debug.Log(terrainDisplayMaterial.ToString());


        if (genome.terrainGenome.terrainGlobalRockPasses != null) {  // if any globalrockPasses
            for(int i = 0; i < genome.terrainGenome.terrainGlobalRockPasses.Count; i++) {
                GlobalRockPass(genome.terrainGenome.terrainGlobalRockPasses[i]);
            }
        }
        
        if (genome.terrainGenome.terrainGlobalSedimentPasses != null) {  // if any globalrockPasses
            for (int i = 0; i < genome.terrainGenome.terrainGlobalSedimentPasses.Count; i++) {
                GlobalDebrisPass(genome.terrainGenome.terrainGlobalSedimentPasses[i]);
            }
        }


        if (genome.terrainGenome.heightStampHills.Count > 0) {  // if any globalrockPasses
            TerrainGenome.HeightStampData[] stampDataArray = new TerrainGenome.HeightStampData[genome.terrainGenome.heightStampHills.Count];
            for (int i = 0; i < genome.terrainGenome.heightStampHills.Count; i++) {
                // Convert List to Array for CBuffer                
                stampDataArray[i] = genome.terrainGenome.heightStampHills[i];
            }
            HeightStamps(stampDataArray, 0);
        }

        if (genome.terrainGenome.heightStampCraters.Count > 0) {  // if any globalrockPasses
            TerrainGenome.HeightStampData[] stampDataArray = new TerrainGenome.HeightStampData[genome.terrainGenome.heightStampCraters.Count];
            for (int i = 0; i < genome.terrainGenome.heightStampCraters.Count; i++) {
                // Convert List to Array for CBuffer                
                stampDataArray[i] = genome.terrainGenome.heightStampCraters[i];
            }
            HeightStamps(stampDataArray, 2); // 2 = crater
        }
        

        // SNOW:
        if (genome.terrainGenome.terrainGlobalSnowPasses != null) {  // if any globalrockPasses
            for (int i = 0; i < genome.terrainGenome.terrainGlobalSnowPasses.Count; i++) {
                GlobalSnowPass(genome.terrainGenome.terrainGlobalSnowPasses[i]);
            }
        }

        for (int i = 0; i < genome.terrainGenome.numRockSmoothPasses; i++) {
            SmoothHeights();
        }

        // needs to use StartPositions:  
        ArenaAdjustments(genome);
        CenterHeightTextures(genome);
        


        if (updateDetailTextures) {
            if (heightMapCascadeTexturesRender == null) {
                heightMapCascadeTexturesRender = new RenderTexture[4];
                // Initialize Cascade Textures
                for (int i = 0; i < heightMapCascadeTexturesRender.Length; i++) {
                    RenderTexture renderTexture = new RenderTexture(xResolution, yResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Default);
                    renderTexture.wrapMode = TextureWrapMode.Clamp;
                    renderTexture.filterMode = FilterMode.Bilinear;
                    renderTexture.enableRandomWrite = true;
                    renderTexture.useMipMap = true;
                    renderTexture.Create();

                    heightMapCascadeTexturesRender[i] = renderTexture;

                    
                }
            }

            for (int i = 0; i < heightMapCascadeTexturesRender.Length; i++) {
                Graphics.Blit(heightMapCascadeTextures[i], heightMapCascadeTexturesRender[i]);
            }


            Material makeTileableXMat = new Material(Shader.Find("TerrainBlit/TerrainBlitMakeTileableX"));
            makeTileableXMat.SetPass(0);
            Material makeTileableYMat = new Material(Shader.Find("TerrainBlit/TerrainBlitMakeTileableY"));
            makeTileableYMat.SetPass(0);
            Debug.Log("updateDetailTextures");
            Graphics.Blit(heightMapCascadeTextures[3], temporaryRT, makeTileableXMat);  // 
            Graphics.Blit(temporaryRT, detailTexRock, makeTileableYMat);
            Graphics.Blit(heightMapCascadeTextures[0], temporaryRT, makeTileableXMat);
            Graphics.Blit(temporaryRT, detailTexSedi, makeTileableYMat);
            Graphics.Blit(heightMapCascadeTextures[2], temporaryRT, makeTileableXMat);
            Graphics.Blit(temporaryRT, detailTexSnow, makeTileableYMat);
        }
        
        //Debug.Log(MeasureHeights().ToString());
    }
    
    private static void GlobalRockPass(TerrainGenome.GlobalRockPass pass) {

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyRockHeightGlobal"));
        modifyHeightMat.SetPass(0);

        // _HEIGHT_ADD _HEIGHT_SUBTRACT _HEIGHT_MULTIPLY _HEIGHT_AVERAGE
        if(pass.heightOperation == 0) {
            modifyHeightMat.EnableKeyword("_HEIGHT_ADD");
        }
        else if (pass.heightOperation == 1) {
            modifyHeightMat.EnableKeyword("_HEIGHT_SUBTRACT");
        }
        else if (pass.heightOperation == 2) {
            modifyHeightMat.EnableKeyword("_HEIGHT_MULTIPLY");
        }
        else {
            modifyHeightMat.EnableKeyword("_HEIGHT_AVERAGE");
        }

        //modifyHeightMat.EnableKeyword("_USE_NEW_NOISE"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_NOISE");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_NOISE");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_NOISE");

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = pass.numNoiseOctavesHeight;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.heightSampleData.amplitude, pass.heightSampleData.frequency, pass.heightSampleData.offset, pass.heightSampleData.rotation, pass.heightSampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", pass.heightLevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", pass.heightFlowAmount);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesMask1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.mask1SampleData.amplitude, pass.mask1SampleData.frequency, pass.mask1SampleData.offset, pass.mask1SampleData.rotation, pass.mask1SampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", pass.mask1LevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", pass.mask1FlowAmount);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesMask2;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.mask2SampleData.amplitude, pass.mask2SampleData.frequency, pass.mask2SampleData.offset, pass.mask2SampleData.rotation, pass.mask2SampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", pass.mask2LevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", pass.mask2FlowAmount);

        // FLOW TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesFlow;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.flowSampleData.amplitude, pass.flowSampleData.frequency, pass.flowSampleData.offset, pass.flowSampleData.rotation, pass.flowSampleData.ridgeNoise));
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
    private static void GlobalDebrisPass(TerrainGenome.GlobalSedimentPass pass) {

        Vector3 heights = MeasureHeights();


        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyDebrisHeightGlobal"));
        modifyHeightMat.SetPass(0);

        modifyHeightMat.SetFloat("_MaxAltitudeSedimentDrape", Mathf.Lerp(heights.y, heights.x, pass.maxAltitudeSedimentDrape));  // percentage based so use as lerp driver
        modifyHeightMat.SetFloat("_SedimentDrapeMagnitude", pass.sedimentDrapeMagnitude);
        modifyHeightMat.SetFloat("_UniformSedimentHeight", pass.uniformSedimentHeight);
        modifyHeightMat.SetFloat("_TalusAngle", pass.talusAngle);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = pass.numNoiseOctavesHeight;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.heightSampleData.amplitude, pass.heightSampleData.frequency, pass.heightSampleData.offset, pass.heightSampleData.rotation, pass.heightSampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", pass.heightLevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", pass.heightFlowAmount);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesMask1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.mask1SampleData.amplitude, pass.mask1SampleData.frequency, pass.mask1SampleData.offset, pass.mask1SampleData.rotation, pass.mask1SampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", pass.mask1LevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", pass.mask1FlowAmount);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesMask2;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.mask2SampleData.amplitude, pass.mask2SampleData.frequency, pass.mask2SampleData.offset, pass.mask2SampleData.rotation, pass.mask2SampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", pass.mask2LevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", pass.mask2FlowAmount);

        // FLOW TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesFlow;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.flowSampleData.amplitude, pass.flowSampleData.frequency, pass.flowSampleData.offset, pass.flowSampleData.rotation, pass.flowSampleData.ridgeNoise));
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
    private static void GlobalSnowPass(TerrainGenome.GlobalSnowPass pass) {

        Vector3 heights = MeasureHeights();


        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifySnowHeightGlobal"));
        modifyHeightMat.SetPass(0);

        modifyHeightMat.SetFloat("_SnowLineStart", Mathf.Lerp(heights.y, heights.x, pass.snowLineStart)); // y=min, x=max // percentage based so use as lerp driver
        modifyHeightMat.SetFloat("_SnowLineEnd", Mathf.Lerp(heights.y, heights.x, pass.snowLineEnd));  // percentage based so use as lerp driver
        modifyHeightMat.SetFloat("_SnowAmount", pass.snowAmount);
        modifyHeightMat.SetVector("_SnowDirection", new Vector4(pass.snowDirection.x, pass.snowDirection.y, 0f, 0f));

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = pass.numNoiseOctavesHeight;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        newTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.heightSampleData.amplitude, pass.heightSampleData.frequency, pass.heightSampleData.offset, pass.heightSampleData.rotation, pass.heightSampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);
        modifyHeightMat.SetVector("_NewTexLevels", pass.heightLevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_NewTexFlowAmount", pass.heightFlowAmount);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesMask1;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex1SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.mask1SampleData.amplitude, pass.mask1SampleData.frequency, pass.mask1SampleData.offset, pass.mask1SampleData.rotation, pass.mask1SampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex1Levels", pass.mask1LevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex1FlowAmount", pass.mask1FlowAmount);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesMask2;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        maskTex2SampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.mask2SampleData.amplitude, pass.mask2SampleData.frequency, pass.mask2SampleData.offset, pass.mask2SampleData.rotation, pass.mask2SampleData.ridgeNoise));
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);
        modifyHeightMat.SetVector("_MaskTex2Levels", pass.mask2LevelsAdjust);   // blackIn, whiteIn, blackOut, whiteOut
        modifyHeightMat.SetFloat("_MaskTex2FlowAmount", pass.mask2FlowAmount);

        // FLOW TEXTURE:::::
        numNoiseOctaves = pass.numNoiseOctavesFlow;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        flowTexSampleParamsCBuffer.SetData(SetNoiseSamplerSettings(numNoiseOctaves, pass.flowSampleData.amplitude, pass.flowSampleData.frequency, pass.flowSampleData.offset, pass.flowSampleData.rotation, pass.flowSampleData.ridgeNoise));
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

    private static void SmoothHeights() {
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitSmooth"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetInt("_PixelsWidth", xResolution);
        modifyHeightMat.SetInt("_PixelsHeight", yResolution);
        
        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }

    }

    private static void HeightStamps(TerrainGenome.HeightStampData[] stampData, int heightOp) {
        Material heightStampMat = new Material(Shader.Find("TerrainBlit/TerrainBlitHeightStamp"));
        heightStampMat.SetPass(0);
        //heightStampMat.SetInt("_PixelsWidth", xResolution);
        //heightStampMat.SetInt("_PixelsHeight", yResolution);
        //heightStampMat.SetFloat("_RadiusStartFade", stampData.radiusStartFade);
        //heightStampMat.SetFloat("_RadiusEndFade", stampData.radiusEndFade);
        //heightStampMat.SetVector("_WorldPivot", stampData.stampPivot);
        //heightStampMat.SetInt("_PixelsHeight", yResolution);
        // _HEIGHT_ADD _HEIGHT_SUBTRACT _HEIGHT_MULTIPLY _HEIGHT_AVERAGE
        int heightOperation = heightOp; // stampData.heightOperation;
        if (heightOperation == 0) {
            heightStampMat.EnableKeyword("_HEIGHT_ADD");
        }
        else if (heightOperation == 1) {
            heightStampMat.EnableKeyword("_HEIGHT_SUBTRACT");
        }
        else if (heightOperation == 2) {
            heightStampMat.EnableKeyword("_HEIGHT_MULTIPLY");
        }
        else {
            heightStampMat.EnableKeyword("_HEIGHT_AVERAGE");
        }

        // NEW HEIGHTS TEXTURE:::::
        //int numNoiseOctaves = stampData.heightSampleData.numOctaves;
        ComputeBuffer heightStampDataCBuffer = new ComputeBuffer(stampData.Length, sizeof(float) * 19 + sizeof(int) * 2);
        heightStampDataCBuffer.SetData(stampData);
        heightStampMat.SetBuffer("heightStampDataCBuffer", heightStampDataCBuffer);

        //ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(1, sizeof(float) * 11 + sizeof(int) * 1);
        //newTexSampleParamsCBuffer.SetData(SetHeightStampSettings(numNoiseOctaves, stampData.heightSampleData.amplitude, stampData.heightSampleData.frequency, stampData.heightSampleData.offset, stampData.heightSampleData.rotation, stampData.heightSampleData.ridgeNoise));
        //heightStampMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);

        heightStampMat.SetVector("_NewTexLevels", new Vector4(0f,1f,0f,1f));   // blackIn, whiteIn, blackOut, whiteOut
        heightStampMat.SetFloat("_NewTexFlowAmount", 0f);

        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            heightStampMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, heightStampMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture
        }
        heightStampDataCBuffer.Release();
    }

    private static Vector3 MeasureHeights() {
        ComputeBuffer altitudeMeasurementsCBuffer = new ComputeBuffer(1, sizeof(float) * 3);
        Vector3[] altitudeMeasurements = new Vector3[1];
        altitudeMeasurements[0] = new Vector3(-100f, 100f, 0f);
        altitudeMeasurementsCBuffer.SetData(altitudeMeasurements);

        int measureHeightsKernelID = terrainConstructorGPUCompute.FindKernel("CSMeasureHeights");
        terrainConstructorGPUCompute.SetTexture(measureHeightsKernelID, "heightTexture0", heightMapCascadeTextures[0]);   // Read-Only 
        terrainConstructorGPUCompute.SetBuffer(measureHeightsKernelID, "altitudeMeasurementsCBuffer", altitudeMeasurementsCBuffer);
        // Measure Stats:
        terrainConstructorGPUCompute.Dispatch(measureHeightsKernelID, xResolution, yResolution, 1);

        altitudeMeasurementsCBuffer.GetData(altitudeMeasurements);

        altitudeMeasurementsCBuffer.Release();

        altitudeMeasurements[0].z /= ((float)xResolution * (float)yResolution);
        return altitudeMeasurements[0];
    }

    private static void ArenaAdjustments(EnvironmentGenome genome) {
        //Debug.Log("ArenaAdjustments: startPos: " + genome.agentStartPositionsList[0].agentStartPosition.ToString());
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitArenaAdjustments"));
        modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, 3), 1f / Mathf.Pow(2f, 3), -1f / Mathf.Pow(2f, 3), 1f / Mathf.Pow(2f, 3)));
        modifyHeightMat.SetVector("_StartPosition", new Vector4(genome.agentStartPositionsList[0].agentStartPosition.x, genome.agentStartPositionsList[0].agentStartPosition.y, genome.agentStartPositionsList[0].agentStartPosition.z, 0f));
        modifyHeightMat.SetPass(0);
        Graphics.Blit(heightMapCascadeTextures[3], temporaryRT, modifyHeightMat);  // perform calculations on texture
        Graphics.Blit(temporaryRT, heightMapCascadeTextures[3]); // copy results back into main texture       

    }

    private static  GenomeNoiseOctaveData[] SetNoiseSamplerSettings(int numOctaves, Vector3 baseAmplitude, Vector3 baseFrequency, Vector3 baseOffset, float baseRotation, float ridgeNoise) {

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

    private static void CenterHeightTextures(EnvironmentGenome genome) {
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitCenterHeightTextures"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetVector("_StartPosition", new Vector4(genome.agentStartPositionsList[0].agentStartPosition.x, genome.agentStartPositionsList[0].agentStartPosition.y, genome.agentStartPositionsList[0].agentStartPosition.z, 0f));
        modifyHeightMat.SetTexture("_CenterTex", heightMapCascadeTextures[3]);

        for (int i = 0; i < heightMapCascadeTextures.Length; i++) {
            modifyHeightMat.SetVector("_GridBounds", new Vector4(-1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i), -1f / Mathf.Pow(2f, i), 1f / Mathf.Pow(2f, i)));
            Graphics.Blit(heightMapCascadeTextures[i], temporaryRT, modifyHeightMat);  // perform calculations on texture
            Graphics.Blit(temporaryRT, heightMapCascadeTextures[i]); // copy results back into main texture    
        }   
    }

    private static void ClearCascadeHeightTextures() {
        // Generate list of Triangle Indices (grouped into 3 per triangle):::
        int clearRenderTexturesKernelID = terrainConstructorGPUCompute.FindKernel("CSClearRenderTextures");
        terrainConstructorGPUCompute.SetTexture(clearRenderTexturesKernelID, "heightTextureWrite0", heightMapCascadeTextures[0]);   // Write-Only 
        terrainConstructorGPUCompute.SetTexture(clearRenderTexturesKernelID, "heightTextureWrite1", heightMapCascadeTextures[1]); // 
        terrainConstructorGPUCompute.SetTexture(clearRenderTexturesKernelID, "heightTextureWrite2", heightMapCascadeTextures[2]); //
        terrainConstructorGPUCompute.SetTexture(clearRenderTexturesKernelID, "heightTextureWrite3", heightMapCascadeTextures[3]); // 
        //terrainConstructorGPUCompute.SetBuffer(clearRenderTexturesKernelID, "terrainTriangleCBuffer", terrainTriangleCBuffer);

        // GENERATE MESH DATA!!!!
        terrainConstructorGPUCompute.Dispatch(clearRenderTexturesKernelID, heightMapCascadeTextures[0].width, heightMapCascadeTextures[0].height, 1);
    }
    
    public static Mesh GetTerrainMesh(int xResolution, int zResolution, float xCenter, float zCenter, float xSize, float zSize) {

        
        if (terrainConstructorGPUCompute == null) {
            Debug.LogError("NO COMPUTE SHADER SET!!!!");
           // terrainConstructorGPUCompute = new ComputeShader();
        }

        if (terrainVertexCBuffer != null)
            terrainVertexCBuffer.Release();
        terrainVertexCBuffer = new ComputeBuffer(xResolution * zResolution, sizeof(float) * 3);
        if (terrainUVCBuffer != null)
            terrainUVCBuffer.Release();
        terrainUVCBuffer = new ComputeBuffer(xResolution * zResolution, sizeof(float) * 2);
        if (terrainNormalCBuffer != null)
            terrainNormalCBuffer.Release();
        terrainNormalCBuffer = new ComputeBuffer(xResolution * zResolution, sizeof(float) * 3);
        if (terrainColorCBuffer != null)
            terrainColorCBuffer.Release();
        terrainColorCBuffer = new ComputeBuffer(xResolution * zResolution, sizeof(float) * 4);
        if (terrainTriangleCBuffer != null)
            terrainTriangleCBuffer.Release();
        terrainTriangleCBuffer = new ComputeBuffer((xResolution - 1) * (zResolution - 1) * 2, sizeof(int) * 3);
                

        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);
        // Set Shader properties so it knows where and what to build::::
        terrainConstructorGPUCompute.SetInt("resolutionX", xResolution);
        terrainConstructorGPUCompute.SetInt("resolutionZ", zResolution);
        terrainConstructorGPUCompute.SetVector("_QuadBounds", new Vector4(xCenter - offset.x, xCenter + offset.x, zCenter - offset.z, zCenter + offset.z));
        terrainConstructorGPUCompute.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));

        // Creates Actual Mesh data by reading from existing main Height Texture!!!!::::::
        int generateMeshDataKernelID = terrainConstructorGPUCompute.FindKernel("CSGenerateMeshData");
        terrainConstructorGPUCompute.SetTexture(generateMeshDataKernelID, "heightTexture0", heightMapCascadeTextures[0]);   // Read-Only 
        terrainConstructorGPUCompute.SetTexture(generateMeshDataKernelID, "heightTexture1", heightMapCascadeTextures[1]); // Read-Only
        terrainConstructorGPUCompute.SetTexture(generateMeshDataKernelID, "heightTexture2", heightMapCascadeTextures[2]); // Read-Only
        terrainConstructorGPUCompute.SetTexture(generateMeshDataKernelID, "heightTexture3", heightMapCascadeTextures[3]); // Read-Only

        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainVertexCBuffer", terrainVertexCBuffer);
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainUVCBuffer", terrainUVCBuffer);
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainNormalCBuffer", terrainNormalCBuffer);
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainColorCBuffer", terrainColorCBuffer);        
        
        // Generate list of Triangle Indices (grouped into 3 per triangle):::
        int triangleIndicesKernelID = terrainConstructorGPUCompute.FindKernel("CSGenerateTriangleIndices");        
        terrainConstructorGPUCompute.SetBuffer(triangleIndicesKernelID, "terrainTriangleCBuffer", terrainTriangleCBuffer);
        
        // GENERATE MESH DATA!!!!
        terrainConstructorGPUCompute.Dispatch(generateMeshDataKernelID, xResolution, 1, zResolution);
        terrainConstructorGPUCompute.Dispatch(triangleIndicesKernelID, xResolution - 1, 1, zResolution - 1);

        Mesh mesh = GenerateMesh();

        // CLEANUP!!
        terrainVertexCBuffer.Release();
        terrainUVCBuffer.Release();
        terrainNormalCBuffer.Release();
        terrainColorCBuffer.Release();
        terrainTriangleCBuffer.Release();

        return mesh;

    }

    private static Mesh GenerateMesh() {

        Mesh terrainMesh = new Mesh();

        TriangleIndexData[] triangleIndexDataArray = new TriangleIndexData[terrainTriangleCBuffer.count];
        terrainTriangleCBuffer.GetData(triangleIndexDataArray);
        int[] tris = new int[triangleIndexDataArray.Length * 3];
        for (int i = 0; i < triangleIndexDataArray.Length; i++) {
            tris[i * 3] = triangleIndexDataArray[i].v1;
            tris[i * 3 + 1] = triangleIndexDataArray[i].v2;
            tris[i * 3 + 2] = triangleIndexDataArray[i].v3;

            //Debug.Log(i.ToString() + " index0: " + triangleIndexDataArray[i].v1.ToString() + ", " + triangleIndexDataArray[i].v2.ToString() + ", " + triangleIndexDataArray[i].v3.ToString() + ", ");
        }

        Vector3[] vertices = new Vector3[terrainVertexCBuffer.count];
        Vector2[] uvs = new Vector2[terrainUVCBuffer.count];
        Vector3[] normals = new Vector3[terrainNormalCBuffer.count];
        Color[] colors = new Color[terrainColorCBuffer.count];
        terrainVertexCBuffer.GetData(vertices);
        terrainUVCBuffer.GetData(uvs);
        terrainNormalCBuffer.GetData(normals);
        terrainColorCBuffer.GetData(colors);

        // CONSTRUCT ACTUAL MESH
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uvs; //Unwrapping.GeneratePerTriangleUV(NewMesh);
        terrainMesh.triangles = tris;
        terrainMesh.normals = normals; //NewMesh.RecalculateNormals();
        terrainMesh.colors = colors;
        terrainMesh.RecalculateNormals();
        terrainMesh.RecalculateBounds();

        return terrainMesh;
    }    

    public static float GetAltitude(float worldX, float worldZ) {
        ComputeBuffer altitudeCBuffer = new ComputeBuffer(1, sizeof(float) * 1);

        int getAltitudeKernelID = terrainConstructorGPUCompute.FindKernel("CSGetAltitude");
        terrainConstructorGPUCompute.SetTexture(getAltitudeKernelID, "heightTexture3", heightMapCascadeTextures[3]);   // Read-Only 
        terrainConstructorGPUCompute.SetBuffer(getAltitudeKernelID, "altitudeCBuffer", altitudeCBuffer);
        terrainConstructorGPUCompute.SetFloat("_WorldX", worldX);
        terrainConstructorGPUCompute.SetFloat("_WorldZ", worldZ);
        // Measure Stats:
        terrainConstructorGPUCompute.Dispatch(getAltitudeKernelID, 1, 1, 1);
        
        float[] altitudeArray = new float[1];
        altitudeCBuffer.GetData(altitudeArray);
        //float altitude = modifyHeightMat.GetFloat("_AltitudeAtCoords");
        //Debug.Log("( " + worldX.ToString() + ", " + worldZ.ToString() + ") alt: " + altitudeArray[0].ToString());

        altitudeCBuffer.Release();

        return altitudeArray[0];
    }
}

