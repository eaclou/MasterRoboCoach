using System.Collections;
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

    public Transform transformModuleListSpace;
    [SerializeField] GameObject goModuleListItemPrefab;

    //public FitnessManager currentFitnessManagerRef;
    //public FitnessComponentType newComponentType = FitnessComponentType.ContactHazard;
    public AgentGenome currentTemplateAgentGenome;
    public EnvironmentGenome currentTemplateEnvironmentGenome;
    public bool isEnvironment;

    //public Button buttonNewComponentType;
    public Button buttonAddNewModule;

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
            currentTemplateEnvironmentGenome = trainerRef.teamsConfig.environmentPopulation.templateGenome;
            //currentFitnessManagerRef = trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            isEnvironment = false;
            currentTemplateAgentGenome = trainerRef.teamsConfig.playersList[focusPop - 1].templateGenome;
            //currentFitnessManagerRef = trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
        }        

        // CURRENT MODULE LIST!
        if(currentModulesPanelOn) {
            foreach (Transform child in transformModuleListSpace) {
                GameObject.Destroy(child.gameObject);
            }

            if (isEnvironment) {
                
            }
            else { // AGENT:


                for (int i = 0; i < currentTemplateAgentGenome.basicJointList.Count; i++) {
                    GameObject moduleListItemGO = (GameObject)Instantiate(goModuleListItemPrefab);

                    ModuleListItemUI moduleListItemScript = moduleListItemGO.GetComponent<ModuleListItemUI>();

                    moduleListItemScript.moduleIndex = i; // CHANGE LATER!!!!!!!
                    moduleListItemScript.trainerRef = trainerRef;
                    moduleListItemScript.moduleViewUI = this;
                    moduleListItemScript.SetStatusFromData();
                    moduleListItemGO.transform.SetParent(transformModuleListSpace);
                }
            }
        }
            
        
        


        //buttonNewComponentType.GetComponentInChildren<Text>().text = newComponentType.ToString();
    }

    public void ClickAddNewModule() {

    }
}
