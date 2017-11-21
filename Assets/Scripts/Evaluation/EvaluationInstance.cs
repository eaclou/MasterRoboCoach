using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationInstance : MonoBehaviour {
    public bool visible = false;
    public bool isExhibition = false;
    public bool gameWonOrLost = false;
    public bool emit = false;
    //public int winnerPopIndex = -1;

    public ParticleSystem particleCurves; // reference to particle system that 'lives' in ExhibitionParticleCurves class
    public ParticleSystem.EmitParams emitterParamsDefault;
    public ParticleSystem.EmitParams emitterParamsWin;
    public ParticleSystem.EmitParams emitterParamsLose;
    public ParticleSystem.EmitParams emitterParamsDraw;

    public EvaluationTicket currentEvalTicket;
    private TeamsConfig teamsConfig;
    private Challenge.Type challengeType;
    // this will hold reference to which agents & environment are being tested, for score allocation

    public FitnessComponentEvaluationGroup fitnessComponentEvaluationGroup;

    public Agent[] currentAgentsArray;
    public float[][] agentGameScoresArray;
    private Environment currentEnvironment;
    private ChallengeBase currentChallenge;

    public int maxTimeSteps;
    public int currentTimeStep = 0;
    
    public void Tick() {
        //print("Tick! " + currentTimeStep.ToString());

        CalculateGameScores();
        CalculateFitnessScores();

        //currentEnvironment.RunModules();
        for (int i = 0; i < currentAgentsArray.Length; i++) {
            if(currentTimeStep % 4 == 0) {
                currentAgentsArray[i].TickBrain();
            }            
            currentAgentsArray[i].RunModules(currentTimeStep, currentEnvironment);

            if(isExhibition) {
                DoodadManager doodadManager = currentAgentsArray[i].gameObject.GetComponent<DoodadManager>();
                if (doodadManager != null) {
                    float inputVal01 = currentAgentsArray[i].brain.neuronList[doodadManager.neuronID_01].currentValue[0];
                    float inputVal02 = currentAgentsArray[i].brain.neuronList[doodadManager.neuronID_02].currentValue[0];
                    float inputVal03 = currentAgentsArray[i].brain.neuronList[doodadManager.neuronID_03].currentValue[0];
                    doodadManager.Tick(inputVal01, inputVal02, inputVal03);
                }
            }            
        }        

        if(CheckForEvaluationEnd()) {
            currentEvalTicket.status = EvaluationTicket.EvaluationStatus.PendingComplete;

            if (!gameWonOrLost) {
                if (emit && currentEvalTicket.focusPopIndex == 2) {  // Only Agents
                    //emitterParamsDraw.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                    //particleCurves.Emit(emitterParamsDraw, 8);
                }
                if (emit && currentEvalTicket.focusPopIndex == 1) {  // Only Agents
                    //emitterParamsDraw.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                    //particleCurves.Emit(emitterParamsDraw, 8);
                }
            }
            

            if (!isExhibition) {
                AverageFitnessComponentsByTimeSteps();
            }
            else {
                if(gameWonOrLost) {                    
                    if(agentGameScoresArray.Length > 1) {
                        if (agentGameScoresArray[0][0] > 0f) {
                            //Debug.Log("Player 1 WINS!!!");
                            
                        }
                        else {
                            //Debug.Log("Player 2 WINS!!!");
                            
                        }
                    }
                    else {
                        if (agentGameScoresArray[0][0] < 0f) {
                            //Debug.Log("Player 1 DIED!!!");
                        }
                    }
                }
            }
        }
        else {
            currentTimeStep++;
        }

        if (emit && currentEvalTicket.focusPopIndex > 0) {  // Only Agents
            //emitterParamsDefault.startColor = new Color(1f, 1f, 1f, 0.2f);
            if (currentAgentsArray[currentEvalTicket.focusPopIndex - 1].healthModuleList.Count > 0) {
                emitterParamsDefault.startColor = Color.Lerp(new Color(1f, 0f, 0f, 0.25f), new Color(0f, 1f, 0f, 0.25f), currentAgentsArray[currentEvalTicket.focusPopIndex - 1].healthModuleList[0].healthSensor[0]);
                //emitterParamsDefault.startColor.a = 0.5f;
            }
            emitterParamsDefault.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].rootObject.transform.localPosition + currentAgentsArray[currentEvalTicket.focusPopIndex - 1].rootCOM) - gameObject.transform.position;

            if(particleCurves != null) {
                //Debug.Log("Emit!");
                if(isExhibition) {
                    emitterParamsDefault.startColor = new Color(1f, 1f, 1f, 1f);
                }
                particleCurves.Emit(emitterParamsDefault, 1);
            }
        }       
    }    

    private bool CheckForEvaluationEnd() {
        bool isEnded = false;

        if(currentTimeStep > maxTimeSteps) {
            isEnded = true;
            //DeleteAllGameObjects();
        }
        if(gameWonOrLost) {
            isEnded = true;
            //DeleteAllGameObjects();
        }
        
        return isEnded;
    }
    public void ClearInstance() {
        if(currentEvalTicket != null) {
            currentEvalTicket.status = EvaluationTicket.EvaluationStatus.Pending;
            currentEvalTicket = null;
        }        
        DeleteAllGameObjects();
    }
    public void DeleteAllGameObjects() {
        if(currentAgentsArray != null) {
            for (int i = 0; i < currentAgentsArray.Length; i++) {
                if(currentAgentsArray[i] != null) {
                    currentAgentsArray[i].gameObject.SetActive(false);
                }                
            }
        }        
        var children = new List<GameObject>();
        foreach (Transform child in gameObject.transform) children.Add(child.gameObject);
        //children.ForEach(child => gameObject.SetActive(false));
        children.ForEach(child => Destroy(child));
    }

    public void SetUpInstance(EvaluationTicket evalTicket, TeamsConfig teamsConfig, ExhibitionParticleCurves exhibitionParticleCurves) {
        this.teamsConfig = teamsConfig;
        this.challengeType = teamsConfig.challengeType;
        this.maxTimeSteps = evalTicket.maxTimeSteps;

        /*string debugname = "";
        for( int i = 0; i < evalTicket.genomeIndices.Length; i++) {
            debugname += evalTicket.genomeIndices[i].ToString() + ",";
        }
        debugname += evalTicket.focusPopIndex.ToString();
        gameObject.name = debugname;*/

        // create particle key:

        /*int[] indices = new int[teamsConfig.playersList.Count + 2];
        indices[0] = evalTicket.focusPopIndex;
        for(int i = 0; i < evalTicket.genomeIndices.Length; i++) {
            indices[i + 1] = evalTicket.genomeIndices[i];
        }
        indices[indices[0] + 1] = 0; // focusPop is 0
        */
        /*int[] indices = new int[teamsConfig.playersList.Count + 2];
        indices[0] = evalTicket.focusPopIndex;
        for (int i = 0; i < evalTicket.agentGenomesList.Count + 1; i++) {
            if (i == 0) {
                indices[i + 1] = evalTicket.environmentGenome.index;
            }
            else {
                indices[i + 1] = evalTicket.agentGenomesList[i - 1].index;
            }
            //indices[i + 1] = ticket.genomeIndices[i];
        }

        string txt = "";
        for(int i = 0; i < indices.Length; i++) {
            txt += indices[i].ToString();
        }*/
        //Debug.Log(txt);
        /*if (exhibitionParticleRef.particleDictionary != null) {
            if (exhibitionParticleRef.particleDictionary.TryGetValue(txt, out particleCurves)) {
                // particleCurves
                //Debug.Log("FOUND IT! set up " + txt);

                emit = true;
                if (isExhibition)
                    emit = false;
            }
            else {
                //if (!isExhibition)
                //Debug.Log("Eval Instance Setup FAIL " + txt);
                emit = false;
            }
        }*/
        if(evalTicket.environmentGenome.index == 0) { // if this instance is testing an agent vs. the Top Environment, record its curves:            

            if (isExhibition) {
                //emit = false;
                particleCurves = exhibitionParticleCurves.singleTrajectoryCurvesPS;
                emit = true;
            }
            else {
                particleCurves = exhibitionParticleCurves.singleTrajectoryCurvesPS;
                emit = true;
            }               
        }
        else {
            emit = false;
        }
        

        currentEvalTicket = evalTicket;
              
        BruteForceInit();

        currentEvalTicket.status = EvaluationTicket.EvaluationStatus.InProgress;


        //emitterParamsDefault.startSize = 0.12f;
        //emitterParamsDefault.startColor = new Color(1f, 1f, 1f, 0.1f);
        
        emitterParamsWin.startSize = 1.2f;
        emitterParamsWin.startColor = new Color(0.1f, 1f, 0.1f, 1f);

        emitterParamsLose.startSize = 1.2f;
        emitterParamsLose.startColor = new Color(1f, 0.1f, 0.1f, 1f);

        emitterParamsDraw.startSize = 1.0f;
        emitterParamsDraw.startColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    private void BruteForceInit() {
        // REFACTOR THIS!!!!!!!!!!!!!!

        // Clear Everything:
        DeleteAllGameObjects();        

        currentTimeStep = 0;
        gameWonOrLost = false; // <-- revisit this shit
        //winnerPopIndex = -1; // <-- revisit this shit

        currentAgentsArray = new Agent[currentEvalTicket.agentGenomesList.Count];
        agentGameScoresArray = new float[currentEvalTicket.agentGenomesList.Count][];
        for(int i = 0; i < agentGameScoresArray.Length; i++) {
            agentGameScoresArray[i] = new float[1];
        }

        // Create Environment:
        CreateEnvironment();

        // Create Agents:        
        for(int i = 0; i < currentAgentsArray.Length; i++) {
            
            // Create Agent Base Body:
            GameObject agentGO = Instantiate(Resources.Load(AgentBodyGenomeTemplate.GetAgentBodyTypeURL(currentEvalTicket.agentGenomesList[i].bodyGenome.bodyType))) as GameObject;
            agentGO.transform.parent = gameObject.transform;
            //Debug.Log("null check: " + currentEvalTicket.environmentGenome.agentStartPositionsList.Count.ToString());
            agentGO.transform.localPosition = currentEvalTicket.environmentGenome.agentStartPositionsList[i].agentStartPosition;
            agentGO.transform.localRotation = currentEvalTicket.environmentGenome.agentStartPositionsList[i].agentStartRotation;
            Agent agentScript = agentGO.GetComponent<Agent>();
            agentScript.rootObject.GetComponent<Rigidbody>().centerOfMass = agentScript.rootCOM;
            agentScript.isVisible = visible;
            
            agentScript.InitializeAgentFromTemplate(currentEvalTicket.agentGenomesList[i]);
                        
            currentAgentsArray[i] = agentScript;            
        }        

        // Create Challenge Instance:
        GameObject challengeGO = new GameObject("challenge" + challengeType.ToString());
        switch(challengeType) {
            case Challenge.Type.Test:
                ChallengeTest challengeTest = challengeGO.AddComponent<ChallengeTest>();
                currentChallenge = challengeTest;
                break;
            case Challenge.Type.Racing:
                ChallengeRacing challengeRacing = challengeGO.AddComponent<ChallengeRacing>();
                currentChallenge = challengeRacing;
                break;
            case Challenge.Type.Combat:
                ChallengeCombat challengeCombat = challengeGO.AddComponent<ChallengeCombat>();
                currentChallenge = challengeCombat;
                break;
            default:
                break;
        }
        challengeGO.transform.parent = gameObject.transform;
        challengeGO.transform.localPosition = new Vector3(0f, 0f, 0f);

        // !!! RETHINK ROLE OF CHALLENGE CLASS!!!
        currentChallenge.agent = currentAgentsArray[0]; // hacky to prevent error, hardcoded for Test challengeType
        currentChallenge.environment = currentEnvironment;
        //currentChallenge.HookUpModules(); // maybe do this through the modules themselves -- by passing relevant agent/environment info through function?

        /*if (isExhibition) {
            if(currentEvalTicket.environmentGenome.useAtmosphere) {
                Debug.Log("Wind: " + currentEvalTicket.environmentGenome.atmosphereGenome.windForce.ToString());
            }
        }*/
            

        HookUpModules();

        //SetInvisibleTraverse(gameObject);
        if (visible) {
            currentEnvironment.AddRenderableContent(currentEvalTicket.environmentGenome);
            SetVisibleTraverse(gameObject);
            //Debug.Log("IS VISIBLE" + gameObject.name);
            //currentEnvironment.AddRenderableContent(teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]]);
            //SetVisibleTraverse(gameObject);
        }
        else {
            SetInvisibleTraverse(gameObject);
        }

        if(isExhibition) {
            
        }
        else {
            // Fitness Crap only if NON-exhibition!:
            FitnessManager fitnessManager;
            int genomeIndex;
            if (currentEvalTicket.focusPopIndex == 0) {  // environment
                fitnessManager = teamsConfig.environmentPopulation.fitnessManager;
                genomeIndex = currentEvalTicket.environmentGenome.index;
            }
            else {  // a player
                fitnessManager = teamsConfig.playersList[currentEvalTicket.focusPopIndex - 1].fitnessManager;
                genomeIndex = currentEvalTicket.agentGenomesList[currentEvalTicket.focusPopIndex - 1].index;
            }            
            fitnessComponentEvaluationGroup = new FitnessComponentEvaluationGroup();
            // Creates a copy inside this, and also a copy in the FitnessManager, but they share refs to the FitComps themselves:
            //Debug.Log("focusPop: " + currentEvalTicket.focusPopIndex.ToString() + ", index: " + genomeIndex.ToString());
            fitnessComponentEvaluationGroup.CreateFitnessComponentEvaluationGroup(fitnessManager, genomeIndex);
            //Debug.Log("currentEvalTicket.focusPopIndex: " + currentEvalTicket.focusPopIndex.ToString() + ", index: " + currentEvalTicket.genomeIndices[currentEvalTicket.focusPopIndex].ToString());
            HookUpFitnessComponents();
        }        
    }
    private void CreateEnvironment() {
        
        GameObject environmentGO = new GameObject("environment");
        Environment environmentScript = environmentGO.AddComponent<Environment>();
        currentEnvironment = environmentScript;
        environmentGO.transform.parent = gameObject.transform;
        environmentGO.transform.localPosition = new Vector3(0f, 0f, 0f);

        if (currentEvalTicket.environmentGenome.gameplayPrefab == null) {            
            // This might only work if environment is completely static!!!! otherwise it could change inside original evalInstance and then that
            // changed environment would be instantiated as fresh Environments for subsequent Evals!            
            environmentScript.CreateCollisionAndGameplayContent(currentEvalTicket.environmentGenome);
        }
        else {
            // Already built
            CollisionHitEffects collideFX = currentEvalTicket.environmentGenome.gameplayPrefab.groundCollision.GetComponent<CollisionHitEffects>();
            bool prefabState = false;
            if (collideFX != null) {
                //currentEnvironment.environmentGameplay.groundCollision.GetComponent<CollisionHitEffects>().active = false;
                prefabState = collideFX.active;
                collideFX.active = false;
                //Destroy(collideFX);
            }
            EnvironmentGameplay environmentGameplayScript = Instantiate<EnvironmentGameplay>(currentEvalTicket.environmentGenome.gameplayPrefab) as EnvironmentGameplay;
            // restore state after instantiation:
            if (collideFX != null) {
                collideFX.active = prefabState;
            }
            

            currentEnvironment.environmentGameplay = environmentGameplayScript;
            //CollisionHitEffects collideFX = currentEnvironment.environmentGameplay.groundCollision.GetComponent<CollisionHitEffects>();
                       
            currentEnvironment.environmentGameplay.gameObject.transform.parent = currentEnvironment.gameObject.transform;
            currentEnvironment.environmentGameplay.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public void HookUpModules() {

        switch (challengeType) {
            case Challenge.Type.Test:
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentEnvironment.environmentGameplay.targetColumn.gameObject.transform;

                    if (isExhibition) {
                        DoodadManager doodadManager = currentAgentsArray[i].gameObject.GetComponent<DoodadManager>();
                        if (doodadManager != null) {

                            doodadManager.neuronID_01 = UnityEngine.Random.Range(0, currentAgentsArray[i].brain.neuronList.Count);
                            doodadManager.neuronID_02 = UnityEngine.Random.Range(0, currentAgentsArray[i].brain.neuronList.Count);
                            doodadManager.neuronID_03 = UnityEngine.Random.Range(0, currentAgentsArray[i].brain.neuronList.Count);
                        }
                    }
                }
                break;
            case Challenge.Type.Racing:
                // TEMP!
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentEnvironment.transform;
                }
                break;
            case Challenge.Type.Combat:
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentAgentsArray[1].rootObject.transform;
                }
                for (int i = 0; i < currentAgentsArray[1].targetSensorList.Count; i++) {
                   currentAgentsArray[1].targetSensorList[i].targetPosition = currentAgentsArray[0].rootObject.transform;
                }
                break;
            default:
                break;
        }
    }
    public void HookUpFitnessComponents() {

        for(int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
            int populationIndex = 0; // defaults to player1
            if(currentEvalTicket.focusPopIndex != 0) {  // if environment is not the focus Pop, set correct playerIndex:
                populationIndex = currentEvalTicket.focusPopIndex - 1;
            }
            switch (fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type) {
                case FitnessComponentType.DistanceToTargetSquared:
                    FitCompDistanceToTargetSquared fitCompDistToTargetSquared = (FitCompDistanceToTargetSquared)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompDistanceToTargetSquared;
                    fitCompDistToTargetSquared.pointA = currentAgentsArray[populationIndex].rootObject.transform.localPosition;                    
                    fitCompDistToTargetSquared.pointB = currentAgentsArray[populationIndex].targetSensorList[0].targetPosition.localPosition;
                    break;
                case FitnessComponentType.Velocity:
                    FitCompVelocity fitCompVelocity = (FitCompVelocity)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompVelocity;
                    fitCompVelocity.vel = currentAgentsArray[populationIndex].rootObject.GetComponent<Rigidbody>().velocity;
                    break;
                case FitnessComponentType.ContactHazard:
                    FitCompContactHazard fitCompContactHazard = (FitCompContactHazard)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompContactHazard;
                    //Debug.Log("NullCheck root: " + currentAgentsArray[populationIndex].rootObject.ToString());
                    //Debug.Log("NullCheck component: " + currentAgentsArray[populationIndex].rootObject.GetComponent<ContactSensorComponent>().ToString());                    
                    fitCompContactHazard.contactingHazard = currentAgentsArray[populationIndex].rootObject.GetComponent<ContactSensorComponent>().contact;
                    //fitCompContactHazard
                    break;
                case FitnessComponentType.DamageInflicted:
                    FitCompDamageInflicted fitCompDamageInflicted = (FitCompDamageInflicted)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompDamageInflicted;
                    fitCompDamageInflicted.damage = currentAgentsArray[populationIndex].weaponProjectileList[0].damageInflicted[0] + currentAgentsArray[populationIndex].weaponTazerList[0].damageInflicted[0];
                    break;
                case FitnessComponentType.Health:
                    FitCompHealth fitCompHealth = (FitCompHealth)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompHealth;
                    fitCompHealth.health = currentAgentsArray[populationIndex].healthModuleList[0].health;
                    break;
                case FitnessComponentType.Random:
                    // handled fully within the FitCompRandom class
                    break;
                case FitnessComponentType.WinLoss:
                    FitCompWinLoss fitCompWinLoss = (FitCompWinLoss)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompWinLoss;                    
                    fitCompWinLoss.score = agentGameScoresArray[populationIndex];
                    break;
                case FitnessComponentType.DistToOrigin:
                    FitCompDistFromOrigin fitCompDistFromOrigin = (FitCompDistFromOrigin)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompDistFromOrigin;
                    fitCompDistFromOrigin.pointA = currentAgentsArray[populationIndex].rootObject.transform.localPosition;
                    fitCompDistFromOrigin.pointB = currentEvalTicket.environmentGenome.agentStartPositionsList[populationIndex].agentStartPosition;
                    break;
                case FitnessComponentType.Altitude:
                    FitCompAltitude fitCompAltitude = (FitCompAltitude)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompAltitude;                    
                    fitCompAltitude.altitude = currentAgentsArray[populationIndex].rootObject.transform.TransformPoint(currentAgentsArray[populationIndex].rootCOM).y - this.transform.position.y - Vector3.Dot(currentAgentsArray[populationIndex].rootObject.transform.up, Physics.gravity.normalized) * 1f;
                    break;
                case FitnessComponentType.Custom:
                    FitCompCustom fitCompCustom = (FitCompCustom)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompCustom;
                    fitCompCustom.custom = currentAgentsArray[populationIndex].segmentList[7].transform.TransformPoint(currentAgentsArray[populationIndex].rootCOM).y - this.transform.position.y - Vector3.Dot(currentAgentsArray[populationIndex].segmentList[7].transform.up, Physics.gravity.normalized) * 1f; ;
                    break;
                default:
                    Debug.LogError("ERROR!!! Fitness Type found!!! " + fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type.ToString());
                    break;
            }
        }
    }
    private void CalculateFitnessScores() {
        if (!isExhibition) {
            // Temp for now: in order to update positions...
            HookUpFitnessComponents();

            for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
                fitnessComponentEvaluationGroup.fitCompList[i].TickScore();
            }
        }
    }
    // !#$!#$!@ HARDCODED FOR 1 or 2 players only!!!!
    public void CalculateGameScores() {  // only applies to players for now...
        //float winLossDraw = 0f;

        if(gameWonOrLost) {  // if game is over

        }
        else {
            if(challengeType == Challenge.Type.Combat) {
                agentGameScoresArray[0][0] = 0f;
                if (currentAgentsArray.Length > 1)
                    agentGameScoresArray[1][0] = 0f;  // undecided

                if (currentAgentsArray[0].rootObject.GetComponent<HealthModuleComponent>()) {
                    // if player 0 dead:
                    if (currentAgentsArray[0].rootObject.GetComponent<HealthModuleComponent>().healthModule.health <= 0f) {
                        // if 2 players:
                        if (currentAgentsArray.Length > 1) {
                            //if player 0 dead AND player 1 dead:
                            if (currentAgentsArray[1].rootObject.GetComponent<HealthModuleComponent>().healthModule.health <= 0f) {
                                // ...then they died simultaneously -- DRAW
                                agentGameScoresArray[0][0] = 0f;
                                agentGameScoresArray[1][0] = 0f;
                                gameWonOrLost = true;
                            }
                            else { // player 0 dead and player 1 alive
                                   // Player 1 WINS!
                                agentGameScoresArray[0][0] = -1f;
                                agentGameScoresArray[1][0] = 1f; // winner
                                gameWonOrLost = true;
                                //Debug.Log("Player 1 WINS!");

                                if (emit && currentEvalTicket.focusPopIndex == 2) {  // Only Agents
                                    emitterParamsWin.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].rootObject.transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                    particleCurves.Emit(emitterParamsWin, 8);
                                }
                                if (emit && currentEvalTicket.focusPopIndex == 1) {  // Only Agents
                                    emitterParamsLose.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].rootObject.transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                    particleCurves.Emit(emitterParamsLose, 8);
                                }
                            }
                        }
                        else { // player0 died and is only player
                            Debug.Log("Only Player managed to kill itself!");
                            agentGameScoresArray[0][0] = -1f;
                            gameWonOrLost = true;
                        }
                    }
                    else {  // if Player 0 is alive:
                            // if 2 players:
                        if (currentAgentsArray.Length > 1) {
                            //if player 0 alive but player 1 dead:
                            if (currentAgentsArray[1].rootObject.GetComponent<HealthModuleComponent>().healthModule.health <= 0f) {
                                // Player 0 WINS!
                                agentGameScoresArray[0][0] = 1f; // winner
                                agentGameScoresArray[1][0] = -1f;
                                gameWonOrLost = true;
                                //Debug.Log("Player 0 WINS!");
                                if (emit && currentEvalTicket.focusPopIndex == 1) {  // Only Agents
                                    emitterParamsWin.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].rootObject.transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                    particleCurves.Emit(emitterParamsWin, 8);
                                }
                                if (emit && currentEvalTicket.focusPopIndex == 2) {  // Only Agents
                                    emitterParamsLose.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].rootObject.transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                    particleCurves.Emit(emitterParamsLose, 8);
                                }
                            }
                            else { // both players are alive!

                            }
                        }
                        else { // player0 alive and is only player

                        }
                    }
                }
            }
            if (challengeType == Challenge.Type.Test) {
                if(currentTimeStep > 1f) {
                    //agentGameScoresArray[0][0] = 1f - (((float)currentTimeStep - 1) / (float)maxTimeSteps);  // 0-1
                }
                else {
                    //agentGameScoresArray[0][0] = 0f;
                }

                Vector2 targetPos = new Vector2(currentEnvironment.environmentGameplay.targetColumn.transform.position.x, currentEnvironment.environmentGameplay.targetColumn.transform.position.z);
                Vector2 agentPos = new Vector2(currentAgentsArray[0].rootObject.transform.position.x + currentAgentsArray[0].rootCOM.x, currentAgentsArray[0].rootObject.transform.position.z + currentAgentsArray[0].rootCOM.z);
                float distanceToTarget = (targetPos - agentPos).magnitude;
                if(distanceToTarget < 2f) {
                    // In target!!!
                    //agentGameScoresArray[0][0] += 2f;
                    gameWonOrLost = true;
                }

                if(currentAgentsArray[0].healthModuleList.Count > 0) {
                    if (currentAgentsArray[0].healthModuleList[0].destroyed) {
                        agentGameScoresArray[0][0] = -5f;
                        gameWonOrLost = true;
                        //Debug.Log("Agent DIED from collision! " + currentTimeStep.ToString());
                    }
                }

                float dotUp = Vector3.Dot(currentAgentsArray[0].rootObject.transform.up, new Vector3(0f,1f,0f));
                if (dotUp < 0.3) {
                   
                    agentGameScoresArray[0][0] = -5f;
                    gameWonOrLost = false;
                        //Debug.Log("Agent DIED from collision! " + currentTimeStep.ToString());
                    
                }
            }
        }
    }
    public void AverageFitnessComponentsByTimeSteps() {
        for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
            if(fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.measure == FitnessComponentMeasure.Avg) {
                if(currentTimeStep > 1)
                    fitnessComponentEvaluationGroup.fitCompList[i].rawScore /= (currentTimeStep - 1);
            }
        }
    }

    public void SetInvisibleTraverse(GameObject obj) {
        //print("SetInvisibleTraverse " + obj.name);
        obj.layer = LayerMask.NameToLayer("Hidden");
        //foreach (Transform child in obj.transform) {
        //    SetInvisibleTraverse(child.gameObject);
        //}
        var children = new List<GameObject>();
        foreach (Transform child in obj.transform) children.Add(child.gameObject);
        children.ForEach(child => SetInvisibleTraverse(child.gameObject));
    }
    public void SetVisibleTraverse(GameObject obj) {
        obj.layer = LayerMask.NameToLayer("Default");
        //foreach (Transform child in obj.transform) {
        //    SetVisibleTraverse(child.gameObject);
        //}
        var children = new List<GameObject>();
        foreach (Transform child in obj.transform) children.Add(child.gameObject);
        children.ForEach(child => SetVisibleTraverse(child.gameObject));
    }
}
