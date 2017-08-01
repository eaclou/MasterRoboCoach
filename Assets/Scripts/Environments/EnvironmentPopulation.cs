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

    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.
    public bool isTraining = true;
    public int numPerformanceReps = 1;

    public EnvironmentPopulation(Challenge.Type challengeType, EnvironmentGenome templateGenome, int numGenomes, int numReps) {
        //agentGenomeList = new List<AgentGenome>();
        //for (int j = 0; j < numGenomes; j++) {
        //    AgentGenome agentGenome = new AgentGenome(j);  // empty constructor
        //    agentGenome.CopyGenomeFromTemplate(templateGenome);  // copies attributes and creates random brain -- roll into Constructor method?
        //    agentGenomeList.Add(agentGenome);
        //}

        environmentGenomeList = new List<EnvironmentGenome>();
        for (int e = 0; e < numGenomes; e++) {
            // Create new environmentGenome
            EnvironmentGenome envGenome = new EnvironmentGenome(e);
            envGenome.CopyGenomeFromTemplate(templateGenome);
            //envGenome.TempInitializeGenome();  // Roll this into Constructor??
            environmentGenomeList.Add(envGenome);
            // Add to envGenomesList:
        }

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();        
        
        historicGenomePool = new List<EnvironmentGenome>();
        baselineGenomePool = new List<EnvironmentGenome>();

        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(challengeType, fitnessManager);
        fitnessManager.InitializeForNewGeneration(numGenomes);

        trainingSettingsManager = new TrainingSettingsManager(0.02f, 0.5f);
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
        if (representativeGenomeList == null) {
            representativeGenomeList = new List<EnvironmentGenome>();
        }
        else {
            representativeGenomeList.Clear();
        }        

        for (int i = 0; i < numPerformanceReps; i++) {            
            representativeGenomeList.Add(environmentGenomeList[i]);                       
        }
    }
}
