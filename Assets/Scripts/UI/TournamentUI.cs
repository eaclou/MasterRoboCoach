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
                UpdateMatchListUI();
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
        textMatchList.text = GetLiveScheduleResultsText();
    }
    private void UpdateResultsUI() {
        textResultsScores.text = GetLiveScheduleResultsText();
    }

    private string GetLiveScheduleResultsText() {
        string[] contestantColors = new string[gameManager.tournamentManager.currentTournamentInfo.numOpponents + 1];
        contestantColors[0] = "<color=lightblue>";
        if(contestantColors.Length > 1) {
            contestantColors[1] = "<color=orange>";
        }
        string defaultColor = "<color=white>";

        
        string txtResults = "";
        for (int i = 0; i < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count; i++) {
            string txtRound = "";
            if(gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].roundFinished) { // -1 encodes 'pending'
                int roundWinnerIndex = gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].winnerID;

                if(roundWinnerIndex == -1) {
                    txtRound += defaultColor;
                }
                else {
                    txtRound += contestantColors[roundWinnerIndex];
                }                
                txtRound += "\n<size=18><b>ROUND " + (i + 1).ToString() + " Winner: Contestant " + roundWinnerIndex.ToString() + "</b></size></color>\n";
            }
            else {
                txtRound += defaultColor + "\n<size=18><b>ROUND " + (i + 1).ToString();
                if (i <= gameManager.tournamentManager.currentRoundNum) {
                    txtRound += ":  IN PROGRESS";
                }
                else {
                    txtRound += ":  PENDING";
                }
                txtRound += "</b></size></color>\n";
            }
            txtResults += txtRound;

            for (int j = 0; j < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList.Count; j++) {
                string txtMatchup = "";

                int index = gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].competitorIDs[0] + 1;  // [-1 for player --> 0], [0 --> 1], etc
                if (gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].matchupFinished) {
                    float score = gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0]; // ASSUMES ! PLAYER AT A TIME!!!

                    txtMatchup += contestantColors[index];
                    txtMatchup += "Match " + (j + 1).ToString() + ", Contestant " + index.ToString() + ", Time: " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresArray[0].ToString();
                }
                else {
                    txtMatchup += defaultColor + "Match " + (j + 1).ToString() + ", Contestant " + index.ToString();
                    if (j <= gameManager.tournamentManager.currentMatchNum && i <= gameManager.tournamentManager.currentRoundNum) {
                        txtMatchup += ":  IN PROGRESS";
                    }
                    else {
                        txtMatchup += ":  PENDING";
                    }                    
                }
                txtMatchup += "</color>\n";

                txtResults += txtMatchup;
            }
            
        }

        //Debug.Log("gameManager.tournamentManager.currentTournamentInfo.tournamentFinished " + gameManager.tournamentManager.currentTournamentInfo.tournamentFinished.ToString());
        if (gameManager.tournamentManager.currentTournamentInfo.tournamentFinished) {
            // FINISHED!
            int tournamentWinnerIndex = gameManager.tournamentManager.currentTournamentInfo.winnerID;
            int numRoundWins = 0;
            for (int i = 0; i < gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count; i++) {
                //Debug.Log("Contestant #" + i.ToString() + " Round Wins: " + competitorRoundWins[i].ToString());
                if (tournamentWinnerIndex == gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].winnerID) {
                    numRoundWins++;
                }
            }

            if(tournamentWinnerIndex == -1) {
                txtResults += "\n\n" + defaultColor;
            }
            else {
                txtResults += "\n\n" + contestantColors[tournamentWinnerIndex];
            }
            txtResults += "Winner: Contestant " + tournamentWinnerIndex.ToString() + "!, Rounds Won: " + numRoundWins.ToString() + " / " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList.Count.ToString() + "\n\n";
            if (tournamentWinnerIndex == 0) {
                txtResults += "<size=24><b>YOU WON!!!</b></size>\n" + "You Received $" + gameManager.tournamentManager.currentTournamentInfo.reward.ToString() + " Reward";

            }
            else {
                txtResults += "you lost...";
            }
            txtResults = txtResults + "</color>";            
        }
        else {
            // IN PROGRESS!!!
        }

        return txtResults;
             
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
