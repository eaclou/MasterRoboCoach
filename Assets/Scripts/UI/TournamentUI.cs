using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentUI : MonoBehaviour {

    public GameManager gameManager;
    //public MainMenu mainMenuRef;
    public GameObject panelTournamentOverview;
    public GameObject panelTournamentActive;

    public GameObject panelMidTournamentStatus;
    public GameObject panelTournamentResults;

    public Button buttonPlayMatch;
    public Text textTournamentName;
    public Text textMatchList;

    public Button buttonPlayPause;
    public Button buttonCycleCamera;
    public Text textMatchInfo;
    public Text textTimer;

    public Text textResultsScores;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void EnterState() {
        InitializeUIFromGameState();
    }

    public void ExitState() {

    }

    public void InitializeUIFromGameState() {
        if(gameManager.tournamentManager.inbetweenMatches) {
            panelTournamentOverview.SetActive(true);
            panelTournamentActive.SetActive(false);
            if (gameManager.tournamentManager.tournamentFinished) {
                panelTournamentResults.SetActive(true);
                panelMidTournamentStatus.SetActive(false);
                UpdateResultsUI();
            }
            else {
                panelTournamentResults.SetActive(false);
                panelMidTournamentStatus.SetActive(true);
            }
        }
        else {
            panelTournamentActive.SetActive(true);

            panelTournamentOverview.SetActive(false);
            panelMidTournamentStatus.SetActive(false);
            panelTournamentResults.SetActive(false);
        }
        //panelTournamentOverview.SetActive(true);
        //panelMidTournamentStatus.SetActive(true);
        //panelTournamentResults.SetActive(false);
        //panelTournamentActive.SetActive(false);
    }

    public void UpdateState() {
        if (gameManager.tournamentManager.inbetweenMatches) {
            panelTournamentOverview.SetActive(true);
            panelTournamentActive.SetActive(false);
            if (gameManager.tournamentManager.tournamentFinished) {
                panelTournamentResults.SetActive(true);
                panelMidTournamentStatus.SetActive(false);
                UpdateResultsUI();
            }
            else {
                panelTournamentResults.SetActive(false);
                panelMidTournamentStatus.SetActive(true);
                UpdateMatchListUI();
            }
        }
        else {
            panelTournamentActive.SetActive(true);
            UpdateActiveMatchUI();

            panelTournamentOverview.SetActive(false);
            panelMidTournamentStatus.SetActive(false);
            panelTournamentResults.SetActive(false);
        }
    }

    private void UpdateActiveMatchUI() {
        if(gameManager.tournamentManager.currentMatchup != null) {
            string txtMatchInfo = "";
            txtMatchInfo += "Round: " + gameManager.tournamentManager.currentRoundNum.ToString() + ", Match: " + gameManager.tournamentManager.currentMatchup.id.ToString() + "\n";
            textMatchInfo.text = txtMatchInfo;

            if(gameManager.tournamentManager.currentMatchup.evalTicket != null) {
                string txtTimer = "";
                txtTimer += gameManager.tournamentManager.tournamentInstance.currentTimeStep.ToString() + " / " + gameManager.tournamentManager.currentMatchup.evalTicket.maxTimeSteps.ToString();
                textTimer.text = txtTimer;
            }            
        }

        UpdatePlayPauseButtonUI();
    }

    private void UpdateMatchListUI() {
        string txt = "";
        for (int i = 0; i < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count; i++) {
            txt += "Round " + (i + 1).ToString() + "\n";
            for (int j = 0; j < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {

                txt += "Match " + (j + 1).ToString() + ", " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status.ToString();
                if(gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status == EvaluationTicket.EvaluationStatus.Complete) {
                    txt += "   Score: " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0].ToString();
                }
                txt += "\n";
                //if (gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status != EvaluationTicket.EvaluationStatus.Complete) {
                 //   return gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j];
                //}
            }
        }
        textMatchList.text = txt;
    }

    private void UpdateResultsUI() {
        string[] contestantColors = new string[gameManager.tournamentManager.currentTournamentInfo.numOpponents + 1];
        contestantColors[0] = "<color=lightblue>";
        if(contestantColors.Length > 1) {
            contestantColors[1] = "<color=orange>";
        }

        string txtWinner = "";
        string txtResults = "";
        int[] competitorRoundWins = new int[gameManager.tournamentManager.currentTournamentInfo.numOpponents + 1];
        // Find winner!
        for (int i = 0; i < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count; i++) {
            float[] competitorScoreTotals = new float[gameManager.tournamentManager.currentTournamentInfo.numOpponents + 1];
            int winningIndex = 0;
            float runningBestScore = float.PositiveInfinity;  // lower is better in this case!!!! WON'T GENERALIZE!!!!!
            string txtMatchups = "";
            for (int j = 0; j < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {

                int index = gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].competitorIDs[0] + 1;  // [-1 for player --> 0], [0 --> 1], etc

                float score = gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0]; // ASSUMES ! PLAYER AT A TIME!!!
                competitorScoreTotals[index] += score;
                //Debug.Log("index: " + index.ToString() + ", i: " + i.ToString() + ", j: " + j.ToString() + ", listLength: " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray.Length.ToString() + ", competitorScoreTotals[index]: " + competitorScoreTotals[index].ToString());
                if (score < runningBestScore) {
                    winningIndex = index;
                    runningBestScore = score;
                }
                txtMatchups += contestantColors[index];
                txtMatchups += "Match " + (j + 1).ToString() + ", Contestant " + index.ToString() + ", Time: " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0].ToString() + "</color>\n";
            }
            competitorRoundWins[winningIndex]++;
            //Debug.Log("Round Winner: " + winningIndex.ToString() + ", Score: " + runningBestScore.ToString());
            txtResults += contestantColors[winningIndex];
            txtResults += "\n<size=18><b>ROUND " + (i + 1).ToString() + " Winner: Contestant " + winningIndex.ToString() + "</b></size></color>\n";
            txtResults += txtMatchups;
        }

        int winnerIndex = 0;
        int runningMostRoundWins = 0;
        for (int i = 0; i < competitorRoundWins.Length; i++) {
            //Debug.Log("Contestant #" + i.ToString() + " Round Wins: " + competitorRoundWins[i].ToString());
            if (competitorRoundWins[i] > runningMostRoundWins) {
                winnerIndex = i;
                runningMostRoundWins = competitorRoundWins[i];
            }
        }
        txtWinner += contestantColors[winnerIndex];
        txtWinner += "Winner: Contestant " + winnerIndex.ToString() + "!, Rounds Won: " + runningMostRoundWins.ToString() + " / " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count.ToString() + "\n\n";
        if(winnerIndex == 0) {
            txtWinner += "<size=24><b>YOU WON!!!</b></size>\n" + "You Received $" + gameManager.tournamentManager.currentTournamentInfo.reward.ToString() + " Reward";
            
        }
        else {
            txtWinner += "you lost...";
        }
        textResultsScores.text = txtResults + "\n\n" + txtWinner + "</color>";
        //Debug.Log("Tournament Winner: " + winnerIndex.ToString() + ", Rounds Won: " + runningMostRoundWins.ToString() + " / " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count.ToString());


        /*string txt = "";
        for (int i = 0; i < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count; i++) {
            txt += "Round " + (i + 1).ToString() + "\n";
            for (int j = 0; j < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {

                txt += "Match " + (j + 1).ToString() + ", " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status.ToString();
                if (gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status == EvaluationTicket.EvaluationStatus.Complete) {
                    txt += "   Score: " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0].ToString();
                }
                txt += "\n";
                //if (gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status != EvaluationTicket.EvaluationStatus.Complete) {
                //   return gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j];
                //}
            }
        }*/


    }

    private void UpdatePlayPauseButtonUI() {
        if (gameManager.tournamentManager.isPlaying) {
            buttonPlayPause.GetComponentInChildren<Text>().text = "||";
        }
        else {
            buttonPlayPause.GetComponentInChildren<Text>().text = ">";
        }
    }

    private void UpdateCameraModeUI() {
        string cameraModeText = "";
        if (gameManager.cameraManager.currentCameraMode == 0) {
            cameraModeText = "Wide";
        }
        else if (gameManager.cameraManager.currentCameraMode == 1) {
            cameraModeText = "Top Down";
        }
        else {
            cameraModeText = "Shoulder";
        }
        buttonCycleCamera.GetComponentInChildren<Text>().text = "Camera:\n" + cameraModeText;
    }

    public void ClickPlayMatch() {
        panelTournamentOverview.SetActive(false);
        panelTournamentActive.SetActive(true);

        // INITIALIZE TOURNAMENT MANAGER
        gameManager.tournamentManager.PlayNextMatch();
    }

    public void SetOverviewPanelFromData(TournamentManager tournamentManager) {

    }

    public void TournamentEndScreen() {
        panelTournamentOverview.SetActive(true);
        panelMidTournamentStatus.SetActive(false);
        panelTournamentResults.SetActive(true);

        panelTournamentActive.SetActive(false);


    }

    private void HideUI() {
        panelTournamentOverview.SetActive(false);
        panelMidTournamentStatus.SetActive(false);
        panelTournamentResults.SetActive(false);

        panelTournamentActive.SetActive(false);
    }

    public void ClickFinished() {
        HideUI();

        gameManager.ExitTournamentMode();
    }

    public void ClickPlayPause() {
        if(gameManager.tournamentManager.isPlaying) {
            gameManager.tournamentManager.Pause();
        }
        else {
            gameManager.tournamentManager.Play();
        }
        
        UpdatePlayPauseButtonUI();


    }

    public void ClickCycleCamera() {
        gameManager.cameraManager.CycleCameraMode();

    }
}
