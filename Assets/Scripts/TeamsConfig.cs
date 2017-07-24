using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class TeamsConfig {
        
    public EnvironmentPopulation environmentPopulation;    
    public List<PlayerPopulation> playersList;
    public Challenge.Type challengeType;
    

    // default population sizes:
    private int numEnvironmentGenomes = 16;
    private int numAgentGenomesPerPlayer = 48;

	public TeamsConfig(int numPlayers, Challenge.Type challengeType, int numEnvironmentReps, int numPlayerReps) {
        this.challengeType = challengeType;
        // Challenges might have required modules for environment (& agent?) genomes:
        // for example, go-to-target would REQUIRE having a target object in the environment
        Debug.Log("TeamsConfig() " + this.challengeType.ToString());
        // Teams:
        // Environment
        environmentPopulation = new EnvironmentPopulation(challengeType, numEnvironmentGenomes, numEnvironmentReps);
        
        // Players:
        playersList = new List<PlayerPopulation>();
        for(int i = 0; i < numPlayers; i++) {
            // Might have to revisit how to pass agent templates per population...
            AgentGenome templateGenome = ((AgentTemplateVacuumBot)AssetDatabase.LoadAssetAtPath("Assets/Templates/TemplateVacuumBot.asset", typeof(AgentTemplateVacuumBot))).templateGenome;
            // List of Agent Genomes
            PlayerPopulation player = new PlayerPopulation(templateGenome, numAgentGenomesPerPlayer, numPlayerReps);

            playersList.Add(player);
        }
    }    
}
