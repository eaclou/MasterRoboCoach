using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[System.Serializable]
public class TeamsConfig {
        
    public EnvironmentPopulation environmentPopulation;    
    public List<PlayerPopulation> playersList;
    public Challenge.Type challengeType;
    

    // default population sizes:
    private int numEnvironmentGenomes = 8;
    private int numAgentGenomesPerPlayer = 40;
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
            AgentBodyGenomeTemplate templateAgentGenome = GetDefaultTemplateAgentGenome(challengeType);
            Debug.Log("TEMPLATE: " + templateAgentGenome.bodyGenome.ToString());
            // List of Agent Genomes
            PlayerPopulation player = new PlayerPopulation(challengeType, templateAgentGenome.bodyGenome, numAgentGenomesPerPlayer, numBaselineGenomes, numPlayerReps);

            playersList.Add(player);
        }
    }

    public void InitializeFromLoadedData() {
        //ReloadAgentTemplates();  // get templates so Agents can be Instantiated!
        // load template from prefab. This will have to be changed in order to support modular upgrades...


        // Initialize the Populations:
        environmentPopulation.InitializeLoadedPopulation();

        for(int i = 0; i < playersList.Count; i++) {
            playersList[i].InitializeLoadedPopulation();
        }
    }

    /*public void ReloadAgentTemplates() {        
        // Players:        
        for (int i = 0; i < playersList.Count; i++) {
            // Might have to revisit how to pass agent templates per population...
            AgentGenomeTemplate templateAgentGenome = GetDefaultTemplateAgentGenome(challengeType);
            playersList[i].templateGenome = templateAgentGenome.templateGenome;
        }
    }*/
    
    private EnvironmentGenome GetDefaultTemplateEnvironmentGenome(Challenge.Type challengeType) {
        EnvironmentGenome templateGenome;
        switch (challengeType) {
            case Challenge.Type.Test:
                templateGenome = (Resources.Load("Templates/Environments/TemplateTestDefault") as EnvironmentGenomeTemplate).templateGenome;
                //templateGenome = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateTestDefault.asset", typeof(EnvironmentGenomeTemplate))).templateGenome;
                break;
            case Challenge.Type.Racing:
                templateGenome = (Resources.Load("Templates/Environments/TemplateRacingDefault") as EnvironmentGenomeTemplate).templateGenome;
                //templateGenome = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateRacingDefault.asset", typeof(EnvironmentGenomeTemplate))).templateGenome;
                break;
            case Challenge.Type.Combat:
                templateGenome = (Resources.Load("Templates/Environments/TemplateCombatDefault") as EnvironmentGenomeTemplate).templateGenome;
                //templateGenome = ((EnvironmentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Environments/TemplateCombatDefault.asset", typeof(EnvironmentGenomeTemplate))).templateGenome;
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                templateGenome = null;
                break;
        }
        return templateGenome;
    }

    private AgentBodyGenomeTemplate GetDefaultTemplateAgentGenome(Challenge.Type challengeType) {
        AgentBodyGenomeTemplate templateGenome;
        switch (challengeType) {
            case Challenge.Type.Test:
                templateGenome = Resources.Load("Templates/Agents/TemplateTelevisionWalker") as AgentBodyGenomeTemplate;
                //templateGenome = ((AgentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Agents/TemplateTelevisionWalker.asset", typeof(AgentGenomeTemplate)));
                break;
            case Challenge.Type.Racing:
                //Debug.Log("Loaded DogCar!");
                templateGenome = Resources.Load("Templates/Agents/TemplateDogCar") as AgentBodyGenomeTemplate;
                //templateGenome = ((AgentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Agents/TemplateDogCar.asset", typeof(AgentGenomeTemplate)));
                break;
            case Challenge.Type.Combat:
                templateGenome = Resources.Load("Templates/Agents/TemplateCombat2") as AgentBodyGenomeTemplate;
                //templateGenome = ((AgentGenomeTemplate)AssetDatabase.LoadAssetAtPath("Assets/Templates/Agents/TemplateCombatBot.asset", typeof(AgentGenomeTemplate)));
                break;
            default:
                Debug.LogError("ChallengeType Not Found! " + challengeType.ToString());
                templateGenome = null;
                break;
        }
        return templateGenome;
    }
}
