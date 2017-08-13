using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainManager : MonoBehaviour {

    public TerrainChunk rootChunk;
    public Vector2 position;
    public Vector2 scale;

    public Material groundMaterial;

    public int maxLOD;

    // Use this for initialization
    void Start () {

        Debug.Log("Noise! " + NoisePrime.Simplex2D(Vector3.one, 1f).value.ToString());

        //Debug.Log("TerrainManager!");
        //maxLOD = 4;
        // whip up an environmentGenome:
        /*EnvironmentGenome genome = new EnvironmentGenome(0);
        EnvironmentGenomeTemplate template = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateTestDefault.asset", typeof(EnvironmentGenomeTemplate)));
        genome.CopyGenomeFromTemplate(template.templateGenome);
        this.Initialize(gameObject, genome, groundMaterial, new Vector2(0f, 0f), new Vector2(320f, 320f), 5);
        */
        /*
        Initialize(EnvironmentGenome genome, Material mat, Vector2 position, Vector2 scale, int maxLOD);
        GameObject rootChunkGO = new GameObject("rootChunk");
        rootChunk = rootChunkGO.AddComponent<TerrainChunk>();
        rootChunk.maxLOD = this.maxLOD;
        position = new Vector2(0f, 0f);
        scale = new Vector2(320f, 320f);
        rootChunk.Initialize(rootChunk, genome, 0, 0, 16, 16, new Vector2(40f, 40f), position, scale, groundMaterial);
        */
    }

    // Update is called once per frame
    void Update () {
		
	}

    public void Initialize(GameObject parentGO, EnvironmentGenome genome, Material mat, Vector2 position, Vector2 scale, int maxLOD) {
        this.maxLOD = maxLOD;
        this.groundMaterial = mat;
        this.position = position;
        this.scale = scale;

        gameObject.transform.parent = parentGO.transform;

                
        GameObject rootChunkGO = new GameObject("rootChunk");
        rootChunkGO.transform.parent = parentGO.transform;
        rootChunk = rootChunkGO.AddComponent<TerrainChunk>();
        rootChunk.maxLOD = this.maxLOD;
        //position = new Vector2(0f, 0f);
        //scale = new Vector2(320f, 320f);
        rootChunk.maxLOD = this.maxLOD;
        rootChunk.Initialize(gameObject, genome, 0, 0, 32, 32, new Vector2(40f, 40f), position, scale, groundMaterial);
    }
}
