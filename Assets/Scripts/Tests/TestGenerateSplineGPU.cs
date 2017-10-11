using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateSplineGPU : MonoBehaviour {

    public ComputeShader shaderComputeGenerateSpline;
    public Shader shaderDisplaySpline;

    private ComputeBuffer schematicsCBuffer;
    private ComputeBuffer trianglesCBuffer;
    private ComputeBuffer argsCBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private Material displayMaterial;

    public struct Schematic {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public float radius;
    }

    public struct Triangle {
        public Vector3 vertA;
        public Vector3 normA;
        public Vector3 vertB;
        public Vector3 normB;
        public Vector3 vertC;
        public Vector3 normC;
    }

    // Use this for initialization
    void Start () {
        Debug.Log(Quaternion.identity.w.ToString() + ", " + Quaternion.identity.x.ToString() + ", " + Quaternion.identity.y.ToString() + ", " + Quaternion.identity.z.ToString() + ", ");
        argsCBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    // CUBIC:
    public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * p0 + 3f * oneMinusT * oneMinusT * t * p1 + 3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
    }

    // QUADRATIC
    //public static Vector3 GetFirstDerivative (Vector3 p0, Vector3 p1, Vector3 p2, float t) {
    //	return 2f * (1f - t) * (p1 - p0) + 2f * t * (p2 - p1);
    //}

    // CUBIC
    public Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return 3f * oneMinusT * oneMinusT * (p1 - p0) + 6f * oneMinusT * t * (p2 - p1) + 3f * t * t * (p3 - p2);
    }

    void UpdateBuffers() {
        Debug.Log("UpdateBuffers!");

        int numSources = 16;
        int numTrianglesPerSourceMax = 8 * 16 * 2;
        // SOURCE DATA:::::
        if (schematicsCBuffer != null)
            schematicsCBuffer.Release();
        schematicsCBuffer = new ComputeBuffer(numSources, sizeof(float) * 13);

        Schematic[] sourceDataArray = new Schematic[numSources]; // for now only one seed data
        for(int x = 0; x < sourceDataArray.Length; x++) {
            Schematic schematic = new Schematic();            
            schematic.radius = 0.12f;
            schematic.p0 = UnityEngine.Random.insideUnitSphere;
            schematic.p0.x += x * 1f;
            schematic.p1 = UnityEngine.Random.insideUnitSphere;
            schematic.p1.x += x * 1f;
            schematic.p1.z += 1f;
            schematic.p2 = UnityEngine.Random.insideUnitSphere;
            schematic.p2.x += x * 1f;
            schematic.p2.z += 2f;
            schematic.p3 = UnityEngine.Random.insideUnitSphere;
            schematic.p3.x += x * 1f;
            schematic.p3.z += 3f;
            sourceDataArray[x] = schematic;
            //sourceDataArray[i].pos = new Vector3(UnityEngine.Random.value * i * 0.15f, i * UnityEngine.Random.value * 0.25f, i * 0.15f + UnityEngine.Random.value * 0.25f);
        }
        schematicsCBuffer.SetData(sourceDataArray);  // set sourceDataBuffer to have one point centered at 1,1,1
                                                     /*Schematic schematicA = new Schematic();
                                                     schematicA.radius = 0.15f;
                                                     schematicA.p0 = Vector3.zero;
                                                     schematicA.p1 = new Vector3(-2.5f, 0.5f, 1f);
                                                     schematicA.p2 = new Vector3(1.5f, 1f, 2f);
                                                     schematicA.p3 = new Vector3(0f, -1.5f, 3f);
                                                     sourceDataArray[0] = schematicA;

                                                     Schematic schematicB = new Schematic();
                                                     schematicB.radius = 0.05f;
                                                     schematicB.p0 = new Vector3(2f, 1f, 1f);
                                                     schematicB.p1 = new Vector3(2f, 2f, 2f);
                                                     schematicB.p2 = new Vector3(2f, 3f, 3f);
                                                     schematicB.p3 = new Vector3(2f, 4f, 4f);
                                                     sourceDataArray[1] = schematicB;*/


        Vector3 testDir = GetFirstDerivative(new Vector3(2f, 1f, 1f), new Vector3(2f, 2f, 2f), new Vector3(2f, 3f, 3f), new Vector3(2f, 4f, 4f), 0.0f).normalized;
        //float3 ringDir = normalize(GetFirstDerivative(schematicsBuffer[id.x].p0, schematicsBuffer[id.x].p1, schematicsBuffer[id.x].p2, schematicsBuffer[id.x].p3, tInc * idy));
        Vector3 tangent = Vector3.Cross(testDir, new Vector3(0.0f, 1.0f, 0.0f)).normalized; // x
        Vector3 bitangent = Vector3.Cross(tangent, testDir).normalized; // y;
        Debug.Log("B0 dir: " + testDir.ToString() + ", tan: " + tangent.ToString() + ", bi-tan: " + bitangent.ToString());
        
        // SET UP GEO BUFFER and REFS:::::
        if (trianglesCBuffer != null)
            trianglesCBuffer.Release();
        trianglesCBuffer = new ComputeBuffer(numSources * numTrianglesPerSourceMax, sizeof(float) * 6 * 3, ComputeBufferType.Append); // vector3 position * 3 verts
        trianglesCBuffer.SetCounterValue(0);

        int kernelID = shaderComputeGenerateSpline.FindKernel("CSMain");
        shaderComputeGenerateSpline.SetBuffer(kernelID, "schematicsBuffer", schematicsCBuffer);
        shaderComputeGenerateSpline.SetBuffer(kernelID, "appendTrianglesBuffer", trianglesCBuffer);// link computeBuffer to both computeShader and displayShader so they share the same one!!
        displayMaterial = new Material(shaderDisplaySpline);
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", trianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!
        shaderComputeGenerateSpline.Dispatch(kernelID, numSources, 1, 1); // Generate geometry data!

        

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
