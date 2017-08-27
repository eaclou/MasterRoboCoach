using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "TournamentInfoWrapper", menuName = "TournamentInfoWrapper/New", order = 0)]
public class TournamentInfoWrapper : ScriptableObject {

    public TournamentInfo tournamentInfo;

    void Awake() {
        Debug.Log("TournamentInfoWrapper Awake!");
    }

    void OnDestroy() {
        Debug.Log("TournamentInfoWrapper OnDestroy!");
    }

    void OnDisable() {
        Debug.Log("TournamentInfoWrapper OnDisable!");
    }

    void OnEnable() {
        Debug.Log("TournamentInfoWrapper OnEnable!");
    }
}
