using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingSettingsUI : MonoBehaviour {

    public TrainingManager trainerRef;
    public TrainingSettingsManager currentTrainingSettingsRef;

    public Button buttonMutationChanceDecrease;
    public Button buttonMutationChanceIncrease;
    public Text textMutationChanceValue;

    public Button buttonMutationStepSizeDecrease;
    public Button buttonMutationStepSizeIncrease;
    public Text textMutationStepSizeValue;

    public Button buttonRepresentativesDecrease;
    public Button buttonRepresentativesIncrease;
    public Text textRepresentativesValue;

    public Button buttonEvolving;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData(TrainingManager trainerRef) {
        this.trainerRef = trainerRef;
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        int numReps = 1;
        bool isEvolving;
        if (focusPop < 1) {
            // env
            numReps = trainerRef.teamsConfig.environmentPopulation.numPerformanceReps;
            currentTrainingSettingsRef = trainerRef.teamsConfig.environmentPopulation.trainingSettingsManager;
            isEvolving = trainerRef.teamsConfig.environmentPopulation.isTraining;
        }
        else {
            numReps = trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps;
            currentTrainingSettingsRef = trainerRef.teamsConfig.playersList[focusPop - 1].trainingSettingsManager;
            isEvolving = trainerRef.teamsConfig.playersList[focusPop - 1].isTraining;
        }

        textRepresentativesValue.text = numReps.ToString();
        textMutationChanceValue.text = (100f * currentTrainingSettingsRef.mutationChance).ToString("F3") + "%";
        textMutationStepSizeValue.text = (100f * currentTrainingSettingsRef.mutationStepSize).ToString("F1") + "%";

        if (isEvolving) {
            buttonEvolving.GetComponent<Image>().color = Color.white;
            buttonEvolving.GetComponentInChildren<Text>().color = Color.black;
            buttonEvolving.GetComponentInChildren<Text>().text = "EVOLVING";
        }
        else {
            buttonEvolving.GetComponent<Image>().color = Color.black;
            buttonEvolving.GetComponentInChildren<Text>().color = Color.white;
            buttonEvolving.GetComponentInChildren<Text>().text = "STATIC";
        }        
    }

    public void ClickButtonMutationChanceDecrease() {
        currentTrainingSettingsRef.mutationChance *= 0.8f;
        if(currentTrainingSettingsRef.mutationChance < 0.0001f) {
            currentTrainingSettingsRef.mutationChance = 0.0001f;
        }
    }
    public void ClickButtonMutationChanceIncrease() {
        currentTrainingSettingsRef.mutationChance *= 1.25f;
        if (currentTrainingSettingsRef.mutationChance > 1f) {
            currentTrainingSettingsRef.mutationChance = 1f;
        }
    }
    public void ClickButtonMutationStepSizeDecrease() {
        currentTrainingSettingsRef.mutationStepSize *= 0.8f;
        if (currentTrainingSettingsRef.mutationStepSize < 0.0001f) {
            currentTrainingSettingsRef.mutationStepSize = 0.0001f;
        }
    }
    public void ClickButtonMutationStepSizeIncrease() {
        currentTrainingSettingsRef.mutationStepSize *= 1.25f;
        if (currentTrainingSettingsRef.mutationStepSize > 1f) {
            currentTrainingSettingsRef.mutationStepSize = 1f;
        }
    }

    public void ClickButtonRepresentativesDecrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        
        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numPerformanceReps -= 1;
            if(trainerRef.teamsConfig.environmentPopulation.numPerformanceReps < 1) {
                trainerRef.teamsConfig.environmentPopulation.numPerformanceReps = 1;
            }
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps -= 1;
            if(trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps < 1) {
                trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps = 1;
            }
        }
    }
    public void ClickButtonRepresentativesIncrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numPerformanceReps += 10;
            if (trainerRef.teamsConfig.environmentPopulation.numPerformanceReps > 10) {
                trainerRef.teamsConfig.environmentPopulation.numPerformanceReps = 10;
            }
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps += 1;
            if (trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps > 10) {
                trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps = 10;
            }
        }
    }

    public void ClickButtonToggleEvolving() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.isTraining = !trainerRef.teamsConfig.environmentPopulation.isTraining;
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].isTraining = !trainerRef.teamsConfig.playersList[focusPop - 1].isTraining;
        }
    }
}
