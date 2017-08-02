using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentPopulation {

    public List<EnvironmentGenome> environmentGenomeList;
    //public TrainingSettings trainingSettings;  // mutation, max eval time, etc.
    //public FitnessSettings fitnessSettings;  // fitness function components and settings
    //public bool isTraining;  // is the population actively learning - i.e will go through selection and crossover, or is static and unchanging
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

        popSize = numGenomes;
        this.numBaseline = numBaseline;

        environmentGenomeList = new List<EnvironmentGenome>();
        historicGenomePool = new List<EnvironmentGenome>();
        baselineGenomePool = new List<EnvironmentGenome>();

        for (int e = 0; e < numGenomes; e++) {
            // Create new environmentGenome
            EnvironmentGenome envGenome = new EnvironmentGenome(e);
            envGenome.CopyGenomeFromTemplate(templateGenome);
            // Add to envGenomesList:
            environmentGenomeList.Add(envGenome);
            
            // Create parallel initial batch of genomes to be used as baseline comparison
            EnvironmentGenome baseGenome = new EnvironmentGenome(e);
            baseGenome.CopyGenomeFromTemplate(templateGenome);
            baselineGenomePool.Add(baseGenome);
        }
        AppendBaselineGenomes();

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();
        historicGenomePool.Add(environmentGenomeList[0]); // init               

        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(challengeType, fitnessManager);
        fitnessManager.InitializeForNewGeneration(environmentGenomeList.Count);

        trainingSettingsManager = new TrainingSettingsManager(0.02f, 0.5f);
    }

    public void TrimBaselineGenomes() {
        environmentGenomeList.RemoveRange(popSize, numBaseline);
    }
    public void AppendBaselineGenomes() {
        for (int j = 0; j < numBaseline; j++) {
            // randomly select x baseline genomes to add into primary genomeList
            // these will be tested alongside the primary pool
            int randIndex = Mathf.RoundToInt(UnityEngine.Random.Range(0f, (float)baselineGenomePool.Count - 1f));
            environmentGenomeList.Add(baselineGenomePool[randIndex]);
        }
    }

    private void SetUpDefaultFitnessComponents(Challenge.Type challengeType, FitnessManager fitnessManager) {

        switch (challengeType) {
            case Challenge.Type.Test:
                FitnessComponentDefinition newComponent = new FitnessComponentDefinition(FitnessComponentType.DistanceToTargetSquared, FitnessComponentMeasure.Avg, 0.1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(newComponent);
                FitnessComponentDefinition contactHazard = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(contactHazard);
                break;
            case Challenge.Type.Racing:
                FitnessComponentDefinition fitCompRacing1 = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompRacing1);
                break;
            case Challenge.Type.Combat:
                FitnessComponentDefinition fitCompCombat1 = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat1);
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                break;
        }

        fitnessManager.SetPendingFitnessListFromMaster(); // make pending list a copy of the primary
    }

    public void ResetRepresentativesList() {
        /*if (representativeGenomeList == null) {
            representativeGenomeList = new List<EnvironmentGenome>();
        }
        else {
            representativeGenomeList.Clear();
        }        

        for (int i = 0; i < numPerformanceReps; i++) {            
            representativeGenomeList.Add(environmentGenomeList[i]);                       
        }*/


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
