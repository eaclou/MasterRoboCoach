using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TournamentManager {
    // Responsibilities of TournamentManager:
    // Holds reference to Tournament Details, and Tournament Status
    // Creates Evaluations
    // Stores Results of Matches
    // Holds Genome/Template data for All Players and Environments
        // Need a way to save just a single population and/or Individual
        // Start with JSON?

	public TournamentManager() {

    }

    public void Initialize(TournamentInfo tournamentInfo) {
        Debug.Log("TournamentManager Initialize");
    }

    public void PlayNextMatch() {
        Debug.Log("TournamentManager PlayNextMatch()");
    }
}
