using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditAtmosphereUI : MonoBehaviour {

    public AtmosphereGenome genome;

    public Text textModuleDescription;

    public Slider sliderMaxWindSpeed;
    public Text textMaxWindSpeedValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Atmosphere!\nWind Force: " + genome.windForce.ToString();
        sliderMaxWindSpeed.value = genome.maxWindSpeed;
        textMaxWindSpeedValue.text = genome.maxWindSpeed.ToString();
    }

    public void SliderMaxWindSpeed(float value) {
        genome.maxWindSpeed = value;
        textMaxWindSpeedValue.text = genome.maxWindSpeed.ToString();
    }
}
