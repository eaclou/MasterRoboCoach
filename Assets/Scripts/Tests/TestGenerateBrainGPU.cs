using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateBrainGPU : MonoBehaviour {

    public List<Neuron> tempNeuronList;
    public List<Axon> tempAxonList;

    public ComputeShader shaderComputeBrain;
    public Shader shaderDisplayBrain;

    private ComputeBuffer neuronInitDataCBuffer;  // sets initial positions for each neuron
    private ComputeBuffer neuronFeedDataCBuffer;  // current value -- separate so CPU only has to push the bare-minimum data to GPU every n frames
    private ComputeBuffer neuronSimDataCBuffer;  // holds data that is updatable purely on GPU, like neuron Positions
    private ComputeBuffer axonInitDataCBuffer;  // holds axon weights ( as well as from/to neuron IDs ?)
    private ComputeBuffer axonSimDataCBuffer;  // axons can be updated entirely on GPU by referencing neuron positions. 
    // ... this includes all spline data --> positions, vertex colors, uv, radii, etc., pulse positions
        // Some other secondary buffers for decorations later
    private ComputeBuffer appendTrianglesCBuffer; // will likely split this out into seperate ones later to support multiple materials/layers, but all-in-one for now...
    private ComputeBuffer argsCBuffer;
    private uint[] args = new uint[5] { 0, 0, 0, 0, 0 };

    private Material displayMaterial;

    public struct NeuronInitData {
        //public Vector3 pos;  // can maybe remove this and just set NeuronSimData.pos once at start of program?
        public float radius;
        public float type;  // in/out/hidden
        public float age;
    }
    public struct NeuronFeedData {
        public float curValue;  // [-1,1]  // set by CPU continually
    }
    public struct NeuronSimData {
        public Vector3 pos;
    }
    public struct AxonInitData {  // set once at start
        public float weight;
        public int fromID;
        public int toID;
    }
    public struct AxonSimData {
        public Vector3 p0;
        public Vector3 p1;
        public Vector3 p2;
        public Vector3 p3;
        public float radiusStart;
        public float radiusEnd;
        public float pulsePos;
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
        //Debug.Log(Quaternion.identity.w.ToString() + ", " + Quaternion.identity.x.ToString() + ", " + Quaternion.identity.y.ToString() + ", " + Quaternion.identity.z.ToString() + ", ");
        argsCBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        //UpdateBuffers();
        InitializeComputeBuffers();
    }

    private void InitializeComputeBuffers() {
        // first-time setup for compute buffers (assume new brain)
        if(tempNeuronList == null || tempAxonList == null) {
            CreateDummyBrain();
        }
        int numNeurons = 2; // refBrain.neuronList.Count;
        int numAxons = 4; // refBrain.axonList.Count;
        int maxTrisPerNeuron = 128;
        int maxTrisPerAxon = 256;

        // NEURON INIT DATA
        if (neuronInitDataCBuffer != null)
            neuronInitDataCBuffer.Release();
        neuronInitDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 3);
        NeuronInitData[] neuronInitDataArray = new NeuronInitData[numNeurons]; // for now only one seed data
        for (int x = 0; x < neuronInitDataArray.Length; x++) {
            NeuronInitData neuronData = new NeuronInitData();
            //neuronData.pos = Vector3.zero; // refBrain.neuronList[x].pos;
            neuronData.radius = 0.25f;
            neuronData.type = 0f; // refBrain.neuronList[x].type;
            neuronData.age = 1f; // refBrain.neuronList[x].age;
            neuronInitDataArray[x] = neuronData;
        }
        neuronInitDataCBuffer.SetData(neuronInitDataArray);

        // NEURON FEED DATA
        if (neuronFeedDataCBuffer != null)
            neuronFeedDataCBuffer.Release();
        neuronFeedDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 1);
        NeuronFeedData[] neuronValuesArray = new NeuronFeedData[numNeurons];
        for(int i = 0; i < neuronValuesArray.Length; i++) {
            neuronValuesArray[i].curValue = 1f; //refBrain.neuronList[i].currentValue[0];
        }
        neuronFeedDataCBuffer.SetData(neuronValuesArray);

        // NEURON SIM DATA
        if (neuronSimDataCBuffer != null)
            neuronSimDataCBuffer.Release();
        neuronSimDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 3);
        // One-time initialization of positions::::
        NeuronSimData[] neuronSimDataArray = new NeuronSimData[numNeurons];
        for (int i = 0; i < neuronSimDataArray.Length; i++) {
            neuronSimDataArray[i].pos = Vector3.zero; //refBrain.neuronList[i].pos;
        }
        neuronSimDataCBuffer.SetData(neuronSimDataArray);

        // AXON INIT DATA
        if (axonInitDataCBuffer != null)
            axonInitDataCBuffer.Release();
        axonInitDataCBuffer = new ComputeBuffer(numAxons, sizeof(float) * 1 + sizeof(int) * 2);
        AxonInitData[] axonInitDataArray = new AxonInitData[numAxons]; // for now only one seed data
        for (int x = 0; x < axonInitDataArray.Length; x++) {
            AxonInitData axonData = new AxonInitData();
            axonData.weight = 1f; // refBrain.axonList[x].weight;
            axonData.fromID = 1; // refBrain.axonList[x].fromID;
            axonData.toID = 1; // refBrain.axonList[x].toID;
            axonInitDataArray[x] = axonData;
        }
        axonInitDataCBuffer.SetData(axonInitDataArray);

        // AXON SIM DATA
        if (axonSimDataCBuffer != null)
            axonSimDataCBuffer.Release();
        axonSimDataCBuffer = new ComputeBuffer(numAxons, sizeof(float) * 15);

        // TRIANGLE BUFFER:
        // SET UP GEO BUFFER and REFS:::::
        if (appendTrianglesCBuffer != null)
            appendTrianglesCBuffer.Release();
        appendTrianglesCBuffer = new ComputeBuffer(numNeurons * maxTrisPerNeuron + numAxons * maxTrisPerAxon, sizeof(float) * 18, ComputeBufferType.Append); // vector3 position * 3 verts
        appendTrianglesCBuffer.SetCounterValue(0);


        // Hook Buffers Up to Shaders!!!
        // populate initial data for neurons
        // populate initial data for axons
        // feed neuronValues data to shader (encapsulate in function since this is ongoing)
        // simulate movements / animation parameters
        // generate neuron triangles
        // generate axon triangles
        int initKernelID = shaderComputeBrain.FindKernel("CSInitializeAxonSimData");
        shaderComputeBrain.SetBuffer(initKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);

        int simulateKernelID = shaderComputeBrain.FindKernel("CSSimulation");
        shaderComputeBrain.SetBuffer(simulateKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simulateKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(simulateKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(simulateKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simulateKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);

        int neuronTrianglesKernelID = shaderComputeBrain.FindKernel("CSGenerateNeuronTriangles");
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        //shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        //shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);

        int axonTrianglesKernelID = shaderComputeBrain.FindKernel("CSGenerateAxonTriangles");
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);
                
        displayMaterial = new Material(shaderDisplayBrain);
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", appendTrianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!
        
        shaderComputeBrain.Dispatch(simulateKernelID, 1, 1, 1); // Simulate!! move neuron and axons around
        shaderComputeBrain.Dispatch(neuronTrianglesKernelID, numNeurons, 1, 1); // create all triangles from Neurons
        shaderComputeBrain.Dispatch(axonTrianglesKernelID, numAxons, 1, 1); // create all geometry for Axons
        

        args[0] = 0; // set later by counter;// 3;  // 3 vertices to start
        args[1] = 1;  // 1 instance/copy
        argsCBuffer.SetData(args);
        ComputeBuffer.CopyCount(appendTrianglesCBuffer, argsCBuffer, 0);
        argsCBuffer.GetData(args);
        Debug.Log("triangle count " + args[0]);
    }

    private void UpdateBrainDataAndBuffers() {
        // NEURON FEED DATA
        //if (neuronFeedDataCBuffer != null)
        //    neuronFeedDataCBuffer.Release();
        //neuronFeedDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 1);
        if (tempNeuronList == null)
            return;
        if (tempAxonList == null)
            return;

        NeuronFeedData[] neuronValuesArray = new NeuronFeedData[tempNeuronList.Count];
        for (int i = 0; i < neuronValuesArray.Length; i++) {
            neuronValuesArray[i].curValue = tempNeuronList[i].currentValue[0];
        }
        neuronFeedDataCBuffer.SetData(neuronValuesArray);

        /*NeuronFeedData[] neuronValuesArray = new NeuronFeedData[refBrain.neuronList.Count];
        for (int i = 0; i < neuronValuesArray.Length; i++) {
            neuronValuesArray[i].curValue = refBrain.neuronList[i].currentValue[0];
        }
        neuronFeedDataCBuffer.SetData(neuronValuesArray);*/

    }

    private void CreateDummyBrain() {
        // create a random small genome brain to test

        tempNeuronList = new List<Neuron>();
        Neuron neuron1 = new Neuron();
        neuron1.neuronType = NeuronGenome.NeuronType.In;
        neuron1.currentValue = new float[1];
        neuron1.currentValue[0] = 1f;
        tempNeuronList.Add(neuron1);

        Neuron neuron2 = new Neuron();
        neuron2.neuronType = NeuronGenome.NeuronType.Out;
        neuron2.currentValue = new float[1];
        neuron2.currentValue[0] = -1f;
        tempNeuronList.Add(neuron2);

        tempAxonList = new List<Axon>();
        Axon axon1 = new Axon(0, 1, -1f);
        tempAxonList.Add(axon1);

        //Brain brain = new Brain();

    }

    // CUBIC:
    /*public Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * oneMinusT * p0 + 3f * oneMinusT * oneMinusT * t * p1 + 3f * oneMinusT * t * t * p2 +
                t * t * t * p3;
    }
    // CUBIC
    public Vector3 GetFirstDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return 3f * oneMinusT * oneMinusT * (p1 - p0) + 6f * oneMinusT * t * (p2 - p1) + 3f * t * t * (p3 - p2);
    }*/

    void UpdateBuffers() {
        Debug.Log("UpdateBuffers!");

        //int numSources = 16;
        //int numTrianglesPerSourceMax = 8 * 16 * 2;

        // SOURCE DATA:::::
        /*if (schematicsCBuffer != null)
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
                                                     


        Vector3 testDir = GetFirstDerivative(new Vector3(2f, 1f, 1f), new Vector3(2f, 2f, 2f), new Vector3(2f, 3f, 3f), new Vector3(2f, 4f, 4f), 0.0f).normalized;
        //float3 ringDir = normalize(GetFirstDerivative(schematicsBuffer[id.x].p0, schematicsBuffer[id.x].p1, schematicsBuffer[id.x].p2, schematicsBuffer[id.x].p3, tInc * idy));
        Vector3 tangent = Vector3.Cross(testDir, new Vector3(0.0f, 1.0f, 0.0f)).normalized; // x
        Vector3 bitangent = Vector3.Cross(tangent, testDir).normalized; // y;
        Debug.Log("B0 dir: " + testDir.ToString() + ", tan: " + tangent.ToString() + ", bi-tan: " + bitangent.ToString());
        
        // SET UP GEO BUFFER and REFS:::::
        if (appendTrianglesCBuffer != null)
            appendTrianglesCBuffer.Release();
        appendTrianglesCBuffer = new ComputeBuffer(numSources * numTrianglesPerSourceMax, sizeof(float) * 6 * 3, ComputeBufferType.Append); // vector3 position * 3 verts
        appendTrianglesCBuffer.SetCounterValue(0);

        int kernelID = shaderComputeBrain.FindKernel("CSMain");
        shaderComputeBrain.SetBuffer(kernelID, "schematicsBuffer", schematicsCBuffer);
        shaderComputeBrain.SetBuffer(kernelID, "appendTrianglesBuffer", appendTrianglesCBuffer);// link computeBuffer to both computeShader and displayShader so they share the same one!!
        displayMaterial = new Material(shaderDisplayBrain);
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", appendTrianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!
        shaderComputeBrain.Dispatch(kernelID, numSources, 1, 1); // Generate geometry data!

        

        // Either figure out how many triangles there are in advance, or use appendBuffer

        args[0] = 0; // set later by counter;// 3;  // 3 vertices to start
        args[1] = 1;  // 1 instance/copy
        argsCBuffer.SetData(args);
        ComputeBuffer.CopyCount(appendTrianglesCBuffer, argsCBuffer, 0);        
        argsCBuffer.GetData(args);
        Debug.Log("triangle count " + args[0]);

        //Triangle[] readoutTrianglesArray = new Triangle[args[0]];
        //trianglesCBuffer.GetData(readoutTrianglesArray);
        //for (int i = 0; i < readoutTrianglesArray.Length; i++) {
        //    Debug.Log("triangle " + i.ToString() + ": " + readoutTrianglesArray[i].vertA.ToString() + readoutTrianglesArray[i].vertB.ToString() + readoutTrianglesArray[i].vertC.ToString());
        //}
        */
    }

    private void OnRenderObject() {
        //int kernelID = shaderComputeGenerateCylinder.FindKernel("CSMain");
        //shaderComputeGenerateCylinder.Dispatch(kernelID, 512, 1, 1); // Stress-test for animation
        displayMaterial.SetPass(0);
        //Graphics.DrawProceduralIndirect(MeshTopology.Points, argsCBuffer, 0);
        Graphics.DrawProceduralIndirect(MeshTopology.Triangles, argsCBuffer, 0);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnDestroy() {
        if (neuronInitDataCBuffer != null)
            neuronInitDataCBuffer.Release();
        if (neuronFeedDataCBuffer != null)
            neuronFeedDataCBuffer.Release();
        if (neuronSimDataCBuffer != null)
            neuronSimDataCBuffer.Release();
        if (axonInitDataCBuffer != null)
            axonInitDataCBuffer.Release();
        if (axonSimDataCBuffer != null)
            axonSimDataCBuffer.Release();
        if (argsCBuffer != null)
            argsCBuffer.Release();
        if (appendTrianglesCBuffer != null)
            appendTrianglesCBuffer.Release();
    }
}
