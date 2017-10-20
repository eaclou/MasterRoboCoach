using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGenerateBrainVisualizationA : MonoBehaviour {

    public List<Neuron> tempNeuronList;
    public List<Axon> tempAxonList;

    public ComputeShader shaderComputeBrain;
    public ComputeShader shaderComputeFloatingGlowyBits;
    public ComputeShader shaderComputeExtraBalls;  // quads w/ nml maps to like like extra blobs attached to neurons & axons
    //public Shader shaderDisplayBrain;
    public Material displayMaterial;
    public Material floatingGlowyBitsMaterial;
    public Material extraBallsMaterial;

    private ComputeBuffer quadVerticesCBuffer;  // holds information for a 2-triangle Quad mesh (6 vertices)
    private ComputeBuffer floatingGlowyBitsCBuffer;  // holds information for placement and attributes of each instance of quadVertices to draw
    private ComputeBuffer extraBallsCBuffer;
    private ComputeBuffer axonBallCBuffer;
    private ComputeBuffer neuronBallCBuffer;

    private ComputeBuffer neuronInitDataCBuffer;  // sets initial positions for each neuron
    private ComputeBuffer neuronFeedDataCBuffer;  // current value -- separate so CPU only has to push the bare-minimum data to GPU every n frames
    private ComputeBuffer neuronSimDataCBuffer;  // holds data that is updatable purely on GPU, like neuron Positions
    //private ComputeBuffer subNeuronSimDataCBuffer; // just 
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
    public struct ExtraBallsAxonsData {
        public int axonID;
        public float t;  // how far along the spline
        public float angle;  // angle of rotation around the axis
        public float baseScale;
    }
    public struct ExtraBallsNeuronsData {
        public int neuronID;
        public Vector3 direction;  // where on the neuron???
        public float baseScale;
    }
    public struct BallData {
        public Vector3 worldPos;        
        public float value;
        public float inOut;
        public float scale;
        // per-particle size / rotation etc.???
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

    int numNeurons = 16; // refBrain.neuronList.Count;
    int numAxons = 128; // refBrain.axonList.Count;
    int maxTrisPerNeuron = 1024;
    int maxTrisPerSubNeuron = 8 * 8 * 2 * 2;
    int maxTrisPerAxon = 2048;
    int numFloatingGlowyBits = 1024;
    int numAxonBalls = 128 * 128;
    int numNeuronBalls = 1024;

    /*public float minAxonRadius = 0.02f;
    public float maxAxonRadius = 0.28f;
    public float minNeuronRadius = 0.8f;
    public float maxNeuronRadius = 1.5f;
    public float neuronAttractForce = 0.0001f;
    public float neuronRepelForce = 240.0f;
    public float axonPerpendicularityForce = 0.01f;
    public float axonAttachStraightenForce = 0.01f;
    public float axonAttachSpreadForce = 0.025f;
    public float axonRepelForce = 360f;*/

    // Core Sizes:
    public float minNeuronRadius = 0.05f;
    public float maxNeuronRadius = 0.5f;
    public float minAxonRadius = 0.05f;
    public float maxAxonRadius = 0.5f;
    public float minSubNeuronScale = 0.25f;
    public float maxSubNeuronScale = 0.75f;  // max size relative to parent Neuron
    public float maxAxonFlareScale = 0.9f;  // max axon flare size relative to SubNeuron
    public float minAxonFlareScale = 0.2f;
    public float axonFlarePos = 0.92f;
    public float axonFlareWidth = 0.08f;
    public float axonMaxPulseMultiplier = 2.0f;

    // Noise Parameters:
    public float neuronExtrudeNoiseFreq = 1.5f;
    public float neuronExtrudeNoiseAmp = 0.0f;
    public float neuronExtrudeNoiseScrollSpeed = 0.6f;
    public float axonExtrudeNoiseFreq = 0.33f;
    public float axonExtrudeNoiseAmp = 0.33f;
    public float axonExtrudeNoiseScrollSpeed = 1.0f;
    public float axonPosNoiseFreq = 0.14f;
    public float axonPosNoiseAmp = 0f;
    public float axonPosNoiseScrollSpeed = 10f;
    public float axonPosSpiralFreq = 20.0f;
    public float axonPosSpiralAmp = 0f;

    // Forces:
    public float neuronAttractForce = 0.004f;
    public float neuronRepelForce = 2.0f;
    public float axonPerpendicularityForce = 0.01f;
    public float axonAttachStraightenForce = 0.01f;
    public float axonAttachSpreadForce = 0.025f;
    public float axonRepelForce = 0.2f;

    // Extra Balls:
    public float neuronBallMaxScale = 1f;

    // Use this for initialization
    void Start () {
        //Debug.Log(Quaternion.identity.w.ToString() + ", " + Quaternion.identity.x.ToString() + ", " + Quaternion.identity.y.ToString() + ", " + Quaternion.identity.z.ToString() + ", ");
        argsCBuffer = new ComputeBuffer(1, args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        //UpdateBuffers();
        InitializeComputeBuffers();
    }

    private void SetCoreBrainDataSharedParameters(ComputeShader computeShader) {
        // Core Sizes:
        computeShader.SetFloat("minNeuronRadius", minNeuronRadius);
        computeShader.SetFloat("maxNeuronRadius", maxNeuronRadius);
        computeShader.SetFloat("minAxonRadius", minAxonRadius);
        computeShader.SetFloat("maxAxonRadius", maxAxonRadius);
        computeShader.SetFloat("minSubNeuronScale", minSubNeuronScale);
        computeShader.SetFloat("maxSubNeuronScale", maxSubNeuronScale);
        computeShader.SetFloat("minAxonFlareScale", minAxonFlareScale);
        computeShader.SetFloat("maxAxonFlareScale", maxAxonFlareScale);        
        computeShader.SetFloat("axonFlarePos", axonFlarePos);
        computeShader.SetFloat("axonFlareWidth", axonFlareWidth);
        computeShader.SetFloat("axonMaxPulseMultiplier", axonMaxPulseMultiplier);

        // Noise Parameters:
        computeShader.SetFloat("neuronExtrudeNoiseFreq", neuronExtrudeNoiseFreq);
        computeShader.SetFloat("neuronExtrudeNoiseAmp", neuronExtrudeNoiseAmp);
        computeShader.SetFloat("neuronExtrudeNoiseScrollSpeed", neuronExtrudeNoiseScrollSpeed);
        computeShader.SetFloat("axonExtrudeNoiseFreq", axonExtrudeNoiseFreq);
        computeShader.SetFloat("axonExtrudeNoiseAmp", axonExtrudeNoiseAmp);
        computeShader.SetFloat("axonExtrudeNoiseScrollSpeed", axonExtrudeNoiseScrollSpeed);
        computeShader.SetFloat("axonPosNoiseFreq", axonPosNoiseFreq);
        computeShader.SetFloat("axonPosNoiseAmp", axonPosNoiseAmp);
        computeShader.SetFloat("axonPosNoiseScrollSpeed", axonPosNoiseScrollSpeed);
        computeShader.SetFloat("axonPosSpiralFreq", axonPosSpiralFreq);
        computeShader.SetFloat("axonPosSpiralAmp", axonPosSpiralAmp);

        // Forces:
        computeShader.SetFloat("neuronAttractForce", neuronAttractForce);
        computeShader.SetFloat("neuronRepelForce", neuronRepelForce);
        computeShader.SetFloat("axonPerpendicularityForce", axonPerpendicularityForce);
        computeShader.SetFloat("axonAttachStraightenForce", axonAttachStraightenForce);
        computeShader.SetFloat("axonAttachSpreadForce", axonAttachSpreadForce);
        computeShader.SetFloat("axonRepelForce", axonRepelForce);

        computeShader.SetFloat("time", Time.fixedTime);

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
                neuronSimDataArray[i].pos.x = -3.0f * Mathf.Abs(neuronSimDataArray[i].pos.x);
            }
            else {
                neuronSimDataArray[i].pos.x = 3.0f * Mathf.Abs(neuronSimDataArray[i].pos.x);
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
        int maxTris = numNeurons * maxTrisPerNeuron + tempAxonList.Count * maxTrisPerAxon + maxTrisPerSubNeuron * tempAxonList.Count * 2;
        Debug.Log("Max Tris: " + maxTris.ToString());
        appendTrianglesCBuffer = new ComputeBuffer(maxTris, sizeof(float) * 45, ComputeBufferType.Append); // vector3 position * 3 verts
        appendTrianglesCBuffer.SetCounterValue(0);


        //  FREE-FLOATING CAMERA-FACING QUADS:::::::::::
        //Create quad buffer
        if (quadVerticesCBuffer != null)
            quadVerticesCBuffer.Release();
        quadVerticesCBuffer = new ComputeBuffer(6, sizeof(float) * 3);
        quadVerticesCBuffer.SetData(new[] {
            new Vector3(-0.5f, 0.5f),
            new Vector3(0.5f, 0.5f),
            new Vector3(0.5f, -0.5f),
            new Vector3(0.5f, -0.5f),
            new Vector3(-0.5f, -0.5f),
            new Vector3(-0.5f, 0.5f)
        });
        
        if (floatingGlowyBitsCBuffer != null)
            floatingGlowyBitsCBuffer.Release();
        floatingGlowyBitsCBuffer = new ComputeBuffer(numFloatingGlowyBits, sizeof(float) * 3);
        floatingGlowyBitsMaterial.SetPass(0);
        floatingGlowyBitsMaterial.SetBuffer("quadVerticesCBuffer", quadVerticesCBuffer);
        floatingGlowyBitsMaterial.SetBuffer("floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);
        int initGlowyBitsKernelID = shaderComputeFloatingGlowyBits.FindKernel("CSInitializePositions");
        shaderComputeFloatingGlowyBits.SetFloat("minRadius", 0.05f);
        shaderComputeFloatingGlowyBits.SetFloat("maxRadius", 12f);
        shaderComputeFloatingGlowyBits.SetBuffer(initGlowyBitsKernelID, "floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);
        shaderComputeFloatingGlowyBits.Dispatch(initGlowyBitsKernelID, numFloatingGlowyBits / 64, 1, 1); // initialize axon positions and attributes

        //  EXTRA BALLS NORMAL-MAPPED CAMERA-FACING QUADS:::::::::::
        
        if (extraBallsCBuffer != null)
            extraBallsCBuffer.Release();
        extraBallsCBuffer = new ComputeBuffer(numAxonBalls + numNeuronBalls, sizeof(float) * 6);
        extraBallsMaterial.SetPass(0);
        extraBallsMaterial.SetBuffer("quadVerticesCBuffer", quadVerticesCBuffer);
        extraBallsMaterial.SetBuffer("extraBallsCBuffer", extraBallsCBuffer);

        if (axonBallCBuffer != null)
            axonBallCBuffer.Release();
        axonBallCBuffer = new ComputeBuffer(numAxonBalls, sizeof(float) * 3 + sizeof(int) * 1);
        if (neuronBallCBuffer != null)
            neuronBallCBuffer.Release();
        neuronBallCBuffer = new ComputeBuffer(numNeuronBalls, sizeof(float) * 4 + sizeof(int) * 1);

        int initAxonBallsKernelID = shaderComputeExtraBalls.FindKernel("CSInitializeAxonBallData");
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "axonBallCBuffer", axonBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "neuronBallCBuffer", neuronBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(initAxonBallsKernelID, "extraBallsCBuffer", extraBallsCBuffer);
        shaderComputeExtraBalls.Dispatch(initAxonBallsKernelID, numAxonBalls, 1, 1); // initialize axon positions and attributes

        int initNeuronBallsKernelID = shaderComputeExtraBalls.FindKernel("CSInitializeNeuronBallData");
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "axonBallCBuffer", axonBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "neuronBallCBuffer", neuronBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(initNeuronBallsKernelID, "extraBallsCBuffer", extraBallsCBuffer);
        shaderComputeExtraBalls.Dispatch(initNeuronBallsKernelID, numNeuronBalls, 1, 1); // initialize axon positions and attributes




        // Hook Buffers Up to Shaders!!!
        // populate initial data for neurons
        // populate initial data for axons
        // feed neuronValues data to shader (encapsulate in function since this is ongoing)
        // simulate movements / animation parameters
        // generate neuron triangles
        // generate axon triangles
        SetCoreBrainDataSharedParameters(shaderComputeBrain);

        int initKernelID = shaderComputeBrain.FindKernel("CSInitializeAxonSimData");
        shaderComputeBrain.SetBuffer(initKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(initKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);

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
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);

        int subNeuronTrianglesKernelID = shaderComputeBrain.FindKernel("CSGenerateSubNeuronTriangles");
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);

        int axonTrianglesKernelID = shaderComputeBrain.FindKernel("CSGenerateAxonTriangles");
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.SetBuffer(axonTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);
                
        displayMaterial.SetPass(0);
        displayMaterial.SetBuffer("appendTrianglesBuffer", appendTrianglesCBuffer);   // link computeBuffer to both computeShader and displayShader so they share the same one!!

        shaderComputeBrain.Dispatch(initKernelID, numAxons, 1, 1); // initialize axon positions and attributes
        shaderComputeBrain.Dispatch(simNeuronAttractKernelID, numAxons, 1, 1); // Simulate!! move neuron and axons around
        shaderComputeBrain.Dispatch(simNeuronRepelKernelID, numNeurons, numNeurons, 1); // Simulate!! move neuron and axons around
        shaderComputeBrain.Dispatch(simAxonRepelKernelID, numAxons, numAxons, 1); // Simulate!! move neuron and axons around
        shaderComputeBrain.Dispatch(neuronTrianglesKernelID, numNeurons, 1, 1); // create all triangles from Neurons
        shaderComputeBrain.Dispatch(subNeuronTrianglesKernelID, tempAxonList.Count * 2, 1, 1); // create all triangles for SubNeurons
        shaderComputeBrain.Dispatch(axonTrianglesKernelID, numAxons, 1, 1); // create all geometry for Axons

        SetCoreBrainDataSharedParameters(shaderComputeExtraBalls);

        int positionAxonBallsKernelID = shaderComputeExtraBalls.FindKernel("CSUpdateAxonBallPositions");
        shaderComputeExtraBalls.SetFloat("minRadius", 2f);
        shaderComputeExtraBalls.SetFloat("maxRadius", 4f);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "axonBallCBuffer", axonBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronBallCBuffer", neuronBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "extraBallsCBuffer", extraBallsCBuffer);
        shaderComputeExtraBalls.Dispatch(positionAxonBallsKernelID, numAxonBalls, 1, 1); // initialize axon positions and attributes
        int positionNeuronBallsKernelID = shaderComputeExtraBalls.FindKernel("CSUpdateNeuronBallPositions");
        shaderComputeExtraBalls.SetFloat("minRadius", 2f);
        shaderComputeExtraBalls.SetFloat("maxRadius", 4f);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "axonBallCBuffer", axonBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronBallCBuffer", neuronBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "extraBallsCBuffer", extraBallsCBuffer);
        shaderComputeExtraBalls.Dispatch(positionNeuronBallsKernelID, numNeuronBalls, 1, 1); // initialize axon positions and attributes


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
        SetCoreBrainDataSharedParameters(shaderComputeBrain);
        SetCoreBrainDataSharedParameters(shaderComputeExtraBalls);
        
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

        // Extra BALLS!
        int positionAxonBallsKernelID = shaderComputeExtraBalls.FindKernel("CSUpdateAxonBallPositions");
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "axonBallCBuffer", axonBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "neuronBallCBuffer", neuronBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionAxonBallsKernelID, "extraBallsCBuffer", extraBallsCBuffer);
        shaderComputeExtraBalls.Dispatch(positionAxonBallsKernelID, axonBallCBuffer.count, 1, 1); // initialize axon positions and attributes
        int positionNeuronBallsKernelID = shaderComputeExtraBalls.FindKernel("CSUpdateNeuronBallPositions");
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "axonBallCBuffer", axonBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "neuronBallCBuffer", neuronBallCBuffer);
        shaderComputeExtraBalls.SetBuffer(positionNeuronBallsKernelID, "extraBallsCBuffer", extraBallsCBuffer);
        shaderComputeExtraBalls.Dispatch(positionNeuronBallsKernelID, neuronBallCBuffer.count, 1, 1); // initialize axon positions and attributes

        extraBallsMaterial.SetPass(0);
        //extraBallsMaterial.SetBuffer("quadVerticesCBuffer", quadVerticesCBuffer);
        extraBallsMaterial.SetBuffer("extraBallsCBuffer", extraBallsCBuffer);

        // Re-Generate TRIANGLES!
        // SET UP GEO BUFFER and REFS:::::
        if (appendTrianglesCBuffer != null)
            appendTrianglesCBuffer.Release();
        //Debug.Log("Max Tris: " + (numNeurons * maxTrisPerNeuron + numAxons * maxTrisPerAxon).ToString());
        //appendTrianglesCBuffer = new ComputeBuffer(numNeurons * maxTrisPerNeuron + numAxons * maxTrisPerAxon, sizeof(float) * 45, ComputeBufferType.Append); // vector3 position * 3 verts
        int maxTris = numNeurons * maxTrisPerNeuron + tempAxonList.Count * maxTrisPerAxon + maxTrisPerSubNeuron * tempAxonList.Count * 2;
        //Debug.Log("Max Tris: " + maxTris.ToString());
        appendTrianglesCBuffer = new ComputeBuffer(maxTris, sizeof(float) * 45, ComputeBufferType.Append); // vector3 position * 3 verts
        appendTrianglesCBuffer.SetCounterValue(0);

        int neuronTrianglesKernelID = shaderComputeBrain.FindKernel("CSGenerateNeuronTriangles");
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        //shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        //shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.SetBuffer(neuronTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);

        int subNeuronTrianglesKernelID = shaderComputeBrain.FindKernel("CSGenerateSubNeuronTriangles");
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "neuronInitDataCBuffer", neuronInitDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "neuronFeedDataCBuffer", neuronFeedDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "neuronSimDataCBuffer", neuronSimDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "axonInitDataCBuffer", axonInitDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "axonSimDataCBuffer", axonSimDataCBuffer);
        shaderComputeBrain.SetBuffer(subNeuronTrianglesKernelID, "appendTrianglesCBuffer", appendTrianglesCBuffer);

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
        shaderComputeBrain.Dispatch(subNeuronTrianglesKernelID, tempAxonList.Count * 2, 1, 1); // create all triangles for SubNeurons
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
        Graphics.DrawProceduralIndirect(MeshTopology.Points, argsCBuffer, 0);  // not sure why at this used to work with Triangles but now requires Points....
        //Graphics.DrawProceduralIndirect(MeshTopology.Triangles, argsCBuffer, 0);

        floatingGlowyBitsMaterial.SetPass(0);
        //floatingGlowyBitsMaterial.SetBuffer("floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);
        Graphics.DrawProcedural(MeshTopology.Triangles, 6, floatingGlowyBitsCBuffer.count);

        extraBallsMaterial.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Triangles, 6, extraBallsCBuffer.count);
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
        if (extraBallsCBuffer != null)
            extraBallsCBuffer.Release();
        if (axonBallCBuffer != null)
            axonBallCBuffer.Release();
        if (neuronBallCBuffer != null)
            neuronBallCBuffer.Release();
    }
}
