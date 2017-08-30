using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TournamentMatchup {

    public int id;
    public EvaluationTicket evalTicket;
    public float[] contestantScoresArray;

    public int environmentID;
    public int[] competitorIDs;

    public int targetScore;

    [System.NonSerialized]
    public int winnerID = -1;
    [System.NonSerialized]
    public bool matchupFinished = false;


    public TournamentMatchup(int id, EvaluationTicket evalTicket, int environmentID, int[] competitorIDs) {
        //contestantScoresList = new List<float>();
        this.id = id;
        this.evalTicket = evalTicket;
        //contestantScoresArray = new float[evalTicket.agentGenomesList.Count]; // Incorrect!

        this.environmentID = environmentID;
        this.competitorIDs = competitorIDs;
    }

    public void PrepareMatchup(List<EnvironmentGenome> environmentGenomeList, List<AgentGenome> competitorGenomeList, AgentGenome playerGenome, int numEntrants, int maxTimeSteps) {
        winnerID = -1;
        matchupFinished = false;
        //EnvironmentGenome loadedGenome1 = JsonUtility.FromJson<EnvironmentGenome>(dataAsJson);
        List<AgentGenome> agentGenomesList = new List<AgentGenome>();
        for(int i = 0; i < competitorIDs.Length; i++) {
            // -1 value is code to use player's genome here
            if(competitorIDs[i] == -1) {
                agentGenomesList.Add(playerGenome);
            }
            else {
                agentGenomesList.Add(competitorGenomeList[competitorIDs[i]]);
            }
        }
        
        evalTicket = new EvaluationTicket(environmentGenomeList[environmentID], agentGenomesList, 1, maxTimeSteps);

        
        contestantScoresArray = new float[1];  // ASSUMES ! PLAYER AT A TIME!!!!

        Debug.Log("contestantScoresArray.Length: " + contestantScoresArray.Length.ToString());
    }
}
