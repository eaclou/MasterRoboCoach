using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditBasicJointUI : MonoBehaviour {

    public BasicJointGenome genome;

    public Text textModuleDescription;

    public Slider sliderMotorStrength;
    public Text textMotorStrengthValue;
    public Slider sliderAngleSensitivity;
    public Text textAngleSensitivityValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Joint " + genome.inno.ToString();
        sliderMotorStrength.value = genome.motorStrength;
        textMotorStrengthValue.text = genome.motorStrength.ToString();
        sliderAngleSensitivity.value = genome.angleSensitivity;
        textAngleSensitivityValue.text = genome.angleSensitivity.ToString();
    }

    public void SliderMotorStrength(float value) {

        genome.motorStrength = value;
        textMotorStrengthValue.text = genome.motorStrength.ToString();
    }

    public void SliderAngleSensitivity(float value) {

        genome.angleSensitivity = value;
        textAngleSensitivityValue.text = genome.angleSensitivity.ToString();
    }
}
