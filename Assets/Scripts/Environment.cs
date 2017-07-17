using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour {

    public EnvironmentGenome genome;

    public List<GameObject> gameObjectsList;

    // THERE'S A BETTER WAY:
    public TargetColumn targetColumn;

    // Use this for initialization
    void Start () {
        //Debug.Log("New Environment!");
    }	

    public void SetGenome(EnvironmentGenome genome) {
        this.genome = genome;
    }

    public void ConstructEnvironmentFromGenome() {
        if (gameObjectsList == null) {
            gameObjectsList = new List<GameObject>();
        }
        else {
            gameObjectsList.Clear();
        }

        // Construct Ground:
        PhysicMaterial noFriction = new PhysicMaterial();
        noFriction.dynamicFriction = 0f;
        noFriction.staticFriction = 0f;
        noFriction.frictionCombine = PhysicMaterialCombine.Minimum;        

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(genome.color.x, genome.color.y, genome.color.z);        

        GameObject ground = new GameObject("terrain");        
        Mesh topology = GetTerrainMesh();
        ground.AddComponent<MeshFilter>().sharedMesh = topology;        
        ground.AddComponent<MeshCollider>().sharedMesh = topology;
        ground.AddComponent<MeshRenderer>();
        gameObjectsList.Add(ground);
        ground.transform.parent = gameObject.transform;
        ground.transform.localPosition = new Vector3(0f, 0f, 0f);
        ground.GetComponent<Collider>().material = noFriction;
        ground.GetComponent<MeshRenderer>().material = mat;

        //=============END GROUND===========
        GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gameObjectsList.Add(northWall);
        northWall.transform.parent = gameObject.transform;
        northWall.transform.localPosition = new Vector3(0f, 0f, genome.arenaBounds.z * 0.5f);
        northWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        northWall.GetComponent<MeshRenderer>().enabled = false;
        northWall.GetComponent<Collider>().material = noFriction;
        northWall.tag = "hazard";

        GameObject southWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gameObjectsList.Add(southWall);
        southWall.transform.parent = gameObject.transform;
        southWall.transform.localPosition = new Vector3(0f, 0f, -genome.arenaBounds.z * 0.5f);
        southWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        southWall.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        southWall.GetComponent<MeshRenderer>().enabled = false;
        southWall.GetComponent<Collider>().material = noFriction;
        southWall.tag = "hazard";

        GameObject eastWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gameObjectsList.Add(eastWall);
        eastWall.transform.parent = gameObject.transform;
        eastWall.transform.localPosition = new Vector3(genome.arenaBounds.x * 0.5f, 0f, 0f);
        eastWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        eastWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        eastWall.GetComponent<MeshRenderer>().enabled = false;
        eastWall.GetComponent<Collider>().material = noFriction;
        eastWall.tag = "hazard";

        GameObject westWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        gameObjectsList.Add(westWall);
        westWall.transform.parent = gameObject.transform;
        westWall.transform.localPosition = new Vector3(-genome.arenaBounds.x * 0.5f, 0f, 0f);
        westWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        westWall.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        westWall.GetComponent<MeshRenderer>().enabled = false;
        westWall.GetComponent<Collider>().material = noFriction;
        westWall.tag = "hazard";

        // Game-Required Modules:
        if (genome.targetColumnGenome != null) {
            GameObject targetColumnGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            targetColumnGO.transform.parent = gameObject.transform;
            targetColumn = targetColumnGO.AddComponent<TargetColumn>();
            targetColumn.Initialize(genome.targetColumnGenome);
            //targetColumn.GetComponent<Collider>().enabled = false;            
        }

        // Obstacles:
        for(int i = 0; i < genome.obstaclePositions.Length; i++) {
            GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            gameObjectsList.Add(obstacle);
            obstacle.GetComponent<MeshRenderer>().material = mat;
            obstacle.transform.parent = gameObject.transform;
            float x = genome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f;
            float z = genome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f;
            float y = GetAltitude(x, z);
            obstacle.transform.localScale = new Vector3(genome.obstacleScales[i], 5f, genome.obstacleScales[i]);
            obstacle.transform.localPosition = new Vector3(x, y, z);
            obstacle.GetComponent<Collider>().material = noFriction;
            obstacle.tag = "hazard";
        }
    }

    private float GetAltitude(float x, float z) {
        float total = 0f;
        for(int i = 0; i < genome.terrainWaves.Length; i++) {
            if(i % 2 == 0) {
                total += Mathf.Sin(x * genome.terrainWaves[i].x + genome.terrainWaves[i].z) * genome.terrainWaves[i].y;
            }
            else {
                total += Mathf.Sin(z * genome.terrainWaves[i].x + genome.terrainWaves[i].z) * genome.terrainWaves[i].y;
            }
        }
        float height = total / genome.terrainWaves.Length;
        float distToSpawn = new Vector2(x, z).magnitude;
        if (distToSpawn < 12f) {
            height = Mathf.Lerp(0f, height, Mathf.Max(distToSpawn - 4f, 0f) / 8f);
        }
        return height;
    }

    private Mesh GetTerrainMesh() {
        int xResolution = 32;
        int zResolution = 32;
        float xSize = Challenge.GetChallengeArenaBounds(genome.challengeType).x / (float)xResolution;
        float zSize = Challenge.GetChallengeArenaBounds(genome.challengeType).z / (float)zResolution;
        Vector3 offset = new Vector3(Challenge.GetChallengeArenaBounds(genome.challengeType).x * 0.5f, 0f, Challenge.GetChallengeArenaBounds(genome.challengeType).z * 0.5f);

        Vector3[] vertices;

        Mesh mesh = new Mesh();

        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xResolution + 1) * (zResolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0, z = 0; z <= zResolution; z++) {
            for (int x = 0; x <= xResolution; x++, i++) {
                float altitude = GetAltitude(x * xSize - offset.x, z * zSize - offset.z);
                vertices[i] = new Vector3(x * xSize, altitude, z * zSize) - offset;
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
