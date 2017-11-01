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

    public static RenderTexture[] heightMapCascadeTextures;

    //public static Texture2D[] presetNoiseTextures;
    //public static RenderTexture[] generatedPaletteNoiseTextures;

    public struct TriangleIndexData {
        public int v1;
        public int v2;
        public int v3;
    }

    /*public struct GenomeNoiseOctaveData {
        public Vector3 amplitude;
        public Vector3 frequency;
        public Vector3 offset;
        public float rotation;
        public float use_ridged_noise;
    }*/


    public static Mesh GetTerrainMesh(EnvironmentGenome genome, int xResolution, int zResolution, float xCenter, float zCenter, float xSize, float zSize) {

        
        if (terrainConstructorGPUCompute == null) {
            terrainConstructorGPUCompute = new ComputeShader();
        }
        //if (mainRenderTexture == null) {
            //mainRenderTexture.Release();
       /* mainRenderTexture = new RenderTexture(xResolution, zResolution, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
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
        //}*/
                
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
        //terrainConstructorGPUCompute.SetFloat("xStart", xCenter - offset.x);
        //terrainConstructorGPUCompute.SetFloat("xEnd", xCenter + offset.x);
        //terrainConstructorGPUCompute.SetFloat("zStart", zCenter - offset.z);
        //terrainConstructorGPUCompute.SetFloat("zEnd", zCenter + offset.z);



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
}
