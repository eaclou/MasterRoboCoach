using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FitnessCompRowUI : MonoBehaviour {

    public TrainingManager trainerRef;
    public int fitnessIndex;
    public FitnessFunctionUI fitnessFunctionUI;

    public Button buttonDelete;
    public Text textFitnessComponentName;
    public Toggle toggleBigIsBetter;
    public Button buttonMeasure;
    public Slider sliderWeight;
    public Text textWeightValue;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textFitnessComponentName.text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].type.ToString();
        toggleBigIsBetter.isOn = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].biggerIsBetter;
        buttonMeasure.GetComponentInChildren<Text>().text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure.ToString();
        sliderWeight.value = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].weight;
        textWeightValue.text = sliderWeight.value.ToString();
    }

    public void ToggleBiggerIsBetter(bool value) {
        Debug.Log("ToggleBiggerIsBetter(bool " + value.ToString() + ")");
        fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].biggerIsBetter = value;
    }
    public void SliderWeight(float value) {
        
        fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].weight = value;
        textWeightValue.text = sliderWeight.value.ToString();
        Debug.Log("SliderWeight(float " + fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].weight.ToString() + ")");
    }
    public void ClickMeasure() {
        Debug.Log("ClickMeasure()");
        int numMeasure = System.Enum.GetNames(typeof(FitnessComponentMeasure)).Length;
        int curMeasure = (int)fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure;
        int newMeasure = curMeasure + 1;
        if (newMeasure >= numMeasure) {
            newMeasure = 0;
        }
        fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure = (FitnessComponentMeasure)newMeasure;
        buttonMeasure.GetComponentInChildren<Text>().text = fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions[fitnessIndex].measure.ToString();
    }
    public void ClickDelete() {
        Debug.Log("ClickDelete()");
        fitnessFunctionUI.currentFitnessManagerRef.pendingFitnessComponentDefinitions.RemoveAt(fitnessIndex);
        fitnessFunctionUI.SetStatusFromData(trainerRef);
    }
}
