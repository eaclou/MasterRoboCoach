using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

    public GameManager gameManagerRef;

    public GameObject PanelMainMenu;
    public GameObject PanelTestChallengeSetup;
    public GameObject PanelTraining;

    public TrainingMenuUI trainingMenuRef;

    // Use this for initialization
    void Start () {
        PanelMainMenu.SetActive(false);
        PanelTestChallengeSetup.SetActive(false);
        PanelTraining.SetActive(true);

        // HACKY! init UI elements:
        // init manual mode toggle to settings in trainingManager:
        trainingMenuRef.toggleManualMode.isOn = gameManagerRef.trainerRef.ManualTrainingMode;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClickTestChallenge() {
        //Debug.Log("Clicked Test Challenge Button!");

        PanelMainMenu.SetActive(false);
        PanelTestChallengeSetup.SetActive(true);
        PanelTraining.SetActive(false);
        //Debug.Assert(false, "Testing Assertion!");
    }

    public void ClickNew() {
        PanelMainMenu.SetActive(false);
        PanelTestChallengeSetup.SetActive(false);
        PanelTraining.SetActive(true);
    }

    public void QuitGame() {
        Application.Quit();
    }
}
