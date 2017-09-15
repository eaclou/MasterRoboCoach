using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTargetUI : MonoBehaviour {

    public TargetSensorGenome genome;

    public Text textModuleDescription;

    public Slider sliderPositionX;
    public Text textPositionXValue;
    public Slider sliderPositionY;
    public Text textPositionYValue;
    public Slider sliderPositionZ;
    public Text textPositionZValue;
    public Slider sliderSensitivity;
    public Text textSensitivityValue;
    public Toggle toggleUseX;
    public Toggle toggleUseY;
    public Toggle toggleUseZ;
    public Toggle toggleUseVel;
    public Toggle toggleUseDist;
    public Toggle toggleUseInvDist;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Target Sensor " + genome.inno.ToString();
        sliderPositionX.value = genome.sensorPosition.x;
        textPositionXValue.text = genome.sensorPosition.x.ToString();
        sliderPositionY.value = genome.sensorPosition.y;
        textPositionYValue.text = genome.sensorPosition.y.ToString();
        sliderPositionZ.value = genome.sensorPosition.z;
        textPositionZValue.text = genome.sensorPosition.z.ToString();
        sliderSensitivity.value = genome.sensitivity;
        textSensitivityValue.text = genome.sensitivity.ToString();
        toggleUseX.isOn = genome.useX;
        toggleUseY.isOn = genome.useY;
        toggleUseZ.isOn = genome.useZ;
        toggleUseVel.isOn = genome.useVel;
        toggleUseDist.isOn = genome.useDist;
        toggleUseInvDist.isOn = genome.useInvDist;
    }

    public void SliderPositionX(float value) {
        genome.sensorPosition.x = value;
        textPositionXValue.text = genome.sensorPosition.x.ToString();
    }
    public void SliderPositionY(float value) {
        genome.sensorPosition.y = value;
        textPositionYValue.text = genome.sensorPosition.y.ToString();
    }
    public void SliderPositionZ(float value) {
        genome.sensorPosition.z = value;
        textPositionZValue.text = genome.sensorPosition.z.ToString();
    }
    public void SliderSensitivity(float value) {
        genome.sensitivity = value;
        textSensitivityValue.text = genome.sensitivity.ToString();
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
    public void ToggleUseVel(bool value) {
        genome.useVel = value;
    }
    public void ToggleUseDist(bool value) {
        genome.useDist = value;
    }
    public void ToggleUseInvDist(bool value) {
        genome.useInvDist = value;
    }
}
