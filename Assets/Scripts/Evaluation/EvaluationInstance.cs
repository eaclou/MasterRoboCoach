using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationInstance : MonoBehaviour {
    public bool visible = true;
    public bool isExhibition = false;

    public ParticleSystem particleCurves;
    public ParticleSystem.EmitParams emitterParams;

    public EvaluationTicket currentEvalTicket;
    private TeamsConfig teamsConfig;
    private Challenge.Type challengeType;
    // this will hold reference to which agents & environment are being tested, for score allocation

    public FitnessComponentEvaluationGroup fitnessComponentEvaluationGroup;

    public Agent currentAgent;
    private Environment currentEnvironment;
    private ChallengeBase currentChallenge;

    public int maxTimeSteps;
    private int currentTimeStep = 0;

    //private Transform targetPos;
    public float score = 0f;

    // Use this for initialization
    void Start () {
        //ParticleSystem.EmitParams emitterParams = new ParticleSystem.EmitParams();        
        //emitterParams.startLifetime = 1000f;       
    }
	
	// Update is called once per frame
	void Update () {
		
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

    public void Tick() {
        //print("Tick! " + currentTimeStep.ToString());
        
        CalculateScore();        
        
        currentAgent.TickBrain();
        currentAgent.RunModules();

        if(CheckForEvaluationEnd()) {
            currentEvalTicket.status = EvaluationTicket.EvaluationStatus.PendingComplete;
        }
        else {
            currentTimeStep++;
        }

        if (currentEvalTicket.focusPopIndex == 1 && currentEvalTicket.genomeIndices[0] == 0) {  // Only Agents on Environment 0
            //emitterParams.position = currentAgent.segmentList[0].transform.localPosition + new Vector3(0f, 0.25f, 0f);
            //particleCurves.Emit(emitterParams, 1);
        }        
    }

    private void CalculateScore() {
        if(!isExhibition) {
            // Temp for now: in order to update positions...
            HookUpFitnessComponents();

            for (int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
                fitnessComponentEvaluationGroup.fitCompList[i].TickScore();
            }
        }                
    }

    private bool CheckForEvaluationEnd() {
        if(currentTimeStep > maxTimeSteps) {
            return true;
        }
        else {
            return false;
        }
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

    public void SetUpInstance(EvaluationTicket evalTicket, TeamsConfig teamsConfig) {
        this.teamsConfig = teamsConfig;
        this.challengeType = teamsConfig.challengeType;
        this.maxTimeSteps = evalTicket.maxTimeSteps;
        
        currentEvalTicket = evalTicket;
        
        // Handle Environment:
        // Handle Agents:
        // Handle Challenge Setup

        //print(currentEvalPair.ToString());        
        BruteForceInit();
        currentEvalTicket.status = EvaluationTicket.EvaluationStatus.InProgress;

        if (currentEvalTicket.genomeIndices[0] != 0 || currentEvalTicket.genomeIndices[1] != 0 || currentEvalTicket.focusPopIndex == 0) {
            //emitterParams.startColor = new Color(0.66f, 0.66f, 0.66f, 0.5f);
        }
        else {
            //emitterParams.startColor = new Color(1f, 1f, 0f, 1f);
            //emitterParams.startSize = 0.6f;
        }
    }

    private void CreateEnvironment() {
        // Check if this has already been built.
        // If it has NOT:
        if(teamsConfig.environmentPopulation.environmentGenomeList[currentEvalTicket.genomeIndices[0]].environmentPrefab == null) {
            GameObject environmentGO = new GameObject("environment" + currentEvalTicket.genomeIndices[0].ToString());
            Environment environmentScript = environmentGO.AddComponent<Environment>();
            currentEnvironment = environmentScript;
            environmentGO.transform.parent = gameObject.transform;
            environmentGO.transform.localPosition = new Vector3(0f, 0f, 0f);
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

    private void BruteForceInit() {
        // REFACTOR THIS!!!!!!!!!!!!!!

        // Clear Everything:
        DeleteAllGameObjects();        

        currentTimeStep = 0;
        score = 0f;

        // Create Agent:
        // Alt: have ONE agent per evalInstance -- passed in during initialization of this EvaluationInstance
        // That Agent holds a reference to a GameObject (the AgentInstance)
        GameObject agentGO = new GameObject("agent" + currentEvalTicket.genomeIndices[1].ToString());
        Agent agentScript = agentGO.AddComponent<Agent>();
        //agentScript.SetGenome(teamsConfig.playersList[0].agentGenomeList[currentEvalPair.evalPairIndices[1]]);
        agentScript.ConstructAgentFromGenome(teamsConfig.playersList[0].agentGenomeList[currentEvalTicket.genomeIndices[1]]);
        currentAgent = agentScript;
        agentGO.transform.parent = gameObject.transform;
        agentGO.transform.localPosition = new Vector3(0f, 0f, 0f);

        // Create Environment:
        CreateEnvironment();        

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

        currentChallenge.agent = currentAgent;
        currentChallenge.environment = currentEnvironment;

        currentChallenge.HookUpModules();
        
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

    public void HookUpFitnessComponents() {

        for(int i = 0; i < fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
            
            switch (fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type) {
                case FitnessComponentType.DistanceToTargetSquared:
                    FitCompDistanceToTargetSquared fitCompDistToTargetSquared = (FitCompDistanceToTargetSquared)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompDistanceToTargetSquared;
                    fitCompDistToTargetSquared.pointA[0] = currentAgent.segmentList[0].transform.position;
                    fitCompDistToTargetSquared.pointB[0] = currentEnvironment.targetColumn.gameObject.transform.position;
                    break;
                case FitnessComponentType.Velocity:
                    FitCompVelocity fitCompVelocity = (FitCompVelocity)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompVelocity;
                    fitCompVelocity.vel = currentAgent.segmentList[0].GetComponent<Rigidbody>().velocity;
                    break;
                case FitnessComponentType.ContactHazard:
                    FitCompContactHazard fitCompContactHazard = (FitCompContactHazard)fitnessComponentEvaluationGroup.fitCompList[i] as FitCompContactHazard;
                    fitCompContactHazard.contactingHazard = currentAgent.segmentList[0].GetComponent<ContactSensor>().contact;
                    //fitCompContactHazard
                    break;
                default:
                    break;
            }
        }
    }
}
