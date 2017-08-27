using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class MainMenuUI : MonoBehaviour {

    public GameManager gameManager;

    public GameObject panelChallengeSelectButtons;
    public GameObject panelFirstTimeStart;
    
    //public Button buttonLoad;
    //public InputField inputFieldSaveName;
    //public Text textLoadError;

    //public TrainingMenuUI trainingMenuRef;

    //public Challenge.Type challengeType;

    // Use this for initialization
    void Start () {
        

        // HACKY! init UI elements:
        // init manual mode toggle to settings in trainingManager:
        //trainingMenuRef.toggleManualMode.isOn = gameManagerRef.trainerRef.ManualTrainingMode;
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
        
        if (gameManager.firstTimePlaythrough) {
            // No challenge select, just show "Begin" button to skip to tutorial training:
            panelChallengeSelectButtons.SetActive(false);
            panelFirstTimeStart.SetActive(true);
        }
        else {
            // Display current suite of Challenges to choose from:
            panelChallengeSelectButtons.SetActive(true);
            panelFirstTimeStart.SetActive(false);
        }
    }

    public void ClickTestChallenge() {
        //Debug.Log("Clicked Test Challenge Button!");
        gameManager.challengeType = Challenge.Type.Test;
        gameManager.GoToChallengeSetup();        
    }

    public void ClickRacingChallenge() {
        gameManager.challengeType = Challenge.Type.Racing;
        gameManager.GoToChallengeSetup();
    }

    public void ClickCombatChallenge() {
        gameManager.challengeType = Challenge.Type.Combat;
        gameManager.GoToChallengeSetup();
    }

    public void ClickQuit() {
        // Cleanup and stuff pre-Exit
        gameManager.QuitGame();
    }

    public void ClickOptions() {
        gameManager.ChangeState(GameManager.GameState.OptionsMenu);
    }

    public void ClickButtonBegin() {
        gameManager.FirstTimePlayGoToTraining();
    }


    /*public void EnterTournamentMode(TournamentInfo tournamentInfo) {
        panelMainMenu.SetActive(false);
        panelTestChallengeSetup.SetActive(false);
        panelTraining.SetActive(false);
        panelTournament.SetActive(true);

        //panelTournament.GetComponent<TournamentUI>().Initialize(tournamentInfo);
        trainingMenuRef.mainMenuRef.gameManagerRef.EnterTournamentMode(tournamentInfo);
    }
    public void ExitTournamentMode() {
        panelMainMenu.SetActive(false);
        panelTestChallengeSetup.SetActive(false);
        panelTraining.SetActive(true);
        panelTournament.SetActive(false);

        trainingMenuRef.HideTournamentSelectScreen();
        //panelTournament.GetComponent<TournamentUI>().Initialize(tournamentInfo);
        //trainingMenuRef.mainMenuRef.gameManagerRef.EnterTournamentMode(tournamentInfo);
    }

    public void ClickTestChallenge() {
        //Debug.Log("Clicked Test Challenge Button!");

        panelMainMenu.SetActive(false);
        panelTestChallengeSetup.SetActive(true);
        panelTraining.SetActive(false);

        challengeType = Challenge.Type.Test;
    }

    public void ClickRacingChallenge() {        

        panelMainMenu.SetActive(false);
        panelTestChallengeSetup.SetActive(true);
        panelTraining.SetActive(false);

        challengeType = Challenge.Type.Racing;
        Debug.Log("Clicked Racing Challenge Button! " + challengeType.ToString());
    }

    public void ClickCombatChallenge() {

        panelMainMenu.SetActive(false);
        panelTestChallengeSetup.SetActive(true);
        panelTraining.SetActive(false);

        challengeType = Challenge.Type.Combat;
        Debug.Log("Clicked Combat Challenge Button! " + challengeType.ToString());
    }

    public void ClickNew() {
        panelMainMenu.SetActive(false);
        panelTestChallengeSetup.SetActive(false);
        panelTraining.SetActive(true);
        Debug.Log("ClickNew(" + challengeType.ToString() + ")");
        gameManagerRef.trainerRef.NewTrainingMode(challengeType);
    }

    public void ClickLoad() {
        string saveFileName = inputFieldSaveName.text;
        textLoadError.text = saveFileName;

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Application.dataPath + "/TrainingSaves/" + saveFileName + ".json";

        if (File.Exists(filePath)) {
            panelMainMenu.SetActive(false);
            panelTestChallengeSetup.SetActive(false);
            panelTraining.SetActive(true);
            gameManagerRef.trainerRef.LoadTraining(filePath);
        }
        else {
            Debug.LogError("Cannot load game data!");
        }

        //gameManagerRef.trainerRef.LoadTraining(filePath);
    }

    public void ClickQuit() {
        // Cleanup and stuff pre-Exit

        QuitGame();
    }

    public void QuitGame() {
        Application.Quit();
    }
    */
}
