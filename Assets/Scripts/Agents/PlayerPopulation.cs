using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerPopulation {

    public List<AgentGenome> agentGenomeList;  // Primary genome population
    //public TrainingSettings trainingSettings;  // mutation, max eval time, etc.
    //public FitnessSettings fitnessSettings;  // fitness function components and settings
    //public bool isTraining;  // is the population actively learning - i.e will go through selection and crossover, or is static and unchanging
    public List<AgentGenome> representativeGenomeList;  // the list of agentGenomes that will be opponents for all other populations this round
    public List<AgentGenome> historicGenomePool;  // a collection of predecessor genomes that can be chosen from
    public List<AgentGenome> baselineGenomePool;  // a collection of blank and random genomes for fitness comparison purposes.

    public FitnessManager fitnessManager; // keeps track of performance data from this population's agents
    public TrainingSettingsManager trainingSettingsManager;  // keeps track of core algorithm settings, like mutation rate, thoroughness, etc.
    public bool isTraining = true;
    public int numPerformanceReps = 1;

    // Representative system will be expanded later - for now, just defaults to Top # of performers
    public PlayerPopulation(AgentGenome templateGenome, int numGenomes, int numReps) {
        // Create blank AgentGenomes for the standard population
        agentGenomeList = new List<AgentGenome>();
        for (int j = 0; j < numGenomes; j++) {
            AgentGenome agentGenome = new AgentGenome(j);  // empty constructor
            agentGenome.CopyGenomeFromTemplate(templateGenome);  // copies attributes and creates random brain -- roll into Constructor method?
            agentGenomeList.Add(agentGenome);
        }

        // Representatives:
        numPerformanceReps = numReps;
        representativeGenomeList = new List<AgentGenome>();
        for(int i = 0; i < numPerformanceReps; i++) {
            representativeGenomeList.Add(agentGenomeList[i]);
        }

        historicGenomePool = new List<AgentGenome>();
        baselineGenomePool = new List<AgentGenome>();

        fitnessManager = new FitnessManager();
        fitnessManager.ResetFitnessScores(numGenomes);
        trainingSettingsManager = new TrainingSettingsManager();
    }
}
