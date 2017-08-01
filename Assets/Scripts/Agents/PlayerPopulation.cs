﻿using System.Collections;
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

    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.
    public bool isTraining = true;
    public int numPerformanceReps = 1;
    public int numHistoricalReps = 0;
    public int numBaselineReps = 0;

    // Representative system will be expanded later - for now, just defaults to Top # of performers
    public PlayerPopulation(Challenge.Type challengeType, AgentGenome templateGenome, int numGenomes, int numReps) {
        //this.templateGenome = new AgentGenome(-1);
        //templateGenome.CopyGenomeFromTemplate(templateGenome);

        // Create blank AgentGenomes for the standard population
        agentGenomeList = new List<AgentGenome>();
        for (int j = 0; j < numGenomes; j++) {
            AgentGenome agentGenome = new AgentGenome(j);  // empty constructor
            agentGenome.CopyGenomeFromTemplate(templateGenome);  // copies attributes and creates random brain -- roll into Constructor method?
            agentGenome.InitializeRandomBrainGenome(0.1f);
            agentGenomeList.Add(agentGenome);
        }

        // Representatives:
        numPerformanceReps = numReps;
        ResetRepresentativesList();

        historicGenomePool = new List<AgentGenome>();
        historicGenomePool.Add(agentGenomeList[0]); // init
        baselineGenomePool = new List<AgentGenome>();
        int numBaselineGenomes = 20;
        for (int j = 0; j < numBaselineGenomes; j++) {
            AgentGenome agentGenome = new AgentGenome(j);  // empty constructor
            agentGenome.CopyGenomeFromTemplate(templateGenome);  // copies attributes and creates random brain -- roll into Constructor method?
            agentGenome.InitializeRandomBrainGenome(0.5f * (float)j/(float)numBaselineGenomes);
            baselineGenomePool.Add(agentGenome);
        }

        fitnessManager = new FitnessManager();
        SetUpDefaultFitnessComponents(challengeType, fitnessManager);
        fitnessManager.InitializeForNewGeneration(numGenomes);
        
        trainingSettingsManager = new TrainingSettingsManager(0.005f, 0.5f);
    }

    private void SetUpDefaultFitnessComponents(Challenge.Type challengeType, FitnessManager fitnessManager) {
         
        switch(challengeType) {
            case Challenge.Type.Test:
                FitnessComponentDefinition distToTargetSquared = new FitnessComponentDefinition(FitnessComponentType.DistanceToTargetSquared, FitnessComponentMeasure.Avg, 0.1f, false);
                fitnessManager.fitnessComponentDefinitions.Add(distToTargetSquared);
                FitnessComponentDefinition contactHazard = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 1f, false);
                fitnessManager.fitnessComponentDefinitions.Add(contactHazard);
                break;
            case Challenge.Type.Racing:
                FitnessComponentDefinition fitCompRacing1 = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 1f, false);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompRacing1);
                FitnessComponentDefinition fitCompRacing2 = new FitnessComponentDefinition(FitnessComponentType.Velocity, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompRacing2);
                break;
            case Challenge.Type.Combat:
                FitnessComponentDefinition fitCompCombat1 = new FitnessComponentDefinition(FitnessComponentType.ContactHazard, FitnessComponentMeasure.Avg, 0.5f, false);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat1);
                //FitnessComponentDefinition fitCompCombat2 = new FitnessComponentDefinition(FitnessComponentType.DistanceToTargetSquared, FitnessComponentMeasure.Average, 0.2f, false);
                //fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat2);
                //FitnessComponentDefinition fitCompCombat3 = new FitnessComponentDefinition(FitnessComponentType.Velocity, FitnessComponentMeasure.Average, 0.2f, true);
                //fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat3);
                FitnessComponentDefinition fitCompCombat2 = new FitnessComponentDefinition(FitnessComponentType.DamageInflicted, FitnessComponentMeasure.Avg, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat2);
                FitnessComponentDefinition fitCompCombat3 = new FitnessComponentDefinition(FitnessComponentType.Health, FitnessComponentMeasure.Avg, 0.1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat3);
                FitnessComponentDefinition fitCompCombat4 = new FitnessComponentDefinition(FitnessComponentType.WinLoss, FitnessComponentMeasure.Last, 1f, true);
                fitnessManager.fitnessComponentDefinitions.Add(fitCompCombat4);
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
    }
}
