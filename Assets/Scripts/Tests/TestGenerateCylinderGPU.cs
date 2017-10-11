using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateCylinderGPU : MonoBehaviour {

    public ComputeShader shaderComputeGenerateCylinder;
    public Shader shaderDisplayDrawProceduralIndirectA;

    private ComputeBuffer schematicsCBuffer;
    private ComputeBuffer trianglesCBuffer;
    private ComputeBuffer argsCBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private Material displayMaterial;

    public struct Schematic {
        public Vector3 pos;
        public float height;
        public float radius;
    }

    public struct Triangle {
        public Vector3 vertA;
        public Vector3 vertB;
        public Vector3 vertC;
    }

    // Use this for initialization
    void Start () {

        argsCBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    void UpdateBuffers() {
        Debug.Log("UpdateBuffers!");

        int numSources = 16;
        int numTrianglesPerSourceMax = 4 * 8 * 2;
        // SOURCE DATA:::::
        if (schematicsCBuffer != null)
            schematicsCBuffer.Release();
        schematicsCBuffer = new ComputeBuffer(numSources, sizeof(float) * 5);

        Schematic[] sourceDataArray = new Schematic[numSources]; // for now only one seed data
        for(int i = 0; i < sourceDataArray.Length; i++) {
            Schematic schematic = new Schematic();
            
            schematic.height = 2f + i * 0.1f;
            schematic.radius = 0.1f + i * 0.05f;
            schematic.pos = new Vector3(i * 2f, 0f, 0f);
            sourceDataArray[i] = schematic;
            //sourceDataArray[i].pos = new Vector3(UnityEngine.Random.value * i * 0.15f, i * UnityEngine.Random.value * 0.25f, i * 0.15f + UnityEngine.Random.value * 0.25f);
        }
        schematicsCBuffer.SetData(sourceDataArray);  // set sourceDataBuffer to have one point centered at 1,1,1
        
        // SET UP GEO BUFFER and REFS:::::
        if (trianglesCBuffer != null)
            trianglesCBuffer.Release();
        trianglesCBuffer = new ComputeBuffer(numSources * numTrianglesPerSourceMax, sizeof(float) * 3 * 3, ComputeBufferType.Append); // vector3 position * 3 verts
        trianglesCBuffer.SetCounterValue(0);

        int kernelID = shaderComputeGenerateCylinder.FindKernel("CSMain");
        shaderComputeGenerateCylinder.SetBuffer(kernelID, "schematicsBuffer", schematicsCBuffer);
        shaderComputeGenerateCylinder.SetBuffer(kernelID, "appendTrianglesBuffer", trianglesCBuffer);// link computeBuffer to both computeShader and displayShader so they share the same one!!
        displayMaterial = new Material(shaderDisplayDrawProceduralIndirectA);
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", trianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!
        shaderComputeGenerateCylinder.Dispatch(kernelID, numSources, 1, 1); // Generate geometry data!

        

        // Either figure out how many triangles there are in advance, or use appendBuffer

        args[0] = 0; // set later by counter;// 3;  // 3 vertices to start
        args[1] = 1;  // 1 instance/copy
        argsCBuffer.SetData(args);
        ComputeBuffer.CopyCount(trianglesCBuffer, argsCBuffer, 0);        
        argsCBuffer.GetData(args);
        Debug.Log("triangle count " + args[0]);

        //Triangle[] readoutTrianglesArray = new Triangle[args[0]];
        //trianglesCBuffer.GetData(readoutTrianglesArray);
        //for (int i = 0; i < readoutTrianglesArray.Length; i++) {
        //    Debug.Log("triangle " + i.ToString() + ": " + readoutTrianglesArray[i].vertA.ToString() + readoutTrianglesArray[i].vertB.ToString() + readoutTrianglesArray[i].vertC.ToString());
        //}
    }

    private void OnRenderObject() {
        //int kernelID = shaderComputeGenerateCylinder.FindKernel("CSMain");
        //shaderComputeGenerateCylinder.Dispatch(kernelID, 512, 1, 1); // Stress-test for animation
        displayMaterial.SetPass(0);
        Graphics.DrawProceduralIndirect(MeshTopology.Points, argsCBuffer, 0);
        Graphics.DrawProceduralIndirect(MeshTopology.Triangles, argsCBuffer, 0);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnDestroy() {
        if (schematicsCBuffer != null)
            schematicsCBuffer.Release();
        if (argsCBuffer != null)
            argsCBuffer.Release();
        if (trianglesCBuffer != null)
            trianglesCBuffer.Release();
    }
}
