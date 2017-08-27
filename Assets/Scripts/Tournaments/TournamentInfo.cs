using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TournamentInfo {
    // What information is needed for a tournament to work?

    // Tournament Format:
    // Independent Trial vs. Competitive Trial
    // round Robin, Bracket, or Best overall score?
    // Best of 1, 3, 5, 7...?

    // Entrance fee / cooldown status

    // Arena Sizes and Competition Environments -- pre-trained Genomes for each round

    // Opponent Agents! pre-trained genomes for each opponent

    // Reward for placing, winning tournament

    // Compatible Agent Types / Required Modules ... (advanced)

    public CompetitionType competitionType;
    public enum CompetitionType {
        Independent,  // opponents compete separately vs. the environment, better score wins
        Direct  // opponents interact directly in competition
    }
    public CompetitionFormat competitionFormat;
    public enum CompetitionFormat {
        HighScore,  // best score wins, single round
        RoundRobin,  // opponents all play each other once, ranked based on table position. win=3, draw=1, loss=0
        Bracket  // Compete 1v1 for each round of the bracket.
    }
    public int bestOfNumRounds;

    public int entranceFee;
    public int reward;
    public float cooldownTime;

    public int numCompetitors;

    public Challenge.Type challengeType;

    public List<TournamentRound> tournamentRoundList;

    public TournamentInfo(TeamsConfig teamsConfig) {
        Debug.Log("TournamentInfo()");

        Init2(teamsConfig);
    }

    public void Init1(TeamsConfig teamsConfig) {
        challengeType = teamsConfig.challengeType;

        // Temp hardcoded!!!!!
        competitionType = CompetitionType.Independent;
        competitionFormat = CompetitionFormat.HighScore;
        numCompetitors = 1;

        tournamentRoundList = new List<TournamentRound>();

        TournamentRound round1 = new TournamentRound(0);

        // MOCKUP:
        string savename = "env1";
        string path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome1 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList1 = new List<AgentGenome>();
        agentGenomesList1.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        EvaluationTicket ticket1 = new EvaluationTicket(loadedGenome1, agentGenomesList1, 1, 1000);
        TournamentMatchup matchup1 = new TournamentMatchup(0, ticket1);
        round1.matchupList.Add(matchup1);

        // MOCKUP:
        savename = "env2";
        path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        // Read the json from the file into a string
        dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome2 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList2 = new List<AgentGenome>();
        agentGenomesList2.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        EvaluationTicket ticket2 = new EvaluationTicket(loadedGenome2, agentGenomesList2, 1, 1000);
        TournamentMatchup matchup2 = new TournamentMatchup(1, ticket2);
        round1.matchupList.Add(matchup2);

        // MOCKUP:
        savename = "env3";
        path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        // Read the json from the file into a string
        dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome3 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList3 = new List<AgentGenome>();
        agentGenomesList3.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        EvaluationTicket ticket3 = new EvaluationTicket(loadedGenome3, agentGenomesList3, 1, 1000);
        TournamentMatchup matchup3 = new TournamentMatchup(2, ticket3);
        round1.matchupList.Add(matchup3);


        tournamentRoundList.Add(round1);
    }

    public void Init2(TeamsConfig teamsConfig) {
        challengeType = teamsConfig.challengeType;

        // Temp hardcoded!!!!!
        competitionType = CompetitionType.Independent;
        competitionFormat = CompetitionFormat.HighScore;
        numCompetitors = 1;

        tournamentRoundList = new List<TournamentRound>();

        TournamentRound round1 = new TournamentRound(0);

        // MOCKUP:
        string savename = "env1";
        string path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        // Read the json from the file into a string
        string dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome1 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList1 = new List<AgentGenome>();
        agentGenomesList1.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        EvaluationTicket ticket1 = new EvaluationTicket(loadedGenome1, agentGenomesList1, 1, 1000);
        TournamentMatchup matchup1 = new TournamentMatchup(0, ticket1);
        round1.matchupList.Add(matchup1);

        tournamentRoundList.Add(round1);

        TournamentRound round2 = new TournamentRound(1);

        // MOCKUP:
        savename = "env2";
        path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        // Read the json from the file into a string
        dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome2 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList2 = new List<AgentGenome>();
        agentGenomesList2.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        EvaluationTicket ticket2 = new EvaluationTicket(loadedGenome2, agentGenomesList2, 1, 1000);
        TournamentMatchup matchup2 = new TournamentMatchup(1, ticket2);
        round2.matchupList.Add(matchup2);

        tournamentRoundList.Add(round2);

        TournamentRound round3 = new TournamentRound(2);

        // MOCKUP:
        savename = "env3";
        path = Application.dataPath + "/IndividualSaves/Environments/" + savename + ".json";
        // Read the json from the file into a string
        dataAsJson = File.ReadAllText(path);
        EnvironmentGenome loadedGenome3 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList3 = new List<AgentGenome>();
        agentGenomesList3.Add(teamsConfig.playersList[0].agentGenomeList[0]);
        EvaluationTicket ticket3 = new EvaluationTicket(loadedGenome3, agentGenomesList3, 1, 1000);
        TournamentMatchup matchup3 = new TournamentMatchup(2, ticket3);
        round3.matchupList.Add(matchup3);


        tournamentRoundList.Add(round3);
    }
}
