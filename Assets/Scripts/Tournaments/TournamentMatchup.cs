using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TournamentMatchup {

    public int id;
    public EvaluationTicket evalTicket;
    public float[] contestantScoresList;

    public int environmentID;
    public int[] competitorIDs; 

    public TournamentMatchup(int id, EvaluationTicket evalTicket, int environmentID, int[] competitorIDs) {
        //contestantScoresList = new List<float>();
        this.id = id;
        this.evalTicket = evalTicket;
        contestantScoresList = new float[evalTicket.agentGenomesList.Count];

        this.environmentID = environmentID;
        this.competitorIDs = competitorIDs;
    }

    public void PrepareMatchup(List<EnvironmentGenome> environmentGenomeList, List<AgentGenome> competitorGenomeList, AgentGenome playerGenome) {
        
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
        
        evalTicket = new EvaluationTicket(environmentGenomeList[environmentID], agentGenomesList, 1, 1000);

        contestantScoresList = new float[evalTicket.agentGenomesList.Count];
    }
}
