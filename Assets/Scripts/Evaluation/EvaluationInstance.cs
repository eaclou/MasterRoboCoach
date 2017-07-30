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

        for (int i = 0; i < currentAgentsArray.Length; i++) {
            currentAgentsArray[i].TickBrain();
            currentAgentsArray[i].RunModules();
        }        

        if(CheckForEvaluationEnd()) {
            currentEvalTicket.status = EvaluationTicket.EvaluationStatus.PendingComplete;

            if (!gameWonOrLost) {
                if (emit && currentEvalTicket.focusPopIndex == 2) {  // Only Agents
                    emitterParamsDraw.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                    particleCurves.Emit(emitterParamsDraw, 8);
                }
                if (emit && currentEvalTicket.focusPopIndex == 1) {  // Only Agents
                    emitterParamsDraw.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                    particleCurves.Emit(emitterParamsDraw, 8);
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
            if(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].healthModuleList != null) {
                emitterParamsDefault.startColor = Color.Lerp(new Color(1f, 0f, 0f, 0.25f), new Color(0f, 1f, 0f, 0.25f), currentAgentsArray[currentEvalTicket.focusPopIndex - 1].healthModuleList[0].healthSensor[0]);
                //emitterParamsDefault.startColor.a = 0.5f;
            }
            emitterParamsDefault.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
            particleCurves.Emit(emitterParamsDefault, 1);
            
        }       
    }    

    private bool CheckForEvaluationEnd() {
        bool isEnded = false;

        if(currentTimeStep > maxTimeSteps) {
            isEnded = true;
        }
        if(gameWonOrLost) {
            isEnded = true;
        }
        
        return isEnded;
    }
    public void ClearInstance() {
        currentEvalTicket.status = EvaluationTicket.EvaluationStatus.Pending;
        currentEvalTicket = null;
        DeleteAllGameObjects();
    }
    public void DeleteAllGameObjects() {
        var children = new List<GameObject>();
        foreach (Transform child in gameObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    public void SetUpInstance(EvaluationTicket evalTicket, TeamsConfig teamsConfig, ExhibitionParticleCurves exhibitionParticleRef) {
        this.teamsConfig = teamsConfig;
        this.challengeType = teamsConfig.challengeType;
        this.maxTimeSteps = evalTicket.maxTimeSteps;

        // create particle key:
        int[] indices = new int[teamsConfig.playersList.Count + 2];
        indices[0] = evalTicket.focusPopIndex;
        for(int i = 0; i < evalTicket.genomeIndices.Length; i++) {
            indices[i + 1] = evalTicket.genomeIndices[i];
        }
        indices[indices[0] + 1] = 0; // focusPop is 0

        string txt = "";
        for(int i = 0; i < indices.Length; i++) {
            txt += indices[i].ToString();
        }
        //Debug.Log(txt);
        if(exhibitionParticleRef.particleDictionary.TryGetValue(txt, out particleCurves)) {
            // particleCurves
            //Debug.Log("FOUND IT! set up " + txt);
            
            emit = true;
            if (isExhibition)
                emit = false;
        }
        else {
            //Debug.Log("Eval Instance Setup FAIL " + txt);
            emit = false;
        }

        currentEvalTicket = evalTicket;
              
        BruteForceInit();

        currentEvalTicket.status = EvaluationTicket.EvaluationStatus.InProgress;


        emitterParamsDefault.startSize = 0.25f;
        emitterParamsDefault.startColor = new Color(1f, 1f, 1f, 0.25f);
        
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

        currentAgentsArray = new Agent[currentEvalTicket.genomeIndices.Length - 1];
        agentGameScoresArray = new float[currentEvalTicket.genomeIndices.Length - 1][];
        for(int i = 0; i < agentGameScoresArray.Length; i++) {
            agentGameScoresArray[i] = new float[1];
        }

        // Create Environment:
        CreateEnvironment();

        // Create Agents:        
        for(int i = 0; i < currentAgentsArray.Length; i++) {
            GameObject agentGO = new GameObject("agent" + currentEvalTicket.genomeIndices[i+1].ToString());
            agentGO.transform.parent = gameObject.transform;
            agentGO.transform.localPosition = teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]].agentStartPositionsList[i].agentStartPosition;
            agentGO.transform.localRotation = teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]].agentStartPositionsList[i].agentStartRotation;
            Agent agentScript = agentGO.AddComponent<Agent>();
            agentScript.isVisible = visible;
            agentScript.ConstructAgentFromGenome(teamsConfig.playersList[i].agentGenomeList[currentEvalTicket.genomeIndices[i+1]]);
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

        HookUpModules();

        //SetInvisibleTraverse(gameObject);
        if (visible) {
            currentEnvironment.AddRenderableContent(teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]]);
            SetVisibleTraverse(gameObject);
        }
        else {
            SetInvisibleTraverse(gameObject);
        }

        if(isExhibition) {
            currentEnvironment.AddRenderableContent(teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]]);
        }
        else {
            // Fitness Crap only if NON-exhibition!:
            FitnessManager fitnessManager;
            if (currentEvalTicket.focusPopIndex == 0) {  // environment
                fitnessManager = teamsConfig.environmentPopulation.fitnessManager;
            }
            else {  // a player
                fitnessManager = teamsConfig.playersList[currentEvalTicket.focusPopIndex - 1].fitnessManager;
            }            
            fitnessComponentEvaluationGroup = new FitnessComponentEvaluationGroup();
            // Creates a copy inside this, and also a copy in the FitnessManager, but they share refs to the FitComps themselves:
            fitnessComponentEvaluationGroup.CreateFitnessComponentEvaluationGroup(fitnessManager, currentEvalTicket.genomeIndices[currentEvalTicket.focusPopIndex]);
            //Debug.Log("currentEvalTicket.focusPopIndex: " + currentEvalTicket.focusPopIndex.ToString() + ", index: " + currentEvalTicket.genomeIndices[currentEvalTicket.focusPopIndex].ToString());
            HookUpFitnessComponents();
        }        
    }
    private void CreateEnvironment() {
        // Check if this has already been built.
        // If it has NOT:
        if (teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]].environmentPrefab == null) {
            GameObject environmentGO = new GameObject("environment" + currentEvalTicket.genomeIndices[0].ToString());
            Environment environmentScript = environmentGO.AddComponent<Environment>();
            currentEnvironment = environmentScript;
            environmentGO.transform.parent = gameObject.transform;
            environmentGO.transform.localPosition = new Vector3(0f, 0f, 0f);
            // This might only work if environment is completely static!!!! otherwise it could change inside original evalInstance and then that
            // changed environment would be instantiated as fresh Environments for subsequent Evals!
            environmentScript.CreateCollisionAndGameplayContent(teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]]);
        }
        else {
            // Already built
            Environment environmentScript = Instantiate<Environment>(teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]].environmentPrefab) as Environment;
            currentEnvironment = environmentScript;
            currentEnvironment.gameObject.transform.parent = gameObject.transform;
            currentEnvironment.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        }
    }

    public void HookUpModules() {

        switch (challengeType) {
            case Challenge.Type.Test:
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentEnvironment.targetColumn.gameObject.transform;
                }
                break;
            case Challenge.Type.Racing:
                //ChallengeRacing challengeRacing = challengeGO.AddComponent<ChallengeRacing>();
                //currentChallenge = challengeRacing;
                break;
            case Challenge.Type.Combat:
                for (int i = 0; i < currentAgentsArray[0].targetSensorList.Count; i++) {
                    currentAgentsArray[0].targetSensorList[i].targetPosition = currentAgentsArray[1].segmentList[0].transform;
                }
                for (int i = 0; i < currentAgentsArray[1].targetSensorList.Count; i++) {
                    currentAgentsArray[1].targetSensorList[i].targetPosition = currentAgentsArray[0].segmentList[0].transform;
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
                    fitCompDistToTargetSquared.pointA[0] = currentAgentsArray[populationIndex].segmentList[0].transform.position;
                    //Debug.Log(currentAgentsArray[populationIndex].targetSensorList.ToString());
                    fitCompDistToTargetSquared.pointB[0] = currentAgentsArray[populationIndex].targetSensorList[0].targetPosition.position; //currentEnvironment.targetColumn.gameObject.transform.position;
                    break;
                case FitnessComponentType.Velocity:
                    FitCompVelocity fitCompVelocity = (FitCompVelocity)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompVelocity;
                    fitCompVelocity.vel = currentAgentsArray[populationIndex].segmentList[0].GetComponent<Rigidbody>().velocity;
                    break;
                case FitnessComponentType.ContactHazard:
                    FitCompContactHazard fitCompContactHazard = (FitCompContactHazard)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompContactHazard;
                    fitCompContactHazard.contactingHazard = currentAgentsArray[populationIndex].segmentList[0].GetComponent<ContactSensorComponent>().contact;
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
            agentGameScoresArray[0][0] = 0f;
            if (currentAgentsArray.Length > 1)
                agentGameScoresArray[1][0] = 0f;  // undecided

            if(currentAgentsArray[0].segmentList[0].GetComponent<HealthModuleComponent>()) {
                // if player 0 dead:
                if (currentAgentsArray[0].segmentList[0].GetComponent<HealthModuleComponent>().healthModule.health <= 0f) {
                    // if 2 players:
                    if (currentAgentsArray.Length > 1) {
                        //if player 0 dead AND player 1 dead:
                        if (currentAgentsArray[1].segmentList[0].GetComponent<HealthModuleComponent>().healthModule.health <= 0f) {
                            // ...then they died simultaneously -- DRAW
                        }
                        else { // player 0 dead and player 1 alive
                               // Player 1 WINS!
                            agentGameScoresArray[0][0] = -1f;
                            agentGameScoresArray[1][0] = 1f;
                            gameWonOrLost = true;
                            //Debug.Log("Player 1 WINS!");
                            
                            if (emit && currentEvalTicket.focusPopIndex == 2) {  // Only Agents
                                emitterParamsWin.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                particleCurves.Emit(emitterParamsWin, 8);
                            }
                            if (emit && currentEvalTicket.focusPopIndex == 1) {  // Only Agents
                                emitterParamsLose.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                particleCurves.Emit(emitterParamsLose, 8);
                            }
                        }
                    }
                    else { // player0 died and is only player
                        agentGameScoresArray[0][0] = -1f;
                        gameWonOrLost = true;
                    }
                }
                else {  // if Player 0 is alive:
                        // if 2 players:
                    if (currentAgentsArray.Length > 1) {
                        //if player 0 alive but player 1 dead:
                        if (currentAgentsArray[1].segmentList[0].GetComponent<HealthModuleComponent>().healthModule.health <= 0f) {
                            // Player 0 WINS!
                            agentGameScoresArray[0][0] = 1f;
                            agentGameScoresArray[1][0] = -1f;
                            gameWonOrLost = true;
                            //Debug.Log("Player 0 WINS!");
                            if (emit && currentEvalTicket.focusPopIndex == 1) {  // Only Agents
                                emitterParamsWin.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
                                particleCurves.Emit(emitterParamsWin, 8);
                            }
                            if (emit && currentEvalTicket.focusPopIndex == 2) {  // Only Agents
                                emitterParamsLose.position = currentAgentsArray[currentEvalTicket.focusPopIndex - 1].gameObject.transform.TransformPoint(currentAgentsArray[currentEvalTicket.focusPopIndex - 1].segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f)) - gameObject.transform.position;
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
    }
    public void AverageFitnessComponentsByTimeSteps() {
        for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
            if(fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.measure == FitnessComponentMeasure.Average) {
                fitnessComponentEvaluationGroup.fitCompList[i].rawScore /= currentTimeStep;
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
