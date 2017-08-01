using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessFunctionUI : MonoBehaviour {

    public TrainingManager trainerRef;

    public Transform transformFitnessCompTableSpace;
    [SerializeField] GameObject goFitnessCompRowPrefab;

    public FitnessManager currentFitnessManagerRef;

    public FitnessComponentType newComponentType = FitnessComponentType.ContactHazard;

    public Button buttonNewComponentType;
    public Button buttonAddNewComponent;

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
            currentFitnessManagerRef = trainerRef.teamsConfig.environmentPopulation.fitnessManager;
        }
        else {
            currentFitnessManagerRef = trainerRef.teamsConfig.playersList[focusPop - 1].fitnessManager;
        }
        Debug.Log("FitnessUI SetStatusFromData! numFitComps: " + currentFitnessManagerRef.pendingFitnessComponentDefinitions.Count.ToString());

        // FITNESS COMPONENTS LIST!
        foreach (Transform child in transformFitnessCompTableSpace) {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < currentFitnessManagerRef.pendingFitnessComponentDefinitions.Count; i++) {
            GameObject fitnessComponentListRow = (GameObject)Instantiate(goFitnessCompRowPrefab);

            FitnessCompRowUI fitnessComponentRowScript = fitnessComponentListRow.GetComponent<FitnessCompRowUI>();
            
            fitnessComponentRowScript.fitnessIndex = i; // CHANGE LATER!!!!!!!
            fitnessComponentRowScript.trainerRef = trainerRef;
            fitnessComponentRowScript.fitnessFunctionUI = this;
            fitnessComponentRowScript.SetStatusFromData();
            fitnessComponentListRow.transform.SetParent(transformFitnessCompTableSpace);
        }

        buttonNewComponentType.GetComponentInChildren<Text>().text = newComponentType.ToString();
    }

    public void ClickCycleComponentType() {
        int numType = System.Enum.GetNames(typeof(FitnessComponentType)).Length;
        int curType = (int)newComponentType; // currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure;
        int newType = curType + 1;
        if (newType >= numType) {
            newType = 0;
        }
        newComponentType = (FitnessComponentType)newType;
        buttonNewComponentType.GetComponentInChildren<Text>().text = newComponentType.ToString();
    }
    public void ClickNewComponent() {
        // check for existing componentType
        bool typeExists = false;
        for(int i = 0; i < currentFitnessManagerRef.pendingFitnessComponentDefinitions.Count; i++) {
            if(currentFitnessManagerRef.pendingFitnessComponentDefinitions[i].type == newComponentType) {
                typeExists = true;
            }
        }
        if(typeExists) {
            return;
        }
        else {
            FitnessComponentDefinition newComponent = new FitnessComponentDefinition(newComponentType, FitnessComponentMeasure.Avg, 1f, true);
            currentFitnessManagerRef.pendingFitnessComponentDefinitions.Add(newComponent);
            SetStatusFromData(trainerRef);
        }
    }
}
