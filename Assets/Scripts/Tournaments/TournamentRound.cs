using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TournamentRound {

    public int id;
    public List<TournamentMatchup> matchupList;

    public int maxTimeSteps = 1000;

    [System.NonSerialized]
    public int winnerID = -1;
    [System.NonSerialized]
    public bool roundFinished = false;

    public TournamentRound(int id) {
        this.id = id;
        matchupList = new List<TournamentMatchup>();
    }
}
