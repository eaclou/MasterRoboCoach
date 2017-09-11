using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditTorqueUI : MonoBehaviour {

    public TorqueGenome genome;

    public Text textModuleDescription;

    public Slider sliderStrength;
    public Text textStrengthValue;


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
    }

    public void SliderValue(float value) {
        genome.strength = value;
        textStrengthValue.text = genome.strength.ToString();
    }
}
