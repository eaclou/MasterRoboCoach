using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameManager gameManager;

    public MainMenuUI panelMainMenu;
    public OptionsUI panelOptions;
    public ChallengeSetupUI panelChallengeSetup;
    public TrainingMenuUI panelTraining;
    public TournamentUI panelTournament;

    //public Button buttonLoad;
    //public InputField inputFieldSaveName;
    //public Text textLoadError;

    //public TrainingMenuUI trainingMenuRef;

    //public Challenge.Type challengeType;

    // Use this for initialization
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {

    }

    public void InitializeUI() {
        InitializeUIFromGameState();
    }    

    public void InitializeUIFromGameState() {
        // Don't need to Exit any states because this is first-time setup... ?
        switch(gameManager.gameState) {
            case GameManager.GameState.MainMenu:
                //do something
                panelMainMenu.gameObject.SetActive(true);
                panelOptions.gameObject.SetActive(false);
                panelChallengeSetup.gameObject.SetActive(false);
                panelTraining.gameObject.SetActive(false);
                panelTournament.gameObject.SetActive(false);

                panelMainMenu.EnterState();
                break;
            case GameManager.GameState.OptionsMenu:
                //do something
                panelMainMenu.gameObject.SetActive(false);
                panelOptions.gameObject.SetActive(true);
                panelChallengeSetup.gameObject.SetActive(false);
                panelTraining.gameObject.SetActive(false);
                panelTournament.gameObject.SetActive(false);

                panelOptions.EnterState();
                break;
            case GameManager.GameState.ChallengeSetup:
                //do something
                panelMainMenu.gameObject.SetActive(false);
                panelOptions.gameObject.SetActive(false);
                panelChallengeSetup.gameObject.SetActive(true);
                panelTraining.gameObject.SetActive(false);
                panelTournament.gameObject.SetActive(false);

                panelChallengeSetup.EnterState();
                break;            
            case GameManager.GameState.Training:
                //do something
                panelMainMenu.gameObject.SetActive(false);
                panelOptions.gameObject.SetActive(false);
                panelChallengeSetup.gameObject.SetActive(false);
                panelTraining.gameObject.SetActive(true);
                panelTournament.gameObject.SetActive(false);

                panelTraining.EnterState();
                break;
            case GameManager.GameState.Tournament:
                //do something
                panelMainMenu.gameObject.SetActive(false);
                panelOptions.gameObject.SetActive(false);
                panelChallengeSetup.gameObject.SetActive(false);
                panelTraining.gameObject.SetActive(false);
                panelTournament.gameObject.SetActive(true);

                panelTournament.EnterState();
                break;
            default:
                //do nothing
                Debug.LogError("[ERROR!] NO SUCH GAMESTATE FOUND! (" + gameManager.gameState.ToString() + ")");
                break;
        }        
    }


    /*public void EnterTournamentMode(TournamentInfo tournamentInfo) {
        panelMainMenu.SetActive(false);
        panelChallengeSetup.SetActive(false);
        panelTraining.SetActive(false);
        panelTournament.SetActive(true);

        //panelTournament.GetComponent<TournamentUI>().Initialize(tournamentInfo);
        trainingMenuRef.mainMenuRef.gameManagerRef.EnterTournamentMode(tournamentInfo);
    }
    public void ExitTournamentMode() {
        panelMainMenu.SetActive(false);
        panelChallengeSetup.SetActive(false);
        panelTraining.SetActive(true);
        panelTournament.SetActive(false);

        trainingMenuRef.HideTournamentSelectScreen();
        //panelTournament.GetComponent<TournamentUI>().Initialize(tournamentInfo);
        //trainingMenuRef.mainMenuRef.gameManagerRef.EnterTournamentMode(tournamentInfo);
    }

    public void ClickTestChallenge() {
        //Debug.Log("Clicked Test Challenge Button!");

        panelMainMenu.SetActive(false);
        panelChallengeSetup.SetActive(true);
        panelTraining.SetActive(false);

        challengeType = Challenge.Type.Test;
    }

    public void ClickRacingChallenge() {

        panelMainMenu.SetActive(false);
        panelChallengeSetup.SetActive(true);
        panelTraining.SetActive(false);

        challengeType = Challenge.Type.Racing;
        Debug.Log("Clicked Racing Challenge Button! " + challengeType.ToString());
    }

    public void ClickCombatChallenge() {

        panelMainMenu.SetActive(false);
        panelChallengeSetup.SetActive(true);
        panelTraining.SetActive(false);

        challengeType = Challenge.Type.Combat;
        Debug.Log("Clicked Combat Challenge Button! " + challengeType.ToString());
    }

    public void ClickNew() {
        panelMainMenu.SetActive(false);
        panelChallengeSetup.SetActive(false);
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
            panelChallengeSetup.SetActive(false);
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
