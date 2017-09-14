using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTorqueUI : MonoBehaviour {

    public TorqueGenome genome;

    public Text textModuleDescription;

    public Slider sliderStrength;
    public Text textStrengthValue;
    public Toggle toggleUseX;
    public Toggle toggleUseY;
    public Toggle toggleUseZ;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Torque " + genome.inno.ToString();
        sliderStrength.value = genome.strength;
        textStrengthValue.text = genome.strength.ToString();

        toggleUseX.isOn = genome.useX;
        toggleUseY.isOn = genome.useY;
        toggleUseZ.isOn = genome.useZ;
    }

    public void SliderStrength(float value) {
        genome.strength = value;
        textStrengthValue.text = genome.strength.ToString();
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
}
