﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleViewUI : MonoBehaviour {

    public TrainingManager trainerRef;

    public Transform panelCurrentModules;
    public Transform panelEditModule;
    public Transform panelAddNewModule;

    public bool currentModulesPanelOn = true;
    public bool editModulePanelOn = false;
    public bool addNewModulePanelOn = false;
        
    public BodyGenome pendingBodyGenomeTemplate;
    public EnvironmentGenome pendingEnvironmentGenomeTemplate;
    public bool isEnvironment;
    
    public AgentModuleGenomeType newAgentModuleType = AgentModuleGenomeType.BasicJoint;
    public EnvironmentModuleGenomeType newEnvironmentModuleType = EnvironmentModuleGenomeType.Terrain;

    // Current Modules Panel:
    public Transform transformModuleListSpace;
    [SerializeField] GameObject goModuleListItemPrefab;
    public Button buttonAddNewModule;

    // Edit selected module Panel:
    public Text textSelectedModule;
    public Text textModuleDescription;
    public Button buttonBackToCurrentModules;

    // Add New Module Panel:
    public Text textModuleType;
    public Button buttonPrevModuleType;
    public Button buttonNextModuleType;
    public Text textModuleTypeInfo;
    public Button buttonAddSelectedModuleType;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData(TrainingManager trainerRef) {

        this.trainerRef = trainerRef;
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;

        if (focusPop < 1) {
            // env
            isEnvironment = true;
            pendingEnvironmentGenomeTemplate.CopyGenomeFromTemplate(trainerRef.teamsConfig.environmentPopulation.templateGenome);
            //currentFitnessManagerRef = trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            isEnvironment = false;
            pendingBodyGenomeTemplate.CopyBodyGenomeFromTemplate(trainerRef.teamsConfig.playersList[focusPop - 1].bodyGenomeTemplate);
            //currentTemplateAgentGenome.brainGenome.CopyCommunalBrainFromTemplate(trainerRef.teamsConfig.playersList[focusPop - 1].templateGenome.brainGenome); // only copies communal neurons
            //currentFitnessManagerRef = trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
        }        

        // CURRENT MODULE LIST!
        if(currentModulesPanelOn) {
            panelCurrentModules.gameObject.SetActive(true);

            panelEditModule.gameObject.SetActive(false);
            panelAddNewModule.gameObject.SetActive(false);

            UpdateCurrentModulesPanelUI();
        }
        // EDIT MODULE:
        if(editModulePanelOn) {
            panelEditModule.gameObject.SetActive(true);

            panelCurrentModules.gameObject.SetActive(false);
            panelAddNewModule.gameObject.SetActive(false);

            //UpdateEditModulePanelUI(); // done ON-Demand due to needing to pass data about selected module
        }
        // ADD NEW MODULE:
        if(addNewModulePanelOn) {
            panelAddNewModule.gameObject.SetActive(true);

            panelCurrentModules.gameObject.SetActive(false);
            panelEditModule.gameObject.SetActive(false);

            UpdateAddNewPanelUI();
        }

        //buttonNewComponentType.GetComponentInChildren<Text>().text = newComponentType.ToString();
    }

    private void UpdateCurrentModulesPanelUI() {

        foreach (Transform child in transformModuleListSpace) {
            GameObject.Destroy(child.gameObject);
        }

        if (isEnvironment) {
            // TERRAIN:
            //for(int i = 0; i < currentTemplateEnvironmentGenome.terrainGenome)
            if(pendingEnvironmentGenomeTemplate.useTerrain) {
                //currentTemplateEnvironmentGenome.terrainGenome
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = 0; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.Terrain, AgentModuleGenomeType.None, true);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // TARGET:
            if (pendingEnvironmentGenomeTemplate.useTargetColumn) {
                //currentTemplateEnvironmentGenome.terrainGenome
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = 0; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.Target, AgentModuleGenomeType.None, true);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // OBSTACLES:
            if (pendingEnvironmentGenomeTemplate.useBasicObstacles) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);
                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = 0; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.Obstacles, AgentModuleGenomeType.None, true);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
        }
        else { // AGENT:

            // BASIC JOINT:
            for (int i = 0; i < pendingBodyGenomeTemplate.basicJointList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.BasicJoint, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // BASIC WHEEL:
            for (int i = 0; i < pendingBodyGenomeTemplate.basicWheelList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.BasicWheel, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // CONTACT SENSOR:
            for (int i = 0; i < pendingBodyGenomeTemplate.contactSensorList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Contact, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // HEALTH SENSOR:
            for (int i = 0; i < pendingBodyGenomeTemplate.healthModuleList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Health, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // OSCILLATOR INPUT:
            for (int i = 0; i < pendingBodyGenomeTemplate.oscillatorInputList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Oscillator, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // RAYCAST SENSOR:
            for (int i = 0; i < pendingBodyGenomeTemplate.raycastSensorList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Raycast, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            //TARGET SENSOR:
            for (int i = 0; i < pendingBodyGenomeTemplate.targetSensorList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Target, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // THRUSTER EFFECTOR:
            for (int i = 0; i < pendingBodyGenomeTemplate.thrusterList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Thruster, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // TORQUE EFFECTOR:
            for (int i = 0; i < pendingBodyGenomeTemplate.torqueList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Torque, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // VALUE INPUT:
            for (int i = 0; i < pendingBodyGenomeTemplate.valueInputList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.Value, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // WEAPON PROJECTIL#E:
            for (int i = 0; i < pendingBodyGenomeTemplate.weaponProjectileList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.WeaponProjectile, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
            // WEAPON TAZER:
            for (int i = 0; i < pendingBodyGenomeTemplate.weaponTazerList.Count; i++) {
                GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                moduleListItemScript.trainerRef = trainerRef;
                moduleListItemScript.moduleViewUI = this;
                moduleListItemScript.SetStatusFromData(EnvironmentModuleGenomeType.None, AgentModuleGenomeType.WeaponTazer, false);
                moduleListItemGO.transform.SetParent(transformModuleListSpace);
            }
        }

    }
    private void UpdateEditModulePanelUI(EnvironmentModuleGenomeType envType, AgentModuleGenomeType agentType, bool isEnv, int moduleListIndex) {

        if (isEnv) {
            textModuleDescription.text = envType.ToString() + " " + moduleListIndex.ToString();

            switch (envType) {
                case EnvironmentModuleGenomeType.Terrain:
                    // do stuff
                    textModuleDescription.text += "\n\nColor: " + pendingEnvironmentGenomeTemplate.terrainGenome.color.ToString();
                    break;
                case EnvironmentModuleGenomeType.Target:
                    // do stuff
                    textModuleDescription.text += "\n\nRadius: " + pendingEnvironmentGenomeTemplate.targetColumnGenome.targetRadius.ToString();
                    textModuleDescription.text += "\nMinX: " + pendingEnvironmentGenomeTemplate.targetColumnGenome.minX.ToString();
                    textModuleDescription.text += "\nMaxX: " + pendingEnvironmentGenomeTemplate.targetColumnGenome.maxX.ToString();
                    textModuleDescription.text += "\nMinZ: " + pendingEnvironmentGenomeTemplate.targetColumnGenome.minZ.ToString();
                    textModuleDescription.text += "\nMaxZ: " + pendingEnvironmentGenomeTemplate.targetColumnGenome.maxZ.ToString();
                    break;
                case EnvironmentModuleGenomeType.Obstacles:
                    // do stuff
                    textModuleDescription.text += "\n\nNumObstacles: " + pendingEnvironmentGenomeTemplate.basicObstaclesGenome.obstaclePositions.Length.ToString();
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! EnvironmentModuleGenomeType: " + envType.ToString());
                    break;
            }
        }
        else {
            textModuleDescription.text = agentType.ToString() + " " + moduleListIndex.ToString();

            switch (agentType) {
                case AgentModuleGenomeType.BasicJoint:
                    // do stuff
                    textModuleDescription.text += "\n\nInno: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].inno.ToString();
                    textModuleDescription.text += "\nParent ID: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].parentID.ToString();
                    textModuleDescription.text += "\nMotor Strength: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].motorStrength.ToString();
                    textModuleDescription.text += "\nAngle Sensitivity: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].angleSensitivity.ToString();
                    textModuleDescription.text += "\nX Axis: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].useX.ToString();
                    textModuleDescription.text += "\nY Axis: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].useY.ToString();
                    textModuleDescription.text += "\nZ Axis: " + pendingBodyGenomeTemplate.basicJointList[moduleListIndex].useZ.ToString();
                    break;
                case AgentModuleGenomeType.BasicWheel:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Contact:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Health:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Oscillator:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Raycast:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Target:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Thruster:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Torque:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.Value:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.WeaponProjectile:
                    // do stuff
                    
                    break;
                case AgentModuleGenomeType.WeaponTazer:
                    // do stuff
                    
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! AgentModuleGenomeType: " + agentType.ToString());
                    break;
            }
        }
    }
    private void UpdateAddNewPanelUI() {
        if (isEnvironment) {
            textModuleType.text = newEnvironmentModuleType.ToString();

            switch (newEnvironmentModuleType) {
                case EnvironmentModuleGenomeType.Terrain:
                    // do stuff
                    textModuleTypeInfo.text = "Controls the topology of the arena ground - not currently addable";
                    buttonAddSelectedModuleType.interactable = false;
                    break;
                case EnvironmentModuleGenomeType.Target:
                    // do stuff
                    textModuleTypeInfo.text = "A target location - not currently addable";
                    buttonAddSelectedModuleType.interactable = false;
                    break;
                case EnvironmentModuleGenomeType.Obstacles:
                    // do stuff
                    textModuleTypeInfo.text = "Boulders that block movement";
                    if(pendingEnvironmentGenomeTemplate.useBasicObstacles) {
                        // already has them
                        textModuleTypeInfo.text += " - already applied!";
                        buttonAddSelectedModuleType.interactable = false;
                    }
                    else {
                        buttonAddSelectedModuleType.interactable = true;
                    }
                    
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! EnvironmentModuleGenomeType: " + newEnvironmentModuleType.ToString());
                    break;
            }
        }
        else {
            textModuleType.text = newAgentModuleType.ToString();

            switch (newAgentModuleType) {
                case AgentModuleGenomeType.BasicJoint:
                    // NOT ADDABLE CURRENTLY:
                    textModuleTypeInfo.text = "A Joint connection between two segments. Not currently available for addition";
                    buttonAddSelectedModuleType.interactable = false;
                    break;
                case AgentModuleGenomeType.BasicWheel:
                    // NOT ADDABLE CURRENTLY:
                    textModuleTypeInfo.text = "A Motorized Wheel. Not currently available for addition";
                    buttonAddSelectedModuleType.interactable = false;
                    break;
                case AgentModuleGenomeType.Contact:
                    // do stuff
                    // Complicated by the fact that it can be added onto any arbitrary segment!
                    // NOT ADDABLE CURRENTLY:
                    textModuleTypeInfo.text = "A Sensor which detects collision pressure against this segment. Not currently available for addition";
                    buttonAddSelectedModuleType.interactable = false;
                    break;
                case AgentModuleGenomeType.Health:
                    // do stuff
                    // NOT ADDABLE CURRENTLY:
                    textModuleTypeInfo.text = "A Sensor which keeps track of the Agent's vitals. Not currently available for addition";
                    buttonAddSelectedModuleType.interactable = false;
                    break;
                case AgentModuleGenomeType.Oscillator:
                    // do stuff
                    textModuleTypeInfo.text = "A cyclical rhythm input";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.Raycast:
                    // do stuff
                    textModuleTypeInfo.text = "A collection of rangefinding lasers to measure distance to nearest solid surface";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.Target:
                    // do stuff
                    textModuleTypeInfo.text = "A sensor which describes the location of a target position relative to the Agent";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.Thruster:
                    // do stuff
                    textModuleTypeInfo.text = "A propulsive thruster which applies force on the segment to which it is attached";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.Torque:
                    // do stuff
                    textModuleTypeInfo.text = "Applies an angular force on the segment to which it is attached";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.Value:
                    // do stuff
                    textModuleTypeInfo.text = "Provides a static input of a fixed value";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.WeaponProjectile:
                    // do stuff
                    textModuleTypeInfo.text = "A long range weapon";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                case AgentModuleGenomeType.WeaponTazer:
                    // do stuff
                    textModuleTypeInfo.text = "A short range weapon";
                    buttonAddSelectedModuleType.interactable = true;
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! AgentModuleGenomeType: " + newAgentModuleType.ToString());
                    break;
            }
        }
    }

    public void ClickAddNewModule() {  // Button to go to add new module Panel, not to apply module to current env/agent
        Debug.Log("Click Add New Module!");
        addNewModulePanelOn = true;

        currentModulesPanelOn = false;
        editModulePanelOn = false;

        SetStatusFromData(trainerRef);
    }

    public void ClickBackToCurrentModule() {
        currentModulesPanelOn = true;

        addNewModulePanelOn = false;        
        editModulePanelOn = false;

        SetStatusFromData(trainerRef);
    }  
    
    public void ClickEditModule(EnvironmentModuleGenomeType envType, AgentModuleGenomeType agentType, bool isEnv, int moduleListIndex) {  // index of its position in the list, NOT its innovation# id
        editModulePanelOn = true;

        currentModulesPanelOn = false;
        addNewModulePanelOn = false;

        UpdateEditModulePanelUI(envType, agentType, isEnv, moduleListIndex);
        SetStatusFromData(trainerRef);
    }

    public void ClickPrevModuleType() {
        if (isEnvironment) {
            int numType = System.Enum.GetNames(typeof(EnvironmentModuleGenomeType)).Length;
            int curType = (int)newEnvironmentModuleType;
            int newType = curType - 1;
            if (newType < 1) {
                newType = numType - 1;
            }
            newEnvironmentModuleType = (EnvironmentModuleGenomeType)newType;
        }
        else {
            int numType = System.Enum.GetNames(typeof(AgentModuleGenomeType)).Length;
            int curType = (int)newAgentModuleType;
            int newType = curType - 1;
            if (newType < 1) {
                newType = numType - 1;
            }
            newAgentModuleType = (AgentModuleGenomeType)newType;
        }
        UpdateAddNewPanelUI();
    }
    public void ClickNextModuleType() {
        if(isEnvironment) {
            int numType = System.Enum.GetNames(typeof(EnvironmentModuleGenomeType)).Length;
            int curType = (int)newEnvironmentModuleType;
            int newType = curType + 1;
            if (newType >= numType) {
                newType = 1;
            }
            newEnvironmentModuleType = (EnvironmentModuleGenomeType)newType;
        }
        else {
            int numType = System.Enum.GetNames(typeof(AgentModuleGenomeType)).Length;
            int curType = (int)newAgentModuleType;
            int newType = curType + 1;
            if (newType >= numType) {
                newType = 1;
            }
            newAgentModuleType = (AgentModuleGenomeType)newType;
        }
        UpdateAddNewPanelUI();
    }
    public void ClickAddSelectedModuleType() {
        //Debug.Log("ClickAddSelectedModuleType startPosCount: " + pendingEnvironmentGenomeTemplate.agentStartPositionsList.Count.ToString());
        // ACTUALLY ADD THE MODULE!!!!
        string debugText = "Added New Module: ";
        if (isEnvironment) {
            debugText += newEnvironmentModuleType.ToString();

            switch (newEnvironmentModuleType) {
                case EnvironmentModuleGenomeType.Terrain:
                    // do stuff
                    break;
                case EnvironmentModuleGenomeType.Target:
                    // do stuff
                    break;
                case EnvironmentModuleGenomeType.Obstacles:
                    // do stuff
                    BasicObstaclesGenome basicObstaclesGenome = new BasicObstaclesGenome();
                    basicObstaclesGenome.obstaclePositions = new Vector2[5]; // default
                    basicObstaclesGenome.obstacleScales = new float[5];
                    basicObstaclesGenome.InitializeRandomGenome();

                    pendingEnvironmentGenomeTemplate.basicObstaclesGenome = basicObstaclesGenome;
                    pendingEnvironmentGenomeTemplate.useBasicObstacles = true;
                    //nextInno++;
                    //oscillatorGenome.freq = 1f;
                    //oscillatorGenome.amp = 1f;

                    //pendingBodyGenomeTemplate.oscillatorInputList.Add(oscillatorGenome);
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! EnvironmentModuleGenomeType: " + newEnvironmentModuleType.ToString());
                    break;
            }
        }
        else {
            debugText += newAgentModuleType.ToString();
                        

            int nextInno = pendingBodyGenomeTemplate.GetCurrentHighestInnoValue() + 1;

            switch (newAgentModuleType) {
                case AgentModuleGenomeType.BasicJoint:
                    // NOT ADDABLE CURRENTLY:
                    break;
                case AgentModuleGenomeType.BasicWheel:
                    // NOT ADDABLE CURRENTLY:
                    break;
                case AgentModuleGenomeType.Contact:
                    // do stuff
                    break;
                case AgentModuleGenomeType.Health:
                    // do stuff
                    break;
                case AgentModuleGenomeType.Oscillator:
                    // do stuff
                    OscillatorGenome oscillatorGenome = new OscillatorGenome(0, nextInno);
                    nextInno++;
                    oscillatorGenome.freq = 1f;
                    oscillatorGenome.amp = 1f;                    

                    pendingBodyGenomeTemplate.oscillatorInputList.Add(oscillatorGenome);                    

                    break;
                case AgentModuleGenomeType.Raycast:
                    // do stuff
                    break;
                case AgentModuleGenomeType.Target:
                    // do stuff
                    break;
                case AgentModuleGenomeType.Thruster:
                    // do stuff
                    break;
                case AgentModuleGenomeType.Torque:
                    // do stuff
                    break;
                case AgentModuleGenomeType.Value:
                    // do stuff
                    break;
                case AgentModuleGenomeType.WeaponProjectile:
                    // do stuff
                    break;
                case AgentModuleGenomeType.WeaponTazer:
                    // do stuff
                    break;
                default:
                    // do stuff
                    Debug.LogError("NO SUCH ENUM TYPE!!! AgentModuleGenomeType: " + newAgentModuleType.ToString());
                    break;
            }
        }
        Debug.Log(debugText);

        // For now:
        int focusPop = trainerRef.evaluationManager.exhibitionTicketList[trainerRef.evaluationManager.exhibitionTicketCurrentIndex].focusPopIndex;
        
        //trainerRef.TogglePlayPause();
        trainerRef.UpdateActorModules(focusPop, pendingEnvironmentGenomeTemplate, pendingBodyGenomeTemplate);
        ClickBackToCurrentModule();

    }
}
