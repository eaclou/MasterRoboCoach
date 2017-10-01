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
    public Toggle toggleUseX;
    public Toggle toggleUseY;
    public Toggle toggleUseZ;
    public Toggle toggleAngle;
    public Toggle toggleVelocity;
    public Toggle togglePosition;
    public Toggle toggleQuaternion;
    public Toggle togglePistonY;

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

        toggleUseX.isOn = genome.useX;
        toggleUseY.isOn = genome.useY;
        toggleUseZ.isOn = genome.useZ;
        toggleAngle.isOn = genome.angleSensors;
        toggleVelocity.isOn = genome.velocitySensors;
        togglePosition.isOn = genome.positionSensors;
        toggleQuaternion.isOn = genome.quaternionSensors;
        togglePistonY.isOn = genome.usePistonY;
    }

    public void SliderMotorStrength(float value) {
        genome.motorStrength = value;
        textMotorStrengthValue.text = genome.motorStrength.ToString();
    }
    public void SliderAngleSensitivity(float value) {
        genome.angleSensitivity = value;
        textAngleSensitivityValue.text = genome.angleSensitivity.ToString();
    }
    public void ToggleUseX(bool value) {
        genome.useX = value;
    }
    public void ToggleUseY(bool value) {
        genome.useY = value;
    }
    public void ToggleUseZ(bool value) {
        genome.useZ = value;
    }
    public void ToggleAngle(bool value) {
        genome.angleSensors = value;
    }
    public void ToggleVelocity(bool value) {
        genome.velocitySensors = value;
    }
    public void TogglePosition(bool value) {
        genome.positionSensors = value;
    }
    public void ToggleQuaternion(bool value) {
        genome.quaternionSensors = value;
    }
    public void TogglePistonY(bool value) {
        genome.usePistonY = value;
    }
}
