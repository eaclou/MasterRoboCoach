using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDrawProceduralIndirectA : MonoBehaviour {

    public ComputeShader shaderComputeDrawProceduralIndirectA;
    public Shader shaderDisplayDrawProceduralIndirectA;

    private ComputeBuffer sourceDataCBuffer;
    private ComputeBuffer trianglesCBuffer;
    private ComputeBuffer argsCBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private Material displayMaterial;

    struct Triangle {
        Vector3 vertA;
        Vector3 vertB;
        Vector3 vertC;
    }

    // Use this for initialization
    void Start () {

        argsCBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    void UpdateBuffers() {
        Debug.Log("UpdateBuffers!");

        int numSources = 1;
        // SOURCE DATA:::::
        if (sourceDataCBuffer != null)
            sourceDataCBuffer.Release();
        sourceDataCBuffer = new ComputeBuffer(numSources, sizeof(float) * 3);

        Vector3[] sourceDataArray = new Vector3[numSources]; // for now only one seed data
        for(int i = 0; i < sourceDataArray.Length; i++) {
            sourceDataArray[i] = new Vector3(UnityEngine.Random.value * i * 0.15f, i * UnityEngine.Random.value * 0.25f, i * 0.15f + UnityEngine.Random.value * 0.25f);
        }
        sourceDataCBuffer.SetData(sourceDataArray);  // set sourceDataBuffer to have one point centered at 1,1,1

        // !@#$!@#$!@$#!@$#!@$#!@3
        // Verts are being added asynchronously, and are degenerate in most cases, due to being co-linear
        // Either go pre-determined route by estimating vert counts/indices (by making assumptions about geo)
        //    for example knowing resolution of cylinder object being built
        // OR:
        //    Rather than structuring append by Vertex, 
        //    Structure it by Triangle, by adding all 3 verts simultaneously



        // @!$#!@#$%!@%!#$%!#$%!#$%

        // SET UP GEO BUFFER and REFS:::::
        if (trianglesCBuffer != null)
            trianglesCBuffer.Release();
        trianglesCBuffer = new ComputeBuffer(numSources, sizeof(float) * 3 * 3, ComputeBufferType.Append); // vector3 position * 3 verts
        trianglesCBuffer.SetCounterValue(0);

        int kernelID = shaderComputeDrawProceduralIndirectA.FindKernel("CSMain");
        shaderComputeDrawProceduralIndirectA.SetBuffer(kernelID, "sourceDataBuffer", sourceDataCBuffer);
        shaderComputeDrawProceduralIndirectA.SetBuffer(kernelID, "appendTrianglesBuffer", trianglesCBuffer);// link computeBuffer to both computeShader and displayShader so they share the same one!!
        displayMaterial = new Material(shaderDisplayDrawProceduralIndirectA);
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", trianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!
        shaderComputeDrawProceduralIndirectA.Dispatch(kernelID, 1, 1, 1); // Generate geometry data!

        // Either figure out how many triangles there are in advance, or use appendBuffer

        args[0] = 0; // set later by counter;// 3;  // 3 vertices to start
        args[1] = 1;  // 1 instance/copy
        argsCBuffer.SetData(args);
        ComputeBuffer.CopyCount(trianglesCBuffer, argsCBuffer, 0);        
        argsCBuffer.GetData(args);
        //args[0] = args[0] * 3;  // each index contains 3 verts
        //argsCBuffer.SetData(args);

        Debug.Log("triangle count " + args[0]);

        //Vector3[] readout = new Vector3[args[0]];
        //trianglesCBuffer.GetData(readout);
        //for (int i = 0; i < readout.Length; i++) {
        //    Debug.Log(i.ToString() + ": " + readout[i].ToString());
        //}

        /*indirectInstanceMaterial.SetBuffer("positionBuffer", indirectPositionBuffer);

        // indirect args
        uint numIndices = (instanceMesh != null) ? (uint)instanceMesh.GetIndexCount(0) : 0;
        indirectArgs[0] = numIndices;
        indirectArgs[1] = (uint)indirectInstanceCount;
        indirectArgsBuffer.SetData(indirectArgs);

        cachedIndirectInstanceCount = indirectInstanceCount;*/
    }

    private void OnRenderObject() {

        displayMaterial.SetPass(0);
        //displayMaterial.SetBuffer("positionsBuffer", vertexPositionsCBuffer);

        //material.SetPass(0);
        //material.SetBuffer("buffer", buffer);
        //Graphics.DrawProcedural(MeshTopology.Triangles, 3, 1);  // COUNTER-CLOCKWISE WINDING!!!

        //Vector3[] readout = new Vector3[3];
        //vertexPositionsCBuffer.GetData(readout);
        //Debug.Log(readout[0].ToString() + ", " + readout[1].ToString() + ", " + readout[2].ToString());
        Graphics.DrawProceduralIndirect(MeshTopology.Points, argsCBuffer, 0);
        Graphics.DrawProceduralIndirect(MeshTopology.Triangles, argsCBuffer, 0);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnDestroy() {
        if (sourceDataCBuffer != null)
            sourceDataCBuffer.Release();
        if (argsCBuffer != null)
            argsCBuffer.Release();
        if (trianglesCBuffer != null)
            trianglesCBuffer.Release();
    }
}
