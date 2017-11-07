using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentRenderable : MonoBehaviour {

    // Using this class as hub for instanced meshes due to there only being one of them -- for the exhibition match

    public GameObject groundRenderable;
    public Material groundMaterial;  // will need to pool these eventually
    

    public Material instancePebbleMaterial;
    public Mesh instancePebbleMesh;
    public int numPebblesSide = 128;    
    private ComputeBuffer instancedPebblesCBuffer;
    private ComputeBuffer indirectArgsPebblesCBuffer;
    private uint[] indirectArgsPebbles = new uint[5] { 0, 0, 0, 0, 0 };

    public Material instanceRockMaterial;
    public Mesh instanceRockMesh;
    public int numRocksSide = 512;
    private ComputeBuffer instancedRocksCBuffer;
    private ComputeBuffer instancedRocksInvMatrixCBuffer;
    private ComputeBuffer indirectArgsRocksCBuffer;
    private uint[] indirectArgsRocks = new uint[5] { 0, 0, 0, 0, 0 };


    //public Matrix4x4[] instancedPebblesMatrixArray;

    public struct TransformData {
        public Vector4 worldPos;
        public Vector3 scale;
        public Quaternion rotation;
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

            Graphics.DrawMeshInstancedIndirect(instancePebbleMesh, 0, instancePebbleMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsPebblesCBuffer, 0, null, UnityEngine.Rendering.ShadowCastingMode.Off, true);
        }

        if (instancedRocksCBuffer != null && instanceRockMesh != null) {
            UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
            Graphics.DrawMeshInstancedIndirect(instanceRockMesh, 0, instanceRockMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsRocksCBuffer, 0, null, castShadows, true);
        }
    }

    private void OnRenderObject() {
        
    }

    public void InitializeInstancedGeometry(Mesh instancePebbleMesh, Material instancePebbleMaterial, Mesh instanceRockMesh, Material instanceRockMaterial, ComputeShader instanceComputeShader) {
        this.instancePebbleMaterial = instancePebbleMaterial;
        this.instancePebbleMesh = instancePebbleMesh;
        this.instanceRockMaterial = instanceRockMaterial;
        this.instanceRockMesh = instanceRockMesh;

        // PEBBLES:
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
        instanceComputeShader.SetTexture(initializeInstancePebblesKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTextures[3]);        
        instanceComputeShader.Dispatch(initializeInstancePebblesKernelID, numPebblesSide / 32, numPebblesSide / 32, 1);
        //Debug.Log("numPebbles: " + instancedPebblesCBuffer.count.ToString());
        
        // Initialize positions and orientations and shit:  // all zeros for now
        // indirect args
        indirectArgsPebblesCBuffer = new ComputeBuffer(1, indirectArgsPebbles.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint numIndices = (instancePebbleMesh != null) ? (uint)instancePebbleMesh.GetIndexCount(0) : 0;
        indirectArgsPebbles[0] = numIndices;
        indirectArgsPebbles[1] = (uint)numPebblesSide * (uint)numPebblesSide;
        indirectArgsPebblesCBuffer.SetData(indirectArgsPebbles);


        // ROCKS:
        if (instancedRocksCBuffer != null) {
            instancedRocksCBuffer.Release();
        }
        instancedRocksCBuffer = new ComputeBuffer(numRocksSide * numRocksSide, sizeof(float) * 11);

        if (instancedRocksInvMatrixCBuffer != null) {
            instancedRocksInvMatrixCBuffer.Release();
        }
        instancedRocksInvMatrixCBuffer = new ComputeBuffer(numRocksSide * numRocksSide, sizeof(float) * 16);

        this.instanceRockMaterial.SetPass(0);
        this.instanceRockMaterial.SetBuffer("instancedRocksCBuffer", instancedRocksCBuffer);
        this.instanceRockMaterial.SetBuffer("instancedRocksInvMatrixCBuffer", instancedRocksInvMatrixCBuffer);

        instanceComputeShader.SetVector("_QuadBounds", new Vector4(-85f, 85f, -85f, 85f));  // worldspace size of smallest heightTexture Region
        instanceComputeShader.SetVector("_GlobalBounds", new Vector4(-680f, 680f, -680f, 680f));
        instanceComputeShader.SetInt("_NumRocksSide", numRocksSide);
        int initializeInstanceRocksKernelID = instanceComputeShader.FindKernel("CSInitializeInstanceRocksData");
        instanceComputeShader.SetBuffer(initializeInstanceRocksKernelID, "instancedRocksCBuffer", instancedRocksCBuffer);
        instanceComputeShader.SetBuffer(initializeInstanceRocksKernelID, "instancedRocksInvMatrixCBuffer", instancedRocksInvMatrixCBuffer);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture0", TerrainConstructorGPU.heightMapCascadeTextures[0]);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture1", TerrainConstructorGPU.heightMapCascadeTextures[1]);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture2", TerrainConstructorGPU.heightMapCascadeTextures[2]);
        instanceComputeShader.SetTexture(initializeInstanceRocksKernelID, "heightTexture3", TerrainConstructorGPU.heightMapCascadeTextures[3]);
        instanceComputeShader.Dispatch(initializeInstanceRocksKernelID, numRocksSide / 32, numRocksSide / 32, 1);
        //Debug.Log("numRocks: " + instancedRocksCBuffer.count.ToString());

        // Initialize positions and orientations and shit:  // all zeros for now
        // indirect args
        indirectArgsRocksCBuffer = new ComputeBuffer(1, indirectArgsRocks.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint numIndicesRock = (instanceRockMesh != null) ? (uint)instanceRockMesh.GetIndexCount(0) : 0;
        indirectArgsRocks[0] = numIndicesRock;
        indirectArgsRocks[1] = (uint)numRocksSide * (uint)numRocksSide;
        indirectArgsRocksCBuffer.SetData(indirectArgsRocks);


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
    }
}
