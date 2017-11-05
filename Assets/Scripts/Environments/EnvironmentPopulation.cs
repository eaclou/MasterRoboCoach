using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentPopulation {

    public EnvironmentGenome templateGenome;
    public List<EnvironmentGenome> environmentGenomeList;
    //public TrainingSettings trainingSettings;  // mutation, max eval time, etc.
    //public FitnessSettings fitnessSettings;  // fitness function components and settings
    //public bool isTraining;  // is the population actively learning - i.e will go through selection and crossover, or is static and unchanging
    [System.NonSerialized]
    public List<EnvironmentGenome> representativeGenomeList;  // the list of agentGenomes that will be opponents for all other populations this round
    public List<EnvironmentGenome> historicGenomePool;  // a collection of predecessor genomes that can be chosen from
    public List<EnvironmentGenome> baselineGenomePool;  // a collection of blank and random genomes for fitness comparison purposes.
    public int maxHistoricGenomePoolSize = 100;

    public int popSize;
    public int numBaseline;
    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.
    public bool isTraining = true;
    public int numPerformanceReps = 1;
    public int numHistoricalReps = 0;
    public int numBaselineReps = 0;

    public EnvironmentPopulation(Challenge.Type challengeType, EnvironmentGenome templateGenome, int numGenomes, int numBaseline, int numReps) {

        this.templateGenome = templateGenome;
        popSize = numGenomes;
        this.numBaseline = numBaseline;

        environmentGenomeList = new List<EnvironmentGenome>();
        historicGenomePool = new List<EnvironmentGenome>();
        baselineGenomePool = new List<EnvironmentGenome>();

        for (int e = 0; e < numGenomes; e++) {
            // Create new environmentGenome
            EnvironmentGenome envGenome = new EnvironmentGenome(e);
            envGenome.InitializeRandomGenomeFromTemplate(templateGenome);
            // Add to envGenomesList:
            environmentGenomeList.Add(envGenome);
            
            // Create parallel initial batch of genomes to be used as baseline comparison
            EnvironmentGenome baseGenome = new EnvironmentGenome(e);
            baseGenome.InitializeRandomGenomeFromTemplate(templateGenome);
            baselineGenomePool.Add(baseGenome);
        }
        AppendBaselineGenomes();

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();
        historicGenomePool.Add(environmentGenomeList[0]); // init               

        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(challengeType, fitnessManager);
        fitnessManager.ResetHistoricalData();
        fitnessManager.InitializeForNewGeneration(environmentGenomeList.Count);

        trainingSettingsManager = new TrainingSettingsManager(1f, 1f, 0f, 0f);
    }

    public void InitializeLoadedPopulation() {
        ResetRepresentativesList();

        // Fitness Manager
        fitnessManager.InitializeLoadedData(environmentGenomeList.Count);
        // Training Settings Manager:
        // -- so simple at this point no init is needed, it's just 2 floats
    }

    public void TrimBaselineGenomes() {
        //Debug.Log("TrimBaselineGenomes RemoveRange " + popSize.ToString() + ", " + numBaseline.ToString() + ", envList: " + environmentGenomeList.Count.ToString());
        environmentGenomeList.RemoveRange(popSize, numBaseline);
    }
    public void AppendBaselineGenomes() {
        int[] scrambledIndices = new int[baselineGenomePool.Count];
        for (int x = 0; x < scrambledIndices.Length; x++) {
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
            environmentGenomeList.Add(baselineGenomePool[scrambledIndices[j]]);
        }

        // Apply correct Indices!
        for(int i = 0; i < environmentGenomeList.Count; i++) {
            environmentGenomeList[i].index = i;
        }
    }

    public void ChangeGenomeTemplate(EnvironmentGenome pendingGenome) {
        //Debug.Log("ChangeGenomeTemplate PENDING startPosCount: " + pendingGenome.agentStartPositionsList.Count.ToString());
        // Apply to Population Template:
        templateGenome.CopyGenomeFromTemplate(pendingGenome); // sets contents of genome to a copy of the sourceGenome

        // Apply to each Environment Genome:
        for (int i = 0; i < environmentGenomeList.Count; i++) {
            environmentGenomeList[i].CopyGenomeFromTemplate(pendingGenome);
        }

        //Debug.Log("ChangeGenomeTemplate TEMPLATE startPosCount: " + templateGenome.agentStartPositionsList.Count.ToString());
        //Debug.Log("ChangeGenomeTemplate ACTIVE startPosCount: " + environmentGenomeList[0].agentStartPositionsList.Count.ToString());
    }

    private void SetUpDefaultFitnessComponents(Challenge.Type challengeType, FitnessManager fitnessManager) {

        switch (challengeType) {
            case Challenge.Type.Test:
                FitnessComponentDefinition newComponent = new FitnessComponentDefinition(FitnessComponentType.Random, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(newComponent);
                break;
            case Challenge.Type.Racing:
                FitnessComponentDefinition newComponentRacing = new FitnessComponentDefinition(FitnessComponentType.Random, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(newComponentRacing);
                break;
            case Challenge.Type.Combat:
                FitnessComponentDefinition newComponentCombat = new FitnessComponentDefinition(FitnessComponentType.Random, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(newComponentCombat);
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                break;
        }

        fitnessManager.SetPendingFitnessListFromMaster(); // make pending list a copy of the primary
    }

    public void ResetRepresentativesList() {
        
        if (representativeGenomeList == null) {
            representativeGenomeList = new List<EnvironmentGenome>();
        }
        else {
            representativeGenomeList.Clear();
        }        

        for (int i = 0; i < numPerformanceReps; i++) {
            representativeGenomeList.Add(environmentGenomeList[i]);
        }
        for (int i = 0; i < numHistoricalReps; i++) {
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)historicGenomePool.Count - 1f));
            representativeGenomeList.Add(historicGenomePool[randIndex]);
        }
        for (int i = 0; i < numBaselineReps; i++) {
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)baselineGenomePool.Count - 1f));
            representativeGenomeList.Add(baselineGenomePool[randIndex]);
        }
    }
}
