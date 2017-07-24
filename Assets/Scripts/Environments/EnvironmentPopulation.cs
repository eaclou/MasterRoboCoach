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

    public EnvironmentPopulation(Challenge.Type challengeType, int numGenomes, int numReps) {
        environmentGenomeList = new List<EnvironmentGenome>();

        for (int e = 0; e < numGenomes; e++) {
            // Create new environmentGenome
            EnvironmentGenome envGenome = new EnvironmentGenome(e, challengeType);
            envGenome.TempInitializeGenome();  // Roll this into Constructor??
            environmentGenomeList.Add(envGenome);
            // Add to envGenomesList:
        }

        // Representatives:
        numPerformanceReps = numReps;
        representativeGenomeList = new List<EnvironmentGenome>();
        for (int i = 0; i < numPerformanceReps; i++) {
            representativeGenomeList.Add(environmentGenomeList[i]);
        }
        
        historicGenomePool = new List<EnvironmentGenome>();
        baselineGenomePool = new List<EnvironmentGenome>();

        fitnessManager = new FitnessManager();
        fitnessManager.ResetFitnessScores(numGenomes);
        trainingSettingsManager = new TrainingSettingsManager();
    }
}
