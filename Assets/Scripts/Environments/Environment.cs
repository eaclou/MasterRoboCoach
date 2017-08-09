using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {
    // Genome holds a master copy of the environment.
    // EvaluationInstance will create a clone of this component & hierarchy, hopefully preserving the references to things like targetPosition
    // Will maybe create an environmentConstructor class to do the actual creation process? - called by EvaluationInstance

    //public EnvironmentGenome genome;
    //public List<GameObject> gameObjectsList; 
    //public GameObject staticCollision;

    // Environment needs to keep references to its child objects so that they can be manipulated or enhanced with renderable parts later
    public GameObject groundCollision;
    public GameObject groundRenderable;
    public GameObject groundVista;
    public List<GameObject> arenaWalls;
    public List<GameObject> obstacles;
    public TargetColumn targetColumn;
    public Material groundMaterial;  // will need to pool these eventually
    public PhysicMaterial groundPhysicMaterial;
    // public List<GameObject> cosmeticDebris;
    // etc.

    // Use this for initialization
    void Start () {
        //Debug.Log("New Environment!");
    }	

    //public void SetGenome(EnvironmentGenome genome) {
    //    this.genome = genome;
    //}

    public void ResetDynamicContent(EnvironmentGenome genome) {
        // Resets all dynamic objects (and possibly meshes) back to their initial conditions, to allow for another evaluation without 
        // having to rebuild anything
    }

    public void AddRenderableContent(EnvironmentGenome genome) {
        // Adds meshRenderer & meshFilter components to all environmental objects
        // assumes that the instance already has minimum collision objects created

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(genome.terrainGenome.color.x, genome.terrainGenome.color.y, genome.terrainGenome.color.z);

        groundRenderable = new GameObject("groundRenderable");
        Mesh topology = GetTerrainMesh(genome, 100, 100, Challenge.GetChallengeArenaBounds(genome.challengeType).x, Challenge.GetChallengeArenaBounds(genome.challengeType).z);
        groundRenderable.AddComponent<MeshFilter>().sharedMesh = topology;
        groundRenderable.AddComponent<MeshRenderer>().material = mat;
        groundRenderable.transform.parent = gameObject.transform;
        groundRenderable.transform.localPosition = new Vector3(0f, 0f, 0f);

        groundVista = new GameObject("groundVista");
        Mesh vistaTopology = GetTerrainMesh(genome, 100, 100, Challenge.GetChallengeArenaBounds(genome.challengeType).x * 10, Challenge.GetChallengeArenaBounds(genome.challengeType).z * 10);
        groundVista.AddComponent<MeshFilter>().sharedMesh = vistaTopology;
        groundVista.AddComponent<MeshRenderer>().material = mat;
        groundVista.transform.parent = gameObject.transform;
        groundVista.transform.localPosition = new Vector3(0f, 0f, 0f);        

        // Target !!!
        if (genome.useTargetColumn) {
            targetColumn.GetComponent<MeshRenderer>().enabled = true; // hide
        }        

        // Obstacles:
        if(genome.useBasicObstacles) {
            for (int i = 0; i < obstacles.Count; i++) {
                obstacles[i].GetComponent<MeshRenderer>().material = mat;
                obstacles[i].GetComponent<MeshRenderer>().enabled = true; // reveal
            }
        }        
    }

    public void CreateCollisionAndGameplayContent(EnvironmentGenome genome) {
        // Construct Ground Physics Material:
        PhysicMaterial noFriction = new PhysicMaterial();
        noFriction.dynamicFriction = 0f;
        noFriction.staticFriction = 0f;
        noFriction.frictionCombine = PhysicMaterialCombine.Minimum;

        if(genome.useTerrain) {
            groundCollision = new GameObject("ground");
            Mesh topology = GetTerrainMesh(genome, 32, 32, Challenge.GetChallengeArenaBounds(genome.challengeType).x, Challenge.GetChallengeArenaBounds(genome.challengeType).z);
            groundCollision.AddComponent<MeshCollider>().sharedMesh = topology;
            groundCollision.transform.parent = gameObject.transform;
            groundCollision.transform.localPosition = new Vector3(0f, 0f, 0f);
            //ground.GetComponent<Collider>().material = noFriction;
        }        

        //=============WALLS===========
        arenaWalls = new List<GameObject>();

        GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        northWall.transform.parent = gameObject.transform;
        northWall.transform.localPosition = new Vector3(0f, 0f, genome.arenaBounds.z * 0.5f);
        northWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        northWall.GetComponent<MeshRenderer>().enabled = false;
        northWall.GetComponent<Collider>().material = noFriction;
        northWall.tag = "hazard";
        arenaWalls.Add(northWall);

        GameObject southWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        southWall.transform.parent = gameObject.transform;
        southWall.transform.localPosition = new Vector3(0f, 0f, -genome.arenaBounds.z * 0.5f);
        southWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        southWall.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        southWall.GetComponent<MeshRenderer>().enabled = false;
        southWall.GetComponent<Collider>().material = noFriction;
        southWall.tag = "hazard";
        arenaWalls.Add(southWall);

        GameObject eastWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        eastWall.transform.parent = gameObject.transform;
        eastWall.transform.localPosition = new Vector3(genome.arenaBounds.x * 0.5f, 0f, 0f);
        eastWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        eastWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        eastWall.GetComponent<MeshRenderer>().enabled = false;
        eastWall.GetComponent<Collider>().material = noFriction;
        eastWall.tag = "hazard";
        arenaWalls.Add(eastWall);

        GameObject westWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        westWall.transform.parent = gameObject.transform;
        westWall.transform.localPosition = new Vector3(-genome.arenaBounds.x * 0.5f, 0f, 0f);
        westWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        westWall.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        westWall.GetComponent<MeshRenderer>().enabled = false;
        westWall.GetComponent<Collider>().material = noFriction;
        westWall.tag = "hazard";
        arenaWalls.Add(westWall);

        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ceiling.transform.parent = gameObject.transform;
        ceiling.transform.localPosition = new Vector3(0f, genome.arenaBounds.y * 0.5f, 0f);
        ceiling.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, 1f);
        ceiling.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        ceiling.GetComponent<MeshRenderer>().enabled = false;
        ceiling.GetComponent<Collider>().material = noFriction;
        ceiling.tag = "hazard";
        arenaWalls.Add(ceiling);

        // ======================================================================

        // Game-Required Modules:
        // Target !!!
        if (genome.useTargetColumn) {
            GameObject targetColumnGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            targetColumnGO.transform.parent = gameObject.transform;
            targetColumn = targetColumnGO.AddComponent<TargetColumn>();
            targetColumn.GetComponent<MeshRenderer>().enabled = false; // hide
            targetColumn.Initialize(genome.targetColumnGenome);
        }
        
        // Obstacles:
        if (genome.useBasicObstacles) {
            obstacles = new List<GameObject>();
            for (int i = 0; i < genome.basicObstaclesGenome.obstaclePositions.Length; i++) {
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                obstacle.transform.parent = gameObject.transform;
                float x = genome.basicObstaclesGenome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f;
                float z = genome.basicObstaclesGenome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f;
                float y = GetAltitude(genome, x, z) + 0.5f;
                obstacle.transform.localScale = new Vector3(genome.basicObstaclesGenome.obstacleScales[i], 1f, genome.basicObstaclesGenome.obstacleScales[i]);
                obstacle.transform.localPosition = new Vector3(x, y, z);
                obstacle.GetComponent<Collider>().material = noFriction;
                obstacle.tag = "hazard";
                obstacle.GetComponent<MeshRenderer>().enabled = false; // hide
                obstacles.Add(obstacle);
            }
        }        

        // Set Genome's prefab environment:
        genome.environmentPrefab = this;
    }

    private float GetAltitude(EnvironmentGenome genome, float x, float z) {
        float total = 0f;
        for(int i = 0; i < genome.terrainGenome.terrainWaves.Length; i++) {
            if(i % 2 == 0) {
                total += Mathf.Sin(x * genome.terrainGenome.terrainWaves[i].x + genome.terrainGenome.terrainWaves[i].z) * genome.terrainGenome.terrainWaves[i].y;
            }
            else {
                total += Mathf.Sin(z * genome.terrainGenome.terrainWaves[i].x + genome.terrainGenome.terrainWaves[i].z) * genome.terrainGenome.terrainWaves[i].y;
            }
        }
        float height = total / genome.terrainGenome.terrainWaves.Length;
        float distToSpawn = new Vector2(x, z).magnitude;
        if (distToSpawn < 12f) {
            height = Mathf.Lerp(0f, height, Mathf.Max(distToSpawn - 4f, 0f) / 8f);
        }
        return height;        
    }

    private Mesh GetTerrainMesh(EnvironmentGenome genome, int xResolution, int zResolution, float xSize, float zSize) {
        
        //int xResolution = 32;
        //int zResolution = 32;
        float xQuadSize = xSize / (float)xResolution;
        float zQuadSize = zSize / (float)zResolution;
        Vector3 offset = new Vector3(xSize * 0.5f, 0f, zSize * 0.5f);

        Vector3[] vertices;

        Mesh mesh = new Mesh();

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xResolution + 1) * (zResolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zResolution; z++) {
            for (int x = 0; x <= xResolution; x++, i++) {
                float altitude = GetAltitude(genome, x * xQuadSize - offset.x, z * zQuadSize - offset.z);
                vertices[i] = new Vector3(x * xQuadSize, altitude, z * zQuadSize) - offset;
                uv[i] = new Vector2((float)x / xResolution, (float)z / zResolution);
                tangents[i] = tangent;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xResolution * zResolution * 6];
        for (int ti = 0, vi = 0, z = 0; z < zResolution; z++, vi++) {
            for (int x = 0; x < xResolution; x++, ti += 6, vi++) {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xResolution + 1;
                triangles[ti + 5] = vi + xResolution + 2;                
            }
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
}
