using UnityEngine;
using System.Collections;

public class TestComputeSharedBufferA : MonoBehaviour {
    
    public ComputeShader computeShaderA;
    public int numPoints = 64;
    public ComputeBuffer computeBuffer;
    public ComputeBuffer timeBuffer;
    public Shader pointShaderA;   
    private Material material;
    int kernelID;

    public Matrix4x4[] instanceMatricesArray;
    public Mesh instanceMesh;
    public Material instanceMaterial;

    public int indirectInstanceCount = 64;
    public Mesh indirectInstanceMesh;
    public Material indirectInstanceMaterial;

    private int cachedIndirectInstanceCount = -1;
    private ComputeBuffer indirectPositionBuffer;
    private ComputeBuffer indirectArgsBuffer;
    private uint[] indirectArgs = new uint[5] { 0, 0, 0, 0, 0 };

    float[] timeArray;

    

    void InitializeBuffers() {
        computeBuffer = new ComputeBuffer(numPoints, 12);
        computeShaderA.SetBuffer(kernelID, "outputBuffer", computeBuffer);
        timeBuffer = new ComputeBuffer(1, 4);
        timeArray = new float[1];
        material.SetBuffer("buf_Points", computeBuffer);        
    }

    // Use this for initialization
    void Start () {        
        kernelID = computeShaderA.FindKernel("CSMainGrid");
        material = new Material(pointShaderA);
        InitializeBuffers();

        instanceMatricesArray = new Matrix4x4[512];
        for(int x = 0; x < 8; x++) {
            for(int y = 0; y < 8; y++) {
                for(int z = 0; z < 8; z++) {
                    instanceMatricesArray[x * (8 * 8) + (y * 8) + z] = Matrix4x4.TRS(new Vector3(x * 0.4f, y * 0.4f, z * 0.4f), UnityEngine.Random.rotation, Vector3.one * 0.15f);
                }                
            }
        }

        // https://forum.unity.com/threads/drawmeshinstancedindirect-example-comments-and-questions.446080/
        // https://github.com/tiiago11/Unity-InstancedIndirectExamples
        // INDIRECT !!!:::::
        indirectArgsBuffer = new ComputeBuffer(1, indirectArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }
    	
	// Update is called once per frame
	void Update () {
        UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
        Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, instanceMatricesArray, instanceMatricesArray.Length, null, castShadows, true, 0, null);

        // INDIRECT !!!:::::
        // Update starting position buffer
        if (cachedIndirectInstanceCount != indirectInstanceCount)
            UpdateBuffers();

        // Pad Input
        //if (Input.GetAxisRaw("Horizontal") != 0.0f)
        //    indirectInstanceCount = (int)Mathf.Clamp(indirectInstanceCount + Input.GetAxis("Horizontal") * 40, 1.0f, 50000.0f);

        //Render:
        Graphics.DrawMeshInstancedIndirect(indirectInstanceMesh, 0, indirectInstanceMaterial, new Bounds(Vector3.zero, new Vector3(100f, 100f, 100f)), indirectArgsBuffer);
    }

    private void OnGUI() {
        // INDIRECT:::::
        GUI.Label(new Rect(265, 25, 200, 30), "Indirect Instance Count: " + indirectInstanceCount.ToString());
        indirectInstanceCount = (int)GUI.HorizontalSlider(new Rect(25, 20, 200, 30), (float)indirectInstanceCount, 1.0f, 50000.0f);

    }

    void UpdateBuffers() {
        Debug.Log("UpdateBuffers!");
        if (indirectInstanceCount < 1) indirectInstanceCount = 1;
        // positions
        if (indirectPositionBuffer != null)
            indirectPositionBuffer.Release();
        indirectPositionBuffer = new ComputeBuffer(indirectInstanceCount, 16);
        Vector4[] positions = new Vector4[indirectInstanceCount];
        for (int i = 0; i < indirectInstanceCount; i++) {
            float angle = Random.Range(0.0f, Mathf.PI * 2.0f);
            float distance = Random.Range(2f, 20.0f);
            float height = Random.Range(-2.0f, 2.0f);
            float size = Random.Range(0.05f, 0.25f);
            positions[i] = new Vector4(Mathf.Sin(angle) * distance, height, Mathf.Cos(angle) * distance, size);
        }
        indirectPositionBuffer.SetData(positions);
        indirectInstanceMaterial.SetBuffer("positionBuffer", indirectPositionBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        indirectArgs[0] = numIndices;
        indirectArgs[1] = (uint)indirectInstanceCount;
        indirectArgsBuffer.SetData(indirectArgs);

        cachedIndirectInstanceCount = indirectInstanceCount;
    }

    void OnRenderObject() {
        timeArray[0] = Time.fixedTime;
        timeBuffer.SetData(timeArray);
        computeShaderA.Dispatch(kernelID, 1, 1, 1);
        material.SetPass(0);
        material.SetBuffer("buf_Time", timeBuffer);
        Graphics.DrawProcedural(MeshTopology.Points, computeBuffer.count);
        //Debug.Log("DrawProcedural! " + material.GetVector("_Size").ToString());

        //Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, instanceMatricesArray, 16, null, UnityEngine.Rendering.ShadowCastingMode.On, true);
        //UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
        //Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, instanceMatricesArray, instanceMatricesArray.Length, null, castShadows, true, 0, null);

        //Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial,);
    }

    void OnDisable() {

        if (indirectPositionBuffer != null)
            indirectPositionBuffer.Release();
        indirectPositionBuffer = null;

        if (indirectArgsBuffer != null)
            indirectArgsBuffer.Release();
        indirectArgsBuffer = null;
    }

    void OnDestroy() {
        //Debug.Log("OnDestroy()");
        computeBuffer.Release();
        //computeBuffer.Dispose();
        timeBuffer.Release();
    }
}
