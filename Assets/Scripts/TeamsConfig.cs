using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsConfig {

    public List<EnvironmentGenome> environmentGenomesList;
    public List<List<AgentGenome>> playersList;
    public Challenge.Type challengeType;

    private int numEnvironmentGenomes = 32;
    private int numAgentGenomesPerPlayer = 64;

	public TeamsConfig(bool activeEnvironment, int numPlayers, Challenge.Type challengeType) {
        this.challengeType = challengeType;
        // Challenges might have required modules for environment (& agent?) genomes:
        // for example, go-to-target would REQUIRE having a target object in the environment
        
        // Teams:
        // Environment
        environmentGenomesList = new List<EnvironmentGenome>();
        for(int e = 0; e < numEnvironmentGenomes; e++) {
            // Create new environmentGenome
            EnvironmentGenome envGenome = new EnvironmentGenome(challengeType);
            envGenome.TempInitializeGenome();
            environmentGenomesList.Add(envGenome);
            // Add to envGenomesList:
        }

        // Players:
        playersList = new List<List<AgentGenome>>();
        for(int i = 0; i < numPlayers; i++) {
            // List of Agent Genomes
            List<AgentGenome> agentGenomeList = new List<AgentGenome>();
            for(int j = 0; j < numAgentGenomesPerPlayer; j++) {
                AgentGenome agentGenome = new AgentGenome();
                agentGenome.TempInitializeTestGenome();
                agentGenomeList.Add(agentGenome);
            }
            playersList.Add(agentGenomeList);
        }
    }
}
