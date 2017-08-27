using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrainingMenuUI : MonoBehaviour {

    public GameManager gameManager;
    //public MainMenuUI mainMenuRef;
    //public TrainingManager trainerRef;

    public GameObject panelTrainingShowHide;

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

    //public GameObject panelDebugRight;
    //public Text textDebugRight;
    //public Button buttonDebugRight;
    //public Button buttonCycleDebugRight;
    public Button buttonTrainingSettings;
    public Button buttonFitnessFunction;
    public GameObject panelTrainingSettings;
    public TrainingSettingsUI trainingSettingsUI;
    public GameObject panelFitnessFunction;
    public FitnessFunctionUI fitnessFunctionUI;
    public bool fitnessFunctionOn = false;
    public bool trainingSettingsOn = false;

    public Button buttonCycleFocusPop;
    public Button buttonPrevGenome;
    public Button buttonRestartGenome;
    public Button buttonNextGenome;
        
    public Text textOpponent;
    public Text textTestingProgress;

    public bool debugLeftOn = false;
    public bool debugRightOn = false;
    //public int focusPopIndex = 0;

    public Button buttonTournaments;
    public Button buttonShowHideUI;

    public bool tournamentSelectOn = false;

    public bool showUI = true;

    public GameObject panelTournamentSelect;
    

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
        //trainerRef = mainMenuRef.gameManager.trainerRef;
    }
	
	// Update is called once per frame
	void Update () {
        //SetStatusFromData();

        
    }

    public void EnterState() {
        InitializeUIFromGameState();
    }

    public void UpdateState() {
        UpdateElementsEveryFrame();  // updates all UI elements that should be refreshed every frame

        if(!gameManager.trainerRef.trainingPaused) {
            UpdateElementsWhilePlaying();  // updates all UI elements that should be refreshed every frame while game is playing
        }
    }

    public void ExitState() {

    }

    public void UpdateElementsEveryFrame() {
        if (gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex < 1) {
            buttonCycleFocusPop.GetComponentInChildren<Text>().text = "ENV";
        }
        else {
            buttonCycleFocusPop.GetComponentInChildren<Text>().text = "P:" + gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex.ToString();
        }

        if(debugLeftOn) {
            UpdateDebugLeftPanelUI();
        }

        if(trainingSettingsOn) {
            UpdateTrainingSettingsPanelUI();
        }
    }

    public void UpdateElementsWhilePlaying() {
        textOpponent.text = GetContestantsText();
        textTestingProgress.text = GetTestingProgressText();
    }

    public void InitializeUIFromGameState() {
        UpdateDebugLeftPanelUI();
        UpdateFitnessPanelUI();
        UpdateTrainingSettingsPanelUI();
        UpdateFocusPopUI();
        UpdateTimeStepsUI();
        UpdateProgressUI();
        UpdateCameraButtonUI();
        UpdateTournamentSelectUI();
    }

    private void UpdateDebugLeftPanelUI() {
        if (debugLeftOn) {
            panelDebugLeft.SetActive(true);
            buttonDebugLeft.GetComponentInChildren<Text>().text = "-";
            SetDebugLeftText();
            buttonCycleDebugLeft.GetComponentInChildren<Text>().text = debugLeftCurrentPage.ToString();
        }
        else {
            panelDebugLeft.SetActive(false);
            buttonDebugLeft.GetComponentInChildren<Text>().text = "+";
        }
    }
    private void UpdateFitnessPanelUI() {
        if (fitnessFunctionOn) {
            panelFitnessFunction.SetActive(true);
            trainingSettingsOn = false;
            panelTrainingSettings.SetActive(false);

        }
        else {
            panelFitnessFunction.SetActive(false);
        }
    }
    private void UpdateTrainingSettingsPanelUI() {
        if (trainingSettingsOn) {
            panelTrainingSettings.SetActive(true);
            fitnessFunctionOn = false;
            panelFitnessFunction.SetActive(false);

            // original bool check should prevent this trying to gather info from Trainer before it is initialized
            trainingSettingsUI.SetStatusFromData(gameManager.trainerRef);
        }
        else {
            panelTrainingSettings.SetActive(false);
        }
    }
    private void UpdateFocusPopUI() {
        if (gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex < 1) {
            buttonCycleFocusPop.GetComponentInChildren<Text>().text = "ENV";
        }
        else {
            buttonCycleFocusPop.GetComponentInChildren<Text>().text = "P:" + gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex.ToString();
        }
        UpdateFitnessPanelUI();
        UpdateTrainingSettingsPanelUI();
    }
    private void UpdateTimeStepsUI() {
        textMaxTimeSteps.text = "MaxTimeSteps:\n" + gameManager.trainerRef.evaluationManager.maxTimeStepsDefault.ToString();
    }
    private void UpdateProgressUI() {
        textOpponent.text = GetContestantsText();
        textTestingProgress.text = GetTestingProgressText();
    }
    private void UpdateTournamentSelectUI() {
        if(tournamentSelectOn) {
            panelTournamentSelect.SetActive(true);
            panelTournamentSelect.GetComponent<TournamentSelectUI>().Initialize();
        }
        else {
            panelTournamentSelect.SetActive(false);
        }
    }

    private void UpdateCameraButtonUI() {
        string cameraModeText = "";
        if (gameManager.cameraManager.currentCameraMode == 0) {
            cameraModeText = "Wide";
        }
        else if (gameManager.cameraManager.currentCameraMode == 1) {
            cameraModeText = "Top Down";
        }
        else {
            cameraModeText = "Shoulder";
        }
        buttonCameraMode.GetComponentInChildren<Text>().text = "Camera:\n" + cameraModeText;
    }

    private string GetTextAgentModules() {
        string txt = "";
        if (gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex < 1) {
            // ENVIRONMENT:
            //EnvironmentGenome currentEnvironmentGenome = trainerRef.teamsConfig.environmentPopulation.environmentGenomeList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[0]];
            EnvironmentGenome currentEnvironmentGenome = gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].environmentGenome;
            txt += "Environment Genome: " + currentEnvironmentGenome.index;
        }
        else {
            // AGENT:
            //AgentGenome currentAgentGenome = trainerRef.teamsConfig.playersList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1].agentGenomeList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex]];
            AgentGenome currentAgentGenome = gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].agentGenomesList[gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1];
            // Index:
            txt += "Player: " + gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex.ToString();
            txt += ", Genome: " + currentAgentGenome.index.ToString() + "\n";
            // Modules:
            Agent curAgent = gameManager.trainerRef.evaluationManager.exhibitionInstance.currentAgentsArray[gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1];
            if (curAgent.healthModuleList.Count > 0)
                txt += "HEALTH: " + curAgent.healthModuleList[0].health.ToString() + " / " + curAgent.healthModuleList[0].maxHealth.ToString() + "\n";
            if (curAgent.targetSensorList.Count > 0) {
                txt += "\nTARGET SENSOR: ";
                if (curAgent.targetSensorList[0].targetPosition != null)
                    txt += curAgent.targetSensorList[0].targetPosition.position.ToString() + "\n";
                txt += "DotX = " + curAgent.targetSensorList[0].dotX[0].ToString() + "\n";
                txt += "DotZ = " + curAgent.targetSensorList[0].dotZ[0].ToString() + "\n";
                txt += "Dist = " + curAgent.targetSensorList[0].dist[0].ToString() + "\n";
                txt += "InvDist = " + curAgent.targetSensorList[0].invDist[0].ToString() + "\n";
                //txt += "Forward = " + curAgent.targetSensorList[0].forward[0].ToString() + "\n";
                //txt += "Horizontal = " + curAgent.targetSensorList[0].horizontal[0].ToString() + "\n";
                //txt += "InTarget = " + curAgent.targetSensorList[0].inTarget[0].ToString() + "\n";
                //txt += "VelX = " + curAgent.targetSensorList[0].velX[0].ToString() + "\n";
                //txt += "VelZ = " + curAgent.targetSensorList[0].velZ[0].ToString() + "\n";
                //txt += "Health = " + curAgent.targetSensorList[0].targetHealth[0].ToString() + "\n";
                //txt += "Attacking = " + curAgent.targetSensorList[0].targetAttacking[0].ToString() + "\n";
            }
            if (curAgent.raycastSensorList.Count > 0) {
                txt += "\nRAYCAST SENSOR:\n";
                txt += "Left = " + curAgent.raycastSensorList[0].distanceLeft[0].ToString() + "\n";
                txt += "LeftCenter = " + curAgent.raycastSensorList[0].distanceLeftCenter[0].ToString() + "\n";
                txt += "CenterShort = " + curAgent.raycastSensorList[0].distanceCenterShort[0].ToString() + "\n";
                txt += "RightCenter = " + curAgent.raycastSensorList[0].distanceRightCenter[0].ToString() + "\n";
                txt += "Right = " + curAgent.raycastSensorList[0].distanceRight[0].ToString() + "\n";
                txt += "CenterLong = " + curAgent.raycastSensorList[0].distanceCenter[0].ToString() + "\n";
                txt += "Back = " + curAgent.raycastSensorList[0].distanceBack[0].ToString() + "\n";
            }
            if (curAgent.thrusterEffectorList.Count > 0) {
                txt += "\nTHRUSTER: ";
                txt += curAgent.thrusterEffectorList[0].throttle[0].ToString() + "\n";
            }
            if (curAgent.torqueEffectorList.Count > 0) {
                txt += "TORQUE: ";
                txt += curAgent.torqueEffectorList[0].throttle[0].ToString() + "\n";
            }
            if (curAgent.weaponProjectileList.Count > 0) {
                txt += "\nWEAPON-PROJECTILE:\n";
                txt += "Throttle = " + curAgent.weaponProjectileList[0].throttle[0].ToString() + "\n";
                txt += "Damage Inflicted = " + curAgent.weaponProjectileList[0].damageInflicted[0].ToString() + "\n";
                txt += "Energy = " + curAgent.weaponProjectileList[0].energy[0].ToString() + "\n";
            }
            if (curAgent.weaponTazerList.Count > 0) {
                txt += "\nWEAPON-TAZER:\n";
                txt += "Throttle = " + curAgent.weaponTazerList[0].throttle[0].ToString() + "\n";
                txt += "Damage Inflicted = " + curAgent.weaponTazerList[0].damageInflicted[0].ToString() + "\n";
                txt += "Energy = " + curAgent.weaponTazerList[0].energy[0].ToString() + "\n";
            }
            if (curAgent.contactSensorList.Count > 0) {
                txt += "\nCONTACT:\n";
                txt += "Contact = " + curAgent.contactSensorList[0].contactSensor[0].ToString() + "\n";
            }
            if (curAgent.healthModuleList.Count > 0) {
                txt += "\nHEALTH:\n";
                txt += "Health = " + curAgent.healthModuleList[0].healthSensor[0].ToString() + "\n";
                txt += "Damage = " + curAgent.healthModuleList[0].takingDamage[0].ToString() + "\n";
            }
            if (curAgent.basicWheelList.Count > 0) {
                txt += "\nBASIC AXLE:\n";
                txt += "Throttle = " + curAgent.basicWheelList[0].throttle[0].ToString() + "\n";
                txt += "SteerAngle = " + curAgent.basicWheelList[0].steerAngle[0].ToString() + "\n";
                txt += "Brake = " + curAgent.basicWheelList[0].brake[0].ToString() + "\n";
                txt += "Speed = " + curAgent.basicWheelList[0].speed[0].ToString() + "\n";
            }
        }
        return txt;
    }
    private string GetTextAgentFitness() {
        string txt = "FITNESS!\n\n";
        if(gameManager.trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup != null) {
            for (int i = 0; i < gameManager.trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList.Count; i++) {
                txt += gameManager.trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.type.ToString();
                txt += " (" + gameManager.trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].sourceDefinition.measure.ToString();
                txt += " ) rawScore= " + gameManager.trainerRef.evaluationManager.exhibitionInstance.fitnessComponentEvaluationGroup.fitCompList[i].rawScore.ToString() + "\n";
            }
        }
        int focusPop = gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        FitnessManager fit;
        if (focusPop < 1) {
            // ENV
            fit = gameManager.trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            //Agent
            fit = gameManager.trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
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
        int focusPop = gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        TrainingSettingsManager settings;
        if(focusPop < 1) {
            // ENV
            settings = gameManager.trainerRef.teamsConfig.environmentPopulation.trainingSettingsManager;
        }
        else {
            //Agent
            settings = gameManager.trainerRef.teamsConfig.playersList[focusPop - 1].trainingSettingsManager;
        }
        txt += "Mutation Chance: " + (settings.mutationChance * 100f).ToString("F2") + "%\n";
        txt += "Mutation Step Size: " + (settings.mutationStepSize).ToString() + "\n";
        txt += "Max Evaluation Time Steps: " + gameManager.trainerRef.evaluationManager.maxTimeStepsDefault.ToString();
        return txt;
    }
    private string GetTextLastGenFitness() {
        string txt = "FITNESS! avg: ";
        int focusPop = gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        FitnessManager fit;
        if (focusPop < 1) {
            // ENV
            fit = gameManager.trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            //Agent
            fit = gameManager.trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
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
    
    public string GetContestantsText() {
        string text = "Exhibition Match: " + gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex.ToString() + "\n";
        if (gameManager.trainerRef.isTraining) {
            text += "Environment #" + gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].environmentGenome.index.ToString();
            for(int i = 0; i < gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].agentGenomesList.Count; i++) {
                text += ", Player " + i.ToString() + " #" + gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].agentGenomesList[i].index.ToString();
            }
            float completionPercentage = 100f * (float)gameManager.trainerRef.evaluationManager.exhibitionInstance.currentTimeStep / (float)gameManager.trainerRef.evaluationManager.exhibitionInstance.maxTimeSteps;
            text += "\n[" + completionPercentage.ToString("F2") + "%]";
        }
        return text;
    }
    public string GetTestingProgressText() {
        string text = "Current Gen: " + gameManager.trainerRef.playingCurGen.ToString() + "\n";
        if(gameManager.trainerRef.isTraining) {
            int numComplete = 0;
            int numInProgress = 0;
            int totalEvals = gameManager.trainerRef.evaluationManager.evaluationTicketList.Count;
            for (int i = 0; i < gameManager.trainerRef.evaluationManager.evaluationTicketList.Count; i++) {
                if (gameManager.trainerRef.evaluationManager.evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.Complete) {
                    numComplete++;
                }
                else if (gameManager.trainerRef.evaluationManager.evaluationTicketList[i].status == EvaluationTicket.EvaluationStatus.InProgress) {
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
            gameManager.trainerRef.SaveTraining(savename);
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
        gameManager.trainerRef.TogglePlayPause();
        if(gameManager.trainerRef.TrainingPaused) {
            buttonPlayPause.GetComponentInChildren<Text>().text = "||";
        }
        else {
            buttonPlayPause.GetComponentInChildren<Text>().text = ">";
        }        
    }
    public void ClickButtonFaster() {
        gameManager.trainerRef.IncreasePlaybackSpeed();
        playbackSpeed.text = "Playback Speed:\n" + gameManager.trainerRef.playbackSpeed.ToString();
    }
    public void ClickButtonSlower() {
        gameManager.trainerRef.DecreasePlaybackSpeed();
        playbackSpeed.text = "Playback Speed:\n" + gameManager.trainerRef.playbackSpeed.ToString();
    }

    public void ClickButtonCameraMode() {
        gameManager.trainerRef.ClickCameraMode();
        UpdateCameraButtonUI();
    }
    public void ClickButtonManualKeep() {
        gameManager.trainerRef.ClickButtonManualKeep();
    }
    public void ClickButtonManualAuto() {
        gameManager.trainerRef.ClickButtonManualAuto();
    }
    public void ClickButtonManualKill() {
        gameManager.trainerRef.ClickButtonManualKill();
    }
    public void ClickButtonManualReplay() {
        gameManager.trainerRef.ClickButtonManualReplay();
    }

    public void ClickButtonIncreaseTimeSteps() {
        gameManager.trainerRef.evaluationManager.maxTimeStepsDefault += 30;
        UpdateTimeStepsUI();
    }
    public void ClickButtonDecreaseTimeSteps() {
        gameManager.trainerRef.evaluationManager.maxTimeStepsDefault -= 30;
        if(gameManager.trainerRef.evaluationManager.maxTimeStepsDefault < 30) {
            gameManager.trainerRef.evaluationManager.maxTimeStepsDefault = 30;
        }
        UpdateTimeStepsUI();
    }

    public void ClickButtonCycleFocusPop() {        
        gameManager.trainerRef.evaluationManager.ExhibitionCycleFocusPop(gameManager.trainerRef.teamsConfig);
        fitnessFunctionUI.SetStatusFromData(gameManager.trainerRef);
        UpdateFocusPopUI();
    }

    public void ClickButtonDebugLeft() {
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

    public void ClickButtonPrevGenome() {
        gameManager.trainerRef.evaluationManager.ExhibitionPrevGenome(gameManager.trainerRef.teamsConfig);
    }
    public void ClickButtonRestartGenome() {
        gameManager.trainerRef.evaluationManager.ResetExhibitionInstance(gameManager.trainerRef.teamsConfig);
    }
    public void ClickButtonNextGenome() {
        gameManager.trainerRef.evaluationManager.ExhibitionNextGenome(gameManager.trainerRef.teamsConfig);
    }

    public void ClickButtonTrainingSettings() {
        if(trainingSettingsOn) {
            trainingSettingsOn = false;
        }
        else {
            trainingSettingsOn = true;
        }        
        fitnessFunctionOn = false;
        UpdateTrainingSettingsPanelUI();
    }
    public void ClickButtonFitnessFunction() {
        if (fitnessFunctionOn) {
            fitnessFunctionOn = false;
        }
        else {
            fitnessFunctionOn = true;
            fitnessFunctionUI.SetStatusFromData(gameManager.trainerRef);
        }
        trainingSettingsOn = false;

        UpdateFitnessPanelUI();
    }

    public void ClickButtonTournaments() {
        tournamentSelectOn = true;
        UpdateTournamentSelectUI();
        ClickButtonPlayPause();
        //ShowTournamentSelectScreen();
        //gameManager.trainerRef.EnterTournamentSelectScreen();
    }
    /*public void ShowTournamentSelectScreen() {
        Debug.Log("ShowTournamentSelectScreen");
        panelTournamentSelect.SetActive(true);
        panelTournamentSelect.GetComponent<TournamentSelectUI>().Initialize();
        ClickButtonPlayPause();
    }
    public void HideTournamentSelectScreen() {
        Debug.Log("HideTournamentSelectScreen");
        panelTournamentSelect.SetActive(false);
        ClickButtonPlayPause();
    }*/

    public void ClickButtonShowHideUI() {
        if(showUI) {
            HideUI();
        }
        else {
            ShowUI();
        }
    }
    
    public void ShowUI() {
        showUI = true;
        panelTrainingShowHide.SetActive(true);
        //SetStatusFromData();
    }
    public void HideUI() {
        showUI = false;
        panelTrainingShowHide.SetActive(false);
    }
}
