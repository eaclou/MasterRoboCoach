using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChallengeSetupUI : MonoBehaviour {

    public GameManager gameManager;

    public Button buttonLoad;
    public InputField inputFieldSaveName;
    public Text textLoadError;

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
        
    }

    public void ClickNew() {
        //panelMainMenu.SetActive(false);
        //panelChallengeSetup.SetActive(false);
        //panelTraining.SetActive(true);
        //Debug.Log("ClickNew(" + challengeType.ToString() + ")");

        //gameManager.trainerRef.NewTrainingMode(challengeType);

        gameManager.NewTrainingMode();
    }

    public void ClickLoad() {
        string saveFileName = inputFieldSaveName.text;
        textLoadError.text = saveFileName;

        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Application.dataPath + "/TrainingSaves/" + saveFileName + ".json";

        if (File.Exists(filePath)) {
            //panelMainMenu.SetActive(false);
            //panelChallengeSetup.SetActive(false);
            //panelTraining.SetActive(true);
            gameManager.LoadTrainingMode(filePath);
        }
        else {
            Debug.LogError("Cannot load game data!");
        }

        //gameManagerRef.trainerRef.LoadTraining(filePath);
    }

    public void ClickBack() {
        gameManager.ChangeState(GameManager.GameState.MainMenu);
    }

    public void ClickQuit() {
        // Cleanup and stuff pre-Exit
        gameManager.QuitGame();
    }
}
