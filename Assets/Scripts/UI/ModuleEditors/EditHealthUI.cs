using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EditHealthUI : MonoBehaviour {

    public HealthGenome genome;

    public Text textModuleDescription;

    public Slider sliderMaxHealth;
    public Text textMaxHealthValue;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetStatusFromData() {
        textModuleDescription.text = "Health Module " + genome.inno.ToString();
        sliderMaxHealth.minValue = 5;
        sliderMaxHealth.maxValue = 250;
        sliderMaxHealth.value = genome.maxHealth;
        textMaxHealthValue.text = genome.maxHealth.ToString();
    }

    public void SliderMaxHealth(float value) {

        genome.maxHealth = Mathf.RoundToInt(value);
        textMaxHealthValue.text = genome.maxHealth.ToString();
    }
}
