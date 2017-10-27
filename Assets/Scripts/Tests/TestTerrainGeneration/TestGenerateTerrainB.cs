using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateTerrainB : MonoBehaviour {

    public GameObject TextureDisplayQuadGO;

    public Material blitMaterial;

    public ComputeShader computeGenerateTerrainB;
    
    private ComputeBuffer terrainGenomeCBuffer;
    private ComputeBuffer terrainVertexDataCBuffer;
    private ComputeBuffer terrainTriangleIndexDataCBuffer;

    private RenderTexture mainHeightTexture;
    private RenderTexture secondaryHeightTexture;

    private Mesh terrainMesh;

    public int texturePixelsX = 64;
    public int texturePixelsY = 64;
    public int meshResolutionX = 8;
    public int meshResolutionZ = 8;
    public int numNoiseOctaves = 4;
    public int numFilterIterations = 1;

    public float baseAmplitude = 1f;
    public float baseFrequency = 1f;
    public Vector3 baseOffset = new Vector3(0f, 0f, 0f);

    public float xStart = 0f;
    public float xEnd = 8f;
    public float zStart = 0f;
    public float zEnd = 8f;

    public struct VertexData {
        public Vector3 worldPos;
        public Vector3 normal;
        public Vector2 uv;
        public Vector3 color;
    }
    public struct GenomeNoiseOctaveData {
        public float amplitude;
        public float frequency;
        public Vector3 offset;
    }
    public struct TriangleIndexData {
        public int v1;
        public int v2;
        public int v3;
    }

    // Use this for initialization
    void Start () {

        InitializeBuffers();

        CreateTextures();

        // Upload TerrainGenome to GPU
        SetShaderGlobalValues();
        
        InitializeHeightTextureMap();
        
        GenerateMeshData();
        RecalculateMesh();
    }

    private void CreateTextures() {

        mainHeightTexture = new RenderTexture(texturePixelsX, texturePixelsY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        mainHeightTexture.wrapMode = TextureWrapMode.Clamp;
        mainHeightTexture.filterMode = FilterMode.Point;
        mainHeightTexture.enableRandomWrite = true;
        mainHeightTexture.Create();

        secondaryHeightTexture = new RenderTexture(texturePixelsX, texturePixelsY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);        
        secondaryHeightTexture.wrapMode = TextureWrapMode.Clamp;
        secondaryHeightTexture.filterMode = FilterMode.Point;
        secondaryHeightTexture.enableRandomWrite = true;        
        secondaryHeightTexture.Create();

        TextureDisplayQuadGO.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", mainHeightTexture);
    }

    private void InitializeHeightTextureMap() {
        //CSInitializeTextureHeightData
        int initTextureHeightKernelID = computeGenerateTerrainB.FindKernel("CSInitializeTextureHeightData");
        computeGenerateTerrainB.SetBuffer(initTextureHeightKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        computeGenerateTerrainB.SetTexture(initTextureHeightKernelID, "mainHeightTexture", mainHeightTexture);
        computeGenerateTerrainB.Dispatch(initTextureHeightKernelID, texturePixelsX, texturePixelsY, 1);

        //mainHeightTexture.;
    }

    private void BlitTexture() {
        blitMaterial.SetPass(0);
        blitMaterial.SetInt("_PixelsWidth", mainHeightTexture.width);
        blitMaterial.SetInt("_PixelsHeight", mainHeightTexture.width);
        Graphics.Blit(mainHeightTexture, secondaryHeightTexture, blitMaterial);  // perform calculations on texture
        Graphics.Blit(secondaryHeightTexture, mainHeightTexture); // copy results back into main texture
    }

    public void SimulateAndRemesh() {
        for(int i = 0; i < numFilterIterations; i++) {
            BlitTexture();
        }
        GenerateMeshData();
        RecalculateMesh();
    }

    private void GenerateMeshData() {
        // Generate Vertex Position/Color data from mainTextureHeightMap:
        //int generateVertexDataKernelID = computeGenerateTerrainB.FindKernel("CSGenerateVertData");
        //computeGenerateTerrainB.SetTexture(generateVertexDataKernelID, "mainHeightTexture", mainHeightTexture);
        //computeGenerateTerrainB.SetBuffer(generateVertexDataKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);

        // Calculate Triangle Indices for Mesh:
        int triangleIndicesKernelID = computeGenerateTerrainB.FindKernel("CSGenerateTriangleIndices");
        computeGenerateTerrainB.SetBuffer(triangleIndicesKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        computeGenerateTerrainB.SetBuffer(triangleIndicesKernelID, "terrainTriangleIndexDataCBuffer", terrainTriangleIndexDataCBuffer);

        int testKernelID = computeGenerateTerrainB.FindKernel("CSTest");  // Bypass some weird bug, copied code into new kernel Name and it worked..... fucking hell
        computeGenerateTerrainB.SetTexture(testKernelID, "mainHeightTexture", mainHeightTexture);
        computeGenerateTerrainB.SetTexture(testKernelID, "mainHeightTextureRead", mainHeightTexture);
        //computeGenerateTerrainB.SetBuffer(testKernelID, "testCBuffer", testCBuffer);
        computeGenerateTerrainB.SetBuffer(testKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);

        computeGenerateTerrainB.Dispatch(testKernelID, meshResolutionX, meshResolutionZ, 1);        
        computeGenerateTerrainB.Dispatch(triangleIndicesKernelID, meshResolutionX - 1, 1, meshResolutionZ - 1);
    }

    private void InitializeBuffers() {
        // InitializeBuffers
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();
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

        if (terrainVertexDataCBuffer != null)
            terrainVertexDataCBuffer.Release();
        terrainVertexDataCBuffer = new ComputeBuffer(meshResolutionX * meshResolutionZ, sizeof(float) * 11);

        if (terrainTriangleIndexDataCBuffer != null)
            terrainTriangleIndexDataCBuffer.Release();
        terrainTriangleIndexDataCBuffer = new ComputeBuffer((meshResolutionX - 1) * (meshResolutionZ - 1) * 2, sizeof(int) * 3);
        
    }

    private void SetShaderGlobalValues() {
        // Upload TerrainGenome to GPU
        computeGenerateTerrainB.SetInt("texturePixelsX", texturePixelsX);
        computeGenerateTerrainB.SetInt("texturePixelsY", texturePixelsY);
        computeGenerateTerrainB.SetInt("meshResolutionX", meshResolutionX);
        computeGenerateTerrainB.SetInt("meshResolutionZ", meshResolutionZ);
        computeGenerateTerrainB.SetFloat("xStart", xStart);
        computeGenerateTerrainB.SetFloat("xEnd", xEnd);
        computeGenerateTerrainB.SetFloat("zStart", zStart);
        computeGenerateTerrainB.SetFloat("zEnd", zEnd);
    }

    private void RecalculateMesh() {
        // Download Mesh from GPU
        
        VertexData[] vertexDataArray = new VertexData[terrainVertexDataCBuffer.count];
        terrainVertexDataCBuffer.GetData(vertexDataArray);
        TriangleIndexData[] triangleIndexDataArray = new TriangleIndexData[terrainTriangleIndexDataCBuffer.count];
        terrainTriangleIndexDataCBuffer.GetData(triangleIndexDataArray);

        terrainMesh = new Mesh();

        Vector3[] vertices = new Vector3[vertexDataArray.Length];
        int[] tris = new int[triangleIndexDataArray.Length * 3];
        Vector2[] uvs = new Vector2[vertexDataArray.Length];
        Vector3[] normals = new Vector3[vertexDataArray.Length];
        Color[] colors = new Color[vertexDataArray.Length];
        for (int i = 0; i < vertexDataArray.Length; i++) {
            vertices[i] = vertexDataArray[i].worldPos;
            normals[i] = vertexDataArray[i].normal;
            uvs[i] = vertexDataArray[i].uv;
            colors[i] = new Color(vertexDataArray[i].color.x, vertexDataArray[i].color.y, vertexDataArray[i].color.z);
            //Debug.Log(i.ToString() + " worldPos: " + vertexDataArray[i].worldPos.ToString());
        }
        for (int i = 0; i < triangleIndexDataArray.Length; i++) {
            tris[i * 3] = triangleIndexDataArray[i].v1;
            tris[i * 3 + 1] = triangleIndexDataArray[i].v2;
            tris[i * 3 + 2] = triangleIndexDataArray[i].v3;

            //Debug.Log(i.ToString() + " index0: " + triangleIndexDataArray[i].v1.ToString() + ", " + triangleIndexDataArray[i].v2.ToString() + ", " + triangleIndexDataArray[i].v3.ToString() + ", ");
        }
        Debug.Log(" numVerts: " + vertexDataArray.Length.ToString());
        Debug.Log(" numTris: " + triangleIndexDataArray.Length.ToString());
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uvs; //Unwrapping.GeneratePerTriangleUV(NewMesh);
        terrainMesh.triangles = tris;
        terrainMesh.normals = normals; //NewMesh.RecalculateNormals();
        terrainMesh.colors = colors;
        terrainMesh.RecalculateNormals();

        // Display Mesh (set as MeshFilter's Mesh)
        this.GetComponent<MeshFilter>().sharedMesh = terrainMesh;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnDestroy() {
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();
        if (terrainVertexDataCBuffer != null)
            terrainVertexDataCBuffer.Release();
        if (terrainTriangleIndexDataCBuffer != null)
            terrainTriangleIndexDataCBuffer.Release();
        if (mainHeightTexture != null)
            mainHeightTexture.Release();
        if (secondaryHeightTexture != null)
            secondaryHeightTexture.Release();

    }
}
