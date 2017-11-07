using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public ComputeShader terrainConstructorGPUCompute;  // is this really the best way to do this??
    public ComputeShader terrainInstanceCompute;  // is this really the best way to do this??

    public bool isTraining = false;

    public UIManager uiManager;
    public TrainingManager trainerRef;
    public TournamentManager tournamentManager;

    // Camera
    public CameraManager cameraManager;
    public bool cameraEnabled = false;

    public GameState gameState;
    public enum GameState {
        MainMenu,
        ChallengeSetup,
        OptionsMenu,
        Training,
        Tournament
    }

    public int prestige = 10;
    public List<TournamentInfo> availableTournamentsList;

    public Challenge.Type challengeType;

    // Flags:
    public bool firstTimePlaythrough = true;  // affects contents of main menu

    void Awake() {
        
    }

    // Use this for initialization
    void Start () {
        TerrainConstructorGPU.terrainConstructorGPUCompute = this.terrainConstructorGPUCompute;
        TerrainConstructorGPU.terrainInstanceCompute = this.terrainInstanceCompute;
        FirstTimeInitialization();
    }
	
	// Update is called once per frame
	void Update () {
        
        //UpdateState()

        switch (gameState) {
            case GameManager.GameState.MainMenu:
                //do something
                break;
            case GameManager.GameState.OptionsMenu:
                //do something
                break;
            case GameManager.GameState.ChallengeSetup:
                //do something
                break;
            case GameManager.GameState.Training:
                //do something
                uiManager.panelTraining.UpdateState();
                SetCamera();
                break;
            case GameManager.GameState.Tournament:
                //do something
                uiManager.panelTournament.UpdateState();
                SetCamera();
                break;
            default:
                //do nothing
                Debug.LogError("[ERROR!] NO SUCH GAMESTATE FOUND! (" + gameState.ToString() + ")");
                break;
        }
    }

    void FixedUpdate() {
        switch (gameState) {
            case GameManager.GameState.MainMenu:
                //do something
                break;
            case GameManager.GameState.OptionsMenu:
                //do something
                break;
            case GameManager.GameState.ChallengeSetup:
                //do something
                break;
            case GameManager.GameState.Training:
                //do something
                trainerRef.Tick();
                break;
            case GameManager.GameState.Tournament:
                //do something
                tournamentManager.Tick();
                break;
            default:
                //do nothing
                Debug.LogError("[ERROR!] NO SUCH GAMESTATE FOUND! (" + gameState.ToString() + ")");
                break;
        }
    }

    public void UpdateAvailableTournamentsList() {
        if(availableTournamentsList == null) {
            availableTournamentsList = new List<TournamentInfo>();

            //TournamentInfo tournamentInfo = (Resources.Load("Templates/Tournaments/TutorialTournament") as TournamentInfoWrapper).tournamentInfo;
            //availableTournamentsList.Add(tournamentInfo);
        }
        else {
            availableTournamentsList.Clear();

            
        }

        TournamentInfo tournamentInfo1 = (Resources.Load("Templates/Tournaments/TutorialTournament") as TournamentInfoWrapper).tournamentInfo;
        availableTournamentsList.Add(tournamentInfo1);

        TournamentInfo tournamentInfo2 = (Resources.Load("Templates/Tournaments/Sectionals") as TournamentInfoWrapper).tournamentInfo;
        availableTournamentsList.Add(tournamentInfo2);

        TournamentInfo tournamentInfo3 = (Resources.Load("Templates/Tournaments/Regionals") as TournamentInfoWrapper).tournamentInfo;
        availableTournamentsList.Add(tournamentInfo3);

        TournamentInfo tournamentInfo4 = (Resources.Load("Templates/Tournaments/CombatTournamentA") as TournamentInfoWrapper).tournamentInfo;
        availableTournamentsList.Add(tournamentInfo4);
    }

    private void FirstTimeInitialization() {
        ChangeState(GameManager.GameState.MainMenu);
    }

    public void ChangeState(GameState newState) {

        gameState = newState;

        switch (gameState) {
            case GameManager.GameState.MainMenu:
                //do something
                break;
            case GameManager.GameState.OptionsMenu:
                //do something
                break;
            case GameManager.GameState.ChallengeSetup:
                //do something
                break;
            case GameManager.GameState.Training:
                //do something
                break;
            case GameManager.GameState.Tournament:                
                //do something
                break;
            default:
                //do nothing
                Debug.LogError("[ERROR!] NO SUCH GAMESTATE FOUND! (" + gameState.ToString() + ")");
                break;
        }

        cameraEnabled = false;
        uiManager.InitializeUI();
    }

    private void SetCamera() {

        if (cameraEnabled) {
            if(gameState == GameState.Training) {
                Vector3 agentPosition = Vector3.zero;
                int focusPlayer = 0;

                if (trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex != 0) {
                    focusPlayer = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1;

                    if (trainerRef.evaluationManager.exhibitionInstance != null) {
                        if (trainerRef.evaluationManager.exhibitionInstance.currentAgentsArray[focusPlayer].rootObject != null) {
                            agentPosition = trainerRef.evaluationManager.exhibitionInstance.currentAgentsArray[focusPlayer].rootObject.transform.position + trainerRef.evaluationManager.exhibitionInstance.currentAgentsArray[focusPlayer].rootCOM;
                        }
                    }
                }
                               

                cameraManager.UpdateCameraState(agentPosition);
            }
            if(gameState == GameState.Tournament) {
                Vector3 agentPosition = Vector3.zero;
                int focusPlayer = 0;

                if(tournamentManager.currentMatchup != null) {
                    if (tournamentManager.currentMatchup.evalTicket.focusPopIndex != 0) {
                        focusPlayer = tournamentManager.currentMatchup.evalTicket.focusPopIndex - 1;
                    }
                }                
                if (tournamentManager.tournamentInstance.currentAgentsArray != null) {
                    agentPosition = tournamentManager.tournamentInstance.currentAgentsArray[focusPlayer].rootObject.transform.position + tournamentManager.tournamentInstance.currentAgentsArray[focusPlayer].rootCOM;
                }

                cameraManager.UpdateCameraState(agentPosition);
            }
        }
    }
    
    public void GoToChallengeSetup() {
        switch(challengeType) {
            case Challenge.Type.Test:
                // do something
                break;
            case Challenge.Type.Racing:
                // do something
                break;
            case Challenge.Type.Combat:
                // do something
                break;
            default:
                //do nothing
                Debug.LogError("[ERROR!] NO SUCH CHALLENGETYPE FOUND! (" + challengeType.ToString() + ")");
                break;
        }

        ChangeState(GameState.ChallengeSetup);
    }

    public void NewTrainingMode() {
        trainerRef.NewTrainingMode(challengeType);
        ChangeState(GameState.Training);
        cameraEnabled = true;
    }

    public void LoadTrainingMode(string filePath) {
        trainerRef.LoadTraining(filePath);
        ChangeState(GameState.Training);
        cameraEnabled = true;
    }

    public void FirstTimePlayGoToTraining() {
        challengeType = Challenge.Type.Test;
        NewTrainingMode();
    }
    
    public void EnterTournamentMode(TournamentInfo tournamentInfo) {
        // Set up TournamentManager with info
        tournamentManager.Initialize(trainerRef.teamsConfig, tournamentInfo);
        // Set up UI:
        ChangeState(GameState.Tournament);
        //mainMenuRef.panelTournament.GetComponent<TournamentUI>().Initialize(tournamentInfo);
    }

    public void ExitTournamentMode() {
        tournamentManager.Exit();

        trainerRef.evaluationManager.ResetForNewGeneration(trainerRef.teamsConfig);
        trainerRef.isTraining = true;
        trainerRef.trainingPaused = false;
        uiManager.panelTraining.tournamentSelectOn = false;

        ChangeState(GameState.Training);
        //mainMenuRef.ExitTournamentMode();
    }

    public void QuitGame() {
        Application.Quit();
    }
}
