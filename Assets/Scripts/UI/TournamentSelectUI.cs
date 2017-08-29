using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentSelectUI : MonoBehaviour {

    public TrainingMenuUI trainingMenuRef;

    public GameObject panelAvailableTournamentsAnchor;
    [SerializeField] GameObject goTournamentButtonPrefab;

    public Button buttonBack;

    //public Button buttonFirstTournament;
    //public Button buttonCooldown;
    //public Button buttonLocked;

    public Text textTournamentDescription;

    public GameObject panelTournamentPrompt;
    public InputField inputFieldCompetitorName;
    public Button buttonEnterTournament;

    //public string selectedTournamentTemplateName;
    public int selectedTournamentIndex;

    public bool tournamentSelected = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Initialize() {
        Debug.Log("TournamentSelectUI Initialize!");
        trainingMenuRef.gameManager.UpdateAvailableTournamentsList();

        textTournamentDescription.text = "";
        selectedTournamentIndex = -1;
        inputFieldCompetitorName.text = "";
        panelTournamentPrompt.SetActive(false);
        tournamentSelected = false;

        // Set up tournament list:
        //panelAvailableTournamentsAnchor
        foreach (Transform child in panelAvailableTournamentsAnchor.transform) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < trainingMenuRef.gameManager.availableTournamentsList.Count; i++) {
            Debug.Log("availableTournamentsList " + i.ToString());
            GameObject tournamentButtonGO = (GameObject)Instantiate(goTournamentButtonPrefab);

            TournamentButtonUI tournamentButtonUI = tournamentButtonGO.GetComponent<TournamentButtonUI>();
            tournamentButtonUI.tournamentIndex = i;
            tournamentButtonUI.tournamentSelectUI = this;
            tournamentButtonUI.SetStatusFromData();
            tournamentButtonGO.transform.SetParent(panelAvailableTournamentsAnchor.transform);

            /*FitnessCompRowUI fitnessComponentRowScript = fitnessComponentListRow.GetComponent<FitnessCompRowUI>();

            fitnessComponentRowScript.fitnessIndex = i; // CHANGE LATER!!!!!!!
            fitnessComponentRowScript.trainerRef = trainerRef;
            fitnessComponentRowScript.fitnessFunctionUI = this;
            fitnessComponentRowScript.SetStatusFromData();
            fitnessComponentListRow.transform.SetParent(transformFitnessCompTableSpace);
            */
        }
    }

    public void UpdateState() {

    }

    public void EnterState() {

    }

    private void UpdateEnterTournamentButtonUI() {
        if(tournamentSelected) {
            buttonEnterTournament.interactable = false;
            if(selectedTournamentIndex >= 0) {
                buttonEnterTournament.GetComponentInChildren<Text>().text = "Enter Tournament $" + trainingMenuRef.gameManager.availableTournamentsList[selectedTournamentIndex].entranceFee.ToString();
            }            
        }
    }

    public void ClickButtonBack() {
        Debug.Log("ClickButtonBack");
        //trainingMenuRef.gameManager.trainerRef.ExitTournamentSelectScreen();
        trainingMenuRef.ShowUI();
        trainingMenuRef.tournamentSelectOn = false;
        trainingMenuRef.ClickButtonPlayPause();
    }

    public void MouseOverTournamentButton(int index) {
        textTournamentDescription.text = trainingMenuRef.gameManager.availableTournamentsList[index].tournamentName + "!\n\n" +
            "Compete in * versus " + trainingMenuRef.gameManager.availableTournamentsList[index].numOpponents.ToString() + " competitors!\n" +
            "Entrance Fee: " + trainingMenuRef.gameManager.availableTournamentsList[index].entranceFee.ToString() + " prestige\n" +
            "Reward: " + trainingMenuRef.gameManager.availableTournamentsList[index].reward.ToString() + " prestige";        
    }
    public void MouseOutTournamentButton(int index) {
        if(!tournamentSelected) {
            textTournamentDescription.text = "";
        }        
    }
    public void ClickTournament(int index) {
        tournamentSelected = true;
        panelTournamentPrompt.SetActive(true);


        UpdateEnterTournamentButtonUI();

        // Set selected TournamentInfo
        selectedTournamentIndex = index;
        //selectedTournamentTemplateName = "TutorialTournament";
    }
    /*public void ClickFirstTournament() {
        panelTournamentPrompt.SetActive(true);

        buttonEnterTournament.interactable = false;
        buttonEnterTournament.GetComponentInChildren<Text>().text = "Enter Tournament $10";

        // Set selected TournamentInfo
        selectedTournamentTemplateName = "TutorialTournament";
    }*/
    public void ClickEnterTournament() {
        string competitorName = inputFieldCompetitorName.text;
        Debug.Log("ClickEnterTournament() name:" + competitorName);

        // Grab from Template!
        //TournamentInfo firstTourneyInfo = new TournamentInfo(trainingMenuRef.gameManager.trainerRef.teamsConfig);
        // Error check for incorrect/non-existant filename... eventually
        //TournamentInfo tournamentInfo = (Resources.Load("Templates/Tournaments/" + selectedTournamentTemplateName) as TournamentInfoWrapper).tournamentInfo;
        TournamentInfo tournamentInfo = trainingMenuRef.gameManager.availableTournamentsList[selectedTournamentIndex];
        // fill in stats
        trainingMenuRef.gameManager.cameraEnabled = false;
        trainingMenuRef.gameManager.trainerRef.EnterTournament(tournamentInfo);
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

    /*public void MouseOverCooldownTournament() {
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
    }*/
}
