using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TerrainConstructorGPU {

    public static ComputeShader terrainConstructorGPUCompute;
    public static RenderTexture mainRenderTexture;
    private static RenderTexture secondaryRenderTexture;

    private static ComputeBuffer terrainGenomeCBuffer;

    private static ComputeBuffer terrainVertexCBuffer;
    private static ComputeBuffer terrainUVCBuffer;
    private static ComputeBuffer terrainNormalCBuffer;
    private static ComputeBuffer terrainColorCBuffer;
    private static ComputeBuffer terrainTriangleCBuffer;

    public static Texture2D[] presetNoiseTextures;
    public static RenderTexture[] generatedPaletteNoiseTextures;

    public struct TriangleIndexData {
        public int v1;
        public int v2;
        public int v3;
    }

    public struct GenomeNoiseOctaveData {
        public Vector3 amplitude;
        public Vector3 frequency;
        public Vector3 offset;
        public float rotation;
        public float use_ridged_noise;
    }


    public static Mesh GetTerrainMesh(EnvironmentGenome genome, int xResolution, int zResolution, float xCenter, float zCenter, float xSize, float zSize) {

        
        if (terrainConstructorGPUCompute == null) {
            terrainConstructorGPUCompute = new ComputeShader();
        }
        //if (mainRenderTexture == null) {
            //mainRenderTexture.Release();
        mainRenderTexture = new RenderTexture(xResolution, zResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        mainRenderTexture.wrapMode = TextureWrapMode.Repeat;
        mainRenderTexture.filterMode = FilterMode.Point;
        mainRenderTexture.enableRandomWrite = true;
        mainRenderTexture.useMipMap = true;
        mainRenderTexture.Create();
        //}
        //if (secondaryRenderTexture == null) {
        secondaryRenderTexture = new RenderTexture(xResolution, zResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        secondaryRenderTexture.wrapMode = TextureWrapMode.Repeat;
        secondaryRenderTexture.filterMode = FilterMode.Point;
        secondaryRenderTexture.enableRandomWrite = true;
        secondaryRenderTexture.useMipMap = true;
        secondaryRenderTexture.Create();
        //}
                
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
        terrainConstructorGPUCompute.SetFloat("xStart", xCenter - offset.x);
        terrainConstructorGPUCompute.SetFloat("xEnd", xCenter + offset.x);
        terrainConstructorGPUCompute.SetFloat("zStart", zCenter - offset.z);
        terrainConstructorGPUCompute.SetFloat("zEnd", zCenter + offset.z);

        // Creates Actual Mesh data by reading from existing main Height Texture!!!!::::::
        int generateMeshDataKernelID = terrainConstructorGPUCompute.FindKernel("CSGenerateMeshData");
        terrainConstructorGPUCompute.SetTexture(generateMeshDataKernelID, "mainRenderTexture", mainRenderTexture);  // Write Only  
        terrainConstructorGPUCompute.SetTexture(generateMeshDataKernelID, "mainRenderTextureRead", mainRenderTexture); // Read-Only
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainVertexCBuffer", terrainVertexCBuffer);
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainUVCBuffer", terrainUVCBuffer);
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainNormalCBuffer", terrainNormalCBuffer);
        terrainConstructorGPUCompute.SetBuffer(generateMeshDataKernelID, "terrainColorCBuffer", terrainColorCBuffer);        
        
        // Generate list of Triangle Indices (grouped into 3 per triangle):::
        int triangleIndicesKernelID = terrainConstructorGPUCompute.FindKernel("CSGenerateTriangleIndices");        
        terrainConstructorGPUCompute.SetBuffer(triangleIndicesKernelID, "terrainTriangleCBuffer", terrainTriangleCBuffer);
        
        // PASSES::::::
        FirstPass(xCenter, zCenter, xSize, zSize);
        SecondPass(xCenter, zCenter, xSize, zSize);
        ThirdPass(xCenter, zCenter, xSize, zSize);


        terrainConstructorGPUCompute.Dispatch(generateMeshDataKernelID, xResolution, 1, zResolution);
        terrainConstructorGPUCompute.Dispatch(triangleIndicesKernelID, xResolution - 1, 1, zResolution - 1);

        Mesh mesh = GenerateMesh();

        // CLEANUP!!
        //mainRenderTexture.Release();
        //secondaryRenderTexture.Release();
        //terrainGenomeCBuffer.Release();
        terrainVertexCBuffer.Release();
        terrainUVCBuffer.Release();
        terrainNormalCBuffer.Release();
        terrainColorCBuffer.Release();
        terrainTriangleCBuffer.Release();

        return mesh;

    }

    private static void FirstPass(float xCenter, float zCenter, float xSize, float zSize) {
        
        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);
        
        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyHeight"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetInt("_PixelsWidth", mainRenderTexture.width);
        modifyHeightMat.SetInt("_PixelsHeight", mainRenderTexture.height);

        //modifyHeightMat.EnableKeyword("_USE_NEW_TEX"); // absence uses procedural noise
        modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_TEX");

        modifyHeightMat.SetFloat("_Mask1BlackPointIn", 0.5f);
        modifyHeightMat.SetFloat("_Mask1WhitePointIn", 0.75f);
        modifyHeightMat.SetFloat("_Mask1Gamma", 0.1f);
        modifyHeightMat.SetFloat("_Mask2BlackPointIn", 0.1f);
        modifyHeightMat.SetFloat("_Mask2WhitePointIn", 0.8f);
        modifyHeightMat.SetFloat("_Mask2Gamma", 2f);

        modifyHeightMat.SetVector("_GridBounds", new Vector4(xCenter - offset.x, xCenter + offset.x, zCenter - offset.z, zCenter + offset.z));

        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[0]);
        modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[6]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[7]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 4;
        Vector3 baseAmplitude = Vector3.one * 20f;
        Vector3 baseFrequency = Vector3.one * 5f;
        baseFrequency.x = 3f;
        Vector3 baseOffset = Vector3.zero;
        float baseRotation = 0;
        float use_ridged_noise = 1f;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        GenomeNoiseOctaveData[] sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        newTexSampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 1f;
        baseOffset = Vector3.zero;
        baseRotation = -0.6f;
        use_ridged_noise = 0f;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        maskTex1SampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 0.4f;
        baseOffset = Vector3.zero;
        baseRotation = 1.2f;
        use_ridged_noise = 0f;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        maskTex2SampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 0.04f;
        baseFrequency = Vector3.one * 4f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 0f;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        flowTexSampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);

        Graphics.Blit(mainRenderTexture, secondaryRenderTexture, modifyHeightMat);  // perform calculations on texture
        Graphics.Blit(secondaryRenderTexture, mainRenderTexture); // copy results back into main texture

        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
    }
    private static void SecondPass(float xCenter, float zCenter, float xSize, float zSize) {

        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyHeight"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetInt("_PixelsWidth", mainRenderTexture.width);
        modifyHeightMat.SetInt("_PixelsHeight", mainRenderTexture.height);

        //modifyHeightMat.EnableKeyword("_USE_NEW_TEX"); // absence uses procedural noise
        //modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_NONE");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_TEX");

        modifyHeightMat.SetFloat("_Mask1BlackPointIn", 0.5f);
        modifyHeightMat.SetFloat("_Mask1WhitePointIn", 0.75f);
        modifyHeightMat.SetFloat("_Mask1Gamma", 0.1f);
        modifyHeightMat.SetFloat("_Mask2BlackPointIn", 0.0f);
        modifyHeightMat.SetFloat("_Mask2WhitePointIn", 1f);
        modifyHeightMat.SetFloat("_Mask2Gamma", 1f);

        modifyHeightMat.SetVector("_GridBounds", new Vector4(xCenter - offset.x, xCenter + offset.x, zCenter - offset.z, zCenter + offset.z));

        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[0]);
        modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[6]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[7]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 4;
        Vector3 baseAmplitude = Vector3.one * 60f;
        Vector3 baseFrequency = Vector3.one * 1f;
        baseFrequency.x = 1.7f;
        Vector3 baseOffset = Vector3.zero;
        float baseRotation = 0;
        float use_ridged_noise = 1f;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        GenomeNoiseOctaveData[] sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        newTexSampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 1f;
        baseOffset = Vector3.zero;
        baseRotation = -0.6f;
        use_ridged_noise = 0f;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        maskTex1SampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 1f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 0f;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        maskTex2SampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 0.04f;
        baseFrequency = Vector3.one * 4f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 0f;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        flowTexSampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);

        Graphics.Blit(mainRenderTexture, secondaryRenderTexture, modifyHeightMat);  // perform calculations on texture
        Graphics.Blit(secondaryRenderTexture, mainRenderTexture); // copy results back into main texture

        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
    }
    private static void ThirdPass(float xCenter, float zCenter, float xSize, float zSize) {

        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Material modifyHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyHeight"));
        modifyHeightMat.SetPass(0);
        modifyHeightMat.SetInt("_PixelsWidth", mainRenderTexture.width);
        modifyHeightMat.SetInt("_PixelsHeight", mainRenderTexture.height);

        //modifyHeightMat.EnableKeyword("_USE_NEW_TEX"); // absence uses procedural noise
        modifyHeightMat.EnableKeyword("_USE_MASK1_TEX");
        //modifyHeightMat.EnableKeyword("_USE_MASK2_TEX");
        //modifyHeightMat.EnableKeyword("_USE_FLOW_TEX");

        modifyHeightMat.SetFloat("_Mask1BlackPointIn", 0.6f);
        modifyHeightMat.SetFloat("_Mask1WhitePointIn", 1f);
        modifyHeightMat.SetFloat("_Mask1Gamma", 1f);
        modifyHeightMat.SetFloat("_Mask2BlackPointIn", 0.0f);
        modifyHeightMat.SetFloat("_Mask2WhitePointIn", 1f);
        modifyHeightMat.SetFloat("_Mask2Gamma", 1f);

        modifyHeightMat.SetVector("_GridBounds", new Vector4(xCenter - offset.x, xCenter + offset.x, zCenter - offset.z, zCenter + offset.z));

        modifyHeightMat.SetTexture("_NewTex", presetNoiseTextures[0]);
        modifyHeightMat.SetTexture("_MaskTex1", presetNoiseTextures[3]);
        modifyHeightMat.SetTexture("_MaskTex2", presetNoiseTextures[6]);
        modifyHeightMat.SetTexture("_FlowTex", presetNoiseTextures[7]);

        // NEW HEIGHTS TEXTURE:::::
        int numNoiseOctaves = 4;
        Vector3 baseAmplitude = Vector3.one * 5f;
        Vector3 baseFrequency = Vector3.one * 5f;
        baseFrequency.x = 2.6f;
        Vector3 baseOffset = Vector3.zero;
        float baseRotation = 4;
        float use_ridged_noise = 0f;
        ComputeBuffer newTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        GenomeNoiseOctaveData[] sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        newTexSampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("newTexSampleParamsCBuffer", newTexSampleParamsCBuffer);

        // MASK 1 TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 0.2f;
        baseOffset = Vector3.zero;
        baseRotation = 0f;
        use_ridged_noise = 0f;
        ComputeBuffer maskTex1SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        maskTex1SampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("maskTex1SampleParamsCBuffer", maskTex1SampleParamsCBuffer);

        // MASK 2 TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 1f;
        baseFrequency = Vector3.one * 1f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 0f;
        ComputeBuffer maskTex2SampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        maskTex2SampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("maskTex2SampleParamsCBuffer", maskTex2SampleParamsCBuffer);

        // FLOW TEXTURE:::::
        numNoiseOctaves = 1;
        baseAmplitude = Vector3.one * 0.04f;
        baseFrequency = Vector3.one * 4f;
        baseOffset = Vector3.zero;
        baseRotation = 0;
        use_ridged_noise = 0f;
        ComputeBuffer flowTexSampleParamsCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 11);
        sampleParamsArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < sampleParamsArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveData.rotation = baseRotation;
            genomeNoiseOctaveData.use_ridged_noise = use_ridged_noise;
            sampleParamsArray[i] = genomeNoiseOctaveData;
        }
        flowTexSampleParamsCBuffer.SetData(sampleParamsArray);
        modifyHeightMat.SetBuffer("flowTexSampleParamsCBuffer", flowTexSampleParamsCBuffer);

        Graphics.Blit(mainRenderTexture, secondaryRenderTexture, modifyHeightMat);  // perform calculations on texture
        Graphics.Blit(secondaryRenderTexture, mainRenderTexture); // copy results back into main texture

        newTexSampleParamsCBuffer.Release();
        maskTex1SampleParamsCBuffer.Release();
        maskTex2SampleParamsCBuffer.Release();
        flowTexSampleParamsCBuffer.Release();
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

    /*private static void ModifyHeightNoise(Vector4 gridBounds, float amplitude, float frequency, Vector2 offset, Texture2D maskTexture) {
        
        Material heightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitModifyHeightNoise"));
        heightMat.SetPass(0);
        heightMat.SetInt("_PixelsWidth", mainRenderTexture.width);
        heightMat.SetInt("_PixelsHeight", mainRenderTexture.height);
        heightMat.SetFloat("_FilterStrength", 1f);
        heightMat.SetFloat("_NoiseAmplitude", 10f);
        heightMat.SetFloat("_NoiseFrequency", 0.5f);
        heightMat.SetVector("_NoiseOffset", new Vector4(0f, 0f, 0f, 0f));
        heightMat.SetVector("_GridBounds", gridBounds);
        heightMat.EnableKeyword("USE_MASK");
        heightMat.SetTexture("_MaskTex", maskTexture);

        Graphics.Blit(mainRenderTexture, secondaryRenderTexture, heightMat);  // perform calculations on texture
        Graphics.Blit(secondaryRenderTexture, mainRenderTexture); // copy results back into main texture
    }

    public static float GetAltitude(EnvironmentGenome genome, float x, float z) {


        if (!genome.terrainGenome.useAltitude) {
            return 0f;
        }
        float altitude = 0f;
        float distMultiplier = 0.02f;
        float dist = new Vector2(x, z).magnitude * distMultiplier;
             
        float amplitude = 60f;
        float total = 0f;
        for (int i = 0; i < genome.terrainGenome.terrainWaves.Length; i++) {
            float height = NoisePrime.Simplex3D(new Vector3(x, genome.terrainGenome.terrainWaves[i].z, z), genome.terrainGenome.terrainWaves[i].x * Mathf.Pow(2f, i)).value * (amplitude * genome.terrainGenome.terrainWaves[i].y / Mathf.Pow(2f, i) * dist);
            total += height;
        }
        altitude = total / genome.terrainGenome.terrainWaves.Length;

        // StartPositions!!!
        for(int j = 0; j < genome.agentStartPositionsList.Count; j++) {
            float distToSpawn = (new Vector2(x, z) - new Vector2(genome.agentStartPositionsList[j].agentStartPosition.x, genome.agentStartPositionsList[j].agentStartPosition.z)).magnitude;
            if (distToSpawn < 12f) {
                altitude = Mathf.Lerp(0f, altitude, Mathf.Max(distToSpawn - 4f, 0f) / 8f);
            }
        }

        return altitude; 
        
    }
    */
}
