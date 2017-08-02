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
    private int numEnvironmentGenomes = 2;
    private int numAgentGenomesPerPlayer = 48;
    private int numBaselineGenomes = 4;

	public TeamsConfig(int numPlayers, Challenge.Type challengeType, int numEnvironmentReps, int numPlayerReps) {
        this.challengeType = challengeType;
        // Challenges might have required modules for environment (& agent?) genomes:
        // for example, go-to-target would REQUIRE having a target object in the environment
        Debug.Log("TeamsConfig() " + this.challengeType.ToString());
        // Teams:
        // Environment
        EnvironmentGenome templateEnvironmentGenome = GetDefaultTemplateEnvironmentGenome(challengeType);
        environmentPopulation = new EnvironmentPopulation(challengeType, templateEnvironmentGenome, numEnvironmentGenomes, numBaselineGenomes, numEnvironmentReps);
        
        // Players:
        playersList = new List<PlayerPopulation>();
        for(int i = 0; i < numPlayers; i++) {
            // Might have to revisit how to pass agent templates per population...
            AgentGenome templateAgentGenome = GetDefaultTemplateAgentGenome(challengeType);
            // List of Agent Genomes
            PlayerPopulation player = new PlayerPopulation(challengeType, templateAgentGenome, numAgentGenomesPerPlayer, numBaselineGenomes, numPlayerReps);

            playersList.Add(player);
        }
    }

    private EnvironmentGenome GetDefaultTemplateEnvironmentGenome(Challenge.Type challengeType) {
        EnvironmentGenome templateGenome;
        switch (challengeType) {
            case Challenge.Type.Test:
                templateGenome = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateTestDefault.asset", typeof(EnvironmentGenomeTemplate))).templateGenome;
                break;
            case Challenge.Type.Racing:
                templateGenome = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateRacingDefault.asset", typeof(EnvironmentGenomeTemplate))).templateGenome;
                break;
            case Challenge.Type.Combat:
                templateGenome = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateCombatDefault.asset", typeof(EnvironmentGenomeTemplate))).templateGenome;
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                templateGenome = null;
                break;
        }
        return templateGenome;
    }

    private AgentGenome GetDefaultTemplateAgentGenome(Challenge.Type challengeType) {
        AgentGenome templateGenome;
        switch (challengeType) {
            case Challenge.Type.Test:
                templateGenome = ((AgentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Agents/TemplateVacuumBot.asset", typeof(AgentGenomeTemplate))).templateGenome;
                break;
            case Challenge.Type.Racing:
                templateGenome = ((AgentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Agents/TemplateRacingBot.asset", typeof(AgentGenomeTemplate))).templateGenome;
                break;
            case Challenge.Type.Combat:
                templateGenome = ((AgentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Agents/TemplateCombatBot.asset", typeof(AgentGenomeTemplate))).templateGenome;
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                templateGenome = null;
                break;
        }
        return templateGenome;
    }


}
