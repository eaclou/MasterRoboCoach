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

        instanceMatricesArray = new Matrix4x4[16];
        for(int x = 0; x < 4; x++) {
            for(int y = 0; y < 4; y++) {
                instanceMatricesArray[y * 4 + x] = Matrix4x4.TRS(new Vector3(x * 2f, y * 2f, 0f), Quaternion.identity, Vector3.one);
            }
        }
        
    }
	
	// Update is called once per frame
	void Update () {
        UnityEngine.Rendering.ShadowCastingMode castShadows = UnityEngine.Rendering.ShadowCastingMode.On;
        Graphics.DrawMeshInstanced(instanceMesh, 0, instanceMaterial, instanceMatricesArray, instanceMatricesArray.Length, null, castShadows, true, 0, null);
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
        
        //Graphics.DrawMeshInstancedIndirect(instanceMesh, 0, instanceMaterial,);
    }

    void OnDestroy() {
        //Debug.Log("OnDestroy()");
        computeBuffer.Release();
        //computeBuffer.Dispose();
        timeBuffer.Release();
    }
}
