using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTerrainManagerLOD : MonoBehaviour {

    public TerrainManager terrainManagerRef;
    public ComputeShader terrainConstructorGPUCompute;
    public Material groundMat;
    private EnvironmentGenome envGenome;

    public GameObject TextureDisplayQuadGO;

    RenderTexture displayTexture;

	// Use this for initialization
	void Start () {

        envGenome = (Resources.Load("Templates/Environments/TemplateTestDefault") as EnvironmentGenomeTemplate).templateGenome;

        // original:
        //terrainManager.Initialize(terrainManagerGO, genome, mat, new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), new Vector2(Challenge.GetChallengeArenaBounds(genome.challengeType).x * 17f, Challenge.GetChallengeArenaBounds(genome.challengeType).z * 17f), 6);

        TerrainConstructorGPU.terrainConstructorGPUCompute = this.terrainConstructorGPUCompute;
        
        //Mesh groundMesh = TerrainConstructorGPU.GetTerrainMesh(envGenome, 8, 8, 0, 0,160, 160);
        //this.GetComponent<MeshFilter>().sharedMesh = groundMesh;

        terrainManagerRef.Initialize(this.gameObject, envGenome, groundMat, new Vector2(0f, 0f), new Vector2(680f, 680f), 6);

        //displayTexture = TerrainConstructorGPU.mainRenderTexture;
        //TextureDisplayQuadGO.GetComponent<MeshRenderer>().material.SetTexture("_MainTex", displayTexture);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
