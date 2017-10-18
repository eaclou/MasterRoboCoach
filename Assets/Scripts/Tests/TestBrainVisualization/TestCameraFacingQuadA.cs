using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraFacingQuadA : MonoBehaviour {

    public Material floatingGlowyBitsMaterial;

    private ComputeBuffer quadVerticesCBuffer;  // holds information for a 2-triangle Quad mesh (6 vertices)
    private ComputeBuffer floatingGlowyBitsCBuffer;  // holds information for placement and attributes of each instance of quadVertices to draw

    // Use this for initialization
    void Start () {

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
        for (int i = 0; i < numFloatingGlowyBits; i++) {
            initialGlowyBitsPositions[i] = UnityEngine.Random.insideUnitSphere * 2f;
        }
        floatingGlowyBitsCBuffer = new ComputeBuffer(numFloatingGlowyBits, sizeof(float) * 3);
        floatingGlowyBitsCBuffer.SetData(initialGlowyBitsPositions);
        floatingGlowyBitsMaterial.SetPass(0);
        floatingGlowyBitsMaterial.SetBuffer("floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);
        floatingGlowyBitsMaterial.SetBuffer("quadVerticesCBuffer", quadVerticesCBuffer);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnRenderObject() {        
        floatingGlowyBitsMaterial.SetPass(0);
        floatingGlowyBitsMaterial.SetBuffer("floatingGlowyBitsCBuffer", floatingGlowyBitsCBuffer);
        Graphics.DrawProcedural(MeshTopology.Triangles, 6, floatingGlowyBitsCBuffer.count);
    }

    private void OnDestroy() {        
        if (floatingGlowyBitsCBuffer != null)
            floatingGlowyBitsCBuffer.Release();
        if (quadVerticesCBuffer != null)
            quadVerticesCBuffer.Release();

    }
}
