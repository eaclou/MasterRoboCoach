using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainChunk : MonoBehaviour {

    int id;
    public List<int> idList;
    public int lod;
    public int maxLOD;

    public int resolutionX;
    public int resolutionZ;

    public float thisNorth;
    public float thisEast;
    public float thisSouth;
    public float thisWest;

    public Mesh mesh;

    public GameObject parentChunk;

    public TerrainChunk chunkNE;   
    public TerrainChunk chunkSE;
    public TerrainChunk chunkSW;
    public TerrainChunk chunkNW;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool CheckForContainsRect(float rectNorth, float rectEast, float rectSouth, float rectWest) {
        bool intersection = false;

        // North:
        if(rectSouth < thisNorth) {
            intersection = true;
            if (rectWest > thisEast) {
                intersection = false;
            }
            if (rectEast < thisWest) {
                intersection = false;
            }
            if (rectNorth < thisSouth) {
                intersection = false;
            }
        }

        // East:
        if (rectWest < thisEast) {
            intersection = true;
            if (rectSouth > thisNorth) {
                intersection = false;
            }
            if (rectEast < thisWest) {
                intersection = false;
            }
            if (rectNorth < thisSouth) {
                intersection = false;
            }
        }

        // South:
        if (rectNorth > thisSouth) {
            intersection = true;
            if (rectWest > thisEast) {
                intersection = false;
            }
            if (rectEast < thisWest) {
                intersection = false;
            }
            if (rectSouth > thisNorth) {
                intersection = false;
            }
        }

        // West:
        if (rectEast > thisWest) {
            intersection = true;
            if (rectSouth > thisNorth) {
                intersection = false;
            }
            if (rectWest > thisEast) {
                intersection = false;
            }
            if (rectNorth < thisSouth) {
                intersection = false;
            }
        }

        return intersection;
    }

    public void Initialize(GameObject parentChunk, EnvironmentGenome genome, int id, int lod, int resolutionX, int resolutionZ, Vector2 arenaBounds, Vector2 position, Vector2 scale, Material mat) {
        
        this.parentChunk = parentChunk;               
        this.id = id;
        this.resolutionX = resolutionX;
        this.resolutionZ = resolutionZ;
        this.lod = lod;
        if(idList == null) {
            idList = new List<int>();
        }
        idList.Add(id);

        // Calculate bounds of this chunk:
        thisNorth = position.y + scale.y;
        thisEast = position.x + scale.x;
        thisSouth = position.y - scale.y;
        thisWest = position.x - scale.x;

        bool end = false;
        if(!this.CheckForContainsRect(arenaBounds.y * 0.5f, arenaBounds.x * 0.5f, -arenaBounds.y * 0.5f, -arenaBounds.x * 0.5f)) {
            end = true;
        }
        if(this.lod >= maxLOD) {
            end = true;
        }
        //Debug.Log("TerrainManager! id: " + id.ToString() + ", lod: " + lod.ToString() + ", pos: " + position.ToString() + ", scale: " + scale.ToString() + " end: " + end.ToString());
        if (end) {
            // Build!
            BuildChunk(genome, position, scale, mat);
        }
        else {
            
            //Debug.Log("Split!");
            // Split!
            //NE:            
            int childID = 1;  // 0 Center 1 NE,  2 SE,  3 SW,  4 NW
            GameObject chunkNEGO = new GameObject("chunk_" + childID.ToString() + "." + lod.ToString());
            chunkNE = chunkNEGO.AddComponent<TerrainChunk>();
            chunkNEGO.transform.parent = gameObject.transform;
            chunkNEGO.transform.localPosition = new Vector3(scale.x * 0.5f, 0f, scale.y * 0.5f);
            chunkNE.maxLOD = this.maxLOD;
            chunkNE.Initialize(gameObject, genome, childID, this.lod + 1, this.resolutionX, this.resolutionZ, arenaBounds, position + (scale * 0.5f), scale * 0.5f, mat);

            //SE:            
            childID = 2;  // 0 NE,  1 SE,  2 SW,  3 NW
            GameObject chunkSEGO = new GameObject("chunk_" + childID.ToString() + "." + lod.ToString());
            chunkSE = chunkSEGO.AddComponent<TerrainChunk>();
            chunkSEGO.transform.parent = gameObject.transform;
            chunkSEGO.transform.localPosition = new Vector3(scale.x * 0.5f, 0f, -scale.y * 0.5f);
            chunkSE.maxLOD = this.maxLOD;
            chunkSE.Initialize(gameObject, genome, childID, this.lod + 1, this.resolutionX, this.resolutionZ, arenaBounds, new Vector2(position.x + (scale.x * 0.5f), position.y - (scale.y * 0.5f)), scale * 0.5f, mat);

            //SE:            
            childID = 3;  // 0 NE,  1 SE,  2 SW,  3 NW
            GameObject chunkSWGO = new GameObject("chunk_" + childID.ToString() + "." + lod.ToString());
            chunkSW = chunkSWGO.AddComponent<TerrainChunk>();
            chunkSWGO.transform.parent = gameObject.transform;
            chunkSWGO.transform.localPosition = new Vector3(-scale.x * 0.5f, 0f, -scale.y * 0.5f);
            chunkSW.maxLOD = this.maxLOD;
            chunkSW.Initialize(gameObject, genome, childID, this.lod + 1, this.resolutionX, this.resolutionZ, arenaBounds, new Vector2(position.x - (scale.x * 0.5f), position.y - (scale.y * 0.5f)), scale * 0.5f, mat);

            //SE:            
            childID = 4;  // 0 NE,  1 SE,  2 SW,  3 NW
            GameObject chunkNWGO = new GameObject("chunk_" + childID.ToString() + "." + lod.ToString());
            chunkNW = chunkNWGO.AddComponent<TerrainChunk>();
            chunkNWGO.transform.parent = gameObject.transform;
            chunkNWGO.transform.localPosition = new Vector3(-scale.x * 0.5f, 0f, scale.y * 0.5f);
            chunkNW.maxLOD = this.maxLOD;
            chunkNW.Initialize(gameObject, genome, childID, this.lod + 1, this.resolutionX, this.resolutionZ, arenaBounds, new Vector2(position.x - (scale.x * 0.5f), position.y + (scale.y * 0.5f)), scale * 0.5f, mat);
            
        }
    }

    public void BuildChunk(EnvironmentGenome genome, Vector2 position, Vector2 scale, Material mat) {
        GameObject chunkGO = new GameObject("chunkMesh");
        chunkGO.AddComponent<MeshFilter>().sharedMesh = TerrainConstructor.GetTerrainMesh(genome, 32, 32, position.x, position.y, scale.x * 2f, scale.y *2);
        chunkGO.AddComponent<MeshRenderer>().material = mat;
        chunkGO.transform.parent = gameObject.transform;
        chunkGO.transform.localPosition = new Vector3(0f, 0f, 0f);
        //chunkGO.transform.localScale = new Vector3(scale.x, 1f, scale.y);
        
    }
}
