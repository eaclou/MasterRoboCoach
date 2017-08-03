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

    public Button buttonRepresentativesPerfDecrease;
    public Button buttonRepresentativesPerfIncrease;
    public Text textRepresentativesPerfValue;
    public Button buttonRepresentativesHistDecrease;
    public Button buttonRepresentativesHistIncrease;
    public Text textRepresentativesHistValue;
    public Button buttonRepresentativesBaseDecrease;
    public Button buttonRepresentativesBaseIncrease;
    public Text textRepresentativesBaseValue;

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
        int numPerfReps = 0;
        int numHistReps = 0;
        int numBaseReps = 0;
        bool isEvolving;
        if (focusPop < 1) {
            // env
            numPerfReps = trainerRef.teamsConfig.environmentPopulation.numPerformanceReps;
            numHistReps = trainerRef.teamsConfig.environmentPopulation.numHistoricalReps;
            numBaseReps = trainerRef.teamsConfig.environmentPopulation.numBaselineReps;
            currentTrainingSettingsRef = trainerRef.teamsConfig.environmentPopulation.trainingSettingsManager;
            isEvolving = trainerRef.teamsConfig.environmentPopulation.isTraining;
        }
        else {
            numPerfReps = trainerRef.teamsConfig.playersList[focusPop - 1].numPerformanceReps;
            numHistReps = trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps;
            numBaseReps = trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps;
            currentTrainingSettingsRef = trainerRef.teamsConfig.playersList[focusPop - 1].trainingSettingsManager;
            isEvolving = trainerRef.teamsConfig.playersList[focusPop - 1].isTraining;
        }

        textRepresentativesPerfValue.text = numPerfReps.ToString();
        textRepresentativesHistValue.text = numHistReps.ToString();
        textRepresentativesBaseValue.text = numBaseReps.ToString();
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

    public void ClickButtonRepsPerfDecrease() {
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
    public void ClickButtonRepsPerfIncrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numPerformanceReps += 1;
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

    public void ClickButtonRepsHistDecrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numHistoricalReps -= 1;
            if (trainerRef.teamsConfig.environmentPopulation.numHistoricalReps < 0) {
                trainerRef.teamsConfig.environmentPopulation.numHistoricalReps = 0;
            }
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps -= 1;
            if (trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps < 0) {
                trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps = 0;
            }
        }
    }
    public void ClickButtonRepsHistIncrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numHistoricalReps += 1;
            if (trainerRef.teamsConfig.environmentPopulation.numHistoricalReps > 10) {
                trainerRef.teamsConfig.environmentPopulation.numHistoricalReps = 10;
            }
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps += 1;
            if (trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps > 10) {
                trainerRef.teamsConfig.playersList[focusPop - 1].numHistoricalReps = 10;
            }
        }
    }

    public void ClickButtonRepsBaseDecrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numBaselineReps -= 1;
            if (trainerRef.teamsConfig.environmentPopulation.numBaselineReps < 0) {
                trainerRef.teamsConfig.environmentPopulation.numBaselineReps = 0;
            }
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps -= 1;
            if (trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps < 0) {
                trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps = 0;
            }
        }
    }
    public void ClickButtonRepsBaseIncrease() {
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            trainerRef.teamsConfig.environmentPopulation.numBaselineReps += 1;
            if (trainerRef.teamsConfig.environmentPopulation.numBaselineReps > 10) {
                trainerRef.teamsConfig.environmentPopulation.numBaselineReps = 10;
            }
        }
        else {
            trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps += 1;
            if (trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps > 10) {
                trainerRef.teamsConfig.playersList[focusPop - 1].numBaselineReps = 10;
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
