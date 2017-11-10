using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentRenderable : MonoBehaviour {

    // Using this class as hub for instanced meshes due to there only being one of them -- for the exhibition match

    public GameObject groundRenderable;
    public Material groundMaterial;  // will need to pool these eventually

    /*
     * -normal-mapped Quads as smallest pebbles - combine with nicer nml-map for sediment
	-Tiny Near-Camera Pebbles
		- fake rotation only
	-Scattered Small-To-Medium rocks, Inside Arena (no collision! Hug terrain!)
	-Scattered Larger Rocks (Vista Only)
	-Targeted Rock Zones (search terrain map for high-points and steep sloped, spawn some ‘parent’ rocks, then for each parent rock, instance a bunch of smaller rocks in the same area, inherit properties based on location on terrain)
-Rock Relief -- closely follow slope of terrain, scale way down in Y, way up in XZ -- create some detail silhouette within Arena
-RockRelief LOD - same thing but Vista-Only, low-detail
-Strata Rock Lines -- spawn at specific altitude in a line

    */
    public Material instancePebbleMaterial;  // nearby only
    public Mesh instancePebbleMesh;
    public int numPebblesSide = 256;    
    private ComputeBuffer instancedPebblesCBuffer;
    private ComputeBuffer indirectArgsPebblesCBuffer;
    private uint[] indirectArgsPebbles = new uint[5] { 0, 0, 0, 0, 0 };

    public Material instanceRockMaterial; // nearby only, but bigger -- only on RockMaterial
    public Mesh instanceRockMesh;
    public int numRocksSide = 128;
    private ComputeBuffer instancedRocksCBuffer;
    private ComputeBuffer instancedRocksInvMatrixCBuffer;
    private ComputeBuffer indirectArgsRocksCBuffer;
    private uint[] indirectArgsRocks = new uint[5] { 0, 0, 0, 0, 0 };

    public Material instanceRockReliefArenaMaterial; // nearby only, but bigger -- only on RockMaterial
    public Mesh instanceRockReliefArenaMesh;
    public int numRocksReliefArenaSide = 32;
    private ComputeBuffer instancedRocksReliefArenaCBuffer;
    private ComputeBuffer instancedRocksReliefArenaInvMatrixCBuffer;
    private ComputeBuffer indirectArgsRocksReliefArenaCBuffer;
    private uint[] indirectArgsRocksReliefArena = new uint[5] { 0, 0, 0, 0, 0 };

    public Material instanceRockCliffsMaterial; // nearby only, but bigger -- only on RockMaterial
    public Mesh instanceRockCliffsMesh;
    public int numRocksCliffsSide = 64;
    private ComputeBuffer instancedRocksCliffsMatrixCBuffer;
    private ComputeBuffer instancedRocksCliffsInvMatrixCBuffer;
    private ComputeBuffer indirectArgsRocksCliffsCBuffer;
    private uint[] indirectArgsRocksCliffs = new uint[5] { 0, 0, 0, 0, 0 };

    public Material vistaRockClusterMaterial;
    public Mesh vistaRockClusterMesh;
    // AppendBuffer?
    public int maxClustersPerSide = 32;
    public int numRockInstancesPerCluster = 16;
    private ComputeBuffer vistaRocksClusterMatrixCBuffer;    
    private ComputeBuffer vistaRocksClusterInvMatrixCBuffer;
    private ComputeBuffer argsVistaRocksClusterCBuffer;
    private uint[] argsVistaRocksCluster = new uint[5] { 0, 0, 0, 0, 0 };
    private ComputeBuffer appendRocksClusterCoreCBuffer; // just the origin of each rock cluster
    private ComputeBuffer indirectArgsRocksClusterCoreCBuffer;
    private uint[] indirectArgsRocksClusterCore = new uint[5] { 0, 0, 0, 0, 0 };
    //public Matrix4x4[] instancedPebblesMatrixArray;

    public Material obstacleRockMaterial;
    public Mesh obstacleRockMesh;
    public int numRockInstancesPerObstacle = 4;
    private ComputeBuffer obstacleRockMatrixCBuffer;
    private ComputeBuffer obstacleRockInvMatrixCBuffer;
    private ComputeBuffer argsObstacleRockCBuffer;
    private uint[] argsObstacleRock = new uint[5] { 0, 0, 0, 0, 0 };
    private ComputeBuffer obstacleDataCBuffer; // just the origin of each rock cluster
    //private ComputeBuffer indirectArgsRocksClusterCoreCBuffer;
    //private uint[] indirectArgsRocksClusterCore = new uint[5] { 0, 0, 0, 0, 0 };

    public struct TransformData {
        public Vector4 worldPos;
        public Vector3 scale;
        public Quaternion rotation;
    }

    public struct ObstacleData {
        public Vector3 worldPos;
        public Vector3 scale;
        public Vector3 eulerRotation;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        // Render:

        if (instancedPebblesCBuffer != null && instancePebbleMesh != null) {
            //UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            //Graphics.DrawMeshInstanced(instancePebbleMesh, 0, instancePebbleMaterial, instancedPebblesMatrixArray, instancedPebblesMatrixArray.Length, null, castShadows, true, 0, null);

            Graphics.DrawMeshInstancedIndirect(instancePebbleMesh, 0, instancePebbleMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsPebblesCBuffer, 0, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
        }

        if (instancedRocksCBuffer != null && instanceRockMesh != null) {
            UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            Graphics.DrawMeshInstancedIndirect(instanceRockMesh, 0, instanceRockMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsRocksCBuffer, 0, null, castShadows, true);
        }

        if (instancedRocksReliefArenaCBuffer != null && instanceRockReliefArenaMesh != null) {
            UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            Graphics.DrawMeshInstancedIndirect(instanceRockReliefArenaMesh, 0, instanceRockReliefArenaMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsRocksReliefArenaCBuffer, 0, null, castShadows, true);
        }

        if (instancedRocksCliffsMatrixCBuffer != null && instanceRockCliffsMesh != null) {
            UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.Off;
            Graphics.DrawMeshInstancedIndirect(instanceRockCliffsMesh, 0, instanceRockCliffsMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsRocksCliffsCBuffer, 0, null, castShadows, true);
        }

        if (vistaRocksClusterMatrixCBuffer != null && vistaRockClusterMesh != null) {
            UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            Graphics.DrawMeshInstancedIndirect(vistaRockClusterMesh, 0, vistaRockClusterMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), argsVistaRocksClusterCBuffer, 0, null, castShadows, true);
        }

        if (obstacleRockMatrixCBuffer != null && obstacleRockMesh != null) {
            UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            Graphics.DrawMeshInstancedIndirect(obstacleRockMesh, 0, obstacleRockMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), argsObstacleRockCBuffer, 0, null, castShadows, true);
        }
    }

    private void OnRenderObject() {
        
    }

    public void InitializeInstancedGeometry(EnvironmentGenome genome, Mesh instancePebbleMesh, Material instancePebbleMaterial, Mesh instanceRockMesh, Material instanceRockMaterial, Mesh instanceRockReliefArenaMesh, Material instanceRockReliefArenaMaterial, Mesh instanceRockCliffsMesh, Material instanceRockCliffsMaterial, Mesh vistaRockClusterMesh, Material vistaRockClusterMaterial, Mesh obstacleRockMesh, Material obstacleRockMaterial, ComputeShader instanceComputeShader) {
        this.instancePebbleMaterial = instancePebbleMaterial;
        this.instancePebbleMesh = instancePebbleMesh;
        this.instanceRockMaterial = instanceRockMaterial;
        this.instanceRockMesh = instanceRockMesh;
        this.instanceRockReliefArenaMaterial = instanceRockReliefArenaMaterial;
        this.instanceRockReliefArenaMesh = instanceRockReliefArenaMesh;
        this.instanceRockCliffsMaterial = instanceRockCliffsMaterial;
        this.instanceRockCliffsMesh = instanceRockCliffsMesh;
        this.vistaRockClusterMaterial = vistaRockClusterMaterial;
        this.vistaRockClusterMesh = vistaRockClusterMesh;
        this.obstacleRockMaterial = obstacleRockMaterial;
        this.obstacleRockMesh = obstacleRockMesh;

        // PEBBLES:
        #region Pebbles
        if (instancedPebblesCBuffer != null) {
            instancedPebblesCBuffer.Release();
        }
        instancedPebblesCBuffer = new ComputeBuffer(numPebblesSide * numPebblesSide, sizeof(float) * 11);

        this.instancePebbleMaterial.SetPass(0);
        this.instancePebbleMaterial.SetBuffer("instancedPebblesCBuffer", instancedPebblesCBuffer);

        instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
        instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
        instanceComputeShader.SetInt("_NumPebblesSide", numPebblesSide);
        int initializeInstancePebblesKernelID = instanceComputeShader.FindKernel("CSInitializeInstancePebblesData");
        instanceComputeShader.SetBuffer(initializeInstancePebblesKernelID, "instancedPebblesCBuffer", instancedPebblesCBuffer);
        instanceComputeShader.SetTexture(initializeInstancePebblesKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);        
        instanceComputeShader.Dispatch(initializeInstancePebblesKernelID, numPebblesSide / 32, numPebblesSide / 32, 1);
        //Debug.Log("numPebbles: " + instancedPebblesCBuffer.count.ToString());
        
        // Initialize positions and orientations and shit:  // all zeros for now
        // indirect args
        indirectArgsPebblesCBuffer = new ComputeBuffer(1, indirectArgsPebbles.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint numIndices = (instancePebbleMesh != null) ? (uint)instancePebbleMesh.GetIndexCount(0) : 0;
        indirectArgsPebbles[0] = numIndices;
        indirectArgsPebbles[1] = (uint)numPebblesSide * (uint)numPebblesSide;
        indirectArgsPebblesCBuffer.SetData(indirectArgsPebbles);
        #endregion

        // ROCKS:
        #region Rocks
        if (instancedRocksCBuffer != null) {
            instancedRocksCBuffer.Release();
        }
        instancedRocksCBuffer = new ComputeBuffer(numRocksSide * numRocksSide, sizeof(float) * 16);

        if (instancedRocksInvMatrixCBuffer != null) {
            instancedRocksInvMatrixCBuffer.Release();
        }
        instancedRocksInvMatrixCBuffer = new ComputeBuffer(numRocksSide * numRocksSide, sizeof(float) * 16);

        this.instanceRockMaterial.SetPass(0);
        this.instanceRockMaterial.SetBuffer("matricesCBuffer", instancedRocksCBuffer);
        this.instanceRockMaterial.SetBuffer("invMatricesCBuffer", instancedRocksInvMatrixCBuffer);

        instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
        instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
        instanceComputeShader.SetInt("_NumRocksSide", numRocksSide);
        int initializeInstanceRocksKernelID = instanceComputeShader.FindKernel("CSInitializeInstanceRocksData");
        instanceComputeShader.SetBuffer(initializeInstanceRocksKernelID, "instancedRocksMatricesCBuffer", instancedRocksCBuffer);
        instanceComputeShader.SetBuffer(initializeInstanceRocksKernelID, "instancedRocksInvMatrixCBuffer", instancedRocksInvMatrixCBuffer);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture0", TerrainConstructorGPU.heightMapCascadeTexturesRender[0]);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture1", TerrainConstructorGPU.heightMapCascadeTexturesRender[1]);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture2", TerrainConstructorGPU.heightMapCascadeTexturesRender[2]);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);
        instanceComputeShader.Dispatch(initializeInstanceRocksKernelID, numRocksSide / 32, numRocksSide / 32, 1);
        //Debug.Log("numRocks: " + instancedRocksCBuffer.count.ToString());
        // Initialize positions and orientations and shit:  // all zeros for now
        // indirect args
        indirectArgsRocksCBuffer = new ComputeBuffer(1, indirectArgsRocks.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint numIndicesRock = (instanceRockMesh != null) ? (uint)instanceRockMesh.GetIndexCount(0) : 0;
        indirectArgsRocks[0] = numIndicesRock;
        indirectArgsRocks[1] = (uint)numRocksSide * (uint)numRocksSide;
        indirectArgsRocksCBuffer.SetData(indirectArgsRocks);
        #endregion

        #region ReliefArenaRocks
        // RELIEF ARENA ROCKS: (hug Terrain)
        if (instancedRocksReliefArenaCBuffer != null) {
            instancedRocksReliefArenaCBuffer.Release();
        }
        instancedRocksReliefArenaCBuffer = new ComputeBuffer(numRocksReliefArenaSide * numRocksReliefArenaSide, sizeof(float) * 16);

        if (instancedRocksReliefArenaInvMatrixCBuffer != null) {
            instancedRocksReliefArenaInvMatrixCBuffer.Release();
        }
        instancedRocksReliefArenaInvMatrixCBuffer = new ComputeBuffer(numRocksReliefArenaSide * numRocksReliefArenaSide, sizeof(float) * 16);

        this.instanceRockReliefArenaMaterial.SetPass(0);
        this.instanceRockReliefArenaMaterial.SetBuffer("matricesCBuffer", instancedRocksReliefArenaCBuffer);
        this.instanceRockReliefArenaMaterial.SetBuffer("invMatricesCBuffer", instancedRocksReliefArenaInvMatrixCBuffer);

        instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
        instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
        instanceComputeShader.SetInt("_NumRocksReliefArenaSide", numRocksReliefArenaSide);
        int initializeInstanceRocksReliefArenaKernelID = instanceComputeShader.FindKernel("CSInitializeInstanceRocksReliefArenaData");
        instanceComputeShader.SetBuffer(initializeInstanceRocksReliefArenaKernelID, "instancedRocksReliefArenaMatricesCBuffer", instancedRocksReliefArenaCBuffer);
        instanceComputeShader.SetBuffer(initializeInstanceRocksReliefArenaKernelID, "instancedRocksReliefArenaInvMatrixCBuffer", instancedRocksReliefArenaInvMatrixCBuffer);
        instanceComputeShader.SetTexture(initializeInstanceRocksReliefArenaKernelID, "heightTexture0", TerrainConstructorGPU.heightMapCascadeTexturesRender[0]);
        instanceComputeShader.SetTexture(initializeInstanceRocksReliefArenaKernelID, "heightTexture1", TerrainConstructorGPU.heightMapCascadeTexturesRender[1]);
        instanceComputeShader.SetTexture(initializeInstanceRocksReliefArenaKernelID, "heightTexture2", TerrainConstructorGPU.heightMapCascadeTexturesRender[2]);
        instanceComputeShader.SetTexture(initializeInstanceRocksReliefArenaKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);
        instanceComputeShader.Dispatch(initializeInstanceRocksReliefArenaKernelID, numRocksReliefArenaSide / 32, numRocksReliefArenaSide / 32, 1);
        //Debug.Log("numRocks: " + instancedRocksCBuffer.count.ToString());
        //Matrix4x4[] matrixArray = new Matrix4x4[instancedRocksReliefArenaInvMatrixCBuffer.count];
        //instancedRocksReliefArenaInvMatrixCBuffer.GetData(matrixArray);
        //Debug.Log("instancedRocksCBuffer[0] " + matrixArray[0].ToString());
        // Initialize positions and orientations and shit:  // all zeros for now
        // indirect args
        indirectArgsRocksReliefArenaCBuffer = new ComputeBuffer(1, indirectArgsRocksReliefArena.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint numIndicesRockReliefArena = (instanceRockReliefArenaMesh != null) ? (uint)instanceRockReliefArenaMesh.GetIndexCount(0) : 0;
        indirectArgsRocksReliefArena[0] = numIndicesRockReliefArena;
        indirectArgsRocksReliefArena[1] = (uint)numRocksReliefArenaSide * (uint)numRocksReliefArenaSide;
        indirectArgsRocksReliefArenaCBuffer.SetData(indirectArgsRocksReliefArena);
        #endregion

        #region Cliffs Rocks
        // CLIFFS ROCKS: (hug Terrain)
        if (instancedRocksCliffsMatrixCBuffer != null) {
            instancedRocksCliffsMatrixCBuffer.Release();
        }
        instancedRocksCliffsMatrixCBuffer = new ComputeBuffer(numRocksCliffsSide * numRocksCliffsSide, sizeof(float) * 16);

        if (instancedRocksCliffsInvMatrixCBuffer != null) {
            instancedRocksCliffsInvMatrixCBuffer.Release();
        }
        instancedRocksCliffsInvMatrixCBuffer = new ComputeBuffer(numRocksCliffsSide * numRocksCliffsSide, sizeof(float) * 16);

        this.instanceRockCliffsMaterial.SetPass(0);
        this.instanceRockCliffsMaterial.SetBuffer("matricesCBuffer", instancedRocksCliffsMatrixCBuffer);
        this.instanceRockCliffsMaterial.SetBuffer("invMatricesCBuffer", instancedRocksCliffsInvMatrixCBuffer);

        instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
        instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
        instanceComputeShader.SetInt("_NumRocksCliffsSide", numRocksCliffsSide);
        int initializeInstanceRocksCliffsKernelID = instanceComputeShader.FindKernel("CSInitializeInstanceRocksCliffsData");
        instanceComputeShader.SetBuffer(initializeInstanceRocksCliffsKernelID, "instancedRocksCliffsMatrixCBuffer", instancedRocksCliffsMatrixCBuffer);
        instanceComputeShader.SetBuffer(initializeInstanceRocksCliffsKernelID, "instancedRocksCliffsInvMatrixCBuffer", instancedRocksCliffsInvMatrixCBuffer);
        instanceComputeShader.SetTexture(initializeInstanceRocksCliffsKernelID, "heightTexture0", TerrainConstructorGPU.heightMapCascadeTexturesRender[0]);
        instanceComputeShader.SetTexture(initializeInstanceRocksCliffsKernelID, "heightTexture1", TerrainConstructorGPU.heightMapCascadeTexturesRender[1]);
        instanceComputeShader.SetTexture(initializeInstanceRocksCliffsKernelID, "heightTexture2", TerrainConstructorGPU.heightMapCascadeTexturesRender[2]);
        instanceComputeShader.SetTexture(initializeInstanceRocksCliffsKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);
        instanceComputeShader.Dispatch(initializeInstanceRocksCliffsKernelID, numRocksCliffsSide / 32, numRocksCliffsSide / 32, 1);
        
        // indirect args
        indirectArgsRocksCliffsCBuffer = new ComputeBuffer(1, indirectArgsRocksCliffs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint numIndicesRockCliffs = (instanceRockCliffsMesh != null) ? (uint)instanceRockCliffsMesh.GetIndexCount(0) : 0;
        indirectArgsRocksCliffs[0] = numIndicesRockCliffs;
        indirectArgsRocksCliffs[1] = (uint)numRocksCliffsSide * (uint)numRocksCliffsSide;
        indirectArgsRocksCliffsCBuffer.SetData(indirectArgsRocksCliffs);
        #endregion


        #region VISTA ROCK CLUSTERS!!!! WOOOOOOOO
        // ROCKS CLUSTERS!!!!!!:
        if (appendRocksClusterCoreCBuffer != null)
            appendRocksClusterCoreCBuffer.Release();
        indirectArgsRocksClusterCoreCBuffer = new ComputeBuffer(1, indirectArgsRocksClusterCore.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        appendRocksClusterCoreCBuffer = new ComputeBuffer(maxClustersPerSide * maxClustersPerSide, sizeof(float) * 3, ComputeBufferType.Append);
        appendRocksClusterCoreCBuffer.SetCounterValue(0);

        
        // Set Shader Params:
        instanceComputeShader.SetInt("_MaxClustersPerSide", maxClustersPerSide);
        instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
        instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
        int generateRockClustersKernelID = instanceComputeShader.FindKernel("CSGenerateRockClusters");
        instanceComputeShader.SetTexture(generateRockClustersKernelID, "heightTexture0", TerrainConstructorGPU.heightMapCascadeTexturesRender[0]);
        instanceComputeShader.SetTexture(generateRockClustersKernelID, "heightTexture1", TerrainConstructorGPU.heightMapCascadeTexturesRender[1]);
        instanceComputeShader.SetTexture(generateRockClustersKernelID, "heightTexture2", TerrainConstructorGPU.heightMapCascadeTexturesRender[2]);
        instanceComputeShader.SetTexture(generateRockClustersKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);
        instanceComputeShader.SetBuffer(generateRockClustersKernelID, "appendRocksClusterCoreCBuffer", appendRocksClusterCoreCBuffer);
        // Search through height-map texture for suitable rock cluster locations:
        // DISPATCH HERE:
        instanceComputeShader.Dispatch(generateRockClustersKernelID, maxClustersPerSide / 32, maxClustersPerSide / 32, 1); // create all triangles from Neurons

        indirectArgsRocksClusterCore[0] = 0; // set later by counter;// 3;  // 3 vertices to start
        indirectArgsRocksClusterCore[1] = 1;  // 1 instance/copy
        indirectArgsRocksClusterCoreCBuffer.SetData(indirectArgsRocksClusterCore);
        ComputeBuffer.CopyCount(appendRocksClusterCoreCBuffer, indirectArgsRocksClusterCoreCBuffer, 0);
        indirectArgsRocksClusterCoreCBuffer.GetData(indirectArgsRocksClusterCore);
        int numRockClusters = (int)indirectArgsRocksClusterCore[0];
        Debug.Log("Rock Cluster Count: " + numRockClusters.ToString());

        // OK! now that we have cluster locations and data inside appendRocksClusterCoreCBuffer, create individual rock insatnce data based on those:
        if(numRockClusters > 0) {
            if (vistaRocksClusterMatrixCBuffer != null) {
                vistaRocksClusterMatrixCBuffer.Release();
            }
            vistaRocksClusterMatrixCBuffer = new ComputeBuffer(numRockClusters * numRockInstancesPerCluster, sizeof(float) * 16);

            if (vistaRocksClusterInvMatrixCBuffer != null) {
                vistaRocksClusterInvMatrixCBuffer.Release();
            }
            vistaRocksClusterInvMatrixCBuffer = new ComputeBuffer(numRockClusters * numRockInstancesPerCluster, sizeof(float) * 16);
            //CSGenerateVistaClusterRocks
            //_NumRockInstancesPerCluster

            instanceComputeShader.SetInt("_NumRockClusters", numRockClusters);
            instanceComputeShader.SetInt("_NumRockInstancesPerCluster", numRockInstancesPerCluster);
            this.vistaRockClusterMaterial.SetPass(0);
            this.vistaRockClusterMaterial.SetBuffer("matricesCBuffer", vistaRocksClusterMatrixCBuffer);
            this.vistaRockClusterMaterial.SetBuffer("invMatricesCBuffer", vistaRocksClusterInvMatrixCBuffer);

            instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
            instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
            int generateVistaClusterRocksKernelID = instanceComputeShader.FindKernel("CSGenerateVistaClusterRocks");
            instanceComputeShader.SetTexture(generateVistaClusterRocksKernelID, "heightTexture0", TerrainConstructorGPU.heightMapCascadeTexturesRender[0]);
            instanceComputeShader.SetTexture(generateVistaClusterRocksKernelID, "heightTexture1", TerrainConstructorGPU.heightMapCascadeTexturesRender[1]);
            instanceComputeShader.SetTexture(generateVistaClusterRocksKernelID, "heightTexture2", TerrainConstructorGPU.heightMapCascadeTexturesRender[2]);
            instanceComputeShader.SetTexture(generateVistaClusterRocksKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);
            instanceComputeShader.SetBuffer(generateVistaClusterRocksKernelID, "rocksClusterCoreCBuffer", appendRocksClusterCoreCBuffer);
            instanceComputeShader.SetBuffer(generateVistaClusterRocksKernelID, "vistaRocksClusterMatrixCBuffer", vistaRocksClusterMatrixCBuffer);
            instanceComputeShader.SetBuffer(generateVistaClusterRocksKernelID, "vistaRocksClusterInvMatrixCBuffer", vistaRocksClusterInvMatrixCBuffer);
            // Search through height-map texture for suitable rock cluster locations:
            // DISPATCH HERE:
            instanceComputeShader.Dispatch(generateVistaClusterRocksKernelID, numRockClusters, numRockInstancesPerCluster, 1); // create all triangles from Neurons

            // indirect args
            argsVistaRocksClusterCBuffer = new ComputeBuffer(1, argsVistaRocksCluster.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            uint numIndicesVistaRockCluster = (vistaRockClusterMesh != null) ? (uint)vistaRockClusterMesh.GetIndexCount(0) : 0;
            argsVistaRocksCluster[0] = numIndicesVistaRockCluster;
            argsVistaRocksCluster[1] = (uint)numRockClusters * (uint)numRockInstancesPerCluster;
            argsVistaRocksClusterCBuffer.SetData(argsVistaRocksCluster);
        }

        #endregion

        #region OBSTACLE ROCKS
        if(genome.useBasicObstacles) {
            // Process data from Obstacles Genome into GPU-friendly structure:
            if (obstacleDataCBuffer != null)
                obstacleDataCBuffer.Release();
            int numObstacles = genome.basicObstaclesGenome.numObstacles;
            obstacleDataCBuffer = new ComputeBuffer(numObstacles, sizeof(float) * 9);

            ObstacleData[] obstacleDataArray = new ObstacleData[numObstacles];
            for(int i = 0; i < numObstacles; i++) {

                /*
                obstacle.transform.parent = environmentGameplay.gameObject.transform;
                //float x = genome.basicObstaclesGenome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f;
                //float z = genome.basicObstaclesGenome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f;
                float x = genome.basicObstaclesGenome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f;
                float z = genome.basicObstaclesGenome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f;
                if (genome.useTargetColumn) {
                    float distToTarget = (new Vector2(environmentGameplay.targetColumn.transform.localPosition.x, environmentGameplay.targetColumn.transform.localPosition.z) - new Vector2(x, z)).magnitude;
                    if(distToTarget < genome.basicObstaclesGenome.obstacleScales[i] * 0.6f) {
                        obstacle.SetActive(false);
                    }
                }
                float y = TerrainConstructor.GetAltitude(genome, x, z) + 0.5f;
                obstacle.transform.localScale = new Vector3(genome.basicObstaclesGenome.obstacleScales[i], 1f, genome.basicObstaclesGenome.obstacleScales[i]);
                obstacle.transform.localPosition = new Vector3(x, y, z);                
                */

                ObstacleData data = new ObstacleData();

                Vector3 obstaclePos = new Vector3(genome.basicObstaclesGenome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f,
                                                  0f,
                                                  genome.basicObstaclesGenome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f);
                data.worldPos = obstaclePos;
                data.scale = Vector3.one * genome.basicObstaclesGenome.obstacleScales[i];
                data.eulerRotation = Vector3.zero;

                obstacleDataArray[i] = data;
            }
            obstacleDataCBuffer.SetData(obstacleDataArray);

            // Create and initialize Transform Matrices CBuffers:
            if (obstacleRockMatrixCBuffer != null) {
                obstacleRockMatrixCBuffer.Release();
            }
            obstacleRockMatrixCBuffer = new ComputeBuffer(numObstacles * numRockInstancesPerObstacle, sizeof(float) * 16);

            if (obstacleRockInvMatrixCBuffer != null) {
                obstacleRockInvMatrixCBuffer.Release();
            }
            obstacleRockInvMatrixCBuffer = new ComputeBuffer(numObstacles * numRockInstancesPerObstacle, sizeof(float) * 16);
                        
            // Pass XForm Matrix data to display shader so it can draw rock instances!
            this.obstacleRockMaterial.SetPass(0);
            this.obstacleRockMaterial.SetBuffer("matricesCBuffer", obstacleRockMatrixCBuffer);
            this.obstacleRockMaterial.SetBuffer("invMatricesCBuffer", obstacleRockInvMatrixCBuffer);

            //  Initialize ComputeShader with all the data it needs to generate individual instanced rock xForm Matrices:
            // Set Shader Params:
            instanceComputeShader.SetInt("_NumRockInstancesPerObstacle", numRockInstancesPerObstacle);
            instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
            instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
            int generateObstacleRocksKernelID = instanceComputeShader.FindKernel("CSGenerateObstacleRocks");
            // Only needs closest LOD level since obstacles are inside the Arena (or barely outside - sans collision)
            instanceComputeShader.SetTexture(generateObstacleRocksKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTexturesRender[3]);
            instanceComputeShader.SetBuffer(generateObstacleRocksKernelID, "obstacleDataCBuffer", obstacleDataCBuffer);
            instanceComputeShader.SetBuffer(generateObstacleRocksKernelID, "obstacleRockMatrixCBuffer", obstacleRockMatrixCBuffer);
            instanceComputeShader.SetBuffer(generateObstacleRocksKernelID, "obstacleRockInvMatrixCBuffer", obstacleRockInvMatrixCBuffer);

            // Generate Transform Matrices for all obstalce rock instances:
            instanceComputeShader.Dispatch(generateObstacleRocksKernelID, numObstacles * numRockInstancesPerObstacle, 1, 1); // create all triangles from Neurons

            // indirect args
            argsObstacleRockCBuffer = new ComputeBuffer(1, argsObstacleRock.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
            uint numIndicesObstacleRock = (obstacleRockMesh != null) ? (uint)obstacleRockMesh.GetIndexCount(0) : 0;
            argsObstacleRock[0] = numIndicesObstacleRock;
            argsObstacleRock[1] = (uint)numObstacles * (uint)numRockInstancesPerObstacle;
            argsObstacleRockCBuffer.SetData(argsObstacleRock);
        }
        


        #endregion
    }

    private void OnDestroy() {
        if (instancedPebblesCBuffer != null) {
            instancedPebblesCBuffer.Release();
        }
        if (indirectArgsPebblesCBuffer != null) {
            indirectArgsPebblesCBuffer.Release();
        }
        if (instancedRocksCBuffer != null) {
            instancedRocksCBuffer.Release();
        }
        if (indirectArgsRocksCBuffer != null) {
            indirectArgsRocksCBuffer.Release();
        }
        if (instancedRocksInvMatrixCBuffer != null) {
            instancedRocksInvMatrixCBuffer.Release();
        }
        if (instancedRocksReliefArenaCBuffer != null) {
            instancedRocksReliefArenaCBuffer.Release();
        }
        if (instancedRocksReliefArenaInvMatrixCBuffer != null) {
            instancedRocksReliefArenaInvMatrixCBuffer.Release();
        }
        if (indirectArgsRocksReliefArenaCBuffer != null) {
            indirectArgsRocksReliefArenaCBuffer.Release();
        }
        if (instancedRocksCliffsMatrixCBuffer != null) {
            instancedRocksCliffsMatrixCBuffer.Release();
        }
        if (instancedRocksCliffsInvMatrixCBuffer != null) {
            instancedRocksCliffsInvMatrixCBuffer.Release();
        }
        if (indirectArgsRocksCliffsCBuffer != null) {
            indirectArgsRocksCliffsCBuffer.Release();
        }
        // CLUSTERS!!!
        if (vistaRocksClusterMatrixCBuffer != null) {
            vistaRocksClusterMatrixCBuffer.Release();
        }
        if (vistaRocksClusterInvMatrixCBuffer != null) {
            vistaRocksClusterInvMatrixCBuffer.Release();
        }
        if (argsVistaRocksClusterCBuffer != null) {
            argsVistaRocksClusterCBuffer.Release();
        }
        if (appendRocksClusterCoreCBuffer != null) {
            appendRocksClusterCoreCBuffer.Release();
        }
        if (indirectArgsRocksClusterCoreCBuffer != null) {
            indirectArgsRocksClusterCoreCBuffer.Release();
        }
        if (argsObstacleRockCBuffer != null) {
            argsObstacleRockCBuffer.Release();
        }
        if (obstacleRockMatrixCBuffer != null) {
            obstacleRockMatrixCBuffer.Release();
        }
        if (obstacleRockInvMatrixCBuffer != null) {
            obstacleRockInvMatrixCBuffer.Release();
        }
        if (obstacleDataCBuffer != null) {
            obstacleDataCBuffer.Release();
        }
    }
}
