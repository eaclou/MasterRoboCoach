using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public bool isTraining = false;
    
    public TrainingManager trainerRef;
    
    // Use this for initialization
    void Start () {
        //Debug.Log(Quaternion.Euler(0f, -180f, 0f).ToString());
        
        //trainerRef.EnterTrainingMode(Challenge.Type.Test);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    /*
    void FixedUpdate() {
        if(isTraining) {
            UpdateTargetSensor();
            agentGOList[playingCurAgent].GetComponent<Agent>().TickBrain();
            agentGOList[playingCurAgent].GetComponent<Agent>().RunModules();

            UpdatePlayingState();
        }
        Time.timeScale = timeScale;
    }

    public void UpdatePlayingState() {
        
        // Update gameState timestep etc.
        playingCurTimeStep++;
        // Check trainingState
        if (playingCurTimeStep > maxTimeSteps) {
            // Next Agent
            playingCurTimeStep = 0;

            DisableCurrentAgent();

            playingCurAgent++;
            //Debug.Log("UpdatePlayingState! " + playingCurAgent.ToString());
            if (playingCurAgent >= populationSize) {
                // Next Gen
                NextGeneration();
                playingCurAgent = 0;
                playingCurGen++;
                EnableCurrentAgent();
                Debug.Log("NextGen! " + playingCurGen.ToString());
            }
            else {
                EnableCurrentAgent();
            }
        }
        else {

        }
    }

    private void NextGeneration() {
        List<BrainGenome> newGenBrainGenomeList = new List<BrainGenome>(); // new population!

        // find best performers of last gen:
        float[] rankedFitnessList = new float[populationSize];
        int[] rankedAgentIndices = new int[populationSize];

        // GROSS brute force:
        // populate arrays:
        for(int i = 0; i < populationSize; i++) {
            rankedAgentIndices[i] = i;
            rankedFitnessList[i] = agentGOList[i].GetComponent<Agent>().fitTotalDistance;
        }
        for(int i = 0; i < populationSize - 1; i++) {
            for(int j = 0; j < populationSize - 1; j++) {
                float swapFitA = rankedFitnessList[j];
                float swapFitB = rankedFitnessList[j+1];
                int swapIdA = rankedAgentIndices[j];
                int swapIdB = rankedAgentIndices[j+1];

                if(swapFitA > swapFitB) {
                    rankedFitnessList[j] = swapFitB;
                    rankedFitnessList[j+1] = swapFitA;
                    rankedAgentIndices[j] = swapIdB;
                    rankedAgentIndices[j+1] = swapIdA;
                }
            }
        }
        string fitnessRankText = "";
        for (int i = 0; i < populationSize; i++) {
            fitnessRankText += "Agent[" + rankedAgentIndices[i].ToString() + "]: " + rankedFitnessList[i].ToString() + "\n";
        }
        Debug.Log(fitnessRankText);

        // Keep top-half peformers + mutations:
        for(int x = 0; x < (populationSize / 2); x++) {
            BrainGenome newBrainGenome = new BrainGenome();
            // new BrainGenome creates new neuronList and linkList
            BrainGenome parentGenome = agentGOList[rankedAgentIndices[x]].GetComponent<Agent>().genome.brainGenome;

            newBrainGenome.neuronList = parentGenome.neuronList; // UNSUSTAINABLE!!! might work now since all neuronLists are identical
            for (int i = 0; i < parentGenome.linkList.Count; i++) {
                LinkGenome newLinkGenome = new LinkGenome(parentGenome.linkList[i].fromModuleID, parentGenome.linkList[i].fromNeuronID, parentGenome.linkList[i].toModuleID, parentGenome.linkList[i].toNeuronID, parentGenome.linkList[i].weight, true);
                float rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < mutationChance) {
                    float randomWeight = Gaussian.GetRandomGaussian();
                    newLinkGenome.weight = randomWeight;
                }
                newBrainGenome.linkList.Add(newLinkGenome);
            }
            newGenBrainGenomeList.Add(newBrainGenome);
            //agentGOList[x].GetComponent<Agent>().genome.brainGenome = newBrainGenome;            
        }
        // bottom half:
        for (int x = 0; x < (populationSize / 2); x++) {
            BrainGenome newBrainGenome = new BrainGenome();
            // new BrainGenome creates new neuronList and linkList
            BrainGenome parentGenome = agentGOList[rankedAgentIndices[x]].GetComponent<Agent>().genome.brainGenome;

            newBrainGenome.neuronList = parentGenome.neuronList; // UNSUSTAINABLE!!! might work now since all neuronLists are identical
            for (int i = 0; i < parentGenome.linkList.Count; i++) {
                LinkGenome newLinkGenome = new LinkGenome(parentGenome.linkList[i].fromModuleID, parentGenome.linkList[i].fromNeuronID, parentGenome.linkList[i].toModuleID, parentGenome.linkList[i].toNeuronID, parentGenome.linkList[i].weight, true);
                float rand = UnityEngine.Random.Range(0f, 1f);
                if (rand < mutationChance) {
                    float randomWeight = Gaussian.GetRandomGaussian();
                    newLinkGenome.weight = randomWeight;
                }
                newBrainGenome.linkList.Add(newLinkGenome);
            }
            newGenBrainGenomeList.Add(newBrainGenome);

            //BrainGenome newBrainGenome = new BrainGenome();
            //newBrainGenome = agentGOList[rankedAgentIndices[x]].GetComponent<Agent>().genome.brainGenome;
            //newBrainGenome.MutateRandomly(mutationChance);
            //newGenBrainGenomeList.Add(newBrainGenome);
            //AgentGenome agentGenome = new AgentGenome();
            //agentGenome.TempInitializeTestGenome();
            //agentGOList[x + (populationSize / 2)].GetComponent<Agent>().genome = agentGenome;
        }        

        for(int i = 0; i < populationSize; i++) {
            agentGOList[i].GetComponent<Agent>().genome.brainGenome = newGenBrainGenomeList[i];
        }
    }

    private void DisableCurrentAgent() {
        //Debug.Log("Agent[" + playingCurAgent.ToString() + "] fitness: " + agentGOList[playingCurAgent].GetComponent<Agent>().fitTotalDistance.ToString());
        agentGOList[playingCurAgent].SetActive(false);
    }
    private void EnableCurrentAgent() {
        agentGOList[playingCurAgent].SetActive(true);
        agentGOList[playingCurAgent].GetComponent<Agent>().ConstructAgentFromGenome(agentGOList[playingCurAgent]);

        // GROSS:
        targetX = agentGOList[playingCurAgent].GetComponent<Agent>().targetSensorList[0].dotX;
        targetY = agentGOList[playingCurAgent].GetComponent<Agent>().targetSensorList[0].dotY;
        targetZ = agentGOList[playingCurAgent].GetComponent<Agent>().targetSensorList[0].dotZ;
    }

    private void UpdateTargetSensor() {
        Vector3 segmentToTargetVect = new Vector3(target.position.x - agentGOList[playingCurAgent].GetComponent<Agent>().segmentList[0].transform.position.x, target.position.y - agentGOList[playingCurAgent].GetComponent<Agent>().segmentList[0].transform.position.y, target.position.z - agentGOList[playingCurAgent].GetComponent<Agent>().segmentList[0].transform.position.z);
        Vector3 rightVector;
        Vector3 upVector;
        Vector3 forwardVector;

        rightVector = agentGOList[playingCurAgent].GetComponent<Agent>().segmentList[0].transform.right;
        upVector = agentGOList[playingCurAgent].GetComponent<Agent>().segmentList[0].transform.up;
        forwardVector = agentGOList[playingCurAgent].GetComponent<Agent>().segmentList[0].transform.forward;

        float dotRight = Vector3.Dot(segmentToTargetVect, rightVector);
        float dotUp = Vector3.Dot(segmentToTargetVect, upVector);
        float dotForward = Vector3.Dot(segmentToTargetVect, forwardVector);

        targetX[0] = dotRight;
        targetY[0] = dotUp;
        targetZ[0] = dotForward;

        float distanceSquared = dotRight * dotRight + dotUp * dotUp + dotForward * dotForward;
        agentGOList[playingCurAgent].GetComponent<Agent>().fitTotalDistance += distanceSquared;
    }

    private void ResetTrainedAgentsList() {
        trainedAgentsList.Clear();
        for (int i = 0; i < populationSize; i++) {
            trainedAgentsList.Add(0);
        }
    }

    public void testInitializeTraining() {
        agentGenomeList = new List<AgentGenome>();

        ResetTrainedAgentsList();

        // Create initial Player Population        
        for (int i = 0; i < populationSize; i++) {
            AgentGenome agentGenome = new AgentGenome();
            agentGenome.TempInitializeTestGenome();
            agentGenomeList.Add(agentGenome);
        }

        // Instantiate Agents
        for(int i = 0; i < populationSize; i++) {
            GameObject agentGO = new GameObject("agent" + i.ToString());            
            Agent agentScript = agentGO.AddComponent<Agent>();
            agentGOList.Add(agentGO);
            agentScript.SetGenome(agentGenomeList[i]);
            agentGO.SetActive(false);
        }

        // HACKY! first agent:
        EnableCurrentAgent();

        isTraining = true;
    }
    */
}
