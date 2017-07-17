using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingMenuUI : MonoBehaviour {

    public MainMenu mainMenuRef;
    public Toggle toggleManualMode;
    public Button buttonPlayPause;
    public Button buttonFaster;
    public Button buttonSlower;
    public Button buttonCameraMode;
    public Text playbackSpeed;

    public GameObject panelManualSelectionToolbar;
    public Button buttonManualKeep;
    public Button buttonManualAuto;
    public Button buttonManualKill;
    public Button buttonManualReplay;

    public Text textCurrentGen;
    public Text textContestant;
    public Text textOpponent;
    public Text textTestingProgress;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        SetStatusFromData();

    }

    public void SetStatusFromData() {
        if(mainMenuRef.gameManagerRef.trainerRef.ManualTrainingMode) {
            panelManualSelectionToolbar.SetActive(true);
        }
        else {
            panelManualSelectionToolbar.SetActive(false);
        }

        textCurrentGen.text = mainMenuRef.gameManagerRef.trainerRef.GetCurrentGenText();
        textContestant.text = mainMenuRef.gameManagerRef.trainerRef.GetContestantText();
        textOpponent.text = mainMenuRef.gameManagerRef.trainerRef.GetOpponentText();
        textTestingProgress.text = mainMenuRef.gameManagerRef.trainerRef.GetTestingProgressText();

        string cameraModeText = "";
        if(mainMenuRef.gameManagerRef.trainerRef.currentCameraMode == 0) {
            cameraModeText = "Wide";
        }
        else if (mainMenuRef.gameManagerRef.trainerRef.currentCameraMode == 1) {
            cameraModeText = "Top Down";
        }
        else {
            cameraModeText = "Shoulder";
        }
        buttonCameraMode.GetComponentInChildren<Text>().text = "Camera:\n" + cameraModeText;
    }

    public void ClickToggleManualMode(bool value) {
        
        mainMenuRef.gameManagerRef.trainerRef.ManualTrainingMode = value;
        print("clicked toggle manual mode! " + mainMenuRef.gameManagerRef.trainerRef.ManualTrainingMode.ToString());
    }

    public void ClickButtonPlayPause() {
        mainMenuRef.gameManagerRef.trainerRef.TogglePlayPause();
        if(mainMenuRef.gameManagerRef.trainerRef.TrainingPaused) {
            buttonPlayPause.GetComponentInChildren<Text>().text = "||";
        }
        else {
            buttonPlayPause.GetComponentInChildren<Text>().text = ">";
        }        
    }

    public void ClickButtonFaster() {
        mainMenuRef.gameManagerRef.trainerRef.IncreasePlaybackSpeed();
        playbackSpeed.text = "Playback Speed:\n" + mainMenuRef.gameManagerRef.trainerRef.playbackSpeed.ToString();
    }

    public void ClickButtonSlower() {
        mainMenuRef.gameManagerRef.trainerRef.DecreasePlaybackSpeed();
        playbackSpeed.text = "Playback Speed:\n" + mainMenuRef.gameManagerRef.trainerRef.playbackSpeed.ToString();
    }

    public void ClickButtonCameraMode() {
        mainMenuRef.gameManagerRef.trainerRef.ClickCameraMode();
    }

    public void ClickButtonManualKeep() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualKeep();
    }

    public void ClickButtonManualAuto() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualAuto();
    }

    public void ClickButtonManualKill() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualKill();
    }

    public void ClickButtonManualReplay() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualReplay();
    }
}
