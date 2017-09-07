using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleListItemUI : MonoBehaviour {

    public TrainingManager trainerRef;
    public int moduleIndex;
    public ModuleViewUI moduleViewUI;

    public Button buttonEditModule;
    public Text textModuleName;
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
    }

    public void ClickEdit() {
        //Debug.Log("ClickDelete()");
        //fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions.RemoveAt(fitnessIndex);
        //fitnessFunctionUI.SetStatusFromData(trainerRef);
    }
}
