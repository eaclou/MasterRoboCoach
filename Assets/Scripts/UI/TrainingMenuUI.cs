using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingMenuUI : MonoBehaviour {

    public MainMenu mainMenuRef;
    public TrainingManager trainerRef;

    public Toggle toggleManualMode;
    public Button buttonPlayPause;
    public Button buttonFaster;
    public Button buttonSlower;
    public Button buttonCameraMode;
    public Text playbackSpeed;

    public InputField inputFieldSaveName;
    public Button buttonSave;

    public GameObject panelManualSelectionToolbar;
    public Button buttonManualKeep;
    public Button buttonManualAuto;
    public Button buttonManualKill;
    public Button buttonManualReplay;

    public Button buttonIncreaseTimeSteps;
    public Button buttonDecreaseTimeSteps;
    public Text textMaxTimeSteps;

    public GameObject panelDebugLeft;
    public Text textDebugLeft;
    public Button buttonDebugLeft;
    public Button buttonCycleDebugLeft;

    public GameObject panelDebugRight;
    public Text textDebugRight;
    public Button buttonDebugRight;
    public Button buttonCycleDebugRight;

    public Button buttonCycleFocusPop;
    public Button buttonPrevGenome;
    public Button buttonRestartGenome;
    public Button buttonNextGenome;
        
    public Text textOpponent;
    public Text textTestingProgress;

    public bool debugLeftOn = false;
    public bool debugRightOn = false;
    //public int focusPopIndex = 0;

    public enum DebugLeftCurrentPage {
        AgentModules,
        AgentFitness,
        TrainingSettings,
        LastGenFitness
    }
    public DebugLeftCurrentPage debugLeftCurrentPage = DebugLeftCurrentPage.AgentModules;

    public enum DebugRightCurrentPage {
        Fitness,
    }
    public DebugRightCurrentPage debugRightCurrentPage = DebugRightCurrentPage.Fitness;

    // Use this for initialization
    void Start () {
        trainerRef = mainMenuRef.gameManagerRef.trainerRef;
    }
	
	// Update is called once per frame
	void Update () {
        SetStatusFromData();
    }

    public void SetStatusFromData() {
        if(debugLeftOn) {
            panelDebugLeft.SetActive(true);
            buttonDebugLeft.GetComponentInChildren<Text>().text = "-";
            SetDebugLeftText();
            buttonCycleDebugLeft.GetComponentInChildren<Text>().text = debugLeftCurrentPage.ToString();
        }
        else {
            panelDebugLeft.SetActive(false);
            buttonDebugLeft.GetComponentInChildren<Text>().text = "+";
        }

        if (debugRightOn) {
            panelDebugRight.SetActive(true);
            buttonDebugRight.GetComponentInChildren<Text>().text = "-";
            SetDebugRightText();
        }
        else {
            panelDebugRight.SetActive(false);
            buttonDebugRight.GetComponentInChildren<Text>().text = "+";
        }

        if(trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex < 1) {
            buttonCycleFocusPop.GetComponentInChildren<Text>().text = "ENV";            
        }
        else {
            buttonCycleFocusPop.GetComponentInChildren<Text>().text = "P:" + trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex.ToString();
        }

        textMaxTimeSteps.text = "MaxTimeSteps:\n" + trainerRef.evaluationManager.maxTimeStepsDefault.ToString();

        textOpponent.text = GetContestantsText();
        textTestingProgress.text = GetTestingProgressText();

        string cameraModeText = "";
        if(mainMenuRef.gameManagerRef.trainerRef.currentCameraMode == 0) {
            cameraModeText = "Wide";
        }
        else if (mainMenuRef.gameManagerRef.trainerRef.currentCameraMode == 1) {
            cameraModeText = "Top Down";
        }
        else {
            cameraModeText = "Shoulder";
        }
        buttonCameraMode.GetComponentInChildren<Text>().text = "Camera:\n" + cameraModeText;
    }

    private string GetTextAgentModules() {
        string txt = "";
        if (trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex < 1) {
            // ENVIRONMENT:
            EnvironmentGenome currentEnvironmentGenome = trainerRef.teamsConfig.environmentPopulation.environmentGenomeList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[0]];
            txt += "Environment Genome: " + currentEnvironmentGenome.index;
        }
        else {
            // AGENT:
            AgentGenome currentAgentGenome = trainerRef.teamsConfig.playersList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1].agentGenomeList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex]];
            // Index:
            txt += "Player: " + trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex.ToString();
            txt += ", Genome: " + currentAgentGenome.index.ToString() + "\n";
            // Modules:
            Agent curAgent = trainerRef.evaluationManager.exhibitionInstance.currentAgentsArray[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1];
            if (curAgent.healthModuleList != null)
                txt += "HEALTH: " + curAgent.healthModuleList[0].health.ToString() + " / " + curAgent.healthModuleList[0].maxHealth.ToString() + "\n";
            if (curAgent.targetSensorList != null) {
                txt += "\nTARGET SENSOR: ";
                if (curAgent.targetSensorList[0].targetPosition != null)
                    txt += curAgent.targetSensorList[0].targetPosition.position.ToString() + "\n";
                txt += "DotX = " + curAgent.targetSensorList[0].dotX[0].ToString() + "\n";
                txt += "DotZ = " + curAgent.targetSensorList[0].dotZ[0].ToString() + "\n";
                txt += "Forward = " + curAgent.targetSensorList[0].forward[0].ToString() + "\n";
                txt += "Horizontal = " + curAgent.targetSensorList[0].horizontal[0].ToString() + "\n";
                txt += "InTarget = " + curAgent.targetSensorList[0].inTarget[0].ToString() + "\n";
                txt += "VelX = " + curAgent.targetSensorList[0].velX[0].ToString() + "\n";
                txt += "VelZ = " + curAgent.targetSensorList[0].velZ[0].ToString() + "\n";
                txt += "Health = " + curAgent.targetSensorList[0].targetHealth[0].ToString() + "\n";
                txt += "Attacking = " + curAgent.targetSensorList[0].targetAttacking[0].ToString() + "\n";
            }
            if (curAgent.raycastSensorList != null) {
                txt += "\nRAYCAST SENSOR:\n";
                txt += "Left = " + curAgent.raycastSensorList[0].distanceLeft[0].ToString() + "\n";
                txt += "LeftCenter = " + curAgent.raycastSensorList[0].distanceLeftCenter[0].ToString() + "\n";
                txt += "CenterShort = " + curAgent.raycastSensorList[0].distanceCenterShort[0].ToString() + "\n";
                txt += "RightCenter = " + curAgent.raycastSensorList[0].distanceRightCenter[0].ToString() + "\n";
                txt += "Right = " + curAgent.raycastSensorList[0].distanceRight[0].ToString() + "\n";
                txt += "CenterLong = " + curAgent.raycastSensorList[0].distanceCenter[0].ToString() + "\n";
                txt += "Back = " + curAgent.raycastSensorList[0].distanceBack[0].ToString() + "\n";
            }
            if (curAgent.thrusterEffectorList != null) {
                txt += "\nTHRUSTER: ";
                txt += curAgent.thrusterEffectorList[0].throttle[0].ToString() + "\n";
            }
            if (curAgent.torqueEffectorList != null) {
                txt += "TORQUE: ";
                txt += curAgent.torqueEffectorList[0].throttle[0].ToString() + "\n";
            }
            if (curAgent.weaponProjectileList != null) {
                txt += "\nWEAPON-PROJECTILE:\n";
                txt += "Throttle = " + curAgent.weaponProjectileList[0].throttle[0].ToString() + "\n";
                txt += "Damage Inflicted = " + curAgent.weaponProjectileList[0].damageInflicted[0].ToString() + "\n";
                txt += "Energy = " + curAgent.weaponProjectileList[0].energy[0].ToString() + "\n";
            }
            if (curAgent.weaponTazerList != null) {
                txt += "\nWEAPON-TAZER:\n";
                txt += "Throttle = " + curAgent.weaponTazerList[0].throttle[0].ToString() + "\n";
                txt += "Damage Inflicted = " + curAgent.weaponTazerList[0].damageInflicted[0].ToString() + "\n";
                txt += "Energy = " + curAgent.weaponTazerList[0].energy[0].ToString() + "\n";
            }
            if (curAgent.contactSensorList != null) {
                txt += "\nCONTACT:\n";
                txt += "Contact = " + curAgent.contactSensorList[0].contactSensor[0].ToString() + "\n";
            }
            if (curAgent.healthModuleList != null) {
                txt += "\nHEALTH:\n";
                txt += "Health = " + curAgent.healthModuleList[0].healthSensor[0].ToString() + "\n";
                txt += "Damage = " + curAgent.healthModuleList[0].takingDamage[0].ToString() + "\n";
            }
        }
        return txt;
    }
    private string GetTextAgentFitness() {
        string txt = "FITNESS!\n\n";
        if(trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup != null) {
            for (int i = 0; i < trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
                txt += trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type.ToString();
                txt += " (" + trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.measure.ToString();
                txt += " ) rawScore= " + trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].rawScore.ToString() + "\n";
            }
        }
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        FitnessManager fit;
        if (focusPop < 1) {
            // ENV
            fit = trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            //Agent
            fit = trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
        }
        for(int i = 0; i < fit.fitnessComponentDefinitions.Count; i++) {
            txt += fit.fitnessComponentDefinitions[i].type.ToString();
            txt += " (" + fit.fitnessComponentDefinitions[i].measure.ToString() + ") " + fit.fitnessComponentDefinitions[i].biggerIsBetter.ToString() + ", " + fit.fitnessComponentDefinitions[i].weight.ToString() + "\n";
            //txt += " ) rawScore= " + trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].rawScore.ToString() + "\n";
        }
        txt += "\n";
        for (int i = 0; i < fit.FitnessEvalGroupArray.Length; i++) {
            txt += i.ToString() + ": ";
            for (int j = 0; j < fit.fitnessComponentDefinitions.Count; j++) {
                float fitCompTotal = 0f;
                for (int k = 0; k < fit.FitnessEvalGroupArray[i].Count; k++) {
                    fitCompTotal += fit.FitnessEvalGroupArray[i][k].fitCompList[j].rawScore;                    
                }
                txt += "    " + fitCompTotal.ToString("F4");
            }
            txt += "\n";
        }
        return txt;
    }
    private string GetTextTrainingSettings() {
        string txt = "TRAINING SETTINGS:\n\n";
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        TrainingSettingsManager settings;
        if(focusPop < 1) {
            // ENV
            settings = trainerRef.teamsConfig.environmentPopulation.trainingSettingsManager;
        }
        else {
            //Agent
            settings = trainerRef.teamsConfig.playersList[focusPop - 1].trainingSettingsManager;
        }
        txt += "Mutation Chance: " + (settings.mutationChance * 100f).ToString("F2") + "%\n";
        txt += "Mutation Step Size: " + (settings.mutationStepSize).ToString() + "\n";
        txt += "Max Evaluation Time Steps: " + trainerRef.evaluationManager.maxTimeStepsDefault.ToString();
        return txt;
    }
    private string GetTextLastGenFitness() {
        string txt = "FITNESS! avg: ";
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        FitnessManager fit;
        if (focusPop < 1) {
            // ENV
            fit = trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            //Agent
            fit = trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
        }        
        if(fit.rankedFitnessList != null && fit.rankedIndicesList != null) {
            float total = 0f;
            for (int i = 0; i < fit.rankedIndicesList.Length; i++) {
                total += fit.rankedFitnessList[i];
            }
            total /= fit.rankedIndicesList.Length;
            txt += total.ToString() + "\n\n";
            for (int i = 0; i < fit.rankedIndicesList.Length; i++) {
                txt += "Genome[" + fit.rankedIndicesList[i].ToString() + "] = " + fit.rankedFitnessList[i].ToString() + "\n";
            }
        }
        return txt;
    }
    private void SetDebugLeftText() {
        
        switch(debugLeftCurrentPage) {
            case DebugLeftCurrentPage.AgentModules:
                textDebugLeft.text = GetTextAgentModules();
                break;
            case DebugLeftCurrentPage.AgentFitness:
                textDebugLeft.text = GetTextAgentFitness();
                break;
            case DebugLeftCurrentPage.TrainingSettings:
                textDebugLeft.text = GetTextTrainingSettings();
                break;
            case DebugLeftCurrentPage.LastGenFitness:
                textDebugLeft.text = GetTextLastGenFitness();
                break;
            default:
                break;
        }
    }
    private void SetDebugRightText() {

    }

    //public string GetCurrentGenText() {
    //    return "Current Gen:\n" + mainMenuRef.gameManagerRef.trainerRef.playingCurGen.ToString();
    //}
    //public string GetExhibitionText() {
    //    string text = "Exhibition Match:\n" + trainerRef.evaluationManager.exhibitionTicketCurrentIndex.ToString();        
    //    return text;
    //}
    public string GetContestantsText() {
        string text = "Exhibition Match: " + trainerRef.evaluationManager.exhibitionTicketCurrentIndex.ToString() + "\n";
        if (trainerRef.isTraining) {
            text += "Environment #" + trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[0].ToString();
            for(int i = 1; i < trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices.Length; i++) {
                text += ", Player " + i.ToString() + " #" + trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[i].ToString();
            }
            float completionPercentage = 100f * (float)trainerRef.evaluationManager.exhibitionInstance.currentTimeStep / (float)trainerRef.evaluationManager.exhibitionInstance.maxTimeSteps;
            text += "\n[" + completionPercentage.ToString("F2") + "%]";
        }
        return text;
    }
    public string GetTestingProgressText() {
        string text = "Current Gen: " + mainMenuRef.gameManagerRef.trainerRef.playingCurGen.ToString() + "\n";
        if(trainerRef.isTraining) {
            int numComplete = 0;
            int numInProgress = 0;
            int totalEvals = trainerRef.evaluationManager.evaluationTicketList.Count;
            for (int i = 0; i < trainerRef.evaluationManager.evaluationTicketList.Count; i++) {
                if (trainerRef.evaluationManager.evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.Complete) {
                    numComplete++;
                }
                else if (trainerRef.evaluationManager.evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.InProgress) {
                    numInProgress++;
                }
            }
            float completionPercentage = 100f * (float)numComplete / (float)totalEvals;
            text += "In Progress: " + numInProgress.ToString() + "   Complete: " + numComplete.ToString() + "/" + totalEvals.ToString() + "\n[" + completionPercentage.ToString("F2") + "%]";
        }
        return text;
    }

    public void ClickSave() {
        
        string savename = inputFieldSaveName.text;
        if(savename != "") {
            mainMenuRef.gameManagerRef.trainerRef.SaveTraining(savename);
        }
        else {
            Debug.Log("SAVE FAILED! no name");
        }
    }

    public void ClickToggleManualMode(bool value) {
        
        //mainMenuRef.gameManagerRef.trainerRef.ManualTrainingMode = value;
        //print("clicked toggle manual mode! " + mainMenuRef.gameManagerRef.trainerRef.ManualTrainingMode.ToString());
    }

    public void ClickButtonPlayPause() {
        mainMenuRef.gameManagerRef.trainerRef.TogglePlayPause();
        if(mainMenuRef.gameManagerRef.trainerRef.TrainingPaused) {
            buttonPlayPause.GetComponentInChildren<Text>().text = "||";
        }
        else {
            buttonPlayPause.GetComponentInChildren<Text>().text = ">";
        }        
    }
    public void ClickButtonFaster() {
        mainMenuRef.gameManagerRef.trainerRef.IncreasePlaybackSpeed();
        playbackSpeed.text = "Playback Speed:\n" + mainMenuRef.gameManagerRef.trainerRef.playbackSpeed.ToString();
    }
    public void ClickButtonSlower() {
        mainMenuRef.gameManagerRef.trainerRef.DecreasePlaybackSpeed();
        playbackSpeed.text = "Playback Speed:\n" + mainMenuRef.gameManagerRef.trainerRef.playbackSpeed.ToString();
    }

    public void ClickButtonCameraMode() {
        mainMenuRef.gameManagerRef.trainerRef.ClickCameraMode();
    }
    public void ClickButtonManualKeep() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualKeep();
    }
    public void ClickButtonManualAuto() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualAuto();
    }
    public void ClickButtonManualKill() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualKill();
    }
    public void ClickButtonManualReplay() {
        mainMenuRef.gameManagerRef.trainerRef.ClickButtonManualReplay();
    }

    public void ClickButtonIncreaseTimeSteps() {
        trainerRef.evaluationManager.maxTimeStepsDefault += 30;
    }
    public void ClickButtonDecreaseTimeSteps() {
        trainerRef.evaluationManager.maxTimeStepsDefault -= 30;
        if(trainerRef.evaluationManager.maxTimeStepsDefault < 30) {
            trainerRef.evaluationManager.maxTimeStepsDefault = 30;
        }
    }

    public void ClickButtonCycleFocusPop() {
        //Debug.Log("ClickButtonCycleFocusPop");
        //int numPops = trainerRef.teamsConfig.playersList.Count + 1;
        //trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex++;
        //if(focusPopIndex >= numPops) {
        //    focusPopIndex = 0;
        //}
        trainerRef.evaluationManager.ExhibitionCycleFocusPop(trainerRef.teamsConfig);
    }

    public void ClickButtonDebugLeft() {
        //Debug.Log("ClickButtonDebugLeft");
        debugLeftOn = !debugLeftOn;
    }
    public void ClickButtonCycleDebugLeft() {
        //Debug.Log("ClickButtonCycleDebugLeft");

        int numPages = System.Enum.GetNames(typeof(DebugLeftCurrentPage)).Length;
        int curPage = (int)debugLeftCurrentPage;
        int newPage = curPage + 1;
        if (newPage >= numPages) {
            newPage = 0;
        }
        debugLeftCurrentPage = (DebugLeftCurrentPage)newPage;        
    }
    public void ClickButtonDebugRight() {
        //Debug.Log("ClickButtonDebugRight");
        debugRightOn = !debugRightOn;
    }
    public void ClickButtonCycleDebugRight() {
        //Debug.Log("ClickButtonCycleDebugRight");

        int numPages = System.Enum.GetNames(typeof(DebugRightCurrentPage)).Length;
        int curPage = (int)debugRightCurrentPage;
        int newPage = curPage + 1;
        if (newPage >= numPages) {
            newPage = 0;
        }
        debugRightCurrentPage = (DebugRightCurrentPage)newPage;
    }

    public void ClickButtonPrevGenome() {
        trainerRef.evaluationManager.ExhibitionPrevGenome(trainerRef.teamsConfig);
    }
    public void ClickButtonRestartGenome() {
        trainerRef.evaluationManager.ResetExhibitionInstance(trainerRef.teamsConfig);
    }
    public void ClickButtonNextGenome() {
        trainerRef.evaluationManager.ExhibitionNextGenome(trainerRef.teamsConfig);
    }
}
