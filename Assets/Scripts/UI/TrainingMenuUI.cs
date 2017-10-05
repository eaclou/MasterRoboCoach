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
    public Button buttonModuleView;
    public GameObject panelTrainingSettings;
    public TrainingSettingsUI trainingSettingsUI;
    public GameObject panelFitnessFunction;
    public FitnessFunctionUI fitnessFunctionUI;
    public GameObject panelModuleView;
    public ModuleViewUI moduleViewUI;

    public bool fitnessFunctionOn = false;
    public bool trainingSettingsOn = false;
    public bool moduleViewOn = false;

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

    public Image imageFitnessGraph;

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

        if(tournamentSelectOn) {
            panelTournamentSelect.GetComponent<TournamentSelectUI>().UpdateState();
        }
    }

    public void UpdateElementsWhilePlaying() {
        textOpponent.text = GetContestantsText();
        textTestingProgress.text = GetTestingProgressText();
    }

    public void UpdateGraphData() {
        imageFitnessGraph.GetComponent<Image>().material.SetTexture("_FitnessTex", gameManager.trainerRef.teamsConfig.playersList[0].graphKing.texFitnessAlltime);
        imageFitnessGraph.GetComponent<Image>().material.SetTexture("_FitnessTexCurrent", gameManager.trainerRef.teamsConfig.playersList[0].graphKing.texFitnessCurrent);
        Debug.Log("UpdateGraphData()!! " + gameManager.trainerRef.teamsConfig.playersList[0].graphKing.texFitnessAlltime.width.ToString());
    }
    public void InitializeUIFromGameState() {
        UpdateDebugLeftPanelUI();
        UpdateFitnessPanelUI();
        UpdateTrainingSettingsPanelUI();
        UpdateFocusPopUI();
        UpdateTimeStepsUI();
        UpdateProgressUI();
        UpdateCameraButtonUI();

        //Debug.Log("InitializeUIFromGameState()!! " );
        //if (gameManager.trainerRef.playingCurGen > 1) {
        //    UpdateGraphData();
        //}

        //UpdateTournamentSelectUI();
        if (tournamentSelectOn) {
            panelTournamentSelect.SetActive(true);
            panelTournamentSelect.GetComponent<TournamentSelectUI>().Initialize();
        }
        else {
            panelTournamentSelect.SetActive(false);
        }
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
            trainingSettingsOn = false; // are these needed?
            panelTrainingSettings.SetActive(false);
            moduleViewOn = false; // are these needed?
            panelModuleView.SetActive(false);
        }
        else {
            panelFitnessFunction.SetActive(false);
        }
    }
    private void UpdateTrainingSettingsPanelUI() {
        if (trainingSettingsOn) {
            panelTrainingSettings.SetActive(true);
            fitnessFunctionOn = false;  // are these needed?
            panelFitnessFunction.SetActive(false);
            moduleViewOn = false; // are these needed?
            panelModuleView.SetActive(false);

            // original bool check should prevent this trying to gather info from Trainer before it is initialized
            trainingSettingsUI.SetStatusFromData(gameManager.trainerRef);
        }
        else {
            panelTrainingSettings.SetActive(false);
        }
    }
    private void UpdateModuleViewPanelUI() {
        if (moduleViewOn) {
            panelModuleView.SetActive(true);
            fitnessFunctionOn = false; // are these needed?
            panelFitnessFunction.SetActive(false);
            trainingSettingsOn = false; // are these needed?
            panelTrainingSettings.SetActive(false);

            // Do I need the below?? v v v 
            moduleViewUI.SetStatusFromData(gameManager.trainerRef);
        }
        else {
            panelModuleView.SetActive(false);
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

        moduleViewUI.currentModulesPanelOn = true;
        moduleViewUI.editModulePanelOn = false;
        moduleViewUI.addNewModulePanelOn = false;
        UpdateModuleViewPanelUI();
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
            panelTournamentSelect.GetComponent<TournamentSelectUI>().UpdateState();
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
            txt += "Environment Genome: " + currentEnvironmentGenome.index + "\n\n";
            if (currentEnvironmentGenome.useAtmosphere) {
                txt += "ATMOSPHERE\nWind: " + currentEnvironmentGenome.atmosphereGenome.windForce.ToString();                
                //txt += "\n";
            }
            if (currentEnvironmentGenome.useBasicObstacles) {
                txt += "\nOBSTACLES ON\n";
            }
            if (currentEnvironmentGenome.useMeteorites) {
                txt += "\nMETEORITES ON\n";
            }
            if (currentEnvironmentGenome.useTargetColumn) {
                txt += "\nTARGET LOCATION ON\n";
            }
            if (currentEnvironmentGenome.terrainGenome.useAltitude) {
                txt += "\nALTITUDE ON\n";
            }
        }
        else {
            // AGENT:
            //AgentGenome currentAgentGenome = trainerRef.teamsConfig.playersList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1].agentGenomeList[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].genomeIndices[trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex]];
            AgentGenome currentAgentGenome = gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].agentGenomesList[gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1];
            // Index:
            txt += "Player: " + gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex.ToString();
            txt += ", Genome: " + currentAgentGenome.index.ToString() + "\n";
            txt += "BodyNeurons: " + currentAgentGenome.brainGenome.bodyNeuronList.Count.ToString() + "\n";
            txt += "HiddenNeurons: " + currentAgentGenome.brainGenome.hiddenNeuronList.Count.ToString() + "\n";
            txt += "Links: " + currentAgentGenome.brainGenome.linkList.Count.ToString() + "\n";
            // Modules:
            Agent curAgent = gameManager.trainerRef.evaluationManager.exhibitionInstance.currentAgentsArray[gameManager.trainerRef.evaluationManager.exhibitionTicketList[gameManager.trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex - 1];
            if (curAgent.healthModuleList.Count > 0)
                txt += "HEALTH: " + curAgent.healthModuleList[0].health.ToString() + " / " + curAgent.healthModuleList[0].maxHealth.ToString() + "\n";
            if (curAgent.oscillatorList.Count > 0) {
                txt += "\nOSCILLATOR INPUTS: ";
                for (int i = 0; i < curAgent.oscillatorList.Count; i++) {
                    txt += "\n" + i.ToString() + ": current value= " + curAgent.oscillatorList[i].value[0].ToString();
                }
                txt += "\n";
            }
            if (curAgent.targetSensorList.Count > 0) {
                txt += "\nTARGET SENSOR: ";            
                if (curAgent.targetSensorList[0].targetPosition != null) {
                    txt += curAgent.targetSensorList[0].targetPosition.position.ToString() + "\n";
                    if(currentAgentGenome.bodyGenome.targetSensorList[0].useX)
                        txt += "DotX = " + curAgent.targetSensorList[0].dotX[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useY)
                        txt += "DotY = " + curAgent.targetSensorList[0].dotY[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useZ)
                        txt += "DotZ = " + curAgent.targetSensorList[0].dotZ[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useX && currentAgentGenome.bodyGenome.targetSensorList[0].useVel)
                        txt += "DotVelX = " + curAgent.targetSensorList[0].dotVelX[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useY && currentAgentGenome.bodyGenome.targetSensorList[0].useVel)
                        txt += "DotVelY = " + curAgent.targetSensorList[0].dotVelY[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useZ && currentAgentGenome.bodyGenome.targetSensorList[0].useVel)
                        txt += "DotVelZ = " + curAgent.targetSensorList[0].dotVelZ[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useDist)
                        txt += "Dist = " + curAgent.targetSensorList[0].dist[0].ToString() + "\n";
                    if (currentAgentGenome.bodyGenome.targetSensorList[0].useInvDist)
                        txt += "InvDist = " + curAgent.targetSensorList[0].invDist[0].ToString() + "\n";
                }
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
                for (int i = 0; i < curAgent.thrusterEffectorList.Count; i++) {
                    //txt += "\n" + i.ToString() + ": current value= " + curAgent.thrusterEffectorList[i].throttleY[0].ToString();
                    if (currentAgentGenome.bodyGenome.thrusterList[0].useX)
                        txt += "X = " + curAgent.torqueEffectorList[0].throttleX[0].ToString() + ", ";
                    if (currentAgentGenome.bodyGenome.thrusterList[0].useY)
                        txt += "Y = " + curAgent.torqueEffectorList[0].throttleY[0].ToString() + ", ";
                    if (currentAgentGenome.bodyGenome.thrusterList[0].useZ)
                        txt += "Z = " + curAgent.torqueEffectorList[0].throttleZ[0].ToString() + "\n";
                }
                //txt += "\n";
                //txt += curAgent.thrusterEffectorList[0].throttleZ[0].ToString() + "\n";
            }
            if (curAgent.torqueEffectorList.Count > 0) {
                //txt += "TORQUE: ";
                txt += "\nTORQUE: ";
                if (currentAgentGenome.bodyGenome.torqueList[0].useX)
                    txt += "X = " + curAgent.torqueEffectorList[0].throttleX[0].ToString() + ", ";
                if (currentAgentGenome.bodyGenome.torqueList[0].useY)
                    txt += "Y = " + curAgent.torqueEffectorList[0].throttleY[0].ToString() + ", ";
                if (currentAgentGenome.bodyGenome.torqueList[0].useZ)
                    txt += "Z = " + curAgent.torqueEffectorList[0].throttleZ[0].ToString() + "\n";
                //txt += curAgent.torqueEffectorList[0].throttleY[0].ToString() + "\n";
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
            if (curAgent.gravitySensorList.Count > 0) {
                txt += "\nGRAVITY SENSOR:\n";
                if (currentAgentGenome.bodyGenome.gravitySensorList[0].useGravityDir) {
                    txt += "DotX = " + curAgent.gravitySensorList[0].dotX[0].ToString() + "\n";
                    txt += "DotY = " + curAgent.gravitySensorList[0].dotY[0].ToString() + "\n";
                    txt += "DotZ = " + curAgent.gravitySensorList[0].dotZ[0].ToString() + "\n";
                }
                if (currentAgentGenome.bodyGenome.gravitySensorList[0].useVel) {
                    txt += "VelX = " + curAgent.gravitySensorList[0].velX[0].ToString() + "\n";
                    txt += "VelY = " + curAgent.gravitySensorList[0].velY[0].ToString() + "\n";
                    txt += "VelZ = " + curAgent.gravitySensorList[0].velZ[0].ToString() + "\n";
                }
                if (currentAgentGenome.bodyGenome.gravitySensorList[0].useAltitude) {
                    txt += "Altitude = " + curAgent.gravitySensorList[0].altitude[0].ToString() + "\n";
                }
            }
            if (curAgent.healthModuleList.Count > 0) {
                txt += "\nHEALTH:\n";
                txt += "Health = " + curAgent.healthModuleList[0].healthSensor[0].ToString() + "\n";
                txt += "Damage = " + curAgent.healthModuleList[0].takingDamage[0].ToString() + "\n";
            }
            if (curAgent.basicWheelList.Count > 0) {
                txt += "\nBASIC AXLE:\n";
                for (int i = 0; i < curAgent.basicWheelList.Count; i++) {
                    txt += "Throttle = " + curAgent.basicWheelList[i].throttle[0].ToString() + "\n";
                    txt += "SteerAngle = " + curAgent.basicWheelList[i].steerAngle[0].ToString() + "\n";
                    txt += "Brake = " + curAgent.basicWheelList[i].brake[0].ToString() + "\n";
                    txt += "Speed = " + curAgent.basicWheelList[i].speed[0].ToString() + "\n\n";
                }
                //txt += "Throttle = " + curAgent.basicWheelList[0].throttle[0].ToString() + "\n";
                //txt += "SteerAngle = " + curAgent.basicWheelList[0].steerAngle[0].ToString() + "\n";
                //txt += "Brake = " + curAgent.basicWheelList[0].brake[0].ToString() + "\n";
                //txt += "Speed = " + curAgent.basicWheelList[0].speed[0].ToString() + "\n";
            }
            if (curAgent.atmosphereSensorList.Count > 0) {
                txt += "\nATMOSPHERE SENSOR:\n";
                txt += "WindX = " + curAgent.atmosphereSensorList[0].windDotX[0].ToString() + "\n";
                txt += "WindY = " + curAgent.atmosphereSensorList[0].windDotY[0].ToString() + "\n";
                txt += "WindZ = " + curAgent.atmosphereSensorList[0].windDotZ[0].ToString() + "\n";
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
        txt += "New Link Chance: " + (settings.newLinkChance).ToString() + "\n";
        txt += "New Hidden Node Chance: " + (settings.newHiddenNodeChance).ToString() + "\n";
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
        gameManager.trainerRef.evaluationManager.maxTimeStepsDefault = Mathf.RoundToInt((float)gameManager.trainerRef.evaluationManager.maxTimeStepsDefault * 4f / 3f);
        UpdateTimeStepsUI();
    }
    public void ClickButtonDecreaseTimeSteps() {
        gameManager.trainerRef.evaluationManager.maxTimeStepsDefault = Mathf.RoundToInt((float)gameManager.trainerRef.evaluationManager.maxTimeStepsDefault * 3f / 4f);
        if (gameManager.trainerRef.evaluationManager.maxTimeStepsDefault < 8) {
            gameManager.trainerRef.evaluationManager.maxTimeStepsDefault = 8;
        }
        UpdateTimeStepsUI();
    }

    public void ResetHistoricalData() {
        gameManager.trainerRef.teamsConfig.playersList[0].fitnessManager.ResetHistoricalData(); // only works for one player for now
        gameManager.trainerRef.teamsConfig.playersList[0].fitnessManager.ResetCurrentHistoricalDataLists();
        gameManager.trainerRef.teamsConfig.environmentPopulation.fitnessManager.ResetHistoricalData();
        gameManager.trainerRef.teamsConfig.environmentPopulation.fitnessManager.ResetCurrentHistoricalDataLists();
    }

    public void ClickButtonCycleFocusPop() {        
        gameManager.trainerRef.evaluationManager.ExhibitionCycleFocusPop(gameManager.trainerRef.teamsConfig);
        fitnessFunctionUI.SetStatusFromData(gameManager.trainerRef);
        UpdateDebugLeftPanelUI();
        moduleViewUI.SetPendingGenomesFromData(gameManager.trainerRef);
        UpdateFocusPopUI();
    }

    public void ClickButtonDebugLeft() {
        debugLeftOn = !debugLeftOn;
        UpdateDebugLeftPanelUI();
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
        moduleViewOn = false;
        UpdateTrainingSettingsPanelUI();
        UpdateFitnessPanelUI();
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
        moduleViewOn = false;
        UpdateFitnessPanelUI();
    }
    public void ClickButtonModuleView() {
        if (moduleViewOn) {
            moduleViewOn = false;
        }
        else {
            //gameManager.trainerRef.Pause();
            moduleViewOn = true;
            moduleViewUI.SetPendingGenomesFromData(gameManager.trainerRef);
            moduleViewUI.SetStatusFromData(gameManager.trainerRef);
        }
        fitnessFunctionOn = false;
        trainingSettingsOn = false;

        UpdateFitnessPanelUI();
        UpdateModuleViewPanelUI();
    }

    public void ClickButtonTournaments() {
        tournamentSelectOn = true;
        //UpdateTournamentSelectUI();
        panelTournamentSelect.SetActive(true);
        panelTournamentSelect.GetComponent<TournamentSelectUI>().Initialize();
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
