using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentSelectUI : MonoBehaviour {

    public TrainingMenuUI trainingMenuRef;

    public Button buttonBack;

    public Button buttonFirstTournament;
    public Button buttonCooldown;
    public Button buttonLocked;

    public Text textTournamentDescription;

    public GameObject panelTournamentPrompt;
    public InputField inputFieldCompetitorName;
    public Button buttonEnterTournament;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize() {
        textTournamentDescription.text = "";
        
        panelTournamentPrompt.SetActive(false);
    }

    public void ClickButtonBack() {
        Debug.Log("ClickButtonBack");
        //trainingMenuRef.gameManager.trainerRef.ExitTournamentSelectScreen();
        trainingMenuRef.ShowUI();
        trainingMenuRef.tournamentSelectOn = false;
        trainingMenuRef.ClickButtonPlayPause();
    }

    public void MouseOverFirstTournament() {
        textTournamentDescription.text = "TUTORIAL TOURNAMENT!\n\n" +
            "Compete in time trials versus a single opponent!\n" +
            "Entrance Fee: 10 prestige\n" +
            "Reward: 25 prestige";
        //buttonFirstTournament.colors.
    }
    public void MouseOutFirstTournament() {
        textTournamentDescription.text = "";
    }
    public void ClickFirstTournament() {
        panelTournamentPrompt.SetActive(true);

        buttonEnterTournament.interactable = false;
        buttonEnterTournament.GetComponentInChildren<Text>().text = "Enter Tournament $10";
    }
    public void ClickEnterTournament() {
        string competitorName = inputFieldCompetitorName.text;
        Debug.Log("ClickEnterTournament() name:" + competitorName);
        TournamentInfo firstTourneyInfo = new TournamentInfo(trainingMenuRef.gameManager.trainerRef.teamsConfig);
        // fill in stats
        trainingMenuRef.gameManager.cameraEnabled = false;
        trainingMenuRef.gameManager.trainerRef.EnterTournament(firstTourneyInfo);
    }
    public void ChangeInputFieldCompetitorName() {
        bool validName = false;
        if(inputFieldCompetitorName.text != "") {
            validName = true;
        }

        if(validName) {
            buttonEnterTournament.interactable = true;
        }
        else {

        }
    }

    public void MouseOverCooldownTournament() {
        textTournamentDescription.text = "SECOND TOURNAMENT!\n\n" +
            "Compete in time trials versus 3 opponents!\n" +
            "Features: Altitude Change\n" +
            "Entrance Fee: 20 prestige\n" +
            "Available in 6 space days\n" +
            "Reward: 50 prestige";
        //buttonFirstTournament.colors.
    }
    public void MouseOutCooldownTournament() {
        textTournamentDescription.text = "";
    }
    public void MouseOverLockedTournament() {
        textTournamentDescription.text = "THIRD TOURNAMENT!\n\n" +
            "Compete in time trials versus 7 opponents!\n" +
            "Features: Obstacles, Altitude Change\n" +
            "Entrance Fee: 50 prestige\n" +
            "Not enough Prestige!\n" +
            "Reward: 120 prestige";
        //buttonFirstTournament.colors.
    }
    public void MouseOutLockedTournament() {
        textTournamentDescription.text = "";
    }
}
