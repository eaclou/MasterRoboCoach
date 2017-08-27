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

            string txtTimer = "";
            txtTimer += gameManager.tournamentManager.tournamentInstance.currentTimeStep.ToString() + " / " + gameManager.tournamentManager.currentMatchup.evalTicket.maxTimeSteps.ToString();
            textTimer.text = txtTimer;
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
                    txt += "   Score: " + gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].contestantScoresList[0].ToString();
                }
                txt += "\n";
                //if (gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j].evalTicket.status != EvaluationTicket.EvaluationStatus.Complete) {
                 //   return gameManager.tournamentManager.currentTournamentInfo.tournamentRoundList[i].matchupList[j];
                //}
            }
        }
        textMatchList.text = txt;
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
