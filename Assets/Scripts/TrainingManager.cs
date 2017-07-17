using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingManager : MonoBehaviour {

    public TrainingMenuUI trainingMenuRef; 
    // Handles all of Training Mode:
    // Setting up evaluations
    // managing challenge & agent instances
    private Challenge.Type currentChallengeType;
    private TeamsConfig teamsConfig;

    private bool trainingModeActive = false; // are we in the training mode screen?
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
    public float playbackSpeed = 1f;

    private bool manualTrainingMode = false;  // toggles between manual sequential evaluations & auto parallel evaluations
    public bool ManualTrainingMode
    {
        get
        {
            return manualTrainingMode;
        }
        set
        {
            ToggleManualMode(value);
        }
    }

    // Training Status:
    private int playingCurGen = 0;
    private int playingCurAgent = 0;
    //private int playingCurTimeStep = 0;

    // Evaluation Settings:
    private int maxTimeSteps = 600;
    public float agentMutationChance = 0.1f;
    public float agentMutationStepSize = 0.1f;
    public float envMutationChance = 0.1f;
    public float envMutationStepSize = 0.1f;

    // Camera:
    public GameObject mainCamGroup;
    public GameObject mainCam;
    public int currentCameraMode = 0;  // 0 = wide, 1 = top-down, 2 = shoulder-cam
    private Vector3 cameraPosWide;
    private Quaternion cameraRotWide;
    private Vector3 cameraPosTop;
    private Quaternion cameraRotTop;
    private Vector3 cameraPosShoulder;
    private Quaternion cameraRotShoulder;

    // Particle Trajectories:
    public ParticleSystem particleTrajectories;
    // Players/Populations Configuration    
    // Teams
    // Environment
    // Populations

    // Evaluations Manager
    // for each generation, what are the required evaluation pairs?
    // what is the status of each of them?
    private List<EvaluationPair> evaluationPairsList;
    private List<EvaluationInstanceManager> evaluationInstancesList;
    // TEMP GROSS:
    private List<float> agentFitnessList;
    private List<float> environmentFitnessList;

    private int maxInstancesX = 8;
    private int maxInstancesZ = 12;
    private float instanceSizeX = 42f;
    private float instanceSizeZ = 42f;
    private float instanceBufferX = 2.5f;
    private float instanceBufferZ = 2.5f;
    
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
            if(evaluationInstancesList[0].currentEvalPair != null) {
                if(evaluationInstancesList[0].currentAgent != null) {
                    if (evaluationInstancesList[0].currentAgent.segmentList != null) {
                        if (evaluationInstancesList[0].currentAgent.segmentList[0] != null) {
                            mainCamGroup.transform.position = evaluationInstancesList[0].currentAgent.segmentList[0].transform.position;
                            mainCamGroup.transform.rotation = evaluationInstancesList[0].currentAgent.segmentList[0].transform.rotation;
                                                        
                        }                       
                    }
                }
            }
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
        int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Keep;
            evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.PendingComplete;
        }
    }

    public void ClickButtonManualAuto() {
        int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Auto;
        }
    }

    public void ClickButtonManualKill() {
        int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Kill;
            evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.PendingComplete;
        }
    }

    public void ClickButtonManualReplay() {
        int currentEvalPairIndex = GetCurrentEvalPairIndex();
        if (currentEvalPairIndex == -1) {
        }
        else {
            evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Replay;
            evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.PendingComplete;
        }
    }

    public string GetCurrentGenText() {
        return "Current Gen:\n" + playingCurGen.ToString();
    }

    public string GetContestantText() {
        string text = "Contestant:\n";
        if(evaluationInstancesList[0].currentEvalPair != null) {
            if (evaluationInstancesList[0].currentEvalPair.focusPopIndex == 0) {
                // environment?
                text += "Enviro " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[0] + 1).ToString() + " / " + teamsConfig.environmentGenomesList.Count.ToString();
            }
            else {
                // agent?
                text += "Agent " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[1] + 1).ToString() + " / " + teamsConfig.playersList[0].Count.ToString();
            }
        }        
        return text;
    }

    public string GetOpponentText() {
        string text = "Opponent:\n";
        if (evaluationInstancesList[0].currentEvalPair != null) {
            if (evaluationInstancesList[0].currentEvalPair.focusPopIndex == 0) {
                // agent?
                text += "Agent " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[1] + 1).ToString() + " / " + teamsConfig.playersList[0].Count.ToString();
            }
            else {
                // environment?
                text += "Enviro " + (evaluationInstancesList[0].currentEvalPair.evalPairIndices[0] + 1).ToString() + " / " + teamsConfig.environmentGenomesList.Count.ToString();
            }
        }        
        return text;
    }

    public string GetTestingProgressText() {
        string text = "Completion:\n";
        int numComplete = 0;
        int numInProgress = 0;
        int totalEvals = evaluationPairsList.Count;
        for (int i = 0; i < evaluationPairsList.Count; i++) {
            if(evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.Complete) {
                numComplete++;
            }
            else if(evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.InProgress) {
                numInProgress++;
            }
        }
        float completionPercentage = 100f * (float)numComplete / (float)totalEvals;
        text += "In Progress: " + numInProgress.ToString() + "   Complete: " + numComplete.ToString() + "/" + totalEvals.ToString() + "   [" + completionPercentage.ToString("F2") + "%]";

        return text;
    }

    public void ClickCameraMode() {
        currentCameraMode++;

        if(currentCameraMode > 2) {
            currentCameraMode = 0;
        }
    }

    void FixedUpdate() {
        if (isTraining) {            

            // Check training status:
            if (ManualTrainingMode) {
                int currentEvalPairIndex = GetCurrentEvalPairIndex();
                if(currentEvalPairIndex == -1) {
                    // All evals complete!
                    print("NEXT GEN! " + playingCurGen.ToString());
                    NextGeneration();
                    // NEXTGEN
                }
                else {
                    if (evaluationPairsList[currentEvalPairIndex].status == EvaluationPair.EvaluationStatus.InProgress) {
                        // Just tick forward current EvaluationInstance
                        evaluationInstancesList[0].Tick();
                    }
                    else {
                        // Pending -- Set up instance:
                        if (evaluationPairsList[currentEvalPairIndex].status == EvaluationPair.EvaluationStatus.Pending) {
                            print("currentEvalPair: " + currentEvalPairIndex.ToString() + " (" + evaluationPairsList[currentEvalPairIndex].status.ToString() + "), [" + evaluationPairsList[currentEvalPairIndex].evalPairIndices[0].ToString() + "," + evaluationPairsList[currentEvalPairIndex].evalPairIndices[1].ToString() + "]");
                            evaluationInstancesList[0].SetUpInstance(evaluationPairsList[currentEvalPairIndex], teamsConfig, currentChallengeType, maxTimeSteps);
                        }
                        if (evaluationPairsList[currentEvalPairIndex].status == EvaluationPair.EvaluationStatus.PendingComplete) {
                            // Instance finished but not fully processed
                            //Pause();
                            //trainingMenuRef
                            if(evaluationPairsList[currentEvalPairIndex].manualSelectStatus == EvaluationPair.ManualSelectStatus.Pending) {
                                // do nothing, wait for user input
                                // Temp! if eval hits max time, just keep going:
                                evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Auto;
                            }
                            else {
                                if (evaluationPairsList[currentEvalPairIndex].manualSelectStatus == EvaluationPair.ManualSelectStatus.Auto) {
                                    // STORE FITNESS
                                    if (evaluationPairsList[currentEvalPairIndex].focusPopIndex == 0) {
                                        // Environment
                                        environmentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[0]] += evaluationInstancesList[0].score;
                                        //print(environmentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[0]].ToString());
                                    }
                                    else {
                                        agentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[1]] += evaluationInstancesList[0].score;
                                        //print(agentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[1]].ToString());
                                    }
                                }
                                else if (evaluationPairsList[currentEvalPairIndex].manualSelectStatus == EvaluationPair.ManualSelectStatus.Keep) {
                                    // STORE FITNESS
                                    if (evaluationPairsList[currentEvalPairIndex].focusPopIndex == 0) { // Environment
                                        environmentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[0]] = float.NegativeInfinity;                                        
                                    }
                                    else {  // Agent
                                        agentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[1]] = float.NegativeInfinity;
                                    }
                                }
                                else if (evaluationPairsList[currentEvalPairIndex].manualSelectStatus == EvaluationPair.ManualSelectStatus.Kill) {
                                    // STORE FITNESS
                                    if (evaluationPairsList[currentEvalPairIndex].focusPopIndex == 0) { // Environment
                                        environmentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[0]] = float.PositiveInfinity;
                                    }
                                    else {  // Agent
                                        agentFitnessList[evaluationPairsList[currentEvalPairIndex].evalPairIndices[1]] = float.PositiveInfinity;
                                    }
                                }
                                
                                if(evaluationPairsList[currentEvalPairIndex].manualSelectStatus == EvaluationPair.ManualSelectStatus.Replay) {
                                    evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.Pending;
                                    evaluationPairsList[currentEvalPairIndex].manualSelectStatus = EvaluationPair.ManualSelectStatus.Pending;
                                    evaluationInstancesList[0].ClearInstance();
                                }
                                else {
                                    evaluationPairsList[currentEvalPairIndex].status = EvaluationPair.EvaluationStatus.Complete;
                                }                                
                            }                            
                        }
                    }
                }
            }
            else {
                // Auto Parallel Mode!!!!!

                // loop through evalInstances -- if pending, start evaluating with next free evalPair
                // if inProgress, Tick()                
                for(int i = 0; i < evaluationInstancesList.Count; i++) {
                    if(evaluationInstancesList[i].currentEvalPair == null) {
                        // available for use
                        // Find next available evalPair:
                        int currentEvalPairIndex = GetNextPendingEvalPairIndex();
                        if (currentEvalPairIndex == -1) {
                            // no evals pending
                        }
                        else {
                            //print("evalPair: " + currentEvalPairIndex.ToString() + " (" + evaluationPairsList[currentEvalPairIndex].status.ToString() + "), [" + evaluationPairsList[currentEvalPairIndex].evalPairIndices[0].ToString() + "," + evaluationPairsList[currentEvalPairIndex].evalPairIndices[1].ToString() + "]");
                            evaluationInstancesList[i].SetUpInstance(evaluationPairsList[currentEvalPairIndex], teamsConfig, currentChallengeType, maxTimeSteps);
                        }
                    }
                    else {
                        if (evaluationInstancesList[i].currentEvalPair.status == EvaluationPair.EvaluationStatus.InProgress) {
                            // Tick 
                            evaluationInstancesList[i].Tick();
                        }
                        if (evaluationInstancesList[i].currentEvalPair.status == EvaluationPair.EvaluationStatus.PendingComplete) {
                            // CleanUp and Process
                            // Instance finished but not fully processed

                            // STORE FITNESS
                            if (evaluationInstancesList[i].currentEvalPair.focusPopIndex == 0) {
                                // Environment
                                environmentFitnessList[evaluationInstancesList[i].currentEvalPair.evalPairIndices[0]] += evaluationInstancesList[i].score;
                                //print(environmentFitnessList[evaluationInstancesList[i].currentEvalPair.evalPairIndices[0]].ToString());
                            }
                            else {
                                agentFitnessList[evaluationInstancesList[i].currentEvalPair.evalPairIndices[1]] += evaluationInstancesList[i].score;
                                //print(agentFitnessList[evaluationInstancesList[i].currentEvalPair.evalPairIndices[1]].ToString());
                            }
                            evaluationInstancesList[i].currentEvalPair.status = EvaluationPair.EvaluationStatus.Complete;
                            evaluationInstancesList[i].currentEvalPair = null;
                        }
                    }
                }
                // check for Gen complete:
                bool genComplete = true;
                for(int i = 0; i < evaluationPairsList.Count; i++) {
                    if(evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.Complete) {

                    }
                    else {
                        genComplete = false;
                    }
                }
                if(genComplete) {
                    // All evals complete!
                    print("NEXT GEN! " + playingCurGen.ToString());
                    NextGeneration();
                    // NEXTGEN
                }
            }
        }        
    }

    private void ToggleManualMode(bool val) {
        if(val) {
            // Parallel ==> Serial:
            for(int i = 0; i < evaluationInstancesList.Count; i++) {
                if(evaluationInstancesList[i].currentEvalPair != null) {
                    if (evaluationInstancesList[i].currentEvalPair.status == EvaluationPair.EvaluationStatus.InProgress) {
                        evaluationInstancesList[i].ClearInstance();
                        // sets to pending, sets evalPair = null, deletes all gameObjects                    
                    }
                }
            }
        }
        else {
            // Serial ==> Parallel:
        }
        // Actually set bool:
        manualTrainingMode = val;
    }

    private void NextGeneration() {
        particleTrajectories.Clear();

        Agent();
        Environment();
        ResetEvaluationPairsList();
        for(int i = 0; i < agentFitnessList.Count; i++) {
            agentFitnessList[i] = 0f;
        }
        for (int i = 0; i < environmentFitnessList.Count; i++) {
            environmentFitnessList[i] = 0f;
        }
        playingCurGen++;
    }

    private void Environment() {
        List<EnvironmentGenome> newGenGenomeList = new List<EnvironmentGenome>(); // new population!     
        // find best performers of last gen:
        float[] rankedFitnessList = new float[environmentFitnessList.Count];
        int[] rankedEnvironmentIndices = new int[environmentFitnessList.Count];

        // GROSS brute force:
        // populate arrays:
        for (int i = 0; i < environmentFitnessList.Count; i++) {
            rankedEnvironmentIndices[i] = i;
            rankedFitnessList[i] = environmentFitnessList[i];
        }
        for (int i = 0; i < environmentFitnessList.Count - 1; i++) {
            for (int j = 0; j < environmentFitnessList.Count - 1; j++) {
                float swapFitA = rankedFitnessList[j];
                float swapFitB = rankedFitnessList[j + 1];
                int swapIdA = rankedEnvironmentIndices[j];
                int swapIdB = rankedEnvironmentIndices[j + 1];

                if (swapFitA > swapFitB) {
                    rankedFitnessList[j] = swapFitB;
                    rankedFitnessList[j + 1] = swapFitA;
                    rankedEnvironmentIndices[j] = swapIdB;
                    rankedEnvironmentIndices[j + 1] = swapIdA;
                }
            }
        }
        string fitnessRankText = "";
        for (int i = 0; i < environmentFitnessList.Count; i++) {
            fitnessRankText += "Environment[" + rankedEnvironmentIndices[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }
        //Debug.Log(fitnessRankText);

        // Keep top-half peformers + mutations:
        for (int x = 0; x < environmentFitnessList.Count; x++) {
            if (x == 0) {
                EnvironmentGenome parentGenome = teamsConfig.environmentGenomesList[rankedEnvironmentIndices[x]];
                newGenGenomeList.Add(parentGenome);
            }
            else {
                EnvironmentGenome newGenome = new EnvironmentGenome(teamsConfig.challengeType);

                EnvironmentGenome parentGenome = teamsConfig.environmentGenomesList[rankedEnvironmentIndices[Mathf.FloorToInt(x / 2)]];

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
                        float newAmp = UnityEngine.Random.Range(0f, 1f);
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
                        float newScale = UnityEngine.Random.Range(0.1f, 4f);
                        newGenome.obstacleScales[i] = Mathf.Lerp(newGenome.obstacleScales[i], newScale, envMutationStepSize);
                    }
                }
                // TARGET:
                newGenome.targetColumnGenome = new TargetColumnGenome(newGenome);
                newGenome.targetColumnGenome.targetRadius = parentGenome.targetColumnGenome.targetRadius;
                newGenome.targetColumnGenome.minX = parentGenome.targetColumnGenome.minX;
                newGenome.targetColumnGenome.maxX = parentGenome.targetColumnGenome.maxX;
                newGenome.targetColumnGenome.minZ = parentGenome.targetColumnGenome.minZ;
                newGenome.targetColumnGenome.maxZ = parentGenome.targetColumnGenome.maxZ;
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float newX = Mathf.Lerp(newGenome.targetColumnGenome.minX, UnityEngine.Random.Range(0f, 1f), envMutationStepSize);
                    newGenome.targetColumnGenome.minX = Mathf.Min(newX, parentGenome.targetColumnGenome.maxX);  // prevent min being bigger than max
                }
                rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < envMutationChance) {
                    float newX = Mathf.Lerp(newGenome.targetColumnGenome.maxX, UnityEngine.Random.Range(0f, 1f), envMutationStepSize);
                    newGenome.targetColumnGenome.maxX = Mathf.Max(newX, parentGenome.targetColumnGenome.minX);
                }
                rand = UnityEngine.Random.Range(0f, 1f);
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

        for (int i = 0; i < environmentFitnessList.Count; i++) {
            teamsConfig.environmentGenomesList[i] = newGenGenomeList[i];
            //EnvironmentGOList[i].GetComponent<Environment>().genome.brainGenome = newGenBrainGenomeList[i];
        }
    }

    private void Agent() {
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!        

        // find best performers of last gen:
        float[] rankedFitnessList = new float[agentFitnessList.Count];
        int[] rankedAgentIndices = new int[agentFitnessList.Count];

        // GROSS brute force:
        // populate arrays:
        for (int i = 0; i < agentFitnessList.Count; i++) {
            rankedAgentIndices[i] = i;
            rankedFitnessList[i] = agentFitnessList[i];
        }
        for (int i = 0; i < agentFitnessList.Count - 1; i++) {
            for (int j = 0; j < agentFitnessList.Count - 1; j++) {
                float swapFitA = rankedFitnessList[j];
                float swapFitB = rankedFitnessList[j + 1];
                int swapIdA = rankedAgentIndices[j];
                int swapIdB = rankedAgentIndices[j + 1];

                if (swapFitA > swapFitB) {
                    rankedFitnessList[j] = swapFitB;
                    rankedFitnessList[j + 1] = swapFitA;
                    rankedAgentIndices[j] = swapIdB;
                    rankedAgentIndices[j + 1] = swapIdA;
                }
            }
        }
        string fitnessRankText = "";
        for (int i = 0; i < agentFitnessList.Count; i++) {
            fitnessRankText += "Agent[" + rankedAgentIndices[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }
        //Debug.Log(fitnessRankText);

        // Keep top-half peformers + mutations:
        for (int x = 0; x < agentFitnessList.Count; x++) {
            if(x == 0) {
                BrainGenome parentGenome = teamsConfig.playersList[0][rankedAgentIndices[x]].brainGenome;
                newGenBrainGenomeList.Add(parentGenome);
            }
            else {
                BrainGenome newBrainGenome = new BrainGenome();
                // new BrainGenome creates new neuronList and linkList

                BrainGenome parentGenome = teamsConfig.playersList[0][rankedAgentIndices[Mathf.FloorToInt(x / 2)]].brainGenome;

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

        for (int i = 0; i < agentFitnessList.Count; i++) {
            teamsConfig.playersList[0][i].brainGenome = newGenBrainGenomeList[i];
            //agentGOList[i].GetComponent<Agent>().genome.brainGenome = newGenBrainGenomeList[i];
        }
    }

    private int GetNextPendingEvalPairIndex() {
        //int index = 0;
        for (int i = 0; i < evaluationPairsList.Count; i++) {
            if (evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.Pending) {
                //print("eval " + i.ToString() + " Complete!");
                return i;
            }
            else {
                // in progress, pending, or pendingComplete:
                //print("eval " + i.ToString() + " return!");                
            }
        }

        return -1;
    }
    private int GetCurrentEvalPairIndex() {
        //int index = 0;
        for(int i = 0; i < evaluationPairsList.Count; i++) {
            if(evaluationPairsList[i].status == EvaluationPair.EvaluationStatus.Complete) {
                //print("eval " + i.ToString() + " Complete!");
            }
            else {
                // in progress, pending, or pendingComplete:
                //print("eval " + i.ToString() + " return!");
                return i;
            }
        }

        return -1;
    }

    public void ResetEvaluationPairsList() {
        for(int i = 0; i < evaluationPairsList.Count; i++) {
            evaluationPairsList[i].status = EvaluationPair.EvaluationStatus.Pending;
            evaluationPairsList[i].manualSelectStatus = EvaluationPair.ManualSelectStatus.Pending;
        }
    }

    public void EnterTrainingMode(Challenge.Type challengeType) {
        currentChallengeType = challengeType;
        trainingModeActive = true;

        // Initialize

        // Setup Teams, Environment, and Populations - held inside teamsConfig
        switch (challengeType) {
            case Challenge.Type.Test:
                //print("test challenge");
                
                // environment is evolvable, 1 player:
                teamsConfig = new TeamsConfig(true, 1, Challenge.Type.Test);

                playingCurGen = 0;
                playingCurAgent = 0;

                // Set up eval pairs:
                evaluationPairsList = new List<EvaluationPair>();
                agentFitnessList = new List<float>();
                environmentFitnessList = new List<float>();

                int numAgentReps = 1;
                int numEnvironmentReps = 1;
                // Hardcoded for ONE PLAYER!!!! Agent evals:
                for(int i = 0; i < teamsConfig.playersList[0].Count; i++) {
                    for(int j = 0; j < numEnvironmentReps; j++) {
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
                    for(int j = 0; j < numAgentReps; j++) {
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
                evaluationInstancesList = new List<EvaluationInstanceManager>();
                for(int x = 0; x < maxInstancesX; x++) {
                    for(int z = 0; z < maxInstancesZ; z++) {
                        GameObject evalInstanceGO = new GameObject("EvaluationInstance [" + x.ToString() + "," + z.ToString() + "]");
                        EvaluationInstanceManager evaluationInstance = evalInstanceGO.AddComponent<EvaluationInstanceManager>();
                        evaluationInstance.particleCurves = particleTrajectories;
                        evalInstanceGO.transform.position = new Vector3(x * (arenaBounds.x + instanceBufferX), 0f, z * (arenaBounds.z + instanceBufferZ));
                        evaluationInstancesList.Add(evaluationInstance);

                        if(x == 0 & z == 0) {
                            evaluationInstance.visible = true;
                        }
                        else {
                            evaluationInstance.visible = false;
                        }
                    }                    
                }                

                // PRINT
                //for(int i = 0; i < evaluationPairsList.Count; i++) {
                //    print(evaluationPairsList[i].status.ToString() + ": [" + evaluationPairsList[i].evalPairIndices[0].ToString() + "," + evaluationPairsList[i].evalPairIndices[1].ToString() + "] " + evaluationPairsList[i].focusPopIndex.ToString());
                //}

                isTraining = true;

                break;
            default:
                print("default");
                break;
        }
    }    
}
