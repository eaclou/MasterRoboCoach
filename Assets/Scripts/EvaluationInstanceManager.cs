using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvaluationInstanceManager : MonoBehaviour {
    public bool visible = true;

    public ParticleSystem particleCurves;
    public ParticleSystem.EmitParams emitterParams;

    public EvaluationPair currentEvalPair;
    private TeamsConfig teamsConfig;
    private Challenge.Type challengeType;

    public Agent currentAgent;
    private Environment currentEnvironment;
    private ChallengeBase currentChallenge;

    public int maxTimeSteps;
    private int currentTimeStep = 0;

    //private Transform targetPos;
    public float score = 0f;

    // Use this for initialization
    void Start () {
        ParticleSystem.EmitParams emitterParams = new ParticleSystem.EmitParams();
        //emitterParams.position = Vector3.one;
        emitterParams.startLifetime = 1000f;
        //particleTrajectories.Emit(emitterParams, 1);        
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

        //UpdateTargetSensor();
        CalculateScore();
        currentAgent.TickBrain();
        currentAgent.RunModules();

        if(CheckForEvaluationEnd()) {
            currentEvalPair.status = EvaluationPair.EvaluationStatus.PendingComplete;
        }
        else {
            currentTimeStep++;
        }

        if (currentEvalPair.focusPopIndex == 1) {  // Only Agents
            emitterParams.position = currentAgent.segmentList[0].transform.localPosition;
            particleCurves.Emit(emitterParams, 1);
        }        
    }

    private void CalculateScore() {
        Vector3 agentToTarget = currentEnvironment.targetColumn.gameObject.transform.position - currentAgent.segmentList[0].transform.position;
        float agentScore = 0f;
        agentScore += agentToTarget.sqrMagnitude * 1f;
        //agentScore += currentAgent.raycastSensorList[0].distanceLeftCenter[0] * 0.1f;
        //agentScore += currentAgent.raycastSensorList[0].distanceCenter[0] * 0.2f;
        //agentScore += currentAgent.raycastSensorList[0].distanceRightCenter[0] * 0.1f;
        if (currentAgent.contactSensorList[0].contact) {
            agentScore += 10000f;
        }

        if (currentEvalPair.focusPopIndex == 0) {
            // environment
            score -= agentScore;
        }
        else {
            // agent
            score += agentScore;            
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
        currentEvalPair.status = EvaluationPair.EvaluationStatus.Pending;
        currentEvalPair = null;
        DeleteAllGameObjects();
    }

    public void DeleteAllGameObjects() {
        var children = new List<GameObject>();
        foreach (Transform child in gameObject.transform) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
    }

    public void SetUpInstance(EvaluationPair evalPair, TeamsConfig teamsConfig, Challenge.Type challengeType, int maxTimeSteps) {
        this.teamsConfig = teamsConfig;
        this.challengeType = challengeType;
        this.maxTimeSteps = maxTimeSteps;
        
        currentEvalPair = evalPair;
        
        // Handle Environment:
        // Handle Agents:
        // Handle Challenge Setup

        //print(currentEvalPair.ToString());        
        BruteForceInit();
        currentEvalPair.status = EvaluationPair.EvaluationStatus.InProgress;

        if (currentEvalPair.evalPairIndices[0] != 0 || currentEvalPair.evalPairIndices[1] != 0 || currentEvalPair.focusPopIndex == 0) {
            emitterParams.startColor = new Color(0.66f, 0.66f, 0.66f, 0.6f);
        }
        else {
            emitterParams.startColor = new Color(0.8f, 0.95f, 0.66f, 1f);
            emitterParams.startSize = 1.1f;
        }
    }

    private void BruteForceInit() {
        // Clear Everything:
        DeleteAllGameObjects();        

        currentTimeStep = 0;
        score = 0f;

        // Create Agent:
        GameObject agentGO = new GameObject("agent" + currentEvalPair.evalPairIndices[1].ToString());
        Agent agentScript = agentGO.AddComponent<Agent>();
        agentScript.SetGenome(teamsConfig.playersList[0][currentEvalPair.evalPairIndices[1]]);
        agentScript.ConstructAgentFromGenome(agentGO);
        currentAgent = agentScript;
        agentGO.transform.parent = gameObject.transform;
        agentGO.transform.localPosition = new Vector3(0f, 0f, 0f);        

        // Create Environment:
        GameObject environmentGO = new GameObject("environment" + currentEvalPair.evalPairIndices[0].ToString());
        Environment environmentScript = environmentGO.AddComponent<Environment>();
        currentEnvironment = environmentScript;
        environmentGO.transform.parent = gameObject.transform;
        environmentGO.transform.localPosition = new Vector3(0f, 0f, 0f);
        environmentScript.SetGenome(teamsConfig.environmentGenomesList[currentEvalPair.evalPairIndices[0]]);
        environmentScript.ConstructEnvironmentFromGenome();

        // Create Challenge Instance:
        GameObject challengeGO = new GameObject("challenge" + challengeType.ToString());
        ChallengeTest challengeScript = challengeGO.AddComponent<ChallengeTest>();
        currentChallenge = challengeScript;
        challengeGO.transform.parent = gameObject.transform;
        challengeGO.transform.localPosition = new Vector3(0f, 0f, 0f);

        challengeScript.agent = agentScript;
        challengeScript.environment = environmentScript;

        //targetPos = targetSphere.transform;

        // Construct / Initialize Challenge
        // create target
        // hook into agent modules (targetSensor)
        //for (int i = 0; i < agentScript.targetSensorList.Count; i++) {
        //    agentScript.targetSensorList[i].targetPosition = environmentScript.targetColumn.gameObject.transform;
        //}
        challengeScript.HookUpModules();

        if(visible) {
            SetVisibleTraverse(gameObject);
        }
        else {
            SetInvisibleTraverse(gameObject);
        }
    }
}
