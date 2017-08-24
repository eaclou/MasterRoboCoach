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
    public Challenge.Type challengeType;
    public TeamsConfig teamsConfig; // holds all the data    
    public EvaluationManager evaluationManager;  // keeps track of evaluation pairs & instances
    public CameraManager cameraManager;
    public bool cameraEnabled = false;
    //private bool trainingModeActive = false; // are we in the training mode screen?
    public bool isTraining = false; // actively evaluating
    public bool trainingPaused = false;
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
    
    // Training Status:
    public int playingCurGen = 0;
    
    // Particle Trajectories: -- this should have its own Manager --
    // but needs to coordinate with evaluationInstances to know when to emit and with what settings...
    //public ParticleSystem particleTrajectories;

    public int debugFrameCounter = 0;

    // TEMP GROSS: -- move this into each Population
    //private List<float> agentFitnessList;
    //private List<float> environmentFitnessList;

    void Start() {
        /*cameraPosWide = new Vector3(0f, 20f, -40f);
        cameraRotWide = Quaternion.Euler(26.2f, 0f, 0f);
        cameraPosTop = new Vector3(0f, 50f, 0f);
        cameraRotTop = Quaternion.Euler(90f, 0f, 0f);
        cameraPosShoulder = new Vector3(0f, 2.5f, -5f);
        cameraRotShoulder = Quaternion.Euler(10f, 0f, 0f);

        ParticleSystem.EmitParams emitterParams = new ParticleSystem.EmitParams();
        emitterParams.position = Vector3.one;
        emitterParams.startLifetime = 100f;
        particleTrajectories.Emit(emitterParams, 1);
        */
    }

    void Update() {
        SetCamera();
    }

    void FixedUpdate() {
        if (isTraining) { // && debugFrameCounter < 2) {
            //Debug.Log("FixedUpdate isTraining");
            if (evaluationManager.allEvalsComplete) {
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
        cameraEnabled = true;
    }
    public void LoadTrainingMode() {
        
        
        Debug.Log("LoadTrainingMode(TeamsConfig teamsConfig)!");
        //Debug.Log(teamsConfig.environmentPopulation.environmentGenomeList[0]..ToString());
        Debug.Log(teamsConfig.challengeType.ToString());

        // Initialize!
        teamsConfig.InitializeFromLoadedData();  // Sets up non-serialized data so it's ready for training
          

        playingCurGen = 0; // for now - eventually I can save this too, but it doesn't seem critically important

        evaluationManager = new EvaluationManager();
        // Need to make sure all populations have their representatives set up before calling this:
        // Right now this is done through the teamsConfig Constructor
        evaluationManager.InitializeNewTraining(teamsConfig, challengeType); // should I just roll this into the Constructor?

        isTraining = true;
        cameraEnabled = true;


        /*
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
    public void SaveCurrentGenome(string savename) {
        Debug.Log("SaveCurrentGenome: " + savename);
        // Find current genome:
        string json;
        string subFolder;
        int focusPopIndex = evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        if (focusPopIndex > 0) {
            // it's an Agent
            subFolder = "Agents/";
            //json = JsonUtility.ToJson(teamsConfig.playersList[focusPopIndex - 1].agentGenomeList[evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[focusPopIndex - 1]]);
            json = JsonUtility.ToJson(evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].agentGenomesList[focusPopIndex - 1]);
        }
        else {
            //it's an environment
            subFolder = "Environments/";
            //json = JsonUtility.ToJson(teamsConfig.environmentPopulation.environmentGenomeList[evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[0]]);
            json = JsonUtility.ToJson(evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].environmentGenome);
        }

        //string json = JsonUtility.ToJson(teamsConfig);
        Debug.Log(json);
        Debug.Log(Application.dataPath);
        string path = Application.dataPath + "/IndividualSaves/" + subFolder + savename + ".json";
        //Debug.Log(Application.persistentDataPath);
        Debug.Log(path);
        System.IO.File.WriteAllText(path, json);
    }

    private void SetCamera() {
        
        if (cameraEnabled) {
            //Debug.Log("SetCamera()!");
            Vector3 agentPosition = Vector3.zero;
            int focusPlayer = 0;
            if (evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex != 0) {
                focusPlayer = evaluationManager.exhibitionTicketList[evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1;
            }
            if (evaluationManager.exhibitionInstance.currentAgentsArray != null) {
                agentPosition = evaluationManager.exhibitionInstance.currentAgentsArray[focusPlayer].rootObject.transform.position + evaluationManager.exhibitionInstance.currentAgentsArray[focusPlayer].rootCOM;
            }
            cameraManager.UpdateCameraState(agentPosition);
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
        print("Pause");
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
    
    public void ClickCameraMode() {
        cameraManager.CycleCameraMode();
    }

    public void EnterTournamentSelectScreen() {
        Debug.Log("EnterTournamentSelect()");

        // Pause Evaluations 
        Pause();
        // Switch UI panel from trainer to Tournaments

        // Display upcoming tournaments, with some eligible, some on cooldown, and some locked


    }
    public void ExitTournamentSelectScreen() {
        // Switch UI panel back to trainerUI
        // resume paused evaluations
        TogglePlayPause(); // UGLY!! assumes it was paused during tournament select screen!
    }
    public void EnterTournament(TournamentInfo tournamentInfo) {
        Debug.Log("TrainingManager.EnterTournament");
        // Clean up current training, evalManager etc. - reset this generation
        isTraining = false;
        evaluationManager.ClearCurrentTraining();
        // Bring up Tournament Summary Page: name, type, opponent, etc.
        // Pass everything to TournamentManager?
        trainingMenuRef.mainMenuRef.EnterTournamentMode(tournamentInfo);        
    }
    

    private void NextGeneration() {
        Debug.Log("Next Generation! (" + playingCurGen.ToString() + ")");
        //particleTrajectories.Clear();
        // Crossover:
        teamsConfig.environmentPopulation.fitnessManager.ProcessAndRankRawFitness(teamsConfig.environmentPopulation.popSize);
        // Record and Remove Baseline Genomes:
        teamsConfig.environmentPopulation.TrimBaselineGenomes();
        for (int i = 0; i < teamsConfig.playersList.Count; i++) {
            //Debug.Log("Player " + i.ToString());
            teamsConfig.playersList[i].fitnessManager.ProcessAndRankRawFitness(teamsConfig.playersList[i].popSize);
            // Record and Remove Baseline Genomes:
            teamsConfig.playersList[i].TrimBaselineGenomes();
        }

        teamsConfig.ReloadAgentTemplates(); // see if I can hot-edit templates
        
        Crossover();

        // Cleanup for next Gen:
        // Reset fitness data:
        // RE-Sample and Add Baseline Genomes:
        teamsConfig.environmentPopulation.AppendBaselineGenomes();
        teamsConfig.environmentPopulation.fitnessManager.InitializeForNewGeneration(teamsConfig.environmentPopulation.environmentGenomeList.Count);
        teamsConfig.environmentPopulation.historicGenomePool.Add(teamsConfig.environmentPopulation.environmentGenomeList[0]);
        teamsConfig.environmentPopulation.ResetRepresentativesList();
        
        for (int i = 0; i < teamsConfig.playersList.Count; i++) {
            // RE-Sample and Add Baseline Genomes:
            teamsConfig.playersList[i].AppendBaselineGenomes();
            teamsConfig.playersList[i].fitnessManager.InitializeForNewGeneration(teamsConfig.playersList[i].agentGenomeList.Count);
            teamsConfig.playersList[i].historicGenomePool.Add(teamsConfig.playersList[i].agentGenomeList[0]);
            teamsConfig.playersList[i].ResetRepresentativesList();
            
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

        if(teamsConfig.environmentPopulation.isTraining) {
            EnvironmentCrossover();
        }        
        for(int i = 0; i < teamsConfig.playersList.Count; i++) {
            if(teamsConfig.playersList[i].isTraining) {
                AgentCrossover(i);
            }            
        }               
    }
    private void EnvironmentCrossover() {
        List<EnvironmentGenome> newGenGenomeList = new List<EnvironmentGenome>(); // new population!     
                
        FitnessManager fitnessManager = teamsConfig.environmentPopulation.fitnessManager;
        TrainingSettingsManager trainingSettingsManager = teamsConfig.environmentPopulation.trainingSettingsManager;
        float mutationChance = trainingSettingsManager.mutationChance;
        float mutationStepSize = trainingSettingsManager.mutationStepSize;
        
        // Keep top-half peformers + mutations:
        for (int x = 0; x < teamsConfig.environmentPopulation.environmentGenomeList.Count; x++) {
            if (x == 0) {
                // Top performer stays
                EnvironmentGenome parentGenome = teamsConfig.environmentPopulation.environmentGenomeList[fitnessManager.rankedIndicesList[x]];
                parentGenome.index = 0;
                newGenGenomeList.Add(parentGenome);
            }
            else {
                int parentIndex = fitnessManager.GetAgentIndexByLottery();
                
                EnvironmentGenome parentGenome = teamsConfig.environmentPopulation.environmentGenomeList[parentIndex];
                EnvironmentGenome newGenome = EnvironmentGenome.BirthNewGenome(parentGenome, newGenGenomeList.Count, teamsConfig.challengeType, mutationChance, mutationStepSize);
                                
                newGenGenomeList.Add(newGenome);
            }                       
        }       

        for (int i = 0; i < teamsConfig.environmentPopulation.environmentGenomeList.Count; i++) {
            teamsConfig.environmentPopulation.environmentGenomeList[i] = newGenGenomeList[i];
        }        
    }
    private void AgentCrossover(int playerIndex) {
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!        

        FitnessManager fitnessManager = teamsConfig.playersList[playerIndex].fitnessManager;
        TrainingSettingsManager trainingSettingsManager = teamsConfig.playersList[playerIndex].trainingSettingsManager;
        float mutationChance = trainingSettingsManager.mutationChance;
        float mutationStepSize = trainingSettingsManager.mutationStepSize;

        // Keep top-half peformers + mutations:
        for (int x = 0; x < teamsConfig.playersList[playerIndex].agentGenomeList.Count; x++) {
            if(x == 0) {
                BrainGenome parentGenome = teamsConfig.playersList[playerIndex].agentGenomeList[fitnessManager.rankedIndicesList[x]].brainGenome;
                //parentGenome.index = 0;
                newGenBrainGenomeList.Add(parentGenome);
            }
            else {
                BrainGenome newBrainGenome = new BrainGenome();
                // new BrainGenome creates new neuronList and linkList
                int parentIndex = fitnessManager.GetAgentIndexByLottery();

                BrainGenome parentGenome = teamsConfig.playersList[playerIndex].agentGenomeList[parentIndex].brainGenome;

                newBrainGenome.neuronList = parentGenome.neuronList; // UNSUSTAINABLE!!! might work now since all neuronLists are identical
                for (int i = 0; i < parentGenome.linkList.Count; i++) {
                    LinkGenome newLinkGenome = new LinkGenome(parentGenome.linkList[i].fromModuleID, parentGenome.linkList[i].fromNeuronID, parentGenome.linkList[i].toModuleID, parentGenome.linkList[i].toNeuronID, parentGenome.linkList[i].weight, true);
                    float rand = UnityEngine.Random.Range(0f, 1f);
                    if (rand < mutationChance) {
                        float randomWeight = Gaussian.GetRandomGaussian();
                        newLinkGenome.weight = Mathf.Lerp(newLinkGenome.weight, randomWeight, mutationStepSize);
                    }
                    newBrainGenome.linkList.Add(newLinkGenome);
                }
                newGenBrainGenomeList.Add(newBrainGenome);
            }
        }        

        for (int i = 0; i < teamsConfig.playersList[playerIndex].agentGenomeList.Count; i++) {
            teamsConfig.playersList[playerIndex].agentGenomeList[i].brainGenome = newGenBrainGenomeList[i];
        }        
    }       
}
