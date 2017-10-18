using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateBrainVisualizationA : MonoBehaviour {

    public List<Neuron> tempNeuronList;
    public List<Axon> tempAxonList;

    public ComputeShader shaderComputeBrain;
    public ComputeShader shaderComputeFloatingGlowyBits;
    //public Shader shaderDisplayBrain;
    public Material displayMaterial;
    public Material floatingGlowyBitsMaterial;

    private ComputeBuffer quadVerticesCBuffer;  // holds information for a 2-triangle Quad mesh (6 vertices)
    private ComputeBuffer floatingGlowyBitsCBuffer;  // holds information for placement and attributes of each instance of quadVertices to draw

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

    //private Material displayMaterial;

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
        public float pulsePos;
    }
    public struct Triangle {
        public Vector3 vertA;
        public Vector3 normA;
        public Vector3 tanA;
        public Vector3 uvwA;
        public Vector3 colorA;

        public Vector3 vertB;
        public Vector3 normB;
        public Vector3 tanB;
        public Vector3 uvwB;
        public Vector3 colorB;

        public Vector3 vertC;
        public Vector3 normC;
        public Vector3 tanC;
        public Vector3 uvwC;
        public Vector3 colorC;
    }

    int numNeurons = 4; // refBrain.neuronList.Count;
    int numAxons = 8; // refBrain.axonList.Count;
    int maxTrisPerNeuron = 1024;
    int maxTrisPerAxon = 2048;

    public float minAxonRadius = 0.02f;
    public float maxAxonRadius = 0.28f;
    public float minNeuronRadius = 0.8f;
    public float maxNeuronRadius = 1.5f;
    public float neuronAttractForce = 0.0001f;
    public float axonStraightenForce = .00001f;
    public float neuronRepelForce = 240.0f;
    public float axonRepelForce = 360f;

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

        // NEURON INIT DATA
        if (neuronInitDataCBuffer != null)
            neuronInitDataCBuffer.Release();
        neuronInitDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 3);
        NeuronInitData[] neuronInitDataArray = new NeuronInitData[numNeurons]; // for now only one seed data
        for (int x = 0; x < neuronInitDataArray.Length; x++) {
            NeuronInitData neuronData = new NeuronInitData();
            //neuronData.pos = Vector3.zero; // refBrain.neuronList[x].pos;
            neuronData.radius = 1.6f;
            neuronData.type = (float)tempNeuronList[x].neuronType / 2.0f;
            neuronData.age = UnityEngine.Random.Range(0f, 1f); // refBrain.neuronList[x].age;
            neuronInitDataArray[x] = neuronData;
        }
        neuronInitDataCBuffer.SetData(neuronInitDataArray);

        // NEURON FEED DATA
        if (neuronFeedDataCBuffer != null)
            neuronFeedDataCBuffer.Release();
        neuronFeedDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 1);
        NeuronFeedData[] neuronValuesArray = new NeuronFeedData[numNeurons];
        Debug.Log(neuronValuesArray.Length.ToString() + ", numNeurons: " + numNeurons.ToString());
        for(int i = 0; i < neuronValuesArray.Length; i++) {
            neuronValuesArray[i].curValue = tempNeuronList[i].currentValue[0];
        }
        neuronFeedDataCBuffer.SetData(neuronValuesArray);

        // NEURON SIM DATA
        if (neuronSimDataCBuffer != null)
            neuronSimDataCBuffer.Release();
        neuronSimDataCBuffer = new ComputeBuffer(numNeurons, sizeof(float) * 3);
        // One-time initialization of positions::::
        NeuronSimData[] neuronSimDataArray = new NeuronSimData[numNeurons];
        for (int i = 0; i < neuronSimDataArray.Length; i++) {
            neuronSimDataArray[i].pos = UnityEngine.Random.onUnitSphere * 12f; //refBrain.neuronList[i].pos;
            if(tempNeuronList[i].neuronType == NeuronGenome.NeuronType.In) {
                neuronSimDataArray[i].pos.x = -6.0f * Mathf.Abs(neuronSimDataArray[i].pos.x);
            }
            else {
                neuronSimDataArray[i].pos.x = 6.0f * Mathf.Abs(neuronSimDataArray[i].pos.x);
            }
            
        }
        neuronSimDataCBuffer.SetData(neuronSimDataArray);

        // AXON INIT DATA
        if (axonInitDataCBuffer != null)
            axonInitDataCBuffer.Release();
        axonInitDataCBuffer = new ComputeBuffer(tempAxonList.Count, sizeof(float) * 1 + sizeof(int) * 2);
        AxonInitData[] axonInitDataArray = new AxonInitData[tempAxonList.Count]; // for now only one seed data
        for (int x = 0; x < axonInitDataArray.Length; x++) {
            AxonInitData axonData = new AxonInitData();
            axonData.weight = tempAxonList[x].weight;
            axonData.fromID = tempAxonList[x].fromID;
            axonData.toID = tempAxonList[x].toID;
            axonInitDataArray[x] = axonData;
        }
        axonInitDataCBuffer.SetData(axonInitDataArray);

        // AXON SIM DATA
        if (axonSimDataCBuffer != null)
            axonSimDataCBuffer.Release();
        axonSimDataCBuffer = new ComputeBuffer(tempAxonList.Count, sizeof(float) * 13);

        // TRIANGLE BUFFER:
        // SET UP GEO BUFFER and REFS:::::
        if (appendTrianglesCBuffer != null)
            appendTrianglesCBuffer.Release();
        Debug.Log("Max Tris: " + (numNeurons * maxTrisPerNeuron + tempAxonList.Count * maxTrisPerAxon).ToString());
        appendTrianglesCBuffer = new ComputeBuffer(numNeurons * maxTrisPerNeuron + tempAxonList.Count * maxTrisPerAxon, sizeof(float) * 45, ComputeBufferType.Append); // vector3 position * 3 verts
        appendTrianglesCBuffer.SetCounterValue(0);


        //  FREE-FLOATING CAMERA-FACING QUADS:::::::::::
        //Create quad buffer
        quadVerticesCBuffer = new ComputeBuffer(6, sizeof(float) * 3);
        quadVerticesCBuffer.SetData(new[] {
            new Vector3(-0.5f, 0.5f),
            new Vector3(0.5f, 0.5f),
            new Vector3(0.5f, -0.5f),
            new Vector3(0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f)
        });
        int numFloatingGlowyBits = 64;
        Vector3[] initialGlowyBitsPositions = new Vector3[numFloatingGlowyBits];  // At first, populate this on CPU.... later, do so within a compute shader!!
        for(int i = 0; i < numFloatingGlowyBits; i++) {
            initialGlowyBitsPositions[i] = UnityEngine.Random.insideUnitSphere * 2f;
        }
        floatingGlowyBitsCBuffer = new ComputeBuffer(numFloatingGlowyBits, sizeof(float) * 3);
        floatingGlowyBitsCBuffer.SetData(initialGlowyBitsPositions);
        floatingGlowyBitsMaterial.SetPass(0);
        floatingGlowyBitsMaterial.SetBuffer("quadVerticesCBuffer", quadVerticesCBuffer);
        floatingGlowyBitsMaterial.SetBuffer("floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);


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
        shaderComputeBrain.SetFloat("minAxonRadius", 0.02f);
        shaderComputeBrain.SetFloat("maxAxonRadius", 0.28f);
        shaderComputeBrain.SetFloat("minNeuronRadius", 0.8f);
        shaderComputeBrain.SetFloat("maxNeuronRadius", 1.5f);
        shaderComputeBrain.SetFloat("neuronAttractForce", 0.0001f);
        shaderComputeBrain.SetFloat("axonStraightenForce", .00001f);
        shaderComputeBrain.SetFloat("neuronRepelForce", 240.0f);
        shaderComputeBrain.SetFloat("axonRepelForce", 360f);

        int simNeuronAttractKernelID = shaderComputeBrain.FindKernel("CSSimNeuronAttract");
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        int simNeuronRepelKernelID = shaderComputeBrain.FindKernel("CSSimNeuronRepel");
        int simAxonRepelKernelID = shaderComputeBrain.FindKernel("CSSimAxonRepel");

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
                
        //displayMaterial = new Material(shaderDisplayBrain);
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", appendTrianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!

        shaderComputeBrain.Dispatch(initKernelID, numAxons, 1, 1); // initialize axon positions and attributes
        shaderComputeBrain.Dispatch(simNeuronAttractKernelID, numAxons, 1, 1); // Simulate!! move neuron and axons around
        shaderComputeBrain.Dispatch(simNeuronRepelKernelID, numNeurons, numNeurons, 1); // Simulate!! move neuron and axons around
        shaderComputeBrain.Dispatch(simAxonRepelKernelID, numAxons, numAxons, 1); // Simulate!! move neuron and axons around
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
            neuronValuesArray[i].curValue = Mathf.Sin(Time.fixedTime * 1.25f + tempNeuronList[i].currentValue[0]);
        }
        neuronFeedDataCBuffer.SetData(neuronValuesArray);

        // For some reason I have to setBuffer on all of these for it to WORK!!!!!!!! (even though they are all the same in the shader...)
        // For some reason I have to setBuffer on all of these for it to WORK!!!!!!!!
        // For some reason I have to setBuffer on all of these for it to WORK!!!!!!!!
        shaderComputeBrain.SetFloat("minAxonRadius", minAxonRadius);
        shaderComputeBrain.SetFloat("maxAxonRadius", maxAxonRadius);
        shaderComputeBrain.SetFloat("minNeuronRadius", minNeuronRadius);
        shaderComputeBrain.SetFloat("maxNeuronRadius", maxNeuronRadius);
        shaderComputeBrain.SetFloat("neuronAttractForce", neuronAttractForce);
        shaderComputeBrain.SetFloat("axonStraightenForce", axonStraightenForce);
        shaderComputeBrain.SetFloat("neuronRepelForce", neuronRepelForce);
        shaderComputeBrain.SetFloat("axonRepelForce", axonRepelForce);

        shaderComputeBrain.SetFloat("time", Time.fixedTime);

        int simNeuronAttractKernelID = shaderComputeBrain.FindKernel("CSSimNeuronAttract");        
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronAttractKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.Dispatch(simNeuronAttractKernelID, numAxons, 1, 1); // Simulate!! move neuron and axons around
        int simNeuronRepelKernelID = shaderComputeBrain.FindKernel("CSSimNeuronRepel");
        shaderComputeBrain.SetBuffer(simNeuronRepelKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronRepelKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronRepelKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronRepelKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simNeuronRepelKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.Dispatch(simNeuronRepelKernelID, numNeurons, numNeurons, 1); // Simulate!! move neuron and axons around
        int simAxonRepelKernelID = shaderComputeBrain.FindKernel("CSSimAxonRepel");
        shaderComputeBrain.SetBuffer(simAxonRepelKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simAxonRepelKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(simAxonRepelKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(simAxonRepelKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(simAxonRepelKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.Dispatch(simAxonRepelKernelID, numAxons, numAxons, 1); // Simulate!! move neuron and axons around

        // Re-Generate TRIANGLES!
        // SET UP GEO BUFFER and REFS:::::
        if (appendTrianglesCBuffer != null)
            appendTrianglesCBuffer.Release();
        //Debug.Log("Max Tris: " + (numNeurons * maxTrisPerNeuron + numAxons * maxTrisPerAxon).ToString());
        appendTrianglesCBuffer = new ComputeBuffer(numNeurons * maxTrisPerNeuron + numAxons * maxTrisPerAxon, sizeof(float) * 45, ComputeBufferType.Append); // vector3 position * 3 verts
        appendTrianglesCBuffer.SetCounterValue(0);

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
        
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", appendTrianglesCBuffer);
        
        shaderComputeBrain.Dispatch(neuronTrianglesKernelID, tempNeuronList.Count, 1, 1); // create all triangles from Neurons
        shaderComputeBrain.Dispatch(axonTrianglesKernelID, tempAxonList.Count, 1, 1); // create all geometry for Axons

        args[0] = 0; // set later by counter;// 3;  // 3 vertices to start
        args[1] = 1;  // 1 instance/copy
        argsCBuffer.SetData(args);
        ComputeBuffer.CopyCount(appendTrianglesCBuffer, argsCBuffer, 0);
        argsCBuffer.GetData(args);
        //Debug.Log("triangle count " + args[0]);
        
    }

    private void CreateDummyBrain() {
        // create a random small genome brain to test
        // Neurons!
        tempNeuronList = new List<Neuron>();
        int numInputs = UnityEngine.Random.Range(Mathf.RoundToInt((float)numNeurons * 0.2f), Mathf.RoundToInt((float)numNeurons * 0.8f));
        for (int i = 0; i < numNeurons; i++) {
            Neuron neuron = new Neuron();
            if(i < numInputs) {
                neuron.neuronType = NeuronGenome.NeuronType.In;
            }
            else {
                neuron.neuronType = NeuronGenome.NeuronType.Out;
            }
            neuron.currentValue = new float[1];
            neuron.currentValue[0] = UnityEngine.Random.Range(-2f, 2f);
            tempNeuronList.Add(neuron);
        }

        tempAxonList = new List<Axon>();
        // Axons:
        for (int i = 0; i < numInputs; i++) {
            for(int j = 0; j < numNeurons - numInputs; j++) {
                if(j + i * numInputs < numAxons) {
                    Axon axon = new Axon(i, numInputs + j, UnityEngine.Random.Range(-1f, 1f));
                    tempAxonList.Add(axon);
                }                
            }
        }

        numAxons = tempAxonList.Count;
    }

    private void OnRenderObject() {
        
        displayMaterial.SetPass(0);
        Graphics.DrawProceduralIndirect(MeshTopology.Points, argsCBuffer, 0);
        //Graphics.DrawProceduralIndirect(MeshTopology.Triangles, argsCBuffer, 0);

        floatingGlowyBitsMaterial.SetPass(0);
        //floatingGlowyBitsMaterial.SetBuffer("floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);
        Graphics.DrawProcedural(MeshTopology.Triangles, 6, floatingGlowyBitsCBuffer.count);
    }

    // Update is called once per frame
    void Update () {
        UpdateBrainDataAndBuffers();

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
        if (floatingGlowyBitsCBuffer != null)
            floatingGlowyBitsCBuffer.Release();
        if (quadVerticesCBuffer != null)
            quadVerticesCBuffer.Release();
        
    }
}
