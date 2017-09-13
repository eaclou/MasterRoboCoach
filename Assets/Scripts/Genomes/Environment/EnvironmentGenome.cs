using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentGenome {

    public int index = -1;

    public Challenge.Type challengeType;
    public Vector3 arenaBounds;

    // I don't really like this!!! Try to revisit in the future! -- maybe I can store this somewhere other than the genome...
    //[System.NonSerialized]
    //public Environment environmentPrefab; // reference to disabled starting conditions of Environment that is then cloned for actual use
    [System.NonSerialized]
    public EnvironmentGameplay gameplayPrefab;
    // This prefab would be the invisible, purely-collision, functional skeleton of the environment, for use in hidden EvaluationInstances
    // If it is to be used in the exhibition instance, would need to call a function on Environment script to create all the renderable stuff

    
        
        // Environment should be split between static/dynamic and visible/collision
    // !$#$!#$!#
    // Think about how to split terrain/objects into chunks of a grid so they can be loaded only when necessary
    // %$#%@$#%@$#%@34

    // For now, due to Serialization concerns, all environmentGenomes will have fields for all possible environmental Modules
    // If they are unused they will simply be null. The challengeType / preset Templates will determine which ones are necessary initially
    
    

    // MODULES:    
    public List<StartPositionGenome> agentStartPositionsList;
    public bool useTerrain;
    public TerrainGenome terrainGenome;
    public bool useBasicObstacles;
    public BasicObstaclesGenome basicObstaclesGenome;
    public bool useTargetColumn;
    public TargetColumnGenome targetColumnGenome;
    public bool useAtmosphere;
    public AtmosphereGenome atmosphereGenome;
    public bool useMeteorites;
    public MeteoritesGenome meteoritesGenome;

    public EnvironmentGenome(int index) {
        this.index = index;
        //this.challengeType = challengeType;
        //arenaBounds = Challenge.GetChallengeArenaBounds(challengeType); // Look into support for expandable bounds
        //terrainWaves = new Vector3[6];
        //obstaclePositions = new Vector2[6];
        //obstacleScales = new float[6];        
    }
    public void InitializeRandomGenomeFromTemplate(EnvironmentGenome templateGenome) {
        CopyGenomeFromTemplate(templateGenome);
        if (useTerrain) {
            terrainGenome.InitializeRandomGenome();
        }
        if (useBasicObstacles) {
            basicObstaclesGenome.InitializeRandomGenome();
        }        
        if (useTargetColumn) {
            targetColumnGenome.InitializeRandomGenome();
        }
        if (useAtmosphere) {
            atmosphereGenome.InitializeRandomGenome();
        }
        if (useMeteorites) {
            meteoritesGenome.InitializeRandomGenome();
        }
    }
    public void CopyGenomeFromTemplate(EnvironmentGenome templateGenome) {
        //Debug.Log("CopyGenomeFromTemplate BEFORE startPosCount: " + templateGenome.agentStartPositionsList.Count.ToString());
        // This method creates a clone of the provided ScriptableObject Genome - should have no shared references!!!
        this.challengeType = templateGenome.challengeType;
        arenaBounds = new Vector3(templateGenome.arenaBounds.x, templateGenome.arenaBounds.y, templateGenome.arenaBounds.z);

        agentStartPositionsList = new List<StartPositionGenome>(); 
        for(int i = 0; i < templateGenome.agentStartPositionsList.Count; i++) {
            //Debug.Log("CopyGenomeFromTemplate DURING i: " + i.ToString());
            StartPositionGenome genomeCopy = new StartPositionGenome(templateGenome.agentStartPositionsList[i]);
            agentStartPositionsList.Add(genomeCopy);
        }
        //Debug.Log("CopyGenomeFromTemplate AFTER startPosCount: " + agentStartPositionsList.Count.ToString());

        useTerrain = templateGenome.useTerrain;
        if (useTerrain) {
            terrainGenome = new TerrainGenome(templateGenome.terrainGenome);
            //terrainGenome.InitializeRandomGenome();
        }
        useBasicObstacles = templateGenome.useBasicObstacles;
        if (useBasicObstacles) {
            basicObstaclesGenome = new BasicObstaclesGenome(templateGenome.basicObstaclesGenome);
            //basicObstaclesGenome.InitializeRandomGenome();
        }
        useTargetColumn = templateGenome.useTargetColumn;
        if (useTargetColumn) {
            targetColumnGenome = new TargetColumnGenome();
            //targetColumnGenome.InitializeRandomGenome();
        }
        useAtmosphere = templateGenome.useAtmosphere;
        if (useAtmosphere) {
            atmosphereGenome = new AtmosphereGenome(templateGenome.atmosphereGenome);
            //basicObstaclesGenome.InitializeRandomGenome();
        }
        useMeteorites = templateGenome.useMeteorites;
        if (useMeteorites) {
            meteoritesGenome = new MeteoritesGenome(templateGenome.meteoritesGenome);
            //basicObstaclesGenome.InitializeRandomGenome();
        }

        // For now this is fine -- but eventually might want to copy brainGenome from saved asset!
        //brainGenome = new BrainGenome();  // creates neuron and axonLists
        //InitializeRandomBrainGenome();        
    }
     
    public void ClearEnvironmentPrefab() {
        gameplayPrefab = null;
    }

    public static EnvironmentGenome BirthNewGenome(EnvironmentGenome parentGenome, int index, Challenge.Type challengeType, float mutationRate, float mutationDriftAmount) {
        EnvironmentGenome newGenome = new EnvironmentGenome(index);

        newGenome.challengeType = parentGenome.challengeType;
        newGenome.arenaBounds = new Vector3(parentGenome.arenaBounds.x, parentGenome.arenaBounds.y, parentGenome.arenaBounds.z);

        newGenome.useTerrain = parentGenome.useTerrain;
        if (parentGenome.useTerrain) {
            newGenome.terrainGenome = TerrainGenome.BirthNewGenome(parentGenome.terrainGenome, mutationRate, mutationDriftAmount);
        }
        newGenome.useBasicObstacles = parentGenome.useBasicObstacles;
        if (parentGenome.useBasicObstacles) {
            newGenome.basicObstaclesGenome = BasicObstaclesGenome.BirthNewGenome(parentGenome.basicObstaclesGenome, mutationRate, mutationDriftAmount);
        }
        newGenome.useTargetColumn = parentGenome.useTargetColumn;
        if (parentGenome.useTargetColumn) {
            newGenome.targetColumnGenome = TargetColumnGenome.BirthNewGenome(parentGenome.targetColumnGenome, mutationRate, mutationDriftAmount);
        }
        newGenome.useAtmosphere = parentGenome.useAtmosphere;
        if (parentGenome.useAtmosphere) {
            newGenome.atmosphereGenome = AtmosphereGenome.BirthNewGenome(parentGenome.atmosphereGenome, mutationRate, mutationDriftAmount);
        }
        newGenome.useMeteorites = parentGenome.useMeteorites;
        if (parentGenome.useMeteorites) {
            newGenome.meteoritesGenome = MeteoritesGenome.BirthNewGenome(parentGenome.meteoritesGenome, mutationRate, mutationDriftAmount);
        }

        // StartPositions:
        // HACKY! DOES NOT SUPPORT EVOLVING START POSITIONS! ALL THE SAME!!!!
        newGenome.agentStartPositionsList = parentGenome.agentStartPositionsList;

        /*newGenome.agentStartPositionsList = new List<Vector3>();
        //Debug.Log("(parentGenome.agentStartPositionsList.Count" + parentGenome.agentStartPositionsList.Count.ToString());
        for (int i = 0; i < parentGenome.agentStartPositionsList.Count; i++) {
            newGenome.agentStartPositionsList.Add(new Vector3(parentGenome.agentStartPositionsList[i].x, parentGenome.agentStartPositionsList[i].y, parentGenome.agentStartPositionsList[i].z));
        }
        newGenome.agentStartRotationsList = new List<Quaternion>();
        for (int i = 0; i < parentGenome.agentStartRotationsList.Count; i++) {
            newGenome.agentStartRotationsList.Add(new Quaternion(parentGenome.agentStartRotationsList[i].x, parentGenome.agentStartRotationsList[i].y, parentGenome.agentStartRotationsList[i].z, parentGenome.agentStartRotationsList[i].w));
        }*/

        return newGenome;
    }
}
