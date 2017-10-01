using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditGravityUI : MonoBehaviour {

    public GravitySensorGenome genome;

    public Text textModuleDescription;

    //public Slider sliderValue;
    //public Text textValueValue;
    public Toggle toggleUseGravityDirection;
    public Toggle toggleUseVelocity;
    public Toggle toggleUseAltitude;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Gravity Sensor " + genome.inno.ToString();
        //sliderValue.value = genome.val;
        //textValueValue.text = genome.val.ToString();

        toggleUseGravityDirection.isOn = genome.useGravityDir;
        toggleUseVelocity.isOn = genome.useVel;
        toggleUseAltitude.isOn = genome.useAltitude;
    }

    /*public void SliderValue(float value) {
        genome.val = value;
        textValueValue.text = genome.val.ToString();
    }  */
    public void ToggleGravityDirection(bool value) {
        genome.useGravityDir = value;
    }
    public void ToggleVelocity(bool value) {
        genome.useVel = value;
    }
    public void ToggleAltitude(bool value) {
        genome.useAltitude = value;
    }
}
