using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class TerrainConstructorGPU {

    public static ComputeShader terrainConstructorGPUCompute;
    public static RenderTexture mainRenderTexture;
    private static RenderTexture secondaryRenderTexture;
        
    private static ComputeBuffer terrainVertexCBuffer;
    private static ComputeBuffer terrainUVCBuffer;
    private static ComputeBuffer terrainNormalCBuffer;
    private static ComputeBuffer terrainColorCBuffer;
    private static ComputeBuffer terrainTriangleCBuffer;

    private static ComputeBuffer terrainGenomeCBuffer;

    //

    public struct TriangleIndexData {
        public int v1;
        public int v2;
        public int v3;
    }

    public struct GenomeNoiseOctaveData {
        public float amplitude;
        public float frequency;
        public Vector3 offset;
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

        

        //TextureDisplayQuadGO.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mainRenderTexture);

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

        // PROCESS GENOME DATA FOR COMPUTE SHADER!!!!!!
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();
        int numNoiseOctaves = 1;
        float baseAmplitude = 20f;
        float baseFrequency = 0.012f;
        Vector3 baseOffset = Vector3.zero;

        terrainGenomeCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 5);
        GenomeNoiseOctaveData[] genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for (int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        //float xQuadSize = xSize / (float)xResolution;
        //float zQuadSize = zSize / (float)zResolution;
        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);
        // Set Shader properties so it knows where and what to build::::
        terrainConstructorGPUCompute.SetInt("resolutionX", xResolution);
        terrainConstructorGPUCompute.SetInt("resolutionZ", zResolution);
        terrainConstructorGPUCompute.SetFloat("xStart", xCenter - offset.x);
        terrainConstructorGPUCompute.SetFloat("xEnd", xCenter + offset.x);
        terrainConstructorGPUCompute.SetFloat("zStart", zCenter - offset.z);
        terrainConstructorGPUCompute.SetFloat("zEnd", zCenter + offset.z);

        /*Debug.Log("GetTerrainMesh Center: [" + xCenter.ToString() + "," + zCenter.ToString() + "], Size: [" + xSize.ToString() + "," + zSize.ToString()
            + "], west: " + (xCenter - offset.x).ToString()
             + ", north: " + (zCenter + offset.z).ToString()
              + ", east: " + (xCenter + offset.x).ToString()
               + ", south: " + (zCenter - offset.z).ToString());*/

        // Initialize Simulation Texture Maps From Genome Data (Only Height at first)::::::!!!!!!:::::
        int initMainRenderTexturesKernelID = terrainConstructorGPUCompute.FindKernel("CSInitializeMainRenderTexture");
        terrainConstructorGPUCompute.SetBuffer(initMainRenderTexturesKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        terrainConstructorGPUCompute.SetTexture(initMainRenderTexturesKernelID, "mainRenderTexture", mainRenderTexture);  // Write Only  
        

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

        terrainConstructorGPUCompute.Dispatch(initMainRenderTexturesKernelID, xResolution, zResolution, 1); // Populates the main heightmap texture with base Noise values

        //$#!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        // #################################     EXTRA PASSES:      ##########################################
        Material addHeightMat = new Material(Shader.Find("TerrainBlit/TerrainBlitAddHeight"));
        addHeightMat.SetPass(0);
        addHeightMat.SetInt("_PixelsWidth", mainRenderTexture.width);
        addHeightMat.SetInt("_PixelsHeight", mainRenderTexture.height);
        addHeightMat.SetFloat("_FilterStrength", 1f);
        addHeightMat.SetFloat("_NoiseAmplitude", 10f);
        addHeightMat.SetFloat("_NoiseFrequency", 0.5f);
        addHeightMat.SetVector("_NoiseOffset", new Vector4(0f,0f,0f,0f));
        addHeightMat.SetVector("_GridBounds", new Vector4(xCenter - offset.x, xCenter + offset.x, zCenter - offset.z, zCenter + offset.z));

        addHeightMat.SetTexture("_MaskTex", mainRenderTexture);
        addHeightMat.EnableKeyword("USE_MASK");

        Graphics.Blit(mainRenderTexture, secondaryRenderTexture, addHeightMat);  // perform calculations on texture
        Graphics.Blit(secondaryRenderTexture, mainRenderTexture); // copy results back into main texture
        //$#!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!


        terrainConstructorGPUCompute.Dispatch(generateMeshDataKernelID, xResolution, 1, zResolution);
        terrainConstructorGPUCompute.Dispatch(triangleIndicesKernelID, xResolution - 1, 1, zResolution - 1);


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

        // CLEANUP!!
        //mainRenderTexture.Release();
        //secondaryRenderTexture.Release();
        terrainGenomeCBuffer.Release();
        terrainVertexCBuffer.Release();
        terrainUVCBuffer.Release();
        terrainNormalCBuffer.Release();
        terrainColorCBuffer.Release();
        terrainTriangleCBuffer.Release();


        return terrainMesh;

        // Display Mesh (set as MeshFilter's Mesh)
        //this.GetComponent<MeshFilter>().sharedMesh = terrainMesh;
        

        




        /*float xQuadSize = xSize / (float)xResolution;
        float zQuadSize = zSize / (float)zResolution;
        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Vector3[] vertices;

        Mesh mesh = new Mesh();

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xResolution + 1) * (zResolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zResolution; z++) {
            for (int x = 0; x <= xResolution; x++, i++) {
                float altitude = GetAltitude(genome, x * xQuadSize - offset.x + xCenter, z * zQuadSize - offset.z + zCenter);
                vertices[i] = new Vector3(x * xQuadSize, altitude, z * zQuadSize) - offset;
                uv[i] = new Vector2((float)x / xResolution, (float)z / zResolution);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xResolution * zResolution * 6];
        for (int ti = 0, vi = 0, z = 0; z < zResolution; z++, vi++) {
            for (int x = 0; x < xResolution; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xResolution + 1;
                triangles[ti + 5] = vi + xResolution + 2;
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        Debug.Log("Mesh Created!!!\n" +
            "Center: [" + xCenter.ToString() + ", " + zCenter.ToString() + "]    " +
            "Size: [" + xSize.ToString() + ", " + zSize.ToString() + "]    " +
            "Resolution: [" + xResolution.ToString() + "," + zResolution.ToString() + "]");

        return mesh;

    */
    }

    /*public static float GetAltitude(EnvironmentGenome genome, float x, float z) {


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
