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
    public EnvironmentGameplay environmentGameplay;
    public EnvironmentRenderable environmentRenderable;

    
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
        // Wrappe Object:
        GameObject environmentRenderableGO = new GameObject("environmentRenderable");
        environmentRenderable = environmentRenderableGO.AddComponent<EnvironmentRenderable>();
        environmentRenderable.transform.parent = gameObject.transform;
        environmentRenderable.transform.localPosition = new Vector3(0f, 0f, 0f);

        Material mat = Resources.Load("Materials/Environments/terrainDefault", typeof(Material)) as Material;  //new Material(Shader.Find("Standard"));
        mat.color = new Color(genome.terrainGenome.color.x, genome.terrainGenome.color.y, genome.terrainGenome.color.z);

        GameObject terrainManagerGO = new GameObject("terrainManager");
        terrainManagerGO.transform.parent = environmentRenderable.transform;
        terrainManagerGO.transform.localPosition = Vector3.zero;
        TerrainManager terrainManager = terrainManagerGO.AddComponent<TerrainManager>();
        //Debug.Log("TERRAIN BUILD!");

        

        // Set Data on Constructor:
        //TerrainConstructorGPU.terrainConstructorGPUCompute = this.terrainConstructorGPUCompute; // set in GameManager Start()
        // Set Cascade Height Textures:
        //TerrainConstructorGPU.heightMapCascadeTextures = heightMapCascadeTextures;
        TerrainConstructorGPU.GenerateTerrainTexturesFromGenome(genome);

        terrainManager.Initialize(terrainManagerGO, genome, mat, new Vector2(gameObject.transform.position.x, gameObject.transform.position.z), new Vector2(Challenge.GetChallengeArenaBounds(genome.challengeType).x * 17f, Challenge.GetChallengeArenaBounds(genome.challengeType).z * 17f), 6);

        /*environmentRenderable.groundRenderable = new GameObject("groundRenderable");
        Mesh topology = GetTerrainMesh(genome, 100, 100, Challenge.GetChallengeArenaBounds(genome.challengeType).x, Challenge.GetChallengeArenaBounds(genome.challengeType).z);
        environmentRenderable.groundRenderable.AddComponent<MeshFilter>().sharedMesh = topology;
        environmentRenderable.groundRenderable.AddComponent<MeshRenderer>().material = mat;
        environmentRenderable.groundRenderable.transform.parent = environmentRenderable.gameObject.transform;
        environmentRenderable.groundRenderable.transform.localPosition = new Vector3(0f, 0f, 0f); */

        // Target !!!
        if (genome.useTargetColumn) {
            GameObject targetColumn = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/targetLocation")) as GameObject;
            targetColumn.transform.parent = environmentRenderable.gameObject.transform;
            targetColumn.transform.localPosition = environmentGameplay.targetColumn.gameObject.transform.localPosition;
            //environmentGameplay.targetColumn.GetComponent<MeshRenderer>().enabled = true; // hide
        }        

        // Obstacles:
        if(genome.useBasicObstacles) {
            for (int i = 0; i < environmentGameplay.obstacles.Count; i++) {
                GameObject obstacle;
                int meshID = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f));
                
                if(meshID == 0) {
                    obstacle = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder00")) as GameObject;
                }
                else if (meshID == 1) {
                    obstacle = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder01")) as GameObject;
                }
                else {
                    obstacle = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder02")) as GameObject;
                }
                //obstacle.GetComponent<MeshFilter>().sharedMesh = boulderMesh;
                obstacle.transform.parent = environmentRenderable.gameObject.transform;
                obstacle.transform.localPosition = environmentGameplay.obstacles[i].gameObject.transform.localPosition - new Vector3(0f, UnityEngine.Random.Range(0f, 1f), 0f);
                obstacle.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-10f, 10f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-10f, 10f));
                obstacle.transform.localScale = environmentGameplay.obstacles[i].gameObject.transform.localScale + new Vector3(0f, UnityEngine.Random.Range(0f, 1f), 0f);
                obstacle.GetComponent<MeshRenderer>().material = mat;
                //environmentGameplay.obstacles[i].GetComponent<MeshRenderer>().material = mat;
                //environmentGameplay.obstacles[i].GetComponent<MeshRenderer>().enabled = true; // reveal

                //  SUB-BOULDERS!!!
                int numSubBoulders = Mathf.RoundToInt(UnityEngine.Random.Range(2f, 4f));
                for (int j = 0; j < numSubBoulders; j++) {
                    GameObject subBoulder;
                    int boulderID = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f));
                    if (boulderID == 0) {
                        subBoulder = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder00")) as GameObject;
                    }
                    else if (boulderID == 1) {
                        subBoulder = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder01")) as GameObject;
                    }
                    else {
                        subBoulder = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder02")) as GameObject;
                    }
                    float randScale = UnityEngine.Random.Range(0.2f, 0.6f);
                    //float randScaleY = UnityEngine.Random.Range(0.2f, 0.6f);
                    //float randScaleZ = UnityEngine.Random.Range(0.2f, 0.6f);
                    subBoulder.transform.parent = environmentRenderable.gameObject.transform;

                    float radius = UnityEngine.Random.Range(environmentGameplay.obstacles[i].gameObject.transform.localScale.x * 0.5f, environmentGameplay.obstacles[i].gameObject.transform.localScale.x * 0.95f);
                    float randAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
                    float xPos = Mathf.Sin(randAngle) * radius;
                    float zPos = Mathf.Cos(randAngle) * radius;
                    float yPos = TerrainConstructor.GetAltitude(genome, xPos + environmentGameplay.obstacles[i].gameObject.transform.localPosition.x, zPos + environmentGameplay.obstacles[i].gameObject.transform.localPosition.z);
                    Vector3 pos = new Vector3(Mathf.Sin(randAngle) * radius + environmentGameplay.obstacles[i].gameObject.transform.localPosition.x, yPos - UnityEngine.Random.Range(randScale * 0.05f, randScale * 0.2f) * environmentGameplay.obstacles[i].gameObject.transform.localScale.y, Mathf.Cos(randAngle) + environmentGameplay.obstacles[i].gameObject.transform.localPosition.z);
                    
                    subBoulder.transform.localPosition = pos;
                    subBoulder.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f));
                    Vector3 scale = new Vector3(environmentGameplay.obstacles[i].gameObject.transform.localScale.x * randScale, environmentGameplay.obstacles[i].gameObject.transform.localScale.y * randScale, environmentGameplay.obstacles[i].gameObject.transform.localScale.z * randScale);
                    subBoulder.transform.localScale = scale;
                    subBoulder.GetComponent<MeshRenderer>().material = mat;
                }

                //  PEBBLES!!!
                int numPebbles = Mathf.RoundToInt(UnityEngine.Random.Range(5f, 20f));
                for (int k = 0; k < numPebbles; k++) {
                    GameObject pebble;
                    int boulderID = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f));
                    if (boulderID == 0) {
                        pebble = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder00")) as GameObject;
                    }
                    else if (boulderID == 1) {
                        pebble = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder01")) as GameObject;
                    }
                    else {
                        pebble = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder02")) as GameObject;
                    }
                    float randScale = UnityEngine.Random.Range(0.05f, 0.25f);
                    //float randScaleY = UnityEngine.Random.Range(0.05f, 0.25f);
                    //float randScaleZ = UnityEngine.Random.Range(0.05f, 0.25f);
                    pebble.transform.parent = environmentRenderable.gameObject.transform;

                    float radius = UnityEngine.Random.Range(environmentGameplay.obstacles[i].gameObject.transform.localScale.x * 0.5f, environmentGameplay.obstacles[i].gameObject.transform.localScale.x * 5f);
                    float randAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
                    float xPos = Mathf.Sin(randAngle) * radius;
                    float zPos = Mathf.Cos(randAngle) * radius;
                    float yPos = TerrainConstructor.GetAltitude(genome, xPos + environmentGameplay.obstacles[i].gameObject.transform.localPosition.x, zPos + environmentGameplay.obstacles[i].gameObject.transform.localPosition.z);
                    Vector3 pos = new Vector3(Mathf.Sin(randAngle) * radius + environmentGameplay.obstacles[i].gameObject.transform.localPosition.x, yPos - UnityEngine.Random.Range(randScale * 0.05f, randScale * 0.2f) * environmentGameplay.obstacles[i].gameObject.transform.localScale.y, Mathf.Cos(randAngle) * radius + environmentGameplay.obstacles[i].gameObject.transform.localPosition.z);
                    pebble.transform.localPosition = pos;
                    pebble.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f));
                    Vector3 scale = new Vector3(environmentGameplay.obstacles[i].gameObject.transform.localScale.x * randScale, environmentGameplay.obstacles[i].gameObject.transform.localScale.y * randScale, environmentGameplay.obstacles[i].gameObject.transform.localScale.z * randScale);
                    pebble.transform.localScale = scale;
                    pebble.GetComponent<MeshRenderer>().material = mat;
                }

                // Exterior Boulders!!!
                //  PEBBLES!!!
                int numVistaRocks = Mathf.RoundToInt(UnityEngine.Random.Range(20f, 40f));
                for (int m = 0; m < numVistaRocks; m++) {
                    GameObject vistaRock;
                    int boulderID = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 2f));
                    if (boulderID == 0) {
                        vistaRock = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder00")) as GameObject;
                    }
                    else if (boulderID == 1) {
                        vistaRock = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder01")) as GameObject;
                    }
                    else {
                        vistaRock = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleBoulder02")) as GameObject;
                    }
                    float randScale = UnityEngine.Random.Range(0.5f, 3.6f);
                    //float randScaleY = UnityEngine.Random.Range(0.05f, 0.25f);
                    //float randScaleZ = UnityEngine.Random.Range(0.05f, 0.25f);
                    vistaRock.transform.parent = environmentRenderable.gameObject.transform;

                    float radius = UnityEngine.Random.Range(Challenge.GetChallengeArenaBounds(Challenge.Type.Test).x * 1.33f, Challenge.GetChallengeArenaBounds(Challenge.Type.Test).x * 4f);
                    float randAngle = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
                    float xPos = Mathf.Sin(randAngle) * radius;
                    float zPos = Mathf.Cos(randAngle) * radius;
                    float yPos = TerrainConstructor.GetAltitude(genome, xPos + environmentGameplay.obstacles[i].gameObject.transform.localPosition.x, zPos + environmentGameplay.obstacles[i].gameObject.transform.localPosition.z);
                    Vector3 pos = new Vector3(xPos, yPos - UnityEngine.Random.Range(randScale * 0.05f, randScale * 0.2f), zPos);
                    vistaRock.transform.localPosition = pos;
                    vistaRock.transform.rotation = Quaternion.Euler(UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f), UnityEngine.Random.Range(-180f, 180f));
                    Vector3 scale = new Vector3(environmentGameplay.obstacles[i].gameObject.transform.localScale.x * randScale, environmentGameplay.obstacles[i].gameObject.transform.localScale.y * randScale, environmentGameplay.obstacles[i].gameObject.transform.localScale.z * randScale);
                    vistaRock.transform.localScale = scale;
                    vistaRock.GetComponent<MeshRenderer>().material = mat;
                }
            }
            // PArticle Pebbles!
            GameObject particlePebbles = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/obstacleParticles")) as GameObject;
            particlePebbles.transform.parent = environmentRenderable.transform;
            ParticleSystem.ShapeModule emitterShape = particlePebbles.GetComponent<ParticleSystem>().shape;
            emitterShape.mesh = environmentGameplay.groundCollision.GetComponent<MeshCollider>().sharedMesh;
            Debug.Log(emitterShape.mesh.ToString());
            ParticleSystem.EmissionModule emission = particlePebbles.GetComponent<ParticleSystem>().emission;
            emission.enabled = true;
        }

        // WALLS:
        GameObject northWall = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/arenaWall")) as GameObject; // GameObject.CreatePrimitive(PrimitiveType.Quad);
        northWall.transform.parent = environmentRenderable.gameObject.transform;
        northWall.transform.localPosition = new Vector3(0f, 0f, genome.arenaBounds.z * 0.5f);
        northWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);

        GameObject southWall = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/arenaWall")) as GameObject;
        southWall.transform.parent = environmentRenderable.gameObject.transform;
        southWall.transform.localPosition = new Vector3(0f, 0f, -genome.arenaBounds.z * 0.5f);
        southWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        southWall.transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        GameObject eastWall = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/arenaWall")) as GameObject;
        eastWall.transform.parent = environmentRenderable.gameObject.transform;
        eastWall.transform.localPosition = new Vector3(genome.arenaBounds.x * 0.5f, 0f, 0f);
        eastWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        eastWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        GameObject westWall = Instantiate(Resources.Load("Prefabs/ObjectPrefabs/arenaWall")) as GameObject;
        westWall.transform.parent = environmentRenderable.gameObject.transform;
        westWall.transform.localPosition = new Vector3(-genome.arenaBounds.x * 0.5f, 0f, 0f);
        westWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        westWall.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
    }

    /*public Mesh DeformMesh(Mesh mesh) {
        Mesh deformedMesh = new Mesh();
        
        List<Vector3> verts = new List<Vector3>();
        for(int i = 0; i < mesh.vertices.Length; i++) {
            float randomExtrude = UnityEngine.Random.Range(-1f, 1f) * 0.05f;
            verts.Add(mesh.vertices[i] + mesh.normals[i] * randomExtrude);
        }
        deformedMesh.SetVertices(verts);
        deformedMesh.triangles = mesh.triangles;
        deformedMesh.normals = mesh.normals;
        deformedMesh.RecalculateNormals();
        return deformedMesh;
    }*/

    public void CreateCollisionAndGameplayContent(EnvironmentGenome genome) {
        GameObject environmentGameplayGO = new GameObject("environmentGameplay");
        environmentGameplay = environmentGameplayGO.AddComponent<EnvironmentGameplay>();
        environmentGameplay.transform.parent = gameObject.transform;
        environmentGameplay.transform.localPosition = new Vector3(0f, 0f, 0f);

        // Construct Ground Physics Material:
        PhysicMaterial noFriction = new PhysicMaterial();
        noFriction.dynamicFriction = 0f;
        noFriction.staticFriction = 0f;
        noFriction.frictionCombine = PhysicMaterialCombine.Minimum;

        if(genome.useTerrain) {
            environmentGameplay.groundCollision = new GameObject("ground");

            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&
            // Need to set Terrain RenderTextureCascade and some other data before getting Collision Mesh:
            //TerrainConstructorGPU.GenerateTerrainTexturesFromGenome();
            // INITIALIZE TERRAIN-COMPUTE::
            
            TerrainConstructorGPU.GenerateTerrainTexturesFromGenome(genome);

            Mesh topology = TerrainConstructorGPU.GetTerrainMesh(32, 32, 0f, 0f, Challenge.GetChallengeArenaBounds(genome.challengeType).x, Challenge.GetChallengeArenaBounds(genome.challengeType).z);
            //&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&

            //Mesh topology = TerrainConstructor.GetTerrainMesh(genome, 32, 32, 0f, 0f, Challenge.GetChallengeArenaBounds(genome.challengeType).x, Challenge.GetChallengeArenaBounds(genome.challengeType).z);
            environmentGameplay.groundCollision.AddComponent<MeshCollider>().sharedMesh = topology;
            environmentGameplay.groundCollision.transform.parent = environmentGameplay.gameObject.transform;
            environmentGameplay.groundCollision.transform.localPosition = new Vector3(0f, 0f, 0f);
            //ground.GetComponent<Collider>().material = noFriction;
        }

        //=============WALLS===========
        environmentGameplay.arenaWalls = new List<GameObject>();

        GameObject northWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        northWall.transform.parent = environmentGameplay.gameObject.transform;
        northWall.transform.localPosition = new Vector3(0f, 0f, genome.arenaBounds.z * 0.5f);
        northWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        northWall.GetComponent<MeshRenderer>().enabled = false;
        northWall.GetComponent<Collider>().material = noFriction;
        northWall.tag = "hazard";
        environmentGameplay.arenaWalls.Add(northWall);

        GameObject southWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        southWall.transform.parent = environmentGameplay.gameObject.transform;
        southWall.transform.localPosition = new Vector3(0f, 0f, -genome.arenaBounds.z * 0.5f);
        southWall.transform.localScale = new Vector3(genome.arenaBounds.x, genome.arenaBounds.y, genome.arenaBounds.z);
        southWall.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        southWall.GetComponent<MeshRenderer>().enabled = false;
        southWall.GetComponent<Collider>().material = noFriction;
        southWall.tag = "hazard";
        environmentGameplay.arenaWalls.Add(southWall);

        GameObject eastWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        eastWall.transform.parent = environmentGameplay.gameObject.transform;
        eastWall.transform.localPosition = new Vector3(genome.arenaBounds.x * 0.5f, 0f, 0f);
        eastWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        eastWall.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        eastWall.GetComponent<MeshRenderer>().enabled = false;
        eastWall.GetComponent<Collider>().material = noFriction;
        eastWall.tag = "hazard";
        environmentGameplay.arenaWalls.Add(eastWall);

        GameObject westWall = GameObject.CreatePrimitive(PrimitiveType.Quad);
        westWall.transform.parent = environmentGameplay.gameObject.transform;
        westWall.transform.localPosition = new Vector3(-genome.arenaBounds.x * 0.5f, 0f, 0f);
        westWall.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, genome.arenaBounds.z);
        westWall.transform.rotation = Quaternion.Euler(0f, -90f, 0f);
        westWall.GetComponent<MeshRenderer>().enabled = false;
        westWall.GetComponent<Collider>().material = noFriction;
        westWall.tag = "hazard";
        environmentGameplay.arenaWalls.Add(westWall);

        GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Quad);
        ceiling.transform.parent = environmentGameplay.gameObject.transform;
        ceiling.transform.localPosition = new Vector3(0f, genome.arenaBounds.y * 0.5f, 0f);
        ceiling.transform.localScale = new Vector3(genome.arenaBounds.z, genome.arenaBounds.y, 1f);
        ceiling.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        ceiling.GetComponent<MeshRenderer>().enabled = false;
        ceiling.GetComponent<Collider>().material = noFriction;
        ceiling.tag = "hazard";
        environmentGameplay.arenaWalls.Add(ceiling);

        // ======================================================================

        // Game-Required Modules:
        // Target !!!
        if (genome.useTargetColumn) {
            GameObject targetColumnGO = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            targetColumnGO.transform.parent = environmentGameplay.gameObject.transform;
            environmentGameplay.targetColumn = targetColumnGO.AddComponent<TargetColumn>();
            environmentGameplay.targetColumn.GetComponent<MeshRenderer>().enabled = false; // hide
            environmentGameplay.targetColumn.Initialize(genome.targetColumnGenome, genome);
        }
        
        // Obstacles:
        if (genome.useBasicObstacles) {
            environmentGameplay.obstacles = new List<GameObject>();
            for (int i = 0; i < genome.basicObstaclesGenome.obstaclePositions.Length; i++) {
                GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                obstacle.transform.parent = environmentGameplay.gameObject.transform;
                //float x = genome.basicObstaclesGenome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f;
                //float z = genome.basicObstaclesGenome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f;
                float x = genome.basicObstaclesGenome.obstaclePositions[i].x * genome.arenaBounds.x - genome.arenaBounds.x * 0.5f;
                float z = genome.basicObstaclesGenome.obstaclePositions[i].y * genome.arenaBounds.z - genome.arenaBounds.z * 0.5f;
                if (genome.useTargetColumn) {
                    float distToTarget = (new Vector2(environmentGameplay.targetColumn.transform.localPosition.x, environmentGameplay.targetColumn.transform.localPosition.z) - new Vector2(x, z)).magnitude;
                    if(distToTarget < genome.basicObstaclesGenome.obstacleScales[i] * 0.6f) {
                        obstacle.SetActive(false);
                    }
                }

                float y = TerrainConstructor.GetAltitude(genome, x, z) + 0.5f;
                obstacle.transform.localScale = new Vector3(genome.basicObstaclesGenome.obstacleScales[i], 1f, genome.basicObstaclesGenome.obstacleScales[i]);
                obstacle.transform.localPosition = new Vector3(x, y, z);
                obstacle.GetComponent<Collider>().material = noFriction;
                obstacle.tag = "hazard";
                obstacle.GetComponent<MeshRenderer>().enabled = false; // hide
                environmentGameplay.obstacles.Add(obstacle);
            }
        }

        // Atmosphere (WIND) !!!
        if (genome.useAtmosphere) {
            environmentGameplay.atmosphere = new Atmosphere();
            environmentGameplay.atmosphere.Initialize(genome.atmosphereGenome);
        }

        // Set Genome's prefab environment:
        genome.gameplayPrefab = environmentGameplay;
    }

    /*private float GetAltitude(EnvironmentGenome genome, float x, float z) {
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
    }*/

    /*private Mesh GetTerrainMesh(EnvironmentGenome genome, int xResolution, int zResolution, float xSize, float zSize) {
        
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
    }*/
}
