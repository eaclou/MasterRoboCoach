using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentMatchup {

    public int id;
    public EvaluationTicket evalTicket;
    public float[] contestantScoresList;

    public TournamentMatchup(int id, EvaluationTicket evalTicket) {
        //contestantScoresList = new List<float>();
        this.id = id;
        this.evalTicket = evalTicket;
        contestantScoresList = new float[evalTicket.agentGenomesList.Count];
    }
}
