using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TournamentManager {
    // Responsibilities of TournamentManager:
    // Holds reference to Tournament Details, and Tournament Status
    // Creates Evaluations
    // Stores Results of Matches
    // Holds Genome/Template data for All Players and Environments
    // Need a way to save just a single population and/or Individual
    // Start with JSON?

    EvaluationTicket singleTicket;

	public TournamentManager() {

    }

    public void Initialize(TeamsConfig teamsConfig, TournamentInfo tournamentInfo) {
        //Debug.Log("TournamentManager Initialize");

        // Load Competitors,
        // Create Match Schedule
        //

        // MOCKUP:
        string savename = "env3";
        string path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        //Debug.Log(Application.persistentDataPath);
        Debug.Log("TournamentManager Initialize: " + path);

        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        // Pass the json to JsonUtility, and tell it to create a GameData object from it
        //TeamsConfig loadedData = JsonUtility.FromJson<TeamsConfig>(dataAsJson);
        //teamsConfig = loadedData;
        //LoadTrainingMode();
        List<AgentGenome> agentGenomesList = new List<AgentGenome>();
        agentGenomesList.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        singleTicket = new EvaluationTicket(loadedGenome, agentGenomesList, 1, 1000);

        // Set up Exhibition Instance:
        GameObject tournamentInstanceGO = new GameObject("TournamentInstance");
        EvaluationInstance tournamentInstance = tournamentInstanceGO.AddComponent<EvaluationInstance>();
        tournamentInstance.transform.position = new Vector3(0f, 0f, 0f);
        tournamentInstance.visible = true;
        tournamentInstance.isExhibition = true;

        GameObject exhibitionParticleCurvesGO = new GameObject("ExhibitionParticleCurves");
        ExhibitionParticleCurves exhibitionParticleCurves = exhibitionParticleCurvesGO.AddComponent<ExhibitionParticleCurves>(); // HACK
        tournamentInstance.SetUpInstance(singleTicket, teamsConfig, exhibitionParticleCurves);
    }

    public void PlayNextMatch() {
        Debug.Log("TournamentManager PlayNextMatch()");

        
    }
}
