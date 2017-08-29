using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class TournamentManager : MonoBehaviour {
    // Responsibilities of TournamentManager:
    // Holds reference to Tournament Details, and Tournament Status
    // Creates Evaluations
    // Stores Results of Matches
    // Holds Genome/Template data for All Players and Environments
    // Need a way to save just a single population and/or Individual
    // Start with JSON?

    public GameManager gameManager;

    public TournamentUI tournamentUI;

    public bool isPlaying = false;  // is the match paused or playing?
    public bool isSimulating = false; // are we in the middle of a match?

    public bool inbetweenMatches = true;
    public bool tournamentFinished = false;

    public bool resultsProcessed = false;
    public bool playerWon = false;

    //EvaluationTicket singleTicket;

    public EvaluationInstance tournamentInstance;

    public TournamentInfo currentTournamentInfo;
    public TournamentMatchup currentMatchup;
    public int currentRoundNum = 0;
    public int currentMatchNum = 0;

    void FixedUpdate() {
        
    }
    
    public void AllMatchesComplete() {

        int[] competitorRoundWins = new int[currentTournamentInfo.numOpponents + 1];
        // Find winner!
        for(int i = 0; i < currentTournamentInfo.tournamentRoundList.Count; i++) {
            float[] competitorScoreTotals = new float[currentTournamentInfo.numOpponents + 1];
            int winningIndex = 0;
            float runningBestScore = float.PositiveInfinity;  // lower is better in this case!!!! WON'T GENERALIZE!!!!!
            for (int j = 0; j < currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {

                int index = currentTournamentInfo.tournamentRoundList[i].matchupList[j].competitorIDs[0] + 1;  // [-1 for player --> 0], [0 --> 1], etc
                
                float score = currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0]; // ASSUMES ! PLAYER AT A TIME!!!
                competitorScoreTotals[index] += score;
                //Debug.Log("index: " + index.ToString() + ", i: " + i.ToString() + ", j: " + j.ToString() + ", listLength: " + currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray.Length.ToString() + ", competitorScoreTotals[index]: " + competitorScoreTotals[index].ToString());
                if (score < runningBestScore) {
                    winningIndex = index;
                    runningBestScore = score;
                }
            }
            competitorRoundWins[winningIndex]++;
            //Debug.Log("Round Winner: " + winningIndex.ToString() + ", Score: " + runningBestScore.ToString());
        }

        int winnerIndex = 0;
        int runningMostRoundWins = 0;
        for(int i = 0; i < competitorRoundWins.Length; i++) {
            //Debug.Log("Contestant #" + i.ToString() + " Round Wins: " + competitorRoundWins[i].ToString());
            if(competitorRoundWins[i] > runningMostRoundWins) {
                winnerIndex = i;
                runningMostRoundWins = competitorRoundWins[i];
            }
        }
        //Debug.Log("Tournament Winner: " + winnerIndex.ToString() + ", Rounds Won: " + runningMostRoundWins.ToString() + " / " + currentTournamentInfo.tournamentRoundList.Count.ToString());

        if(winnerIndex == 0) {
            playerWon = true;
        }
    }

    public void Exit() {
        if(playerWon) {
            gameManager.prestige += currentTournamentInfo.reward;
        }

        tournamentInstance.gameObject.SetActive(false);
        isPlaying = false;
        isSimulating = false;
        inbetweenMatches = true;
        tournamentFinished = false;
        currentRoundNum = 0;
        currentMatchNum = 0;
        //tournamentUI.TournamentEndScreen();
    }

    public TournamentMatchup FindCurrentMatchup() {
        //TournamentMatchup currentMatchup = null;
        // Find Next Matchup:
        //bool allComplete = true;
        for (int i = 0; i < currentTournamentInfo.tournamentRoundList.Count; i++) {
            for (int j = 0; j < currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {
                if (currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status != EvaluationTicket.EvaluationStatus.Complete) {
                    currentRoundNum = i;
                    currentMatchNum = j;
                    return currentTournamentInfo.tournamentRoundList[i].matchupList[j];                    
                }
            }
        }
        return null;
    }

    public bool CheckAllMatchesFinished() {
        bool allEvalsComplete = true;
        for (int i = 0; i < currentTournamentInfo.tournamentRoundList.Count; i++) {
            for (int j = 0; j < currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {
                if(currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status != EvaluationTicket.EvaluationStatus.Complete) {
                    allEvalsComplete = false;
                    break;
                }
            }
        }
        return allEvalsComplete;
    }

    public void Tick() {
        if (isSimulating) { // && debugFrameCounter < 2) {
            // Check if it's all over:
            if(CheckAllMatchesFinished()) {
                tournamentFinished = true;
                inbetweenMatches = true;
                if(!resultsProcessed) {                    
                    AllMatchesComplete();
                    resultsProcessed = true;
                }
                
                //Debug.Log("Tournament Finished!");
                // END
            }
            
            // find current match:
            currentMatchup = FindCurrentMatchup();

            // Check if we're between matches:
            if (inbetweenMatches) {
                // ShowUI
                gameManager.cameraEnabled = false;
            }
            else {
                switch (currentMatchup.evalTicket.status) {
                    case EvaluationTicket.EvaluationStatus.Pending:
                        //Found it!
                        // inbetweenMatches is false, so ready to start!
                        Debug.Log("SetUp matchup " + currentMatchup.id.ToString());
                        //Debug.Log(tournamentInstance.ToString());
                        //Debug.Log(currentMatchup.ToString());
                        //Debug.Log(currentMatchup.evalTicket.ToString());
                        tournamentInstance.SetUpInstance(currentMatchup.evalTicket, gameManager.trainerRef.teamsConfig);

                        Play();
                        isSimulating = true;
                        gameManager.cameraEnabled = true;
                        break;
                    case EvaluationTicket.EvaluationStatus.InProgress:
                        // do stuff
                        // Tick
                        tournamentInstance.Tick();
                        break;
                    case EvaluationTicket.EvaluationStatus.PendingComplete:
                        // do stuff
                        // Save Scores
                        for(int i = 0; i < tournamentInstance.agentGameScoresArray.Length; i++) {
                            currentMatchup.contestantScoresArray[0] = tournamentInstance.agentGameScoresArray[0][0];  // @$#!@$# ASSUMES 1 Player at a time!! will break for Combat MiniGame!!!
                            Debug.Log("matchup " + currentMatchup.id + ", PendingComplete, score: " + currentMatchup.contestantScoresArray[0].ToString()); // @$#!@$# ASSUMES 1 Player at a time!! will break for Combat MiniGame!!!
                        }
                        //tournamentInstance.agentGameScoresArray[]
                        inbetweenMatches = true;
                        currentMatchup.evalTicket.status = EvaluationTicket.EvaluationStatus.Complete;


                        // Cleanup?
                        break;
                    case EvaluationTicket.EvaluationStatus.Complete:
                        // do stuff
                        // shouldn't happen
                        break;
                }
            }
            
            
            
            //Debug.Log("FixedUpdate isTraining");
            /*if (singleTicket.status == EvaluationTicket.EvaluationStatus.PendingComplete) {
                // Pending complete!!!
                Debug.Log("PendingComplete!");
                isSimulating = false;
                Exit();
            }
            else {
                if (tournamentInstance.currentEvalTicket.status == EvaluationTicket.EvaluationStatus.Pending) {

                }
                else if (tournamentInstance.currentEvalTicket.status == EvaluationTicket.EvaluationStatus.InProgress) {
                    tournamentInstance.Tick();
                }
                
            }*/
            //debugFrameCounter++;
        }               
    }

    public void Pause() {
        //print("Pause");
        isPlaying = false;
        Time.timeScale = 0f;
    }
    public void Play() {
        //print("Pause");
        isPlaying = true;
        Time.timeScale = 1f;
    }

    public void Initialize(TeamsConfig teamsConfig, TournamentInfo tournamentInfo) {
        Debug.Log("TournamentManager Initialize");
        currentTournamentInfo = tournamentInfo;

        // Load Competitors,
        // Create Match Schedule
        // Process Tournament Info:
        tournamentInfo.PrepareTournament(teamsConfig.playersList[0].agentGenomeList[0]);

        gameManager.prestige -= tournamentInfo.entranceFee;

        // Set up Exhibition Instance:
        if(tournamentInstance == null) {
            GameObject tournamentInstanceGO = new GameObject("TournamentInstance");
            tournamentInstance = tournamentInstanceGO.AddComponent<EvaluationInstance>();
            tournamentInstance.transform.position = new Vector3(0f, 0f, 0f);
            tournamentInstance.visible = true;
            tournamentInstance.isExhibition = true;
        }
        else {
            tournamentInstance.gameObject.SetActive(true);
        }        
    }

    public void PlayNextMatch() {
        Debug.Log("TournamentManager PlayNextMatch()");

        //Debug.Log("singleTicket status: " + singleTicket.status.ToString());

        // Set up match:
        //GameObject exhibitionParticleCurvesGO = new GameObject("ExhibitionParticleCurves");
        //ExhibitionParticleCurves exhibitionParticleCurves = exhibitionParticleCurvesGO.AddComponent<ExhibitionParticleCurves>(); // HACK

        //tournamentInstance.SetUpInstance(singleTicket, gameManager.trainerRef.teamsConfig, exhibitionParticleCurves);

        Play();
        inbetweenMatches = false;
        isSimulating = true;
        
        //singleTicket.status = EvaluationTicket.EvaluationStatus.InProgress;
    }
}
