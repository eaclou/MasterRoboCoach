using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditBasicWheelUI : MonoBehaviour {

    public BasicWheelGenome genome;

    public Text textModuleDescription;

    public Slider sliderHorsepower;
    public Text textHorsepowerValue;
    public Slider sliderMaxSteering;
    public Text textMaxSteeringValue;
    public Slider sliderBrakePower;
    public Text textBrakePowerValue;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Basic Wheel " + genome.inno.ToString();
        sliderHorsepower.value = genome.horsepower;
        textHorsepowerValue.text = genome.horsepower.ToString();
        sliderMaxSteering.value = genome.maxSteering;
        textMaxSteeringValue.text = genome.maxSteering.ToString();
        sliderBrakePower.value = genome.brakePower;
        textBrakePowerValue.text = genome.brakePower.ToString();
    }

    public void SliderHorsepower(float value) {

        genome.horsepower = value;
        textHorsepowerValue.text = genome.horsepower.ToString();
    }

    public void SliderMaxSteering(float value) {

        genome.maxSteering = value;
        textMaxSteeringValue.text = genome.maxSteering.ToString();
    }

    public void SliderBrakePower(float value) {

        genome.brakePower = value;
        textBrakePowerValue.text = genome.brakePower.ToString();
    }
}
