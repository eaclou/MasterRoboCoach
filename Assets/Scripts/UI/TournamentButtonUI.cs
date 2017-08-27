using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TournamentButtonUI : MonoBehaviour {

    //public TrainingManager trainerRef;
    public int tournamentIndex;
    public TournamentSelectUI tournamentSelectUI;

    public Button buttonThis;
    //public Text textFitnessComponentName;
    //public Toggle toggleBigIsBetter;
    //public Button buttonMeasure;
    //public Slider sliderWeight;
    //public Text textWeightValue;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        //textFitnessComponentName.text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].type.ToString();
        //toggleBigIsBetter.isOn = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].biggerIsBetter;
        //buttonMeasure.GetComponentInChildren<Text>().text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure.ToString();
        //sliderWeight.value = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].weight;
        //textWeightValue.text = sliderWeight.value.ToString();

        buttonThis.GetComponentInChildren<Text>().text = tournamentSelectUI.trainingMenuRef.gameManager.availableTournamentsList[tournamentIndex].tournamentName;

        bool locked = false;
        if(tournamentSelectUI.trainingMenuRef.gameManager.prestige < tournamentSelectUI.trainingMenuRef.gameManager.availableTournamentsList[tournamentIndex].entranceFee) {
            locked = true;
        }
        if (tournamentSelectUI.trainingMenuRef.gameManager.availableTournamentsList[tournamentIndex].cooldownTime > 0f) {
            locked = true;
        }
        buttonThis.interactable = !locked;
    }

    public void MouseOver() {
        tournamentSelectUI.MouseOverTournamentButton(tournamentIndex);
    }
    public void MouseOut() {
        tournamentSelectUI.MouseOutTournamentButton(tournamentIndex);
    }
    public void ClickButton() {
        //Debug.Log("ClickMeasure()");
        /*int numMeasure = System.Enum.GetNames(typeof(FitnessComponentMeasure)).Length;
        int curMeasure = (int)fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure;
        int newMeasure = curMeasure + 1;
        if (newMeasure >= numMeasure) {
            newMeasure = 0;
        }
        fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure = (FitnessComponentMeasure)newMeasure;
        buttonMeasure.GetComponentInChildren<Text>().text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure.ToString();
        */

        tournamentSelectUI.ClickTournament(tournamentIndex);
    }
}
