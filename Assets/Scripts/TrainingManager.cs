using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using System.IO;

public class TrainingManager : MonoBehaviour {
    
    public TrainingMenuUI trainingMenuRef; 
    // Handles all of Training Mode:
    // Setting up evaluations
    // managing challenge & agent instances
    private Challenge.Type challengeType;
    private TeamsConfig teamsConfig; // holds all the data    
    private EvaluationManager evaluationManager;  // keeps track of evaluation pairs & instances

    //private bool trainingModeActive = false; // are we in the training mode screen?
    private bool isTraining = false; // actively evaluating
    private bool trainingPaused = false;
    public bool TrainingPaused
    {
        get
        {
            return trainingPaused;
        }
        set
        {

        }
    }
    public float playbackSpeed = 0.1f;
    
    // Training Status:
    private int playingCurGen = 0;

    // Evaluation Settings:    
    public float agentMutationChance = 0.1f;
    public float agentMutationStepSize = 0.1f;
    public float envMutationChance = 0.1f;
    public float envMutationStepSize = 0.1f;

    // Camera: -- need to break this out away from TrainingManager
    public GameObject mainCamGroup;
    public GameObject mainCam;
    public int currentCameraMode = 0;  // 0 = wide, 1 = top-down, 2 = shoulder-cam
    private Vector3 cameraPosWide;
    private Quaternion cameraRotWide;
    private Vector3 cameraPosTop;
    private Quaternion cameraRotTop;
    private Vector3 cameraPosShoulder;
    private Quaternion cameraRotShoulder;

    // Particle Trajectories: -- this should have its own Manager --
    // but needs to coordinate with evaluationInstances to know when to emit and with what settings...
    public ParticleSystem particleTrajectories;

    public int debugFrameCounter = 0;

    // TEMP GROSS: -- move this into each Population
    //private List<float> agentFitnessList;
    //private List<float> environmentFitnessList;

    void Start() {
        cameraPosWide = new Vector3(0f, 20f, -40f);
        cameraRotWide = Quaternion.Euler(26.2f, 0f, 0f);
        cameraPosTop = new Vector3(0f, 50f, 0f);
        cameraRotTop = Quaternion.Euler(90f, 0f, 0f);
        cameraPosShoulder = new Vector3(0f, 2.5f, -5f);
        cameraRotShoulder = Quaternion.Euler(10f, 0f, 0f);

        ParticleSystem.EmitParams emitterParams = new ParticleSystem.EmitParams();
        emitterParams.position = Vector3.one;
        emitterParams.startLifetime = 100f;
        particleTrajectories.Emit(emitterParams, 1);
    }

    void Update() {
        SetCamera();
    }

    void FixedUpdate() {
        if (isTraining) {
            if(evaluationManager.allEvalsComplete) {
                // NEXT GEN!!!
                NextGeneration();
            }
            else {
                evaluationManager.Tick(teamsConfig);
            }            

            debugFrameCounter++;
        }
    }

    // First-time Initialization -- triggered through gameManager & GUI
    public void NewTrainingMode(Challenge.Type challengeType) {
        this.challengeType = challengeType;
        // Initialize
        int numPlayers = 1;
        switch(this.challengeType) {
            case Challenge.Type.Test:
                Debug.Log("Switch: Test");
                numPlayers = 1;
                break;
            case Challenge.Type.Racing:
                Debug.Log("Switch: Racing");
                numPlayers = 1;
                break;
            case Challenge.Type.Combat:
                Debug.Log("Switch: Combat");
                numPlayers = 2;
                break;
            default:
                Debug.Log("Switch: Default");
                break;
        }
        // environment is evolvable, 1 player:
        teamsConfig = new TeamsConfig(numPlayers, this.challengeType, 1, 1);        

        playingCurGen = 0;
        
        evaluationManager = new EvaluationManager();
        // Need to make sure all populations have their representatives set up before calling this:
        // Right now this is done through the teamsConfig Constructor
        evaluationManager.InitializeNewTraining(teamsConfig, challengeType); // should I just roll this into the Constructor?
                
        isTraining = true;        
    }
    public void LoadTrainingMode() {
        /*
        Debug.Log("LoadTrainingMode(TeamsConfig teamsConfig)!");
        Debug.Log(teamsConfig.environmentGenomesList[0].color.ToString());
        Debug.Log(teamsConfig.challengeType.ToString());

        Challenge.Type challengeType = teamsConfig.challengeType;

        // Initialize

        // Setup Teams, Environment, and Populations - held inside teamsConfig
        switch (challengeType) {
            case Challenge.Type.Test:
                //print("test challenge");

                // should be saved/built already:
                //teamsConfig = new TeamsConfig(true, 1, Challenge.Type.Test);

                playingCurGen = 0;
                //playingCurAgent = 0;

                // Set up eval pairs:
                evaluationPairsList = new List<EvaluationPair>();
                agentFitnessList = new List<float>();
                environmentFitnessList = new List<float>();

                int numAgentReps = 1;
                int numEnvironmentReps = 4;
                // Hardcoded for ONE PLAYER!!!! Agent evals:
                for (int i = 0; i < teamsConfig.playersList[0].agentGenomeList.Count; i++) {
                    for (int j = 0; j < numEnvironmentReps; j++) {
                        int[] pairIndices = new int[2];
                        pairIndices[0] = j; // Environment Representative
                        pairIndices[1] = i; // Agent Index
                        EvaluationPair evalPair = new EvaluationPair(pairIndices, 1); // 2 populations (including environment)
                        evaluationPairsList.Add(evalPair); // append pair to list                                                
                    }
                    agentFitnessList.Add(0f);
                }
                // Environment evals:
                for (int i = 0; i < teamsConfig.environmentGenomesList.Count; i++) {
                    for (int j = 0; j < numAgentReps; j++) {
                        int[] pairIndices = new int[2];
                        pairIndices[0] = i; // Environment Index
                        pairIndices[1] = j; // Agent Representative
                        EvaluationPair evalPair = new EvaluationPair(pairIndices, 0); // 2 populations (including environment)
                        evaluationPairsList.Add(evalPair); // append pair to list                        
                    }
                    environmentFitnessList.Add(0f);
                }

                // Set up evaluation Instance Managers:
                Vector3 arenaBounds = Challenge.GetChallengeArenaBounds(challengeType);
                evaluationInstancesList = new List<EvaluationInstance>();
                for (int x = 0; x < maxInstancesX; x++) {
                    for (int z = 0; z < maxInstancesZ; z++) {
                        GameObject evalInstanceGO = new GameObject("EvaluationInstance [" + x.ToString() + "," + z.ToString() + "]");
                        EvaluationInstance evaluationInstance = evalInstanceGO.AddComponent<EvaluationInstance>();
                        evaluationInstance.particleCurves = particleTrajectories;
                        evalInstanceGO.transform.position = new Vector3(x * (arenaBounds.x + instanceBufferX), 0f, z * (arenaBounds.z + instanceBufferZ));
                        evaluationInstancesList.Add(evaluationInstance);

                        if (x == 0 & z == 0) {
                            evaluationInstance.visible = true;
                        }
                        else {
                            evaluationInstance.visible = false;
                        }
                    }
                }

                isTraining = true;

                break;
            default:
                print("default");
                break;
        }
        */
    }

    public void SaveTraining(string savename) {
        Debug.Log("SaveTraining: " + savename);

        string json = JsonUtility.ToJson(teamsConfig);
        Debug.Log(json);
        Debug.Log(Application.dataPath);
        string path = Application.dataPath + "/TrainingSaves/" + savename + ".json";
        //Debug.Log(Application.persistentDataPath);
        Debug.Log(path);
        System.IO.File.WriteAllText(path, json);
    }
    public void LoadTraining(string filePath) {
        Debug.Log("LoadTraining: " + filePath);

        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(filePath);
        // Pass the json to JsonUtility, and tell it to create a GameData object from it
        TeamsConfig loadedData = JsonUtility.FromJson<TeamsConfig>(dataAsJson);
        teamsConfig = loadedData;
        LoadTrainingMode();
    }

    private void SetCamera() {
        if(currentCameraMode == 0) {
            //mainCam.transform.parent = null;
            mainCamGroup.transform.position = new Vector3(0f, 0f, 0f);
            mainCamGroup.transform.rotation = Quaternion.identity;
            mainCam.transform.localPosition = cameraPosWide;
            mainCam.transform.rotation = cameraRotWide;
        }
        else if(currentCameraMode == 1) {
            //mainCam.transform.parent = null;
            mainCamGroup.transform.position = new Vector3(0f, 0f, 0f);
            mainCamGroup.transform.rotation = Quaternion.identity;
            mainCam.transform.localPosition = cameraPosTop;
            mainCam.transform.rotation = cameraRotTop;
        }
        else {
            /*if(evaluationInstancesList[0].currentEvalPair != null) {
                if(evaluationInstancesList[0].currentAgent != null) {
                    if (evaluationInstancesList[0].currentAgent.segmentList != null) {
                        if (evaluationInstancesList[0].currentAgent.segmentList[0] != null) {
                            mainCamGroup.transform.position = evaluationInstancesList[0].currentAgent.segmentList[0].transform.position;
                            mainCamGroup.transform.rotation = evaluationInstancesList[0].currentAgent.segmentList[0].transform.rotation;
                                                        
                        }                       
                    }
                }
            }*/
            mainCam.transform.localPosition = cameraPosShoulder;
            mainCam.transform.localRotation = cameraRotShoulder;
        }        
    }

    public void TogglePlayPause() {
        print("togglePlayPause");
        if(trainingPaused) {
            Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
        }
        else {
            Time.timeScale = 0f;
        }
        trainingPaused = !trainingPaused;
    }
    public void Pause() {
        trainingPaused = true;
        Time.timeScale = 0f;
    }
    
    public void IncreasePlaybackSpeed() {
        playbackSpeed = Mathf.Clamp(playbackSpeed * 1.5f, 0.1f, 100f);
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
    }
    public void DecreasePlaybackSpeed() {
        playbackSpeed = Mathf.Clamp(playbackSpeed * 0.66667f, 0.1f, 100f);
        Time.timeScale = Mathf.Clamp(playbackSpeed, 0.1f, 100f);
    }

    public void ClickButtonManualKeep() {
        /*int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Keep;
            evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.PendingComplete;
        }*/
    }
    public void ClickButtonManualAuto() {
        /*int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Auto;
        }*/
    }
    public void ClickButtonManualKill() {
        /*int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Kill;
            evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.PendingComplete;
        }*/
    }
    public void ClickButtonManualReplay() {
        /*int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Replay;
            evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.PendingComplete;
        }*/
    }

    public string GetCurrentGenText() {
        return "Current Gen:\n" + playingCurGen.ToString();
    }
    public string GetContestantText() {
        string text = "Contestant:\n";
        /*if(isTraining) {
            if (evaluationInstancesList[0].currentEvalPair != null) {
                if (evaluationInstancesList[0].currentEvalPair.focusPopIndex == 0) {
                    // environment?
                    text += "Enviro " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[0] + 1).ToString() + " / " + teamsConfig.environmentGenomesList.Count.ToString();
                }
                else {
                    // agent?
                    text += "Agent " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[1] + 1).ToString() + " / " + teamsConfig.playersList[0].agentGenomeList.Count.ToString();
                }
            }
        }*/              
        return text;
    }
    public string GetOpponentText() {
        string text = "Opponent:\n";
        /*if (isTraining) {
            if (evaluationInstancesList[0].currentEvalPair != null) {
                if (evaluationInstancesList[0].currentEvalPair.focusPopIndex == 0) {
                    // agent?
                    text += "Agent " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[1] + 1).ToString() + " / " + teamsConfig.playersList[0].agentGenomeList.Count.ToString();
                }
                else {
                    // environment?
                    text += "Enviro " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[0] + 1).ToString() + " / " + teamsConfig.environmentGenomesList.Count.ToString();
                }
            }
        } */               
        return text;
    }
    public string GetTestingProgressText() {
        string text = "Completion:\n";
        /*if(isTraining) {
            int numComplete = 0;
            int numInProgress = 0;
            int totalEvals = evaluationPairsList.Count;
            for (int i = 0; i < evaluationPairsList.Count; i++) {
                if (evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.Complete) {
                    numComplete++;
                }
                else if (evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.InProgress) {
                    numInProgress++;
                }
            }
            float completionPercentage = 100f * (float)numComplete / (float)totalEvals;
            text += "In Progress: " + numInProgress.ToString() + "   Complete: " + numComplete.ToString() + "/" + totalEvals.ToString() + "   [" + completionPercentage.ToString("F2") + "%]";
        }*/
        return text;
    }

    public void ClickCameraMode() {
        currentCameraMode++;

        if(currentCameraMode > 2) {
            currentCameraMode = 0;
        }
    }
    
    private void NextGeneration() {
        Debug.Log("Next Generation!");
        particleTrajectories.Clear();
        // Crossover:
        teamsConfig.environmentPopulation.fitnessManager.ProcessAndRankRawFitness();
        for(int i = 0; i < teamsConfig.playersList.Count; i++) {
            teamsConfig.playersList[i].fitnessManager.ProcessAndRankRawFitness();
        }

        Crossover();

        // Cleanup for next Gen:
        // Reset fitness data:
        teamsConfig.environmentPopulation.fitnessManager.ResetFitnessScores(teamsConfig.environmentPopulation.environmentGenomeList.Count);
        for (int i = 0; i < teamsConfig.playersList.Count; i++) {
            teamsConfig.playersList[i].fitnessManager.ResetFitnessScores(teamsConfig.playersList[i].agentGenomeList.Count);
        }
        // Reset default evals + exhibition
        evaluationManager.ResetForNewGeneration(teamsConfig);

        playingCurGen++;
        
    }
    private void Crossover() {
        // Query Fitness Managers to create:
        // List of ranked Fitness scores (processed data)
        // Parallel List of the indices corresponding to those fitness scores.
        // Then the crossover/mutation functions should only need to take in the Fitness Manager in order to operate...

        EnvironmentCrossover();
        for(int i = 0; i < teamsConfig.playersList.Count; i++) {
            AgentCrossover(i);
        }               
    }
    private void EnvironmentCrossover() {
        List<EnvironmentGenome> newGenGenomeList = new List<EnvironmentGenome>(); // new population!     
                
        FitnessManager fitnessManager = teamsConfig.environmentPopulation.fitnessManager;

        // Keep top-half peformers + mutations:
        for (int x = 0; x < teamsConfig.environmentPopulation.environmentGenomeList.Count; x++) {
            if (x == 0) {
                // Top performer stays
                EnvironmentGenome parentGenome = teamsConfig.environmentPopulation.environmentGenomeList[fitnessManager.rankedIndicesList[x]];
                newGenGenomeList.Add(parentGenome);
            }
            else {
                EnvironmentGenome newGenome = new EnvironmentGenome(newGenGenomeList.Count, teamsConfig.challengeType);

                EnvironmentGenome parentGenome = teamsConfig.environmentPopulation.environmentGenomeList[fitnessManager.rankedIndicesList[Mathf.FloorToInt(x / 2)]];

                newGenome.color = parentGenome.color;
                float rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float r = UnityEngine.Random.Range(0f, 1f);
                    newGenome.color = new Vector3(Mathf.Lerp(newGenome.color.x, r, envMutationStepSize), newGenome.color.y, newGenome.color.z);
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float g = UnityEngine.Random.Range(0f, 1f);
                    newGenome.color = new Vector3(newGenome.color.x, Mathf.Lerp(newGenome.color.y, g, envMutationStepSize), newGenome.color.z);
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float b = UnityEngine.Random.Range(0f, 1f);
                    newGenome.color = new Vector3(newGenome.color.x, newGenome.color.y, Mathf.Lerp(newGenome.color.z, b, envMutationStepSize));
                }
                // TERRAIN:
                for (int i = 0; i < parentGenome.terrainWaves.Length; i++) {
                    newGenome.terrainWaves[i] = new Vector3(parentGenome.terrainWaves[i].x, parentGenome.terrainWaves[i].y, parentGenome.terrainWaves[i].z);
                    rand = UnityEngine.Random.Range(0f, 1f);
                    if (rand < envMutationChance) {
                        float newFreq = UnityEngine.Random.Range(0.01f, 1f);
                        newGenome.terrainWaves[i].x = Mathf.Lerp(newGenome.terrainWaves[i].x, newFreq, envMutationStepSize);
                    }
                    rand = UnityEngine.Random.Range(0f, 1f);
                    if (rand < envMutationChance) {
                        float newAmp = UnityEngine.Random.Range(0f, 0f);
                        newGenome.terrainWaves[i].y = Mathf.Lerp(newGenome.terrainWaves[i].y, newAmp, envMutationStepSize);
                    }
                    rand = UnityEngine.Random.Range(0f, 1f);
                    if (rand < envMutationChance) {
                        float newOff = UnityEngine.Random.Range(-10f, 10f);
                        newGenome.terrainWaves[i].z = Mathf.Lerp(newGenome.terrainWaves[i].z, newOff, envMutationStepSize);
                    }
                }
                // OBSTACLES:
                for (int i = 0; i < parentGenome.obstaclePositions.Length; i++) {
                    newGenome.obstaclePositions[i] = new Vector2(parentGenome.obstaclePositions[i].x, parentGenome.obstaclePositions[i].y);
                    newGenome.obstacleScales[i] = parentGenome.obstacleScales[i];
                    rand = UnityEngine.Random.Range(0f, 1f);
                    if (rand < envMutationChance) {
                        float newPosX = UnityEngine.Random.Range(0f, 1f);
                        newGenome.obstaclePositions[i].x = Mathf.Lerp(newGenome.obstaclePositions[i].x, newPosX, envMutationStepSize);
                    }
                    if (rand < envMutationChance) {
                        float newPosZ = UnityEngine.Random.Range(0f, 1f);
                        newGenome.obstaclePositions[i].y = Mathf.Lerp(newGenome.obstaclePositions[i].y, newPosZ, envMutationStepSize);
                    }
                    if ((newGenome.obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude < 0.15f) {
                        newGenome.obstaclePositions[i] = new Vector2(0.5f, 0.5f) + (newGenome.obstaclePositions[i] - new Vector2(0.5f, 0.5f)) * 0.15f / (newGenome.obstaclePositions[i] - new Vector2(0.5f, 0.5f)).magnitude;
                    }
                    if (rand < envMutationChance) {
                        float newScale = UnityEngine.Random.Range(1f, 5f);
                        newGenome.obstacleScales[i] = Mathf.Lerp(newGenome.obstacleScales[i], newScale, envMutationStepSize);
                    }
                }
                // TARGET:
                newGenome.targetColumnGenome = new TargetColumnGenome();
                newGenome.targetColumnGenome.targetRadius = parentGenome.targetColumnGenome.targetRadius;
                newGenome.targetColumnGenome.minX = parentGenome.targetColumnGenome.minX;
                newGenome.targetColumnGenome.maxX = parentGenome.targetColumnGenome.maxX;
                newGenome.targetColumnGenome.minZ = parentGenome.targetColumnGenome.minZ;
                newGenome.targetColumnGenome.maxZ = parentGenome.targetColumnGenome.maxZ;
                rand = UnityEngine.Random.Range(0f, 0f);
                if (rand < envMutationChance) {
                    float newX = Mathf.Lerp(newGenome.targetColumnGenome.minX, UnityEngine.Random.Range(0f, 1f), envMutationStepSize);
                    newGenome.targetColumnGenome.minX = Mathf.Min(newX, parentGenome.targetColumnGenome.maxX);  // prevent min being bigger than max
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float newX = Mathf.Lerp(newGenome.targetColumnGenome.maxX, UnityEngine.Random.Range(0f, 1f), envMutationStepSize);
                    newGenome.targetColumnGenome.maxX = Mathf.Max(newX, parentGenome.targetColumnGenome.minX);
                }
                rand = UnityEngine.Random.Range(0f, 0f);
                if (rand < envMutationChance) {
                    float newZ = Mathf.Lerp(newGenome.targetColumnGenome.minZ, UnityEngine.Random.Range(0f, 1f), envMutationStepSize);
                    newGenome.targetColumnGenome.minZ = Mathf.Min(newZ, parentGenome.targetColumnGenome.maxZ);  // prevent min being bigger than max
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float newZ = Mathf.Lerp(newGenome.targetColumnGenome.maxZ, UnityEngine.Random.Range(0f, 1f), envMutationStepSize);
                    newGenome.targetColumnGenome.maxZ = Mathf.Max(newZ, parentGenome.targetColumnGenome.minZ);
                }
                newGenGenomeList.Add(newGenome);
            }                       
        }       

        for (int i = 0; i < teamsConfig.environmentPopulation.environmentGenomeList.Count; i++) {
            teamsConfig.environmentPopulation.environmentGenomeList[i] = newGenGenomeList[i];
            //EnvironmentGOList[i].GetComponent<Environment>().genome.brainGenome = newGenBrainGenomeList[i];
        }        
    }
    private void AgentCrossover(int playerIndex) {
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!        

        FitnessManager fitnessManager = teamsConfig.playersList[playerIndex].fitnessManager;

        // Keep top-half peformers + mutations:
        for (int x = 0; x < teamsConfig.playersList[playerIndex].agentGenomeList.Count; x++) {
            if(x == 0) {
                BrainGenome parentGenome = teamsConfig.playersList[playerIndex].agentGenomeList[fitnessManager.rankedIndicesList[x]].brainGenome;
                newGenBrainGenomeList.Add(parentGenome);
            }
            else {
                BrainGenome newBrainGenome = new BrainGenome();
                // new BrainGenome creates new neuronList and linkList

                BrainGenome parentGenome = teamsConfig.playersList[playerIndex].agentGenomeList[fitnessManager.rankedIndicesList[Mathf.FloorToInt(x / 2)]].brainGenome;

                newBrainGenome.neuronList = parentGenome.neuronList; // UNSUSTAINABLE!!! might work now since all neuronLists are identical
                for (int i = 0; i < parentGenome.linkList.Count; i++) {
                    LinkGenome newLinkGenome = new LinkGenome(parentGenome.linkList[i].fromModuleID, parentGenome.linkList[i].fromNeuronID, parentGenome.linkList[i].toModuleID, parentGenome.linkList[i].toNeuronID, parentGenome.linkList[i].weight, true);
                    float rand = UnityEngine.Random.Range(0f, 1f);
                    if (rand < agentMutationChance) {
                        float randomWeight = Gaussian.GetRandomGaussian();
                        newLinkGenome.weight = Mathf.Lerp(newLinkGenome.weight, randomWeight, agentMutationStepSize);
                    }
                    newBrainGenome.linkList.Add(newLinkGenome);
                }
                newGenBrainGenomeList.Add(newBrainGenome);
            }
        }        

        for (int i = 0; i < teamsConfig.playersList[playerIndex].agentGenomeList.Count; i++) {
            teamsConfig.playersList[playerIndex].agentGenomeList[i].brainGenome = newGenBrainGenomeList[i];
            //agentGOList[i].GetComponent<Agent>().genome.brainGenome = newGenBrainGenomeList[i];
        }        
    }       
}
