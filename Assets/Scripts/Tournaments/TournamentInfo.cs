using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public int bestOfNumRounds = 1;

    public int entranceFee;
    public int reward;
    public float cooldownTime;

    public Challenge.Type challengeType;

    public TournamentInfo() {

    }
}
