using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateTerrainA : MonoBehaviour {

    public ComputeShader terrainComputeShader;
    private ComputeBuffer terrainGenomeCBuffer;
    private ComputeBuffer terrainVertexDataCBuffer;
    private ComputeBuffer terrainVertexDataSwapCBuffer;
    private ComputeBuffer terrainTriangleIndexDataCBuffer;

    private RenderTexture heightMapTextureA;
    private RenderTexture heightMapTextureB;

    private Mesh terrainMesh;

    public int resolutionX = 8;
    public int resolutionZ = 8;
    public int numNoiseOctaves = 4;
    public int numFilterIterations = 10;

    public float baseAmplitude = 1f;
    public float baseFrequency = 1f;
    public Vector3 baseOffset = new Vector3(0f, 0f, 0f);

    public float xStart = 0f;
    public float xEnd = 8f;
    public float zStart = 0f;
    public float zEnd = 8f;

    private TerrainGenome terrainGenome;

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

        // InitializeBuffers
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();
        terrainGenomeCBuffer = new ComputeBuffer(numNoiseOctaves, sizeof(float) * 5);
        GenomeNoiseOctaveData[] genomeNoiseOctaveDataArray = new GenomeNoiseOctaveData[numNoiseOctaves];
        for(int i = 0; i < genomeNoiseOctaveDataArray.Length; i++) {
            GenomeNoiseOctaveData genomeNoiseOctaveData;
            genomeNoiseOctaveData.amplitude = baseAmplitude / Mathf.Pow(2, i);
            genomeNoiseOctaveData.frequency = baseFrequency * Mathf.Pow(2, i);
            genomeNoiseOctaveData.offset = new Vector3(0f, 0f, 0f) + baseOffset;
            genomeNoiseOctaveDataArray[i] = genomeNoiseOctaveData;
        }
        terrainGenomeCBuffer.SetData(genomeNoiseOctaveDataArray);

        // TEXTURES:
        //heightMapTextureA = new RenderTexture(resolutionX + 1, resolutionZ + 1, 1, RenderTextureFormat.Default);
       // heightMapTextureB = new RenderTexture(resolutionX + 1, resolutionZ + 1, 1, RenderTextureFormat.Default);

        if (terrainVertexDataCBuffer != null)
            terrainVertexDataCBuffer.Release();
        terrainVertexDataCBuffer = new ComputeBuffer((resolutionX + 1) * (resolutionZ + 1), sizeof(float) * 11);
        if (terrainVertexDataSwapCBuffer != null)
            terrainVertexDataSwapCBuffer.Release();
        terrainVertexDataSwapCBuffer = new ComputeBuffer((resolutionX + 1) * (resolutionZ + 1), sizeof(float) * 11);

        if (terrainTriangleIndexDataCBuffer != null)
            terrainTriangleIndexDataCBuffer.Release();
        terrainTriangleIndexDataCBuffer = new ComputeBuffer(resolutionX * resolutionZ * 2, sizeof(int) * 3);


        // Upload TerrainGenome to GPU
        terrainComputeShader.SetInt("resolutionX", resolutionX);
        terrainComputeShader.SetInt("resolutionZ", resolutionZ);
        terrainComputeShader.SetFloat("xStart", xStart);
        terrainComputeShader.SetFloat("xEnd", xEnd);
        terrainComputeShader.SetFloat("zStart", zStart);
        terrainComputeShader.SetFloat("zEnd", zEnd);

        int vertexDataKernelID = terrainComputeShader.FindKernel("CSGenerateVertexData");        
        terrainComputeShader.SetBuffer(vertexDataKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        terrainComputeShader.SetBuffer(vertexDataKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(vertexDataKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
        //terrainComputeShader.SetTexture(vertexDataKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(vertexDataKernelID, "heightMapTextureB", heightMapTextureB);

        int simulateCAKernelID = terrainComputeShader.FindKernel("CSSimulateCA");
        terrainComputeShader.SetBuffer(simulateCAKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        terrainComputeShader.SetBuffer(simulateCAKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(simulateCAKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
        //terrainComputeShader.SetTexture(simulateCAKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(simulateCAKernelID, "heightMapTextureB", heightMapTextureB);
        int baseToSwapKernelID = terrainComputeShader.FindKernel("CSCopyFromBaseToSwap");
        terrainComputeShader.SetBuffer(baseToSwapKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(baseToSwapKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
        //terrainComputeShader.SetTexture(baseToSwapKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(baseToSwapKernelID, "heightMapTextureB", heightMapTextureB);
        int swapToBaseKernelID = terrainComputeShader.FindKernel("CSCopyFromSwapToBase");
        terrainComputeShader.SetBuffer(swapToBaseKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(swapToBaseKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
        //terrainComputeShader.SetTexture(swapToBaseKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(swapToBaseKernelID, "heightMapTextureB", heightMapTextureB);

        // Or should I just do this on the CPU??? probably faster?? .... optimize/profile later on...
        int triangleIndicesKernelID = terrainComputeShader.FindKernel("CSGenerateTriangleIndices");
        terrainComputeShader.SetBuffer(triangleIndicesKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        terrainComputeShader.SetBuffer(triangleIndicesKernelID, "terrainTriangleIndexDataCBuffer", terrainTriangleIndexDataCBuffer);

        // Calculate Terrain Mesh, Dispatch
        terrainComputeShader.Dispatch(vertexDataKernelID, resolutionX + 1, 1, resolutionZ + 1);        
        for(int i = 0; i < numFilterIterations; i++) {
            terrainComputeShader.Dispatch(baseToSwapKernelID, resolutionX + 1, 1, resolutionZ + 1);  // initialize swap buffer as copy of original
            terrainComputeShader.Dispatch(simulateCAKernelID, resolutionX + 1, 1, resolutionZ + 1);  // read from original buffer, write modified values to swap buffer
           terrainComputeShader.Dispatch(swapToBaseKernelID, resolutionX + 1, 1, resolutionZ + 1);  // copy new values in swap buffer back to original to be read
        }        
        terrainComputeShader.Dispatch(triangleIndicesKernelID, resolutionX, 1, resolutionZ);

        // Download Mesh from GPU
        VertexData[] vertexDataArray = new VertexData[terrainVertexDataCBuffer.count];
        terrainVertexDataCBuffer.GetData(vertexDataArray);
        TriangleIndexData[] triangleIndexDataArray = new TriangleIndexData[terrainTriangleIndexDataCBuffer.count];
        terrainTriangleIndexDataCBuffer.GetData(triangleIndexDataArray);

        // Process Mesh on CPU
        //for (int x = 0; x < resolutionX; x++) {
        //    for(int z = 0; z < resolutionZ; z++) {
        //        int index = x * resolutionZ + z;
        //        Debug.Log(index.ToString() + "   [" + x.ToString() + "," + z.ToString() + "] pos: " + vertexDataArray[index].worldPos.ToString()); 
        //    }
        //}
        //for(int i = 0; i < vertexDataArray.Length; i++) {
        //    Debug.Log(i.ToString() + " pos: " + vertexDataArray[i].worldPos.ToString());
        //}
        //for (int i = 0; i < triangleIndexDataArray.Length; i++) {
        //    Debug.Log(i.ToString() + " index0: " + triangleIndexDataArray[i].v1.ToString() + ", " + triangleIndexDataArray[i].v2.ToString() + ", " + triangleIndexDataArray[i].v3.ToString() + ", ");
        //}

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
            //Debug.Log(i.ToString() + " pos: " + vertexDataArray[i].worldPos.ToString());
        }
        for (int i = 0; i < triangleIndexDataArray.Length; i++) {
            tris[i * 3] = triangleIndexDataArray[i].v1;
            tris[i * 3 + 1] = triangleIndexDataArray[i].v2;
            tris[i * 3 + 2] = triangleIndexDataArray[i].v3;

            //Debug.Log(i.ToString() + " index0: " + triangleIndexDataArray[i].v1.ToString() + ", " + triangleIndexDataArray[i].v2.ToString() + ", " + triangleIndexDataArray[i].v3.ToString() + ", ");
        }
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

    public void IterateTimes5() {
        IterateHeightField();
        IterateHeightField();
        IterateHeightField();
        IterateHeightField();
        IterateHeightField();
    }

    public void IterateHeightField() {
        Debug.Log("IterateHeightField");
        int simulateCAKernelID = terrainComputeShader.FindKernel("CSSimulateCA");
        terrainComputeShader.SetBuffer(simulateCAKernelID, "terrainGenomeCBuffer", terrainGenomeCBuffer);
        terrainComputeShader.SetBuffer(simulateCAKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(simulateCAKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
        //terrainComputeShader.SetTexture(simulateCAKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(simulateCAKernelID, "heightMapTextureB", heightMapTextureB);
        int baseToSwapKernelID = terrainComputeShader.FindKernel("CSCopyFromBaseToSwap");
        terrainComputeShader.SetBuffer(baseToSwapKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(baseToSwapKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
        //terrainComputeShader.SetTexture(baseToSwapKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(baseToSwapKernelID, "heightMapTextureB", heightMapTextureB);
        int swapToBaseKernelID = terrainComputeShader.FindKernel("CSCopyFromSwapToBase");
        terrainComputeShader.SetBuffer(swapToBaseKernelID, "terrainVertexDataCBuffer", terrainVertexDataCBuffer);
        terrainComputeShader.SetBuffer(swapToBaseKernelID, "terrainVertexDataSwapCBuffer", terrainVertexDataSwapCBuffer);
       // terrainComputeShader.SetTexture(swapToBaseKernelID, "heightMapTextureA", heightMapTextureA);
        //terrainComputeShader.SetTexture(swapToBaseKernelID, "heightMapTextureB", heightMapTextureB);
        
        terrainComputeShader.Dispatch(baseToSwapKernelID, resolutionX + 1, 1, resolutionZ + 1);  // initialize swap buffer as copy of original
        terrainComputeShader.Dispatch(simulateCAKernelID, resolutionX - 1, 1, resolutionZ - 1);  // read from original buffer, write modified values to swap buffer
        terrainComputeShader.Dispatch(swapToBaseKernelID, resolutionX + 1, 1, resolutionZ + 1);  // copy new values in swap buffer back to original to be read


        VertexData[] vertexDataArray = new VertexData[terrainVertexDataCBuffer.count];
        terrainVertexDataCBuffer.GetData(vertexDataArray);
        TriangleIndexData[] triangleIndexDataArray = new TriangleIndexData[terrainTriangleIndexDataCBuffer.count];
        terrainTriangleIndexDataCBuffer.GetData(triangleIndexDataArray);

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
            //Debug.Log(i.ToString() + " pos: " + vertexDataArray[i].worldPos.ToString());
        }
        for (int i = 0; i < triangleIndexDataArray.Length; i++) {
            tris[i * 3] = triangleIndexDataArray[i].v1;
            tris[i * 3 + 1] = triangleIndexDataArray[i].v2;
            tris[i * 3 + 2] = triangleIndexDataArray[i].v3;

            //Debug.Log(i.ToString() + " index0: " + triangleIndexDataArray[i].v1.ToString() + ", " + triangleIndexDataArray[i].v2.ToString() + ", " + triangleIndexDataArray[i].v3.ToString() + ", ");
        }
        terrainMesh.vertices = vertices;
        terrainMesh.uv = uvs; //Unwrapping.GeneratePerTriangleUV(NewMesh);
        terrainMesh.triangles = tris;
        terrainMesh.normals = normals; //NewMesh.RecalculateNormals();
        terrainMesh.colors = colors;
        terrainMesh.RecalculateNormals();

        // Display Mesh (set as MeshFilter's Mesh)
        //this.GetComponent<MeshFilter>().sharedMesh = terrainMesh;
    }

    private void OnDestroy() {
        if (terrainGenomeCBuffer != null)
            terrainGenomeCBuffer.Release();
        if (terrainVertexDataCBuffer != null)
            terrainVertexDataCBuffer.Release();
        if (terrainVertexDataSwapCBuffer != null)
            terrainVertexDataSwapCBuffer.Release();
        if (terrainTriangleIndexDataCBuffer != null)
            terrainTriangleIndexDataCBuffer.Release();
        
        
    }
}
