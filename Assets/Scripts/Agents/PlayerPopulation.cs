using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPopulation {

    //public AgentGenome templateGenome;
    public List<AgentGenome> agentGenomeList;  // Primary genome population
    //public TrainingSettings trainingSettings;  // mutation, max eval time, etc.
    //public FitnessSettings fitnessSettings;  // fitness function components and settings
    //public bool isTraining;  // is the population actively learning - i.e will go through selection and crossover, or is static and unchanging
    public List<AgentGenome> representativeGenomeList;  // the list of agentGenomes that will be opponents for all other populations this round
    public List<AgentGenome> historicGenomePool;  // a collection of predecessor genomes that can be chosen from
    public List<AgentGenome> baselineGenomePool;  // a collection of blank and random genomes for fitness comparison purposes.
    public int maxHistoricGenomePoolSize = 100;

    public int popSize;
    public int numBaseline;
    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.
    public bool isTraining = true;
    public int numPerformanceReps = 1;
    public int numHistoricalReps = 0;
    public int numBaselineReps = 0;

    [System.NonSerialized]
    public BodyGenome bodyGenomeTemplate;

    // Representative system will be expanded later - for now, just defaults to Top # of performers
    public PlayerPopulation(Challenge.Type challengeType, BodyGenome bodyTemplate, int numGenomes, int numBaseline, int numReps) {
        bodyGenomeTemplate = new BodyGenome();
        bodyGenomeTemplate.CopyBodyGenomeFromTemplate(bodyTemplate);
        
        popSize = numGenomes;
        this.numBaseline = numBaseline;
        
        // Create blank AgentGenomes for the standard population
        agentGenomeList = new List<AgentGenome>();
        historicGenomePool = new List<AgentGenome>();
        baselineGenomePool = new List<AgentGenome>();

        for (int j = 0; j < numGenomes; j++) {
            AgentGenome agentGenome = new AgentGenome(j);
            agentGenome.InitializeBodyGenomeFromTemplate(bodyGenomeTemplate);
            agentGenome.InitializeRandomBrainFromCurrentBody(0.0f);            
            agentGenomeList.Add(agentGenome);


            AgentGenome baselineGenome = new AgentGenome(j);
            baselineGenome.InitializeBodyGenomeFromTemplate(bodyGenomeTemplate);
            baselineGenome.InitializeRandomBrainFromCurrentBody(0.0f);
            baselineGenomePool.Add(baselineGenome);            
        }
        AppendBaselineGenomes();

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();
        historicGenomePool.Add(agentGenomeList[0]); // init

        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(challengeType, fitnessManager);
        fitnessManager.InitializeForNewGeneration(agentGenomeList.Count);
        
        trainingSettingsManager = new TrainingSettingsManager(0.005f, 0.5f, 0f, 0f);
    }
    public void InitializeLoadedPopulation() {
        // Assumes template has been set from defaults!
        ResetRepresentativesList();
        fitnessManager.InitializeLoadedData(agentGenomeList.Count);

        // ENV:
        //ResetRepresentativesList();
        // Fitness Manager
        //fitnessManager.InitializeLoadedData(popSize);
        // Training Settings Manager:
        // -- so simple at this point no init is needed, it's just 2 floats
    }

    public void TrimBaselineGenomes() {
        agentGenomeList.RemoveRange(popSize, numBaseline);
    }
    public void AppendBaselineGenomes() {
        int[] scrambledIndices = new int[baselineGenomePool.Count];
        for(int x = 0; x < scrambledIndices.Length; x++) {
            scrambledIndices[x] = x;
        }
        // scramble!
        int numScrambles = baselineGenomePool.Count * 2;
        for (int y = 0; y < numScrambles; y++) {
            int indexA = UnityEngine.Random.Range(0, scrambledIndices.Length - 1);
            int indexB = UnityEngine.Random.Range(0, scrambledIndices.Length - 1);

            int swapA = scrambledIndices[indexA];
            int swapB = scrambledIndices[indexB];

            scrambledIndices[indexA] = swapB;
            scrambledIndices[indexB] = swapA;
        }
        for (int j = 0; j < numBaseline; j++) {
            // randomly select x baseline genomes to add into primary genomeList
            // these will be tested alongside the primary pool
            //int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)baselineGenomePool.Count - 1f));
            agentGenomeList.Add(baselineGenomePool[scrambledIndices[j]]);
        }

        // Apply correct Indices!
        for (int i = 0; i < agentGenomeList.Count; i++) {
            agentGenomeList[i].index = i;
        }
    }

    public void ChangeBodyTemplate(BodyGenome pendingBody) {
        // Change the Body Composition of this population's Agents:
        // -- Replace the BodyGenome of population's existing templateBody with a copy of the pendingBody
        // -- Do the same for all current Agents in the population
        // -- Replace the bodyNeurons for all current Agents in the population
        // -- Remove vestigial brain connections/nodes

        bodyGenomeTemplate.CopyBodyGenomeFromTemplate(pendingBody); // sets contents of body to a copy of the sourceGenome
             
        for(int i = 0; i < agentGenomeList.Count; i++) {
            agentGenomeList[i].bodyGenome.CopyBodyGenomeFromTemplate(pendingBody);
            agentGenomeList[i].brainGenome.SetBodyNeuronsFromTemplate(pendingBody);
        }

        // once all existing agents are processed, update templateGenome to be the new one
        // population's templateGenome is basically only for body-plan. All agents will share same Input/Output neurons, but differ in their hidden neurons + connections

        //templateGenome = pendingGenome;
    }

    private void SetUpDefaultFitnessComponents(Challenge.Type challengeType, FitnessManager fitnessManager) {
         
        switch(challengeType) {
            case Challenge.Type.Test:
                FitnessComponentDefinition distToTargetSquared = new FitnessComponentDefinition(FitnessComponentType.DistanceToTargetSquared, FitnessComponentMeasure.Avg, 0.1f, false);
                fitnessManager.fitnessComponentDefinitions.Add(distToTargetSquared);
                break;
            case Challenge.Type.Racing:
                FitnessComponentDefinition fitCompRacing1 = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 1f, false);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompRacing1);
                FitnessComponentDefinition fitCompRacing2 = new FitnessComponentDefinition(FitnessComponentType.Velocity, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompRacing2);
                break;
            case Challenge.Type.Combat:
                FitnessComponentDefinition fitCompCombat1 = new FitnessComponentDefinition(FitnessComponentType.Random, FitnessComponentMeasure.Avg, 0.5f, false);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat1);
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                break;
        }

        fitnessManager.SetPendingFitnessListFromMaster(); // make pending list a copy of the primary
    }

    public void ResetRepresentativesList() {
        if(representativeGenomeList == null) {
            representativeGenomeList = new List<AgentGenome>();
        }
        representativeGenomeList.Clear();

        for (int i = 0; i < numPerformanceReps; i++) {
            representativeGenomeList.Add(agentGenomeList[i]);
        }
        for (int i = 0; i < numHistoricalReps; i++) {
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)historicGenomePool.Count - 1f));
            representativeGenomeList.Add(historicGenomePool[randIndex]);
        }
        for (int i = 0; i < numBaselineReps; i++) {
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)baselineGenomePool.Count - 1f));
            representativeGenomeList.Add(baselineGenomePool[randIndex]);
        }

        //string txt = "RepresentativeList:";
        //for(int i = 0; i < representativeGenomeList.Count; i++) {
        //    txt += i.ToString() + ", " + representativeGenomeList[i].index.ToString() + "... ";
        //}
        //Debug.Log(txt);
    }
    
}
